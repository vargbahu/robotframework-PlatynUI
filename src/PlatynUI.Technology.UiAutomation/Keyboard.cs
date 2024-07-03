namespace PlatynUI.Technology.UiAutomation;

using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

public class VirtualKey
{
    internal static readonly IDictionary<string, VirtualKey> Keys = new Dictionary<string, VirtualKey>(
        StringComparer.OrdinalIgnoreCase
    );

    /// <summary>
    ///     The back key
    /// </summary>
    public static readonly VirtualKey Back = new(VIRTUAL_KEY.VK_BACK);

    /// <summary>
    ///     The tab key
    /// </summary>
    public static readonly VirtualKey Tab = new(VIRTUAL_KEY.VK_TAB);

    /// <summary>
    ///     The clear key
    /// </summary>
    public static readonly VirtualKey Clear = new(VIRTUAL_KEY.VK_CLEAR);

    /// <summary>
    ///     The return key
    /// </summary>
    public static readonly VirtualKey Return = new(VIRTUAL_KEY.VK_RETURN);

    /// <summary>
    ///     The enter key
    /// </summary>
    public static readonly VirtualKey Enter = new(VIRTUAL_KEY.VK_RETURN, true);

    /// <summary>
    ///     The shift key
    /// </summary>
    public static readonly VirtualKey Shift = new(VIRTUAL_KEY.VK_SHIFT);

    /// <summary>
    ///     The control key
    /// </summary>
    public static readonly VirtualKey Control = new(VIRTUAL_KEY.VK_CONTROL);

    /// <summary>
    ///     The control key
    /// </summary>
    public static readonly VirtualKey Ctrl = new(VIRTUAL_KEY.VK_CONTROL);

    /// <summary>
    ///     The menu key
    /// </summary>
    public static readonly VirtualKey Menu = new(VIRTUAL_KEY.VK_MENU);

    /// <summary>
    ///     The alt key
    /// </summary>
    public static readonly VirtualKey Alt = new(Menu);

    /// <summary>
    ///     The alt gr key
    /// </summary>
    public static readonly VirtualKey AltGr = new(Control, Menu);

    /// <summary>
    ///     The pause key
    /// </summary>
    public static readonly VirtualKey Pause = new(VIRTUAL_KEY.VK_PAUSE);

    /// <summary>
    ///     The capital key
    /// </summary>
    public static readonly VirtualKey Capital = new(VIRTUAL_KEY.VK_CAPITAL);

    // TODO Keycodes
    //    KANA = 0x15,
    //    HANGEUL = 0x15,  // old name - should be here for compatibility
    //    HANGUL = 0x15,
    //    JUNJA = 0x17,
    //    FINAL = 0x18,
    //    HANJA = 0x19,
    //    KANJI = 0x19,

    /// <summary>
    ///     The escape key
    /// </summary>
    public static readonly VirtualKey Escape = new(VIRTUAL_KEY.VK_ESCAPE);

    // TODO Keycodes
    //    CONVERT = 0x1C,
    //    NONCONVERT = 0x1D,
    //    ACCEPT = 0x1E,
    //    MODECHANGE = 0x1F

    /// <summary>
    ///     The space key
    /// </summary>
    public static readonly VirtualKey Space = new(VIRTUAL_KEY.VK_SPACE);

    /// <summary>
    ///     The prior key
    /// </summary>
    public static readonly VirtualKey Prior = new(VIRTUAL_KEY.VK_PRIOR, true);

    /// <summary>
    ///     The next key
    /// </summary>
    public static readonly VirtualKey Next = new(VIRTUAL_KEY.VK_NEXT, true);

    /// <summary>
    ///     The page up key
    /// </summary>
    public static readonly VirtualKey PageUp = Prior;

    /// <summary>
    ///     The page down key
    /// </summary>
    public static readonly VirtualKey PageDown = Next;

    /// <summary>
    ///     The end key
    /// </summary>
    public static readonly VirtualKey End = new(VIRTUAL_KEY.VK_END, true);

    /// <summary>
    ///     The home key
    /// </summary>
    public static readonly VirtualKey Home = new(VIRTUAL_KEY.VK_HOME, true);

    /// <summary>
    ///     The left key
    /// </summary>
    public static readonly VirtualKey Left = new(VIRTUAL_KEY.VK_LEFT, true);

    /// <summary>
    ///     Up key
    /// </summary>
    public static readonly VirtualKey Up = new(VIRTUAL_KEY.VK_UP, true);

    /// <summary>
    ///     The right key
    /// </summary>
    public static readonly VirtualKey Right = new(VIRTUAL_KEY.VK_RIGHT, true);

