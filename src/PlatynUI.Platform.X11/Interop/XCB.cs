using System.Runtime.InteropServices;

namespace PlatynUI.Platform.X11.Interop.XCB;

public static unsafe partial class XCB
{
    // TODO can we call this on every linux?
    [DllImport(
        "libclang",
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "clang_free",
        ExactSpelling = true
    )]
    public static extern void free(void* buffer);
}
