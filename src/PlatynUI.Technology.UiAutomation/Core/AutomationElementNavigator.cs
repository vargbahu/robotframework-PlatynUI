namespace PlatynUI.Ui.Technology.UIAutomation.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using PlatynUI.Technology.UiAutomation.Client;

internal class AutomationElementNavigator : ChildrenNavigatorBase<IUIAutomationElement, IUIAutomationElement>
{
    public readonly AutomationElementNavigator? ParentNavigator;

    private IReadOnlyList<IUIAutomationElement>? _children;
    private bool _findVirtual;
    private IUIAutomationTreeWalker? _walker;

    private AutomationElementNavigator()
        : base(null) { }

    public AutomationElementNavigator(
        AutomationElementNavigator? parentNavigator,
        IUIAutomationTreeWalker? walker,
        bool findVirtual
    )
        : base(null)
    {
        ParentNavigator = parentNavigator ?? throw new ArgumentNullException(nameof(parentNavigator));

        _walker = walker ?? throw new ArgumentNullException(nameof(walker));
        _findVirtual = findVirtual;

        Parent = parentNavigator.Current;
    }

    public AutomationElementNavigator(IUIAutomationElement? element, IUIAutomationTreeWalker? walker, bool findVirtual)
        : base(null)
    {
        _walker = walker ?? throw new ArgumentNullException(nameof(walker));

        _findVirtual = findVirtual;

        Element = element ?? throw new ArgumentNullException(nameof(element));
        Parent = _walker.GetParentElement(Element);

        Init();
    }

    public IUIAutomationElement? Element { get; private set; }

    protected override IReadOnlyList<IUIAutomationElement> Children => _children ??= GetChildren();

    private void Init()
    {
        if (_children != null || Element == null)
        {
            return;
        }

        CurrentIndex = -1;
        var children = Children;

        for (var i = 0; i < children.Count; i++)
        {
            if (Automation.CompareElements(Element, children[i]))
            {
                CurrentIndex = i;
                break;
            }
        }

        if (CurrentIndex < 0)
        {
            CurrentIndex = 0;
        }
        else
        {
            IsStarted = true;
        }
    }

    private IReadOnlyList<IUIAutomationElement> GetChildren()
    {
        if (Parent == null)
        {
            return Element != null ? new List<IUIAutomationElement> { Element } : new List<IUIAutomationElement>();
        }

        if (_findVirtual && Parent.SupportsPatternId(UIA_PatternIds.UIA_ItemContainerPatternId))
        {
            return EnumerateVirtualizedChildren(Parent).ToList();
        }

        return EnumerateChildren(Parent).ToList();
    }

    private IEnumerable<IUIAutomationElement> EnumerateChildren(IUIAutomationElement parent)
    {
        if (_walker == null)
        {
            yield break;
        }

        var r = _walker.GetFirstChildElement(parent);
        if (r == null)
        {
            yield break;
        }

        r.Realize();
        yield return r;

        while (r != null)
        {
            r = _walker.GetNextSiblingElement(r);
            if (r != null)
            {
                r.Realize();
                yield return r;
            }
        }
    }

    private static IEnumerable<IUIAutomationElement> EnumerateVirtualizedChildren(IUIAutomationElement parent)
    {
        var collapsed = false;
        if (parent.TryGetCurrentPattern(out IUIAutomationExpandCollapsePattern? expandCollapsePattern))
        {
            if (expandCollapsePattern?.CurrentExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
            {
                collapsed = true;
            }
        }

        var pattern = parent.GetCurrentPattern<IUIAutomationItemContainerPattern>();
        if (pattern == null)
        {
            yield break;
        }

        var r = pattern.FindItemByProperty(null, 0, null);
        if (r == null)
        {
            yield break;
        }

        r.Realize();
        yield return r;

        while (r != null)
        {
            r = pattern.FindItemByProperty(r, 0, null);
            if (r != null)
            {
                r.Realize();
                yield return r;
            }
        }

        if (!collapsed)
        {
            yield break;
        }

        if (expandCollapsePattern?.CurrentExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Expanded)
        {
            expandCollapsePattern.Collapse();
        }
    }

    public override ChildrenNavigatorBase<IUIAutomationElement, IUIAutomationElement> Clone()
    {
        return new AutomationElementNavigator
        {
            _findVirtual = _findVirtual,
            _walker = _walker,
            Element = Element,
            Parent = Parent,
            _children = _children,
            CurrentIndex = CurrentIndex,
            IsStarted = IsStarted
        };
    }
}
