using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Ui.Technology.UIAutomation.Core;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PlatynUI.Technology.UiAutomation;

public static class Adapter
{
    public static string GetRole(IUIAutomationElement element)
    {
        return Automation.ControlTypeNameFromId(element.CurrentControlType);
    }

    public static string GetRuntimeId(IUIAutomationElement element)
    {
        return string.Join("-", element.GetRuntimeId().Select(id => id.ToString("X")));
    }

    public static bool IsReadOnly(IUIAutomationElement element)
    {
        if (element.TryGetCurrentPattern(out IUIAutomationValuePattern? pattern))
        {
            return pattern?.CurrentIsReadOnly != 0;
        }

        return false;
    }

    public static Point GetClickablePoint(IUIAutomationElement element)
    {
        if (element.GetClickablePoint(out var tp) != 0)
        {
            return new Point(tp.x, tp.y);
        }

        var r = element.CurrentBoundingRectangle;
        return new Point(r.left + (r.right - r.left) / 2, r.top + (r.bottom - r.top) / 2);
    }

    public static IUIAutomationElement? GetTopLevelParent(IUIAutomationElement? element)
    {
        if (element == null)
        {
            return null;
        }

        var parent = Automation.RawViewWalker.GetParentElement(element);

        if (parent == null)
        {
            return null;
        }

        if (Automation.CompareElements(parent, Automation.RootElement))
        {
            return element;
        }

        return GetTopLevelParent(parent);
    }

    public static bool TryEnsureTopLevelParentIsActive(IUIAutomationElement? element)
    {
        if (element == null)
        {
            return false;
        }

        var topLevelParent = GetTopLevelParent(element);
        if (topLevelParent == null)
        {
            return false;
        }

        var h = topLevelParent.CurrentNativeWindowHandle;
        if (h != IntPtr.Zero)
        {
            var topLevelElement = Automation.UiAutomation.ElementFromHandle(h);
            if (topLevelElement.TryGetCurrentPattern<IUIAutomationWindowPattern>(out var pattern))
            {
                if (
                    pattern != null
                    && pattern.CurrentWindowInteractionState
                        == WindowInteractionState.WindowInteractionState_BlockedByModalWindow
                )
                {
                    return false;
                }

                var foregroundWindow = PInvoke.GetForegroundWindow();
                if (h != foregroundWindow)
                {
                    var s = PInvoke.GetWindow(foregroundWindow, GET_WINDOW_CMD.GW_OWNER);
                    if (s == h)
                    {
                        var foregroundElement = Automation.UiAutomation.ElementFromHandle(foregroundWindow);
                        if (foregroundElement.TryGetCurrentPattern<IUIAutomationWindowPattern>(out var pattern1))
                        {
                            if (pattern1 != null && pattern1.CurrentIsModal != 0)
                            {
                                return false;
                            }
                        }
                    }

                    PInvoke.SwitchToThisWindow((HWND)h, false);

                    return true;
                }
            }
        }

        if (topLevelParent.CurrentIsKeyboardFocusable != 0 && topLevelParent.CurrentHasKeyboardFocus == 0)
        {
            topLevelParent.SetFocus();
            return true;
        }

        return false;
    }

    public static string[] GetSupportedPropertyNames(IUIAutomationElement element)
    {
        return Automation.GetSupportedPropertyNames(element);
    }

    public static object? GetPropertyValue(IUIAutomationElement element, string propertyName)
    {
        return Automation.GetPropertyValue(element, propertyName);
    }
}
