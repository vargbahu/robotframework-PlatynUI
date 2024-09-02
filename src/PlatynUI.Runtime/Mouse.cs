// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Runtime;

public class Mouse
{
    [Import]
    protected IMouseDevice? mouseDevice;

    private Mouse()
    {
        PlatynUiExtensions.ComposeParts(this);
    }

    private static Mouse? _instance;
    private static Mouse Instance => _instance ??= new Mouse();

    public static double GetDoubleClickTime()
    {
        return Instance.mouseDevice?.GetDoubleClickTime() ?? 0;
    }

    public static Size GetDoubleClickSize()
    {
        return Instance.mouseDevice?.GetDoubleClickSize() ?? new Size();
    }

    public static Point GetPosition()
    {
        return Instance.mouseDevice?.GetPosition() ?? new Point();
    }

    public static void Move(double x, double y)
    {
        Instance.mouseDevice?.Move(x, y);
    }

    public static void Press(MouseButton button)
    {
        Instance.mouseDevice?.Press(button);
    }

    public static void Release(MouseButton button)
    {
        Instance.mouseDevice?.Release(button);
    }
}
