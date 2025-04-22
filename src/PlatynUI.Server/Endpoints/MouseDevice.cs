using PlatynUI.JsonRpc.Endpoints;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Server.Endpoints;

partial class MouseDeviceEndpoint : IMouseDeviceEndpoint
{
    public Size GetDoubleClickSize()
    {
        return MouseDevice.GetDoubleClickSize();
    }

    public double GetDoubleClickTime()
    {
        return MouseDevice.GetDoubleClickTime();
    }

    public Point GetPosition()
    {
        return MouseDevice.GetPosition();
    }

    public void Move(double x, double y)
    {
        MouseDevice.Move(x, y);
    }

    public void Press(MouseButton button)
    {
        MouseDevice.Press(button);
    }

    public void Release(MouseButton button)
    {
        MouseDevice.Release(button);
    }
}
