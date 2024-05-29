import time
from abc import *
from enum import Enum

from ....core.strategybase import StrategyBase

__all__ = ["BaseKeyboardDevice", "BaseKeyCode", "InputType"]


class BaseKeyCode:
    def __init__(self, key: any, valid: bool, error_text: str = None):
        self.key = key
        self.valid = valid
        self.error_text = error_text


class InputType(Enum):
    TYPE = 1
    PRESS = 2
    RELEASE = 3


class BaseKeyboardDevice(StrategyBase):
    strategy_name = "org.platynui.devices.Keyboard"

    def start_input(self, input_type: InputType):
        pass

    def end_input(self):
        pass

    def delay(self, seconds: float):
        time.sleep(seconds)

    @abstractmethod
    def key_to_keycode(self, key) -> BaseKeyCode:
        pass

    @abstractmethod
    def send_keycode(self, keycode: BaseKeyCode, pressed: bool) -> bool:
        pass
