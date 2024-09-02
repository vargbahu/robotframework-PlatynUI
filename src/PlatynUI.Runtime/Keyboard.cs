// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Runtime;

public class Keyboard
{
    [Import]
    protected IKeyboardDevice? keyboardDevice;

    private Keyboard()
    {
        PlatynUiExtensions.ComposeParts(this);
    }

    private static Keyboard? _instance;
    private static Keyboard Instance => _instance ??= new Keyboard();

    public static Keycode KeyToKeyCode(object? key)
    {
        return Instance.keyboardDevice?.KeyToKeyCode(key) ?? new Keycode(null, null, false, "No keyboard device found");
    }

    public static bool SendKeyCode(object keyCode, bool pressed)
    {
        return Instance.keyboardDevice?.SendKeyCode(keyCode, pressed) ?? false;
    }
}
