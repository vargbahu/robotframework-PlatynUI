// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using PlatynUI.Runtime.Core;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UiAutomation.Core;
using IAttribute = PlatynUI.Runtime.Core.IAttribute;
using INode = PlatynUI.Runtime.Core.INode;

namespace PlatynUI.Technology.UiAutomation;

public class UiAutomationNode(INode? parent, IUIAutomationElement element) : INode
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
            result.Add(new UiAutomationNode(this, e));
        }
        return result;
    }

    public string LocalName => Element.GetCurrentControlTypeName();

    public string NamespaceURI => Namespaces.Raw;

    Dictionary<string, IAttribute>? _attributes;
    public IDictionary<string, IAttribute> Attributes => _attributes ??= GetAttributes();

    static readonly object InvalidValue = new();

    private Dictionary<string, IAttribute> GetAttributes()
    {
        var result = new Dictionary<string, IAttribute>();
        Automation.PropertyIdAndName[] props =
        [
            new Automation.PropertyIdAndName(-1, "Role"),
            new Automation.PropertyIdAndName(-2, "ProcessName"),
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
        return new UiAutomationNode(Parent, Element);
    }

    public bool IsSamePosition(INode other)
    {
        return other is UiAutomationNode node && Automation.CompareElements(Element, node.Element);
    }

    public void Refresh()
    {
        _children = null;
    }
}
