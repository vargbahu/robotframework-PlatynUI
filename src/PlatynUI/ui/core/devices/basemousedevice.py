from typing import Protocol, runtime_checkable

from ....core.strategybase import StrategyBase
from ....core.types import Point, Size
from .mousebutton import MouseButton

__all__ = ["BaseMouseDevice"]


@runtime_checkable
class BaseMouseDevice(StrategyBase, Protocol):
    strategy_name = "org.platynui.devices.Mouse"

    @property
    def double_click_time(self) -> float:
        pass

    @property
    def double_click_size(self) -> Size:
        pass

    def get_position(self) -> Point:
        pass

    def move_to(self, pos: Point):
        pass

    def press(self, button: MouseButton):
        pass

    def release(self, button: MouseButton):
        pass
