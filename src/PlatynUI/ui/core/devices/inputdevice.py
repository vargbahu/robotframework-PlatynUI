import abc
import time

from ....core.settings import Settings

__all__ = ["InputDevice"]


class InputDevice(metaclass=abc.ABCMeta):
    __after_input_delay = None

    @property
    def after_input_delay(self):
        if self.__after_input_delay is None:
            return Settings.current().input_after_input_delay
        return self.__after_input_delay

    @after_input_delay.setter
    def after_input_delay(self, value: float):
        self.__after_input_delay = value

    def delay(self, seconds: float):
        time.sleep(seconds)
