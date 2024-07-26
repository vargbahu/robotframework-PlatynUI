# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

import threading
from typing import Optional

from ...core import adapterfactory
from ...ui.core import ui_technology
from ...ui.core.devices import DisplayDevice, KeyboardDevice, MouseDevice
from ...ui.core.window_manager import WindowManager

__all__ = ["get_technology"]


class Technology(ui_technology.UiTechnology):
    @property
    def adapter_factory(self) -> adapterfactory.AdapterFactory:
        raise NotImplementedError

    @property
    def mouse_device(self) -> MouseDevice:
        raise NotImplementedError

    @property
    def keyboard_device(self) -> KeyboardDevice:
        raise NotImplementedError

    @property
    def display_device(self) -> DisplayDevice:
        raise NotImplementedError

    @property
    def window_manager(self) -> WindowManager:
        raise NotImplementedError


__instance: Optional[Technology] = None
__lock = threading.Lock()


def get_technology() -> Technology:
    global __instance
    with __lock:
        if __instance is None:
            __instance = Technology()
        return __instance
