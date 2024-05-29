import os
import threading
from typing import Dict, Tuple

from PlatynUI.core import Adapter, AdapterFactory
from PlatynUI.ui.core import UiTechnology, WindowManager
from PlatynUI.ui.core.devices import (
    DefaultDisplayDevice,
    DefaultKeyboardDevice,
    DefaultMouseDevice,
    DisplayDevice,
    KeyboardDevice,
    MouseDevice,
)

# from .automatum.devices import *
# from .automatum.registry import Registry, get_registry

__all__ = ["UiaTechnology", "get_technology"]


class UiaTechnology(UiTechnology):
    pass



__instances = {}  # type: Dict[Tuple[str, str], QtTechnology]
__lock = threading.Lock()


def get_technology(display_name: str, host: str) -> QtTechnology:
    with __lock:
        if (display_name, host) not in __instances:
            __instances[(display_name, host)] = QtTechnology(display_name, host)
            return __instances[(display_name, host)]

        if __instances[(display_name, host)].registry.is_connected:
            return __instances[(display_name, host)]

        del __instances[(display_name, host)]
        __instances[(display_name, host)] = QtTechnology(display_name, host)

        return __instances[(display_name, host)]
