using System.Diagnostics;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PlatynUI.Provider.Core;

namespace PlatynUI.Provider.Avalonia;

internal class ElementNode : Node<Control>
{
    public override NodeType NodeType => NodeType.Element;

    public override string LocalName => Element?.GetType().Name ?? "Unknown";

    private Dictionary<string, Func<object?>>? _attributes = null;

    protected Dictionary<string, Func<object?>> Attributes => _attributes ??= GetAttributes();

    private static Rect GetBounds(Control control)
    {
        var root = control.GetVisualRoot();

        if (root is not Visual rootVisual)
            return default;

        var transform = control.TransformToVisual(rootVisual);

        if (!transform.HasValue)
            return default;

        return new Rect(control.Bounds.Size).TransformToAABB(transform.Value);
    }

    private Dictionary<string, Func<object?>> GetAttributes()
    {
        var result = new Dictionary<string, Func<object?>>
        {
            ["Name"] = () => Element?.Name,
            ["AutomationId"] = () => Dispatcher.UIThread.Invoke(() => AutomationPeer?.GetAutomationId()),
            ["ClassName"] = () => Element?.GetType().ToString() ?? "",
            ["BoundingRectangle"] = () =>
            {
                var pr = Dispatcher.UIThread.Invoke<Rect?>(() =>
                {
                    if (Element == null || AutomationPeer == null)
                    {
                        return null;
                    }

                    var r = GetBounds(Element);

                    var root = Element.GetVisualRoot();

                    if (root == null)
                    {
                        return null;
                    }

                    return new PixelRect(root.PointToScreen(r.TopLeft), root.PointToScreen(r.BottomRight)).ToRect(1);
                });

                if (!pr.HasValue)
                    return null;

                return new double[] { pr.Value.X, pr.Value.Y, pr.Value.Width, pr.Value.Height };
            },
            ["IsEnabled"] = () => Dispatcher.UIThread.Invoke(() => Element?.IsEnabled) ?? false,
            ["IsVisible"] = () => Dispatcher.UIThread.Invoke(() => Element?.IsVisible) ?? false,
            ["IsFocused"] = () => Dispatcher.UIThread.Invoke(() => Element?.IsFocused) ?? false,

            ["IsHitTestVisible"] = () => Dispatcher.UIThread.Invoke(() => Element?.IsHitTestVisible) ?? false,
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
