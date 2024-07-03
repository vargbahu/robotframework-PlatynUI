// using System.Xml;
// using PlatynUI.Technology.UiAutomation;
// using PlatynUI.Technology.UiAutomation.Client;
// using PlatynUI.Technology.UIAutomation.Core;

// Console.WriteLine("Hello, World!");

// var navigator = new UiaXPathNavigator();

// // var r = navigator.Select("//Button").OfType<UiaXPathNavigator>().Select(c=>c.UnderlyingObject).OfType<IUIAutomationElement>().ToList();

// // Console.WriteLine(r.Count);
// var nameTable = new NameTable();

// var nsmgr = new XmlNamespaceManager(nameTable);
// nsmgr.AddNamespace("native", "http://platynui.io/native");
// nsmgr.AddNamespace("element", "http://platynui.io/element");
// nsmgr.AddNamespace("item", "http://platynui.io/item");

// var root = navigator.SelectSingleNode("/Pane[@Name='Taskleiste']//Button[@Name='Start']", nsmgr);

// if (root?.UnderlyingObject is IUIAutomationElement element)
// {
//     Console.WriteLine(element.CurrentName);
//     Console.WriteLine(element.CurrentClassName);
//     Console.WriteLine(element.CurrentAutomationId);
//     Console.WriteLine(element.CurrentBoundingRectangle);
//     Console.WriteLine(element.CurrentFrameworkId);
//     var r = element.CurrentBoundingRectangle;
//     MouseDevice.Move(r.left + (r.right - r.left) / 2, r.top + (r.bottom - r.top) / 2);
//     MouseDevice.Press(MouseButton.Left);
//     MouseDevice.Release(MouseButton.Left);
//     Thread.Sleep(1000);
//     MouseDevice.Press(MouseButton.Left);
//     MouseDevice.Release(MouseButton.Left);
// }
Console.WriteLine("Hello, World!");