    /// <summary>
    ///     The Down key
    /// </summary>
    public static readonly VirtualKey Down = new(VIRTUAL_KEY.VK_DOWN, true);

    /// <summary>
    ///     The select key
    /// </summary>
    public static readonly VirtualKey Select = new(VIRTUAL_KEY.VK_SELECT);

    /// <summary>
    ///     The print key
    /// </summary>
    public static readonly VirtualKey Print = new(VIRTUAL_KEY.VK_PRINT);

    /// <summary>
    ///     The execute key
    /// </summary>
    public static readonly VirtualKey Execute = new(VIRTUAL_KEY.VK_EXECUTE);

    /// <summary>
    ///     The snapshot key
    /// </summary>
    public static readonly VirtualKey Snapshot = new(VIRTUAL_KEY.VK_SNAPSHOT);

    /// <summary>
    ///     The insert key
    /// </summary>
    public static readonly VirtualKey Insert = new(VIRTUAL_KEY.VK_INSERT, true);

    /// <summary>
    ///     The delete key
    /// </summary>
    public static readonly VirtualKey Delete = new(VIRTUAL_KEY.VK_DELETE, true);

    /// <summary>
    ///     The help key
    /// </summary>
    public static readonly VirtualKey Help = new(VIRTUAL_KEY.VK_HELP);

    /// <summary>
    ///     The lwin key
    /// </summary>
    public static readonly VirtualKey Lwin = new(VIRTUAL_KEY.VK_LWIN);

    /// <summary>
    ///     The rwin key
    /// </summary>
    public static readonly VirtualKey Rwin = new(VIRTUAL_KEY.VK_RWIN);

    /// <summary>
    ///     The apps key
    /// </summary>
    public static readonly VirtualKey Apps = new(VIRTUAL_KEY.VK_APPS);

    /// <summary>
    ///     The sleep key
    /// </summary>
    public static readonly VirtualKey Sleep = new(VIRTUAL_KEY.VK_SLEEP);

    /// <summary>
    ///     The numpad0 key
    /// </summary>
    public static readonly VirtualKey Numpad0 = new(VIRTUAL_KEY.VK_NUMPAD0);

    /// <summary>
    ///     The numpad1 key
    /// </summary>
    public static readonly VirtualKey Numpad1 = new(VIRTUAL_KEY.VK_NUMPAD1);

    /// <summary>
    ///     The numpad2 key
    /// </summary>
    public static readonly VirtualKey Numpad2 = new(VIRTUAL_KEY.VK_NUMPAD2);

    /// <summary>
    ///     The numpad3 key
    /// </summary>
    public static readonly VirtualKey Numpad3 = new(VIRTUAL_KEY.VK_NUMPAD3);

    /// <summary>
    ///     The numpad4 key
    /// </summary>
    public static readonly VirtualKey Numpad4 = new(VIRTUAL_KEY.VK_NUMPAD4);

    /// <summary>
    ///     The numpad5 key
    /// </summary>
    public static readonly VirtualKey Numpad5 = new(VIRTUAL_KEY.VK_NUMPAD5);

    /// <summary>
    ///     The numpad6 key
    /// </summary>
    public static readonly VirtualKey Numpad6 = new(VIRTUAL_KEY.VK_NUMPAD6);

    /// <summary>
    ///     The numpad7 key
    /// </summary>
    public static readonly VirtualKey Numpad7 = new(VIRTUAL_KEY.VK_NUMPAD7);

    /// <summary>
    ///     The numpad8 key
    /// </summary>
    public static readonly VirtualKey Numpad8 = new(VIRTUAL_KEY.VK_NUMPAD8);

    /// <summary>
    ///     The numpad9 key
    /// </summary>
    public static readonly VirtualKey Numpad9 = new(VIRTUAL_KEY.VK_NUMPAD9);

    /// <summary>
    ///     The multiply key
    /// </summary>
    public static readonly VirtualKey Multiply = new(VIRTUAL_KEY.VK_MULTIPLY);

    /// <summary>
    ///     The add key
    /// </summary>
    public static readonly VirtualKey Add = new(VIRTUAL_KEY.VK_ADD);

    /// <summary>
    ///     The separator key
    /// </summary>
    public static readonly VirtualKey Separator = new(VIRTUAL_KEY.VK_SEPARATOR);

    /// <summary>
    ///     The subtract key
    /// </summary>
    public static readonly VirtualKey Subtract = new(VIRTUAL_KEY.VK_SUBTRACT);

    /// <summary>
    ///     The decimal key
    /// </summary>
    public static readonly VirtualKey Decimal = new(VIRTUAL_KEY.VK_DECIMAL);

