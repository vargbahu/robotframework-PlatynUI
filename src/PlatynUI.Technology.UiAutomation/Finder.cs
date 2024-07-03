namespace PlatynUI.Technology.UiAutomation;

using System.Xml;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UIAutomation.Core;

public static class Finder
{
    static readonly NameTable nameTable;
    static readonly XmlNamespaceManager nsmgr;

    static Finder()
    {
        nameTable = new NameTable();
        nsmgr = new XmlNamespaceManager(nameTable);
        nsmgr.AddNamespace("native", "http://platynui.io/native");
        nsmgr.AddNamespace("element", "http://platynui.io/element");
        nsmgr.AddNamespace("item", "http://platynui.io/item");
    }

    public static IUIAutomationElement? FindSingleElement(
        IUIAutomationElement? parent,
        string xpath,
        bool findVirtual = false
    )
    {
        var navigator = new UiaXPathNavigator(parent, findVirtual);
        var root = navigator.SelectSingleNode(xpath, nsmgr);

        return root?.UnderlyingObject as IUIAutomationElement;
    }
}
