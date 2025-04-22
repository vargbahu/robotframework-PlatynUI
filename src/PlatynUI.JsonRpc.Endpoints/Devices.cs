using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

namespace PlatynUI.JsonRpc.Endpoints;

[JsonRpcEndpoint("displayDevice")]
public partial interface IDisplayDeviceEndpoint
{
    [JsonRpcRequest("getBoundingRectangle")]
    public Rect GetBoundingRectangle();

    [JsonRpcRequest("highlightRect")]
    public void HighlightRect(double x, double y, double width, double height, double time = 3);
}

[JsonRpcEndpoint("mouseDevice")]
public partial interface IMouseDeviceEndpoint
{
    [JsonRpcRequest("getDoubleClickTime")]
    double GetDoubleClickTime();

    [JsonRpcRequest("getDoubleClickSize")]
    Size GetDoubleClickSize();

    [JsonRpcRequest("getPosition")]
    Point GetPosition();

    [JsonRpcRequest("move")]
    void Move(double x, double y, bool direct = false, int maxDurationMs = 500);

    [JsonRpcRequest("press")]
    void Press(MouseButton button);

    [JsonRpcRequest("release")]
    void Release(MouseButton button);
}
