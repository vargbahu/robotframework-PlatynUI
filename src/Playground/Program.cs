using PlatynUI.Technology.UiAutomation;
using PlatynUI.Technology.UiAutomation.Core;

//Console.OutputEncoding = System.Text.Encoding.Unicode;

//var result = Finder.FindAllElements(null, "Application[@Name='Calculator']");
//var result = Finder.FindAllElements(null, "//*[@Name='Start']");
var result = Finder.FindAllElements(null, "/app:Application");

Console.WriteLine(result);

// var result = Finder.Evaluate(null, "1+2");

foreach (var item in result)
{
    Console.WriteLine($"{item.GetDisplayName()} {item?.GetType()}");
}
