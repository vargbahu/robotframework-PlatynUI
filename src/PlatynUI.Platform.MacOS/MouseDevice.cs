using System.ComponentModel.Composition;

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
        throw new NotImplementedException();
    }

    public void Move(double x, double y)
    {
        throw new NotImplementedException();
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
