// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Runtime;

public class Display
{
    [Import]
    protected IDisplayDevice? displayDevice;

    private Display()
    {
        PlatynUiExtensions.ComposeParts(this);
    }

    private static Display? _instance;
    public static Display Instance => _instance ??= new Display();

    public static Rect GetBoundingRectangle()
    {
        return Instance.displayDevice?.GetBoundingRectangle() ?? new Rect();
    }

    public static void HighlightRect(Rect rect, double time)
    {
        Instance.displayDevice?.HighlightRect(rect.X, rect.Y, rect.Width, rect.Height, time);
    }

    public static void HighlightRect(double x, double y, double width, double height, double time)
    {
        Instance.displayDevice?.HighlightRect(x, y, width, height, time);
    }
}
