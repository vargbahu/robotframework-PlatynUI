using System.Collections.ObjectModel;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UiAutomation.Core;

namespace PlatynUI.Technology.UiAutomation.Spy.ElementsModel;

public class UiaElement : ElementBase
{
    public UiaElement(ElementBase? parent, IUIAutomationElement automationElement)
        : base(parent)
    {
        AutomationElement = automationElement;
        DisplayName =
            $"{Automation.ControlTypeNameFromId(automationElement.CurrentControlType)} \"{automationElement.CurrentName}\"";
    }

    public IUIAutomationElement AutomationElement { get; set; }

    private IEnumerable<IUIAutomationElement> EnumerateChildren(
        IUIAutomationElement parent,
        IUIAutomationTreeWalker walker
    )
    {
        var r = walker.GetFirstChildElement(parent);
        if (r == null)
        {
            yield break;
        }

        if (r.CurrentProcessId == System.Diagnostics.Process.GetCurrentProcess().Id)
        {
            System.Diagnostics.Debug.WriteLine("Skipping element from the same process");
        }
        else
        {
            yield return r.Realize();
        }

        while (r != null)
        {
            r = walker.GetNextSiblingElement(r);
            if (r != null)
            {
                if (r.CurrentProcessId == System.Diagnostics.Process.GetCurrentProcess().Id)
                {
                    System.Diagnostics.Debug.WriteLine("Skipping element from the same process");
                    continue;
                }
                yield return r.Realize();
            }
        }
    }

    protected override ObservableCollection<ElementBase> InitChildren()
    {
        var result = base.InitChildren();

        foreach (var c in EnumerateChildren(AutomationElement, Automation.RawViewWalker))
        {
            if (OldChildren != null)
            {
                var old = OldChildren.FirstOrDefault(e =>
                    e is UiaElement uiaElement && Automation.CompareElements(uiaElement.AutomationElement, c)
                );
                if (old != null)
                {
                    if (old is UiaElement oldElement)
                    {
                        oldElement.AutomationElement = c;
                        oldElement.Refresh();
                    }
                    result.Add(old);
                    continue;
                }
            }
            result.Add(new UiaElement(this, c));
        }

        return result;
    }

    protected static object ConvertPropertyValue(string name, object value)
    {
        switch (name)
        {
            case "RuntimeId":
                return string.Join("-", ((int[])value).Select(id => id.ToString("X")));
            case "BoundingRectangle":
                if (value is double[] rect)
                {
                    return $"({rect[0]}, {rect[1]}) - ({rect[2]}, {rect[3]})";
                }
                break;
            case "ClickablePoint":
                if (value is double[] point)
                {
                    return $"({point[0]}, {point[1]})";
                }
                break;
        }
        return value;
    }

    protected override ObservableCollection<PropertyEntry> InitProperties()
    {
        var result = base.InitProperties();
        result.Insert(
            0,
            new PropertyEntry
            {
                Name = "Role",
                Value = Automation.ControlTypeNameFromId(AutomationElement.CurrentControlType)
            }
        );

        foreach (var n in Automation.GetSupportedPropertyIdAndNames(AutomationElement))
        {
            try
            {
                var v = AutomationElement.GetCurrentPropertyValueEx(n.Id, 1);
                if (Automation.UiAutomation.CheckNotSupported(v) != 0)
                {
                    continue;
                }
                if (v == Automation.NotSupportedValue)
                {
                    continue;
                }
                result.Add(new PropertyEntry { Name = n.Name, Value = ConvertPropertyValue(n.Name, v) });
                if (n.Id == UIA_PropertyIds.UIA_ProcessIdPropertyId)
                {
                    try
                    {
                        var processName = System.Diagnostics.Process.GetProcessById((int)v).ProcessName;
                        result.Add(new PropertyEntry { Name = "ProcessName", Value = processName });
                    }
                    catch
                    {
                        result.Add(new PropertyEntry { Name = "ProcessName", Value = "<Unknown>" });
                    }
                }
            }
            catch
            {
                continue;
            }
        }

        return new ObservableCollection<PropertyEntry>(result.OrderBy(x => x.Name));
    }
}
