using System.Diagnostics;
using PlatynUI.Technology.UiAutomation.Client;

namespace PlatynUI.Technology.UiAutomation.Core;

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

    private List<IUIAutomationElement> GetChildren()
    {
        if (Parent == null)
        {
            return Element != null ? [Element] : [];
        }

        return Parent.EnumerateChildren(_walker, _findVirtual).ToList();
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
