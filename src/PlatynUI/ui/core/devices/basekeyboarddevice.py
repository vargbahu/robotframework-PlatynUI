import time
from abc import abstractmethod
from enum import Enum
from typing import Any, Optional, Union

from ....core.strategybase import StrategyBase

__all__ = ["BaseKeyboardDevice", "BaseKeyCode", "InputType"]

Key = Union[str, Any]


class BaseKeyCode:
    def __init__(self, key: Any, valid: bool, error_text: Optional[str] = None):
        self.key = key
        self.valid = valid
        self.error_text = error_text


class InputType(Enum):
    TYPE = 1
    PRESS = 2
    RELEASE = 3


class BaseKeyboardDevice(StrategyBase):
    strategy_name = "org.platynui.devices.Keyboard"

    def start_input(self, input_type: InputType) -> None:
        pass

    def end_input(self) -> None:
        pass

    def delay(self, seconds: float) -> None:
        time.sleep(seconds)

    @abstractmethod
    def key_to_keycode(self, key: Key) -> BaseKeyCode:
        pass

    @abstractmethod
    def send_keycode(self, keycode: BaseKeyCode, pressed: bool) -> bool:
        pass
