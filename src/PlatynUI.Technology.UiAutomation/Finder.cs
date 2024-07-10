using System.Xml;
using System.Xml.XPath;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UIAutomation.Core;

namespace PlatynUI.Technology.UiAutomation;

public static class Finder
{
    static readonly UiaXsltContext xsltContext = new();

    static Finder()
    {
        xsltContext.AddNamespace("native", "http://platynui.io/native");
        xsltContext.AddNamespace("element", "http://platynui.io/element");
        xsltContext.AddNamespace("item", "http://platynui.io/item");
    }

    public static IUIAutomationElement? FindSingleElement(
        IUIAutomationElement? parent,
        string xpath,
        bool findVirtual = false
    )
    {
        var navigator = new UiaXPathNavigator(parent, findVirtual);
        var expression = XPathExpression.Compile(xpath, xsltContext);

        var root = navigator.SelectSingleNode(expression);

        return root?.UnderlyingObject as IUIAutomationElement;
    }
}
