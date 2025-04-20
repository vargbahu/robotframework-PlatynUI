using PlatynUI.JsonRpc;
using PlatynUI.Runtime;

namespace PlatynUI.Server.Services;

[JsonRpcEndpoint("displayDevice")]
interface IDisplay
{
    [JsonRpcRequest("getBoundingRectangle")]
    public Rect GetBoundingRectangle();

    [JsonRpcRequest("highlightRect")]
    public void HighlightRect(double x, double y, double width, double height, double time = 3);
}

partial class DisplayDeviceService : IDisplay
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