    /// <summary>
    ///     The divide key
    /// </summary>
    public static readonly VirtualKey Divide = new(VIRTUAL_KEY.VK_DIVIDE);

    /// <summary>
    ///     The f1 key
    /// </summary>
    public static readonly VirtualKey F1 = new(VIRTUAL_KEY.VK_F1);

    /// <summary>
    ///     The f2 key
    /// </summary>
    public static readonly VirtualKey F2 = new(VIRTUAL_KEY.VK_F2);

    /// <summary>
    ///     The f3 key
    /// </summary>
    public static readonly VirtualKey F3 = new(VIRTUAL_KEY.VK_F3);

    /// <summary>
    ///     The f4 key
    /// </summary>
    public static readonly VirtualKey F4 = new(VIRTUAL_KEY.VK_F4);

    /// <summary>
    ///     The f5 key
    /// </summary>
    public static readonly VirtualKey F5 = new(VIRTUAL_KEY.VK_F5);

    /// <summary>
    ///     The f6 key
    /// </summary>
    public static readonly VirtualKey F6 = new(VIRTUAL_KEY.VK_F6);

    /// <summary>
    ///     The f7 key
    /// </summary>
    public static readonly VirtualKey F7 = new(VIRTUAL_KEY.VK_F7);

    /// <summary>
    ///     The f8 key
    /// </summary>
    public static readonly VirtualKey F8 = new(VIRTUAL_KEY.VK_F8);

    /// <summary>
    ///     The f9 key
    /// </summary>
    public static readonly VirtualKey F9 = new(VIRTUAL_KEY.VK_F9);

    /// <summary>
    ///     The F10 key
    /// </summary>
    public static readonly VirtualKey F10 = new(VIRTUAL_KEY.VK_F10);

    /// <summary>
    ///     The F11 key
    /// </summary>
    public static readonly VirtualKey F11 = new(VIRTUAL_KEY.VK_F11);

    /// <summary>
    ///     The F12 key
    /// </summary>
    public static readonly VirtualKey F12 = new(VIRTUAL_KEY.VK_F12);

    /// <summary>
    ///     The F13 key
    /// </summary>
    public static readonly VirtualKey F13 = new(VIRTUAL_KEY.VK_F13);

    /// <summary>
    ///     The F14 key
    /// </summary>
    public static readonly VirtualKey F14 = new(VIRTUAL_KEY.VK_F14);

    /// <summary>
    ///     The F15 key
    /// </summary>
    public static readonly VirtualKey F15 = new(VIRTUAL_KEY.VK_F15);

    /// <summary>
    ///     The F16 key
    /// </summary>
    public static readonly VirtualKey F16 = new(VIRTUAL_KEY.VK_F16);

    /// <summary>
    ///     The F17 key
    /// </summary>
    public static readonly VirtualKey F17 = new(VIRTUAL_KEY.VK_F17);

    /// <summary>
    ///     The F18 key
    /// </summary>
    public static readonly VirtualKey F18 = new(VIRTUAL_KEY.VK_F18);

    /// <summary>
    ///     The F19
    /// </summary>
    public static readonly VirtualKey F19 = new(VIRTUAL_KEY.VK_F19);

    /// <summary>
    ///     The F20 key
    /// </summary>
    public static readonly VirtualKey F20 = new(VIRTUAL_KEY.VK_F20);

    /// <summary>
    ///     The F21 key
    /// </summary>
    public static readonly VirtualKey F21 = new(VIRTUAL_KEY.VK_F21);

    /// <summary>
    ///     The F22 key
    /// </summary>
    public static readonly VirtualKey F22 = new(VIRTUAL_KEY.VK_F22);

    /// <summary>
    ///     The F23 key
    /// </summary>
    public static readonly VirtualKey F23 = new(VIRTUAL_KEY.VK_F23);

    /// <summary>
    ///     The F24 key
    /// </summary>
    public static readonly VirtualKey F24 = new(VIRTUAL_KEY.VK_F24);

    /// <summary>
    ///     The numlock key
    /// </summary>
    public static readonly VirtualKey Numlock = new(VIRTUAL_KEY.VK_NUMLOCK);

    /// <summary>
    ///     The scroll key
    /// </summary>
    public static readonly VirtualKey Scroll = new(VIRTUAL_KEY.VK_SCROLL);

    /// <summary>
    ///     The lshift key
    /// </summary>
    public static readonly VirtualKey Lshift = new(VIRTUAL_KEY.VK_LSHIFT);

