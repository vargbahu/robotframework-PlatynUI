using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PlatynUI.Provider.Core;

namespace PlatynUI.Provider.Avalonia;

public class ApplicationNode : Node
{
    public override NodeType NodeType => NodeType.Application;

    public override string LocalName => "Application";

    public override bool IsValid()
    {
        return Application.Current != null;
    }

    private Dictionary<string, Func<object?>>? _attributes = null;

    protected Dictionary<string, Func<object?>> Attributes =>
        _attributes ??= new()
        {
            ["Technology"] = () => "Avalonia",
            ["Role"] = () => new string[] { LocalName },
            ["Name"] = () => Application.Current?.Name,
            ["RuntimeId"] = () => Reference.RuntimeId,
            ["ProcessId"] = () => Environment.ProcessId,
            ["ProcessName"] = () => Process.GetCurrentProcess().ProcessName,
            ["SessionId"] = () => Process.GetCurrentProcess().SessionId,
            ["MainWindowHandle"] = () => Process.GetCurrentProcess().MainWindowHandle,
            ["MainWindowTitle"] = () => Process.GetCurrentProcess().MainWindowTitle,
            ["MainModule.FileName"] = () => Process.GetCurrentProcess().MainModule?.FileName,
            ["MainModule.ModuleName"] = () => Process.GetCurrentProcess().MainModule?.ModuleName,
            ["FileVersionInfo.FileDescription"] = () =>
                Process.GetCurrentProcess().MainModule?.FileVersionInfo.FileDescription,
            ["FileVersionInfo.ProductName"] = () => Process.GetCurrentProcess().MainModule?.FileVersionInfo.ProductName,
            ["FileVersionInfo.InternalName"] = () =>
                Process.GetCurrentProcess().MainModule?.FileVersionInfo.InternalName,
            ["FileVersionInfo.CompanyName"] = () => Process.GetCurrentProcess().MainModule?.FileVersionInfo.CompanyName,
            ["FileVersionInfo.Comments"] = () => Process.GetCurrentProcess().MainModule?.FileVersionInfo.Comments,
            ["FileVersionInfo.FileVersion"] = () => Process.GetCurrentProcess().MainModule?.FileVersionInfo.FileVersion,
            ["FileVersionInfo.ProductVersion"] = () =>
                Process.GetCurrentProcess().MainModule?.FileVersionInfo.ProductVersion,
            ["FileVersionInfo.SpecialBuild"] = () =>
                Process.GetCurrentProcess().MainModule?.FileVersionInfo.SpecialBuild,
            ["FileVersionInfo.IsDebug"] = () => Process.GetCurrentProcess().MainModule?.FileVersionInfo.IsDebug,
            ["RuntimeInformation.FrameworkDescription"] = () =>
                System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
        };

    public override string[] GetAttributeNames()
    {
        return [.. Attributes.Keys];
    }

    public override object? GetAttributeValue(string attributeName)
    {
        if (Attributes.TryGetValue(attributeName, out var value))
        {
            try
            {
                return value();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        return null;
    }

    public override string GetAttributeValueType(string attributeName)
    {
        var value = GetAttributeValue(attributeName);
        return value?.GetType().FullName ?? "";
    }

    internal override IEnumerable<Node> GetChildren()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            // Get main windows
            var windows = lifetime.Windows
                .Select(w => NodeInfo.GetOrCreateNode<ElementNode, Control>(w))
                .Where(n => n != null && n.IsValid());
            
            // Get popup roots by searching for open popups in all windows
            // Must be done on the UI thread
            var popupRoots = Dispatcher.UIThread.Invoke(() =>
            {
                var roots = new List<Node>();
                foreach (var window in lifetime.Windows)
                {
                    var openPopups = window.GetVisualDescendants()
                        .OfType<Control>()
                        .Where(control => control is PopupRoot or OverlayPopupHost);
                    
                    //Console.WriteLine($"Found {openPopups.Count()} open popups in window '{window.Title}'.");
                    //Console.WriteLine($"Popup roots: {string.Join(", ", openPopups.Select(pr => pr!.Name))}");

                    foreach (var popupRoot in openPopups)
                    {
                        /*
                        Console.WriteLine($"\nPopupRoot: {popupRoot!.GetType().Name} (Name: {popupRoot.Name})");
                        
                        // Print all descendants
                        var descendants = popupRoot.GetVisualDescendants().ToList();
                        Console.WriteLine($"  Total descendants: {descendants.Count}");
                        
                        foreach (var descendant in descendants)
                        {
                            var indent = "  ";
                            var level = 0;
                            var parent = descendant;
                            while (parent != null && parent != popupRoot)
                            {
                                parent = parent.GetVisualParent() as Visual;
                                level++;
                            }
                            indent = new string(' ', level * 2 + 2);
                            
                            var controlInfo = $"{indent}{descendant.GetType().Name}";
                            if (descendant is Control ctrl)
                            {
                                controlInfo += $" (Name: {ctrl.Name ?? "null"})";
                                if (descendant is ContentControl contentCtrl && contentCtrl.Content != null)
                                {
                                    controlInfo += $" Content: '{contentCtrl.Content}'";
                                }
                            }
                            Console.WriteLine(controlInfo);
                        }
                        */
                        var node = NodeInfo.GetOrCreateNode<ElementNode, Control>(popupRoot);
                        if (node != null && node.IsValid())
                        {
                            roots.Add(node);
                        }
                    }
                }
                return roots;
            });
            
            return windows.Concat(popupRoots);
        }

        return [];
    }
}
