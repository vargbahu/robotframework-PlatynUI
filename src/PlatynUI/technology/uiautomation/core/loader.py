from pathlib import Path
from typing import TYPE_CHECKING, Optional, cast

__all__ = ["DotNetInterface"]


ASSEMBLY_NAME = "PlatynUI.Technology.UiAutomation.dll"
DEBUG_ASSEMBLY_PATH = (
    Path(__file__).parent / f"../../../../PlatynUI.Technology.UiAutomation/bin/Debug/net8.0-windows/{ASSEMBLY_NAME}"
)
RUNTIME_ASSEMBLY_PATH = Path(__file__).parent / f"runtime/{ASSEMBLY_NAME}"


if TYPE_CHECKING:
    from PlatynUI.Technology.UiAutomation import Adapter, Finder, KeyboardDevice, MouseDevice, Patterns


class DotNetInterface:
    _adapter: Optional["Adapter"] = None
    _finder: Optional["Finder"] = None
    _mouse_device: Optional["MouseDevice"] = None
    _keyboard_device: Optional["KeyboardDevice"] = None
    _patterns: Optional["Patterns"] = None

    __loaded = False

    @classmethod
    def _ensure_loaded(cls) -> None:

        if cls.__loaded:
            return

        cls.__loaded = True

        import clr

        if DEBUG_ASSEMBLY_PATH.exists():
            clr.AddReference(str(DEBUG_ASSEMBLY_PATH))
        elif RUNTIME_ASSEMBLY_PATH.exists():
            clr.AddReference(str(RUNTIME_ASSEMBLY_PATH))

    @classmethod
    def adapter(cls) -> "Adapter":
        if cls._adapter is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Adapter  # type: ignore

            cls._adapter = cast("Adapter", Adapter)

        return cls._adapter

    @classmethod
    def finder(cls) -> "Finder":
        if cls._finder is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Finder  # type: ignore

            cls._finder = cast("Finder", Finder)

        return cls._finder

    @classmethod
    def mouse_device(cls) -> "MouseDevice":
        if cls._mouse_device is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import MouseDevice  # type: ignore

            cls._mouse_device = cast("MouseDevice", MouseDevice)

        return cls._mouse_device

    @classmethod
    def keyboard_device(cls) -> "KeyboardDevice":
        if cls._keyboard_device is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import KeyboardDevice  # type: ignore

            cls._keyboard_device = cast("KeyboardDevice", KeyboardDevice)

        return cls._keyboard_device

    @classmethod
    def patterns(cls) -> "Patterns":
        if cls._patterns is None:
            cls._ensure_loaded()

            from PlatynUI.Technology.UiAutomation import Patterns  # type: ignore

            cls._patterns = cast("Patterns", Patterns)

        return cls._patterns
