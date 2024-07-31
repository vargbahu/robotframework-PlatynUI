using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PlatynUI.Provider.Core;

namespace PlatynUI.Provider.Avalonia;

public abstract class Node
{
    private ElementReference? _nodeReference = null;
    public ElementReference Reference
    {
        get => _nodeReference ?? throw new InvalidOperationException($"{nameof(Reference)} not set.");
        set => _nodeReference = value;
    }

    public abstract NodeType NodeType { get; }

    public abstract bool IsValid();
    public abstract string[] GetAttributeNames();
    public abstract object? GetAttributeValue(string attributeName);
    public abstract string GetAttributeValueType(string attributeName);
    public abstract string LocalName { get; }

    internal abstract IEnumerable<Node> GetChildren();
}

public abstract class Node<TElement> : Node
    where TElement : class
{
    public Node() { }

    public Node(ElementReference reference, TElement element)
    {
        Reference = reference;
        WeakElement = new(element);
    }

    private WeakReference<TElement>? _weakElement = null;
    public WeakReference<TElement> WeakElement
    {
        get => _weakElement ?? throw new InvalidOperationException($"{nameof(WeakElement)} not set.");
        set => _weakElement = value;
    }

    public TElement? Element => WeakElement.TryGetTarget(out var element) ? element : null;
}

public class NodeInfo : INodeInfoAsync
{
    protected static readonly ConditionalWeakTable<object, object> _nodes = [];

    public static TNode GetOrCreateNode<TNode>()
        where TNode : Node, new()
    {
        lock (_nodes)
        {
            var t = typeof(TNode);

            if (!_nodes.TryGetValue(t, out var node))
            {
                var reference = new ElementReference([80, 109, typeof(TNode).GetHashCode()]);

                node = new TNode() { Reference = reference };

                _nodes.Add(t, node);
            }

            return (TNode)node;
        }
    }

    public static TNode GetOrCreateNode<TNode, TElement>(TElement element)
        where TNode : Node<TElement>, new()
        where TElement : class
    {
        lock (_nodes)
        {
            if (!_nodes.TryGetValue(element, out var node))
            {
                var reference = new ElementReference([80, 108, element.GetHashCode()]);

                node = new TNode() { Reference = reference, WeakElement = new(element) };

                _nodes.Add(element, node);
            }

            return (TNode)node;
        }
    }

    public static Node? GetNodeFromReference(ElementReference reference)
    {
        return (Node)_nodes.FirstOrDefault(pair => ((Node)pair.Value).Reference == reference).Value;
    }

    public virtual Task<bool> IsValidAsync(ElementReference reference)
    {
        var node = GetNodeFromReference(reference);
        if (node is null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(node.IsValid());
    }

    public Task<IList<ElementReference>> GetChildrenAsync(ElementReference parentReference)
    {
        var node = GetNodeFromReference(parentReference);
        if (node is null)
        {
            return Task.FromResult<IList<ElementReference>>([]);
        }

        return Task.FromResult<IList<ElementReference>>(node.GetChildren().Select(v => v.Reference).ToList());
    }

    public Task<string[]> GetAttributeNamesAsync(ElementReference reference)
    {
        var node = GetNodeFromReference(reference);
        if (node == null || !node.IsValid())
        {
            return Task.FromResult<string[]>([]);
        }

        return Task.FromResult(node.GetAttributeNames());
    }

    public Task<object?> GetAttributeValueAsync(ElementReference reference, string attributeName)
    {
        var node = GetNodeFromReference(reference);
        if (node == null || !node.IsValid())
        {
            return Task.FromResult<object?>(null);
        }

        return Task.FromResult(node.GetAttributeValue(attributeName));
    }

    public Task<string> GetAttributeTypeAsync(ElementReference reference, string attributeName)
    {
        var node = GetNodeFromReference(reference);
        if (node == null || !node.IsValid())
        {
            return Task.FromResult("");
        }

        return Task.FromResult(node.GetAttributeValueType(attributeName));
    }

    public virtual Task<string> GetLocalNameAsync(ElementReference reference)
    {
        var node = GetNodeFromReference(reference);
        if (node == null || !node.IsValid())
        {
            return Task.FromResult("Unknown");
        }

        return Task.FromResult(node.LocalName);
    }

    public virtual Task<NodeType> GetNodeTypeAsync(ElementReference reference)
    {
        var node = GetNodeFromReference(reference);
        if (node == null || !node.IsValid())
        {
            return Task.FromResult(NodeType.Unknown);
        }

        return Task.FromResult(node.NodeType);
    }
}

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
            ["Name"] = () => Application.Current?.Name,
            ["RuntimeId"] = () => Reference.RuntimeId,
            ["ProcessId"] = () => Environment.ProcessId,
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
            return lifetime
                .Windows.Select(w => NodeInfo.GetOrCreateNode<ElementNode, Control>(w))
                .Where(n => n != null && n.IsValid());
        }

        return [];
    }
}

internal class ElementNode : Node<Control>
{
    public override NodeType NodeType => NodeType.Element;

    public override string LocalName => Element?.GetType().Name ?? "Unknown";

    private Dictionary<string, Func<object?>>? _attributes = null;

    protected Dictionary<string, Func<object?>> Attributes => _attributes ??= GetAttributes();

    private Dictionary<string, Func<object?>> GetAttributes()
    {
        var result = new Dictionary<string, Func<object?>>
        {
            ["Name"] = () => Element?.Name,
            ["AutomationId"] = () => Dispatcher.UIThread.Invoke(() => AutomationPeer?.GetAutomationId()),
            ["ClassName"] = () => Element?.GetType().ToString() ?? "",
            ["BoundingRectangle"] = () =>
            {
                if (Element == null || AutomationPeer == null)
                {
                    return null;
                }

                var pr = Dispatcher.UIThread.Invoke<Rect?>(() =>
                {
                    var r = AutomationPeer?.GetBoundingRectangle();
                    if (!r.HasValue)
                    {
                        return null;
                    }
                    return new PixelRect(
                        Element.PointToScreen(r.Value.TopLeft),
                        Element.PointToScreen(r.Value.BottomRight)
                    ).ToRect(1);
                });

                if (!pr.HasValue)
                    return null;

                return new double[] { pr.Value.X, pr.Value.Y, pr.Value.Width, pr.Value.Height };
            },
            ["IsEnabled"] = () => Element?.IsEnabled,
            ["IsVisible"] = () => Element?.IsVisible,
            ["IsFocused"] = () => Element?.IsFocused,

            ["IsHitTestVisible"] = () => Element?.IsHitTestVisible,
        };
        return result;
    }

    public AutomationPeer? _automationPeer = null;
    public AutomationPeer? AutomationPeer =>
        _automationPeer ??=
            Element != null
                ? Dispatcher.UIThread.Invoke(() => ControlAutomationPeer.CreatePeerForElement(Element))
                : null;

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

    public override bool IsValid()
    {
        return Element != null;
    }

    internal override IEnumerable<Node> GetChildren()
    {
        if (Element == null)
        {
            return [];
        }

        return Element
            .GetVisualChildren()
            .Cast<Control>()
            .Select(e => NodeInfo.GetOrCreateNode<ElementNode, Control>(e))
            .Where(n => n != null && n.IsValid());
    }
}
