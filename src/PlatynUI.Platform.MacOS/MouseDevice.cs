using System.ComponentModel.Composition;

using PlatynUI.Platform.MacOS.SwiftInterop;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Platform.MacOS;

[Export(typeof(IMouseDevice))]
public class MouseDevice() : IMouseDevice
{
    public Size GetDoubleClickSize()
    {
        throw new NotImplementedException();
    }

    public double GetDoubleClickTime()
    {
        throw new NotImplementedException();
    }

    public Point GetPosition()
    {
        Interop.GetMousePosition(out var x, out var y);
        return new Point(x, y);
    }

    public void Move(double x, double y)
    {
        Interop.MouseMove(x, y);
    }

    public void Press(MouseButton button)
    {
        throw new NotImplementedException();
    }

    public void Release(MouseButton button)
    {
        throw new NotImplementedException();
    }
}
