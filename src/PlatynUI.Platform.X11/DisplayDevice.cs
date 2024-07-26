// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Platform.X11;

[Export(typeof(IDisplayDevice))]
[method: ImportingConstructor]
public class DisplayDevice() : IDisplayDevice
{
    public Rect GetBoundingRectangle()
    {
        var size = XCBConnection.Instance.ScreenSize;
        return new Rect(0, 0, size.Width, size.Height);
    }

    public void HighlightRect(double x, double y, double width, double height, double time)
    {
        // TODO
    }
}
