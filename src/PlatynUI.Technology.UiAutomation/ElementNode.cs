// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UiAutomation.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using IAttribute = PlatynUI.Runtime.Core.IAttribute;
using INode = PlatynUI.Runtime.Core.INode;

namespace PlatynUI.Technology.UiAutomation;

public class ElementNode(INode? parent, IUIAutomationElement element) : INode, IElement
{
    public INode? Parent { get; } = parent;
    public IUIAutomationElement Element { get; } = element;

    List<INode>? _children = null;
    public IList<INode> Children => _children ??= GetChildren();

    private List<INode> GetChildren()
    {
        var result = new List<INode>();
        foreach (var e in Element.EnumerateChildren(Automation.RawViewWalker, true))
        {
            result.Add(new ElementNode(this, e));
        }
        return result;
    }

    public string LocalName => Element.GetCurrentControlTypeName();

    public string NamespaceURI => Namespaces.Raw;

    Dictionary<string, IAttribute>? _attributes;
    public IDictionary<string, IAttribute> Attributes => _attributes ??= GetAttributes();

    public bool IsEnabled => throw new NotImplementedException();

    public bool IsVisible => throw new NotImplementedException();

    public bool IsInView => throw new NotImplementedException();

    public bool TopLevelParentIsActive => throw new NotImplementedException();

    public Rect BoundingRectangle => Element.CurrentBoundingRectangle.ToRect();

    public Rect VisibleRectangle => throw new NotImplementedException();

    public Point DefaultClickPosition => throw new NotImplementedException();

    static readonly object InvalidValue = new();

    private Dictionary<string, IAttribute> GetAttributes()
    {
        var result = new Dictionary<string, IAttribute>();
        Automation.PropertyIdAndName[] props =
        [
            new Automation.PropertyIdAndName(-1, "Role"),
            new Automation.PropertyIdAndName(-2, "ProcessName"),
            new Automation.PropertyIdAndName(-3, "Technology"),
            new Automation.PropertyIdAndName(-4, "IsTopLevel"),
            .. Automation.GetSupportedPropertyIdAndNames(Element)
        ];

        result = props
            .Select(x => new Runtime.Core.Attribute(x.Name, () => GetPropertyValue(x)) as IAttribute)
            .Where(x => x.Value != InvalidValue)
            .OrderBy(x => x.Name)
            .ToDictionary(x => x.Name);

        return result;
    }

    private object? GetPropertyValue(Automation.PropertyIdAndName attribute)
    {
        try
        {
            if (attribute.Id == -1)
            {
                if (attribute.Name == "Role")
                    return Element.GetCurrentControlTypeName();

                return null;
            }
            else if (attribute.Id == -2)
            {
                if (attribute.Name == "ProcessName")
                {
                    return System.Diagnostics.Process.GetProcessById(Element.CurrentProcessId).ProcessName;
                }

                return null;
            }
            else if (attribute.Id == -3)
            {
                if (attribute.Name == "Technology")
                {
                    return "UIAutomation";
                }

                return null;
            }
            else if (attribute.Id == -4)
            {
                if (attribute.Name == "IsTopLevel")
                {
                    if (Element.CurrentNativeWindowHandle == IntPtr.Zero)
                    {
                        return false;
                    }

                    return (HWND)Element.CurrentNativeWindowHandle
                        == PInvoke.GetAncestor((HWND)Element.CurrentNativeWindowHandle, GET_ANCESTOR_FLAGS.GA_ROOT);
                }

                return null;
            }

            var value = Element.GetCurrentPropertyValueEx(attribute.Id, 1);
            if (Automation.UiAutomation.CheckNotSupported(value) != 0)
            {
                return InvalidValue;
            }
            return value;
        }
        catch (Exception e)
        {
            return $"<Error>: {e.Message}";
        }
    }

    public INode Clone()
    {
        return new ElementNode(Parent, Element);
    }

    public bool IsSamePosition(INode other)
    {
        return other is ElementNode node && Automation.CompareElements(Element, node.Element);
    }

    public void Refresh()
    {
        _children = null;
    }
}
