using PlatynUI.JsonRpc.Endpoints;
using PlatynUI.Runtime;

namespace PlatynUI.Server.Services;

partial class DisplayDeviceServiceEndPoint : IDisplayDeviceEndpoint
{
    public Rect GetBoundingRectangle()
    {
        return DisplayDevice.GetBoundingRectangle();
    }

    public void HighlightRect(double x, double y, double width, double height, double time = 3)
    {
        DisplayDevice.HighlightRect(x, y, width, height, time);
    }
}
