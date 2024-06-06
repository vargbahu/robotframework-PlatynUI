namespace PlatynUI.Ui.Technology.UIAutomation.Core
{
    using System.Windows;
    using PlatynUI.Technology.UiAutomation.Client;

    public static class UiAutomationElementExtensions
    {
        public static bool TryGetCurrentPattern<T>(this IUIAutomationElement element, out T? pattern)
            where T : class
        {
            return Automation.TryGetCurrentPattern(element, out pattern);
        }

        public static T? GetCurrentPattern<T>(this IUIAutomationElement element)
            where T : class
        {
            return Automation.GetCurrentPattern<T>(element);
        }

        public static bool TryGetCachedPattern<T>(this IUIAutomationElement element, out T? pattern)
            where T : class
        {
            return Automation.TryGetCachedPattern(element, out pattern);
        }

        public static T? GetCachedPattern<T>(this IUIAutomationElement element)
            where T : class
        {
            return Automation.GetCachedPattern<T>(element);
        }

        public static bool SupportsPatternId(this IUIAutomationElement element, int id)
        {
            return Automation.SupportsPatternId(element, id);
        }

        public static bool SupportsPattern<T>(this IUIAutomationElement element)
            where T : class
        {
            return Automation.SupportsPattern<T>(element);
        }

        public static string[] GetSupportedPatternNames(this IUIAutomationElement element)
        {
            return Automation.GetSupportedPatternNames(element);
        }

        public static int[] GetSupportedPatternIds(this IUIAutomationElement element)
        {
            return Automation.GetSupportedPatternIds(element);
        }

        public static int[] GetSupportedPropertyIds(this IUIAutomationElement element)
        {
            return Automation.GetSupportedPropertyIds(element);
        }

        public static string[] GetSupportedPropertyNames(this IUIAutomationElement element)
        {
            return Automation.GetSupportedPropertyNames(element);
        }

        public static bool IsVirtualized(this IUIAutomationElement element)
        {
            return element.SupportsPatternId(UIA_PatternIds.UIA_VirtualizedItemPatternId);
        }

        public static bool IsItemContainer(this IUIAutomationElement element)
        {
            return element.SupportsPatternId(UIA_PatternIds.UIA_ItemContainerPatternId);
        }

        public static IUIAutomationElement? Realize(this IUIAutomationElement element)
        {
            if (element != null && element.TryGetCurrentPattern(out IUIAutomationVirtualizedItemPattern? pattern))
            {
                pattern?.Realize();
            }

            return element;
        }

        public static IUIAutomationElement? GetCurrentParent(this IUIAutomationElement element)
        {
            var result = Automation.RawViewWalker.GetParentElement(element);

            if (result != null && Automation.CompareElements(result, Automation.RootElement))
            {
                return null;
            }

            return result;
        }
    }
}
