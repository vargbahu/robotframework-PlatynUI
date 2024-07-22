// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PlatynUI.Platform.Win32;

[Export(typeof(IDisplayDevice))]
[method: ImportingConstructor]
public class DisplayDevice() : IDisplayDevice
{
    public Rect GetBoundingRectangle()
    {
        return new Rect(
            PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_XVIRTUALSCREEN),
            PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_YVIRTUALSCREEN),
            PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXVIRTUALSCREEN),
            PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYVIRTUALSCREEN)
        );
    }

    private static readonly Highlighter _highlighter = new(true);

    public void HighlightRect(double x, double y, double width, double height, double time)
    {
        _highlighter.Show(new Rect(x, y, width, height), (int)time * 1000);
    }
}
