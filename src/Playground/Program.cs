using PlatynUI.Runtime;

Display.HighlightRect(0, 0, 100, 200, 20);
var r = Display.GetBoundingRectangle();

var yInc = 1 * r.Height / r.Width;

for (double x = 0, y = 0; x < r.Width; x++, y+=yInc)
{
    Console.WriteLine($"{x}, {y}");
    Display.HighlightRect(x, y, 100, 200, 20);
    Thread.Sleep(2);
}

//Thread.Sleep(20000);
