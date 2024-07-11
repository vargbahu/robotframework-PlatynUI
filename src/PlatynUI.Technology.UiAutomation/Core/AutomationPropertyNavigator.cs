namespace PlatynUI.Technology.UiAutomation.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using PlatynUI.Technology.UiAutomation.Client;

internal class AutomationPropertyNavigator
    : ChildrenNavigatorBase<AutomationElementNavigator, Automation.PropertyIdAndName>
{
    private IReadOnlyList<Automation.PropertyIdAndName>? _children;

    public AutomationPropertyNavigator(AutomationElementNavigator parent)
        : base(parent)
    {
        Element = parent?.Current ?? throw new ArgumentNullException(nameof(parent));
    }

    public IUIAutomationElement? Element { get; }

    protected override IReadOnlyList<Automation.PropertyIdAndName> Children => _children ??= GetChildren();

    private List<Automation.PropertyIdAndName> GetChildren()
    {
        return Element != null
            ?
            [
                new Automation.PropertyIdAndName(-1, "Role"),
                new Automation.PropertyIdAndName(-2, "ProcessName"),
                .. Automation.GetSupportedPropertyIdAndNames(Element)
            ]
            : [];
    }

    public override ChildrenNavigatorBase<AutomationElementNavigator, Automation.PropertyIdAndName> Clone()
    {
        return new AutomationPropertyNavigator(Parent!)
        {
            _children = _children,
            CurrentIndex = CurrentIndex,
            IsStarted = IsStarted
        };
    }
}
