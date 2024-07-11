from pathlib import Path
from typing import TYPE_CHECKING, Optional, cast

__all__ = ["DotNetInterface"]


# pyright: reportMissingModuleSource=false

if TYPE_CHECKING:
    from PlatynUI.Technology.UiAutomation import (
        Adapter,
        DisplayDevice,
        Finder,
        KeyboardDevice,
        MouseDevice,
        Patterns,
    )


class DotNetInterface:
    _adapter: Optional["Adapter"] = None
    _finder: Optional["Finder"] = None
    _mouse_device: Optional["MouseDevice"] = None
    _keyboard_device: Optional["KeyboardDevice"] = None
    _display_device: Optional["DisplayDevice"] = None
    _patterns: Optional["Patterns"] = None

    __loaded = False

    @classmethod
    def _ensure_loaded(cls) -> None:
        assembly_name = "PlatynUI.Technology.UiAutomation.dll"
        debug_assembly_path = (
            Path(__file__).parent
            / f"../../../../PlatynUI.Technology.UiAutomation/bin/Debug/net8.0-windows/{assembly_name}"
            # Path(__file__).parent
            # / f"../../../../PlatynUI.Technology.UiAutomation/bin/Debug/net481/{ASSEMBLY_NAME}"
        )
        runtime_assembly_path = Path(__file__).parent.parent / f"runtime/{assembly_name}"

        if cls.__loaded:
            return

        cls.__loaded = True

        import clr

        if debug_assembly_path.exists():
            clr.AddReference(str(debug_assembly_path))
        elif runtime_assembly_path.exists():
            clr.AddReference(str(runtime_assembly_path))

    @classmethod
    def adapter(cls) -> "Adapter":
        if cls._adapter is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Adapter

            cls._adapter = cast("Adapter", Adapter)

        return cls._adapter

    @classmethod
    def finder(cls) -> "Finder":
        if cls._finder is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Finder

            cls._finder = cast("Finder", Finder)

        return cls._finder

    @classmethod
    def mouse_device(cls) -> "MouseDevice":
        if cls._mouse_device is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import MouseDevice

            cls._mouse_device = cast("MouseDevice", MouseDevice)

        return cls._mouse_device

    @classmethod
    def keyboard_device(cls) -> "KeyboardDevice":
        if cls._keyboard_device is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import KeyboardDevice

            cls._keyboard_device = cast("KeyboardDevice", KeyboardDevice)

        return cls._keyboard_device

    @classmethod
    def display_device(cls) -> "DisplayDevice":
        if cls._display_device is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import DisplayDevice

            cls._display_device = cast("DisplayDevice", DisplayDevice)

        return cls._display_device

    @classmethod
    def patterns(cls) -> "Patterns":
        if cls._patterns is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Patterns

            cls._patterns = cast("Patterns", Patterns)

        return cls._patterns
