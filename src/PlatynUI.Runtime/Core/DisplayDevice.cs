// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

namespace PlatynUI.Runtime.Core;

public interface IDisplayDevice
{
    Rect GetBoundingRectangle();
    void HighlightRect(double x, double y, double width, double height, double time);
}
