using PlatynUI.JsonRpc;
using PlatynUI.Runtime;

namespace PlatynUI.JsonRpc.Endpoints;

[JsonRpcEndpoint("displayDevice")]
public partial interface IDisplayDeviceEndpoint
{
    [JsonRpcRequest("getBoundingRectangle")]
    public Rect GetBoundingRectangle();

    [JsonRpcRequest("highlightRect")]
    public void HighlightRect(double x, double y, double width, double height, double time = 3);
}