    /// <summary>
    ///     The rshift key
    /// </summary>
    public static readonly VirtualKey Rshift = new(VIRTUAL_KEY.VK_RSHIFT);

    /// <summary>
    ///     The lcontrol key
    /// </summary>
    public static readonly VirtualKey Lcontrol = new(VIRTUAL_KEY.VK_LCONTROL);

    /// <summary>
    ///     The rcontrol key
    /// </summary>
    public static readonly VirtualKey Rcontrol = new(VIRTUAL_KEY.VK_RCONTROL);

    /// <summary>
    ///     The lmenu key
    /// </summary>
    public static readonly VirtualKey Lmenu = new(VIRTUAL_KEY.VK_LMENU);

    /// <summary>
    ///     The rmenu key
    /// </summary>
    public static readonly VirtualKey Rmenu = new(VIRTUAL_KEY.VK_RMENU);

    /// <summary>
    ///     The browser back key
    /// </summary>
    public static readonly VirtualKey BrowserBack = new(VIRTUAL_KEY.VK_BROWSER_BACK);

    /// <summary>
    ///     The browser forward key
    /// </summary>
    public static readonly VirtualKey BrowserForward = new(VIRTUAL_KEY.VK_BROWSER_FORWARD);

    /// <summary>
    ///     The browser refresh key
    /// </summary>
    public static readonly VirtualKey BrowserRefresh = new(VIRTUAL_KEY.VK_BROWSER_REFRESH);

    /// <summary>
    ///     The browser stop key
    /// </summary>
    public static readonly VirtualKey BrowserStop = new(VIRTUAL_KEY.VK_BROWSER_STOP);

    /// <summary>
    ///     The browser search key
    /// </summary>
    public static readonly VirtualKey BrowserSearch = new(VIRTUAL_KEY.VK_BROWSER_SEARCH);

    /// <summary>
    ///     The browser favorites key
    /// </summary>
    public static readonly VirtualKey BrowserFavorites = new(VIRTUAL_KEY.VK_BROWSER_FAVORITES);

    /// <summary>
    ///     The browser home key
    /// </summary>
    public static readonly VirtualKey BrowserHome = new(VIRTUAL_KEY.VK_BROWSER_HOME);

    /// <summary>
    ///     The volume mute key
    /// </summary>
    public static readonly VirtualKey VolumeMute = new(VIRTUAL_KEY.VK_VOLUME_MUTE);

    /// <summary>
    ///     The volume down key
    /// </summary>
    public static readonly VirtualKey VolumeDown = new(VIRTUAL_KEY.VK_VOLUME_DOWN);

    /// <summary>
    ///     The volume up key
    /// </summary>
    public static readonly VirtualKey VolumeUp = new(VIRTUAL_KEY.VK_VOLUME_UP);

    /// <summary>
    ///     The media next track key
    /// </summary>
    public static readonly VirtualKey MediaNextTrack = new(VIRTUAL_KEY.VK_MEDIA_NEXT_TRACK);

    /// <summary>
    ///     The media previous track key
    /// </summary>
    public static readonly VirtualKey MediaPrevTrack = new(VIRTUAL_KEY.VK_MEDIA_PREV_TRACK);

    /// <summary>
    ///     The media stop key
    /// </summary>
    public static readonly VirtualKey MediaStop = new(VIRTUAL_KEY.VK_MEDIA_STOP);

    /// <summary>
    ///     The media play pause key
    /// </summary>
    public static readonly VirtualKey MediaPlayPause = new(VIRTUAL_KEY.VK_MEDIA_PLAY_PAUSE);

    /// <summary>
    ///     The launch mail key
    /// </summary>
    public static readonly VirtualKey LaunchMail = new(VIRTUAL_KEY.VK_LAUNCH_MAIL);

    /// <summary>
    ///     The launch media select key
    /// </summary>
    public static readonly VirtualKey LaunchMediaSelect = new(VIRTUAL_KEY.VK_LAUNCH_MEDIA_SELECT);

    /// <summary>
    ///     The launch app1 key
    /// </summary>
    public static readonly VirtualKey LaunchApp1 = new(VIRTUAL_KEY.VK_LAUNCH_APP1);

    /// <summary>
    ///     The launch app2 key
    /// </summary>
    public static readonly VirtualKey LaunchApp2 = new(VIRTUAL_KEY.VK_LAUNCH_APP2);

