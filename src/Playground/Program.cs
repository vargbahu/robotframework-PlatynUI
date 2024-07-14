using PlatynUI.Technology.UiAutomation;

//Console.OutputEncoding = System.Text.Encoding.Unicode;

var result = Finder.Evaluate(null, "/Window/text()");

// var result = Finder.Evaluate(null, "1+2");

foreach (var item in result)
{
    Console.WriteLine($"{item} {item?.GetType()}");
}
