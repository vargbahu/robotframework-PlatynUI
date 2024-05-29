from typing import Dict

from automatum.core import context
from automatum.ui.desktopbase import *

from .locator import locator
from .strategies import Environment, SystemInfo

__all__ = ["Desktop"]


@locator(path='/.')
@context
class Desktop(DesktopBase):
    def __init__(self, display_name: str = None, host: str = None):
        super().__init__()

        if display_name is not None:
            self.locator.display_name = display_name

        if host is not None:
            self.locator.host = host

    @property
    def environment(self) -> Dict[str, str]:
        return self.adapter.get_strategy(Environment).environment

    def set_environment_variable(self, variable: str, value: str):
        self.adapter.get_strategy(Environment).set_variable(variable, value)

    def get_environment_variable(self, variable: str, default: str = None) -> str:
        strategy = self.adapter.get_strategy(Environment)
        if strategy.variable_exists(variable):
            return self.adapter.get_strategy(Environment).get_variable(variable)

        return default

    def environment_exists(self, variable: str) -> bool:
        return self.adapter.get_strategy(Environment).variable_exists(variable)

    def system_info(self) -> SystemInfo:
        return self.adapter.get_strategy(SystemInfo)