    /* TODO Keycodes
       OEM_1 = 0xBA,   // ';:' for US
       OEM_PLUS = 0xBB,   // '+' any country
       OEM_COMMA = 0xBC,   // ',' any country
       OEM_MINUS = 0xBD,   // '-' any country
       OEM_PERIOD = 0xBE,   // '.' any country
       OEM_2 = 0xBF,   // '/?' for US
       OEM_3 = 0xC0,   // '`~' for US
       //
       // 0xC1 - 0xD7 : reserved
       //
       //
       // 0xD8 - 0xDA : unassigned
       //
       OEM_4 = 0xDB,  //  '[{' for US
       OEM_5 = 0xDC,  //  '\|' for US
       OEM_6 = 0xDD,  //  ']}' for US
       OEM_7 = 0xDE,  //  ''"' for US
       OEM_8 = 0xDF
   */

    static VirtualKey()
    {
        foreach (
            var fieldInfo in typeof(VirtualKey)
                .GetFields()
                .Where(fieldInfo => fieldInfo.IsStatic && fieldInfo.FieldType == typeof(VirtualKey))
        )
        {
            Keys.Add(fieldInfo.Name, fieldInfo.GetValue(null) as VirtualKey ?? throw new InvalidOperationException());
        }
    }

    internal VirtualKey(VIRTUAL_KEY vk, bool extended = false)
    {
        Value = [(ushort)vk];
        Extended = [extended];
    }

    internal VirtualKey(params VirtualKey[] value)
    {
        Value = value.SelectMany(v => v.Value).ToArray();
        Extended = value.SelectMany(v => v.Extended).ToArray();
    }

    internal VirtualKey(params ushort[] value)
    {
        Value = value;
        Extended = new bool[value.Length];
    }

    internal ushort[] Value { get; set; }

    internal bool[] Extended { get; set; }

    public override string ToString()
    {
        return string.Join(" + ", Value.Select((v, i) => (Extended[i] ? "Extended " : "") + v));
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode() ^ Extended.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is VirtualKey vk)
        {
            return Value.SequenceEqual(vk.Value) && Extended.SequenceEqual(vk.Extended);
        }
        return base.Equals(obj);
    }
}

public class Keycode(object? key, object? code, bool valid, string? errorText)
{
    public object? Key { get; } = key;
    public object? Code { get; } = code;
    public bool Valid { get; } = valid;
    public string? ErrorText { get; } = errorText;

    public override string ToString()
    {
        return $"{Key} - {Code} - {Valid} - {ErrorText}";
    }
}

public static class KeyboardDevice
{
    private static VirtualKey ToVirtualKeyCodes(char c)
    {
        var result = new List<ushort>();
        if (c == '\n')
        {
            return VirtualKey.Return;
        }

        var key = PInvoke.VkKeyScan(c);

        if (key == -1)
        {
            return new VirtualKey(c);
        }

        // SHIFT
        if ((key & 0x0100) != 0)
        {
            result.AddRange(VirtualKey.Shift.Value);
        }

        // SHIFT
        if ((key & 0x0200) != 0)
        {
            result.AddRange(VirtualKey.Control.Value);
        }

        // ALT
        if ((key & 0x0400) != 0)
        {
            result.AddRange(VirtualKey.Menu.Value);
        }

        // TODO: HANAKU
        /*
            if ((key & 0x0800) != 0) {
            result.add(VirtualKey.getValue());
            }
        *
        */

        result.Add((ushort)(key & 0xff));
        return new VirtualKey(result.ToArray());
    }

    public static Keycode KeyToKeyCode(object? key)
    {
        if (key == null)
            return new Keycode(key, null, false, "Key is null");

        if (key is string keystr)
        {
            if (VirtualKey.Keys.TryGetValue(keystr, out var virtualKey))
                return new Keycode(key, virtualKey, true, null);
            if (keystr.Length == 1)
            {
                return new Keycode(key, ToVirtualKeyCodes(keystr[0]), true, null);
            }
        }

        return new Keycode(key, null, false, "Unknown key");
    }

    public static bool SendKeyCode(object keyCode, bool pressed)
    {
        if (keyCode is VirtualKey virtualKey)
        {
            var keys = virtualKey.Value.Select((x, i) => new { Value = x, Extended = virtualKey.Extended[i] });
            if (!pressed)
            {
                keys = keys.Reverse();
            }
            foreach (var it in keys)
            {
                var inputs = new INPUT
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous =
                    {
                        ki =
                        {
                            wVk = (VIRTUAL_KEY)it.Value,
                            wScan = 0,
                            dwFlags =
                                (it.Extended ? KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY : 0)
                                | (pressed ? 0 : KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP),
                        }
                    }
                };

                PInvoke.SendInput(new Span<INPUT>(ref inputs), Marshal.SizeOf(inputs));
            }

            return true;
        }

        return false;
    }
}
