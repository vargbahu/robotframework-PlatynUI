using PlatynUI.Platform.X11;
using PlatynUI.Platform.X11.Interop.XCB;
using PlatynUI.Runtime;
using static PlatynUI.Platform.X11.Interop.XCB.XCB;

unsafe
{
    Point p;
    Point oldP = new(-1, -1);

    while (true)
    {
        var cookie = xcb_query_pointer(XCBConnection.Default, XCBConnection.Default.RootWindow);
        var reply = xcb_query_pointer_reply(XCBConnection.Default, cookie, null);
        p = new Point(reply->root_x, reply->root_y);
        if (p != oldP)
        {
            Console.WriteLine($"{p}");

            Display.HighlightRect(p.X - 1, p.Y - 1, 2, 2);
        }
        oldP = p;
    }
}
//Thread.Sleep(20000);
