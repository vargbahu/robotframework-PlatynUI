import os
from pathlib import Path


def main() -> None:
    exe_name = "PlatynUI.Technology.UiAutomation.Spy.exe"
    debug_path = (
        Path(__file__).parent
        / f"../../../../PlatynUI.Technology.UiAutomation.Spy/bin/Debug/net8.0-windows/{exe_name}"
        # Path(__file__).parent
        # / f"../../../../PlatynUI.Technology.UiAutomation.Spy/bin/Debug/net481/{ASSEMBLY_NAME}"
    )
    runtime_path = Path(__file__).parent.parent / f"runtime/{exe_name}"

    if debug_path.exists():
        os.startfile(str(debug_path))
    elif runtime_path.exists():
        os.startfile(str(runtime_path))
    else:
        print(f"Can't find the executable {exe_name} in the debug or runtime folder.")


if __name__ == "__main__":
    main()
