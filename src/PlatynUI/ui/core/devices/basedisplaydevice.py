from abc import abstractmethod
from typing import ClassVar

from ....core.strategybase import StrategyBase
from ....core.types import Rect

__all__ = ["BaseDisplayDevice"]


class BaseDisplayDevice(StrategyBase):
    strategy_name: ClassVar[str] = "org.platynui.devices.Display"

    @property
    @abstractmethod
    def name(self) -> str: ...

    @property
    @abstractmethod
    def bounding_rectangle(self) -> Rect: ...

    @abstractmethod
    def highlight_rect(self, rect: Rect, timeout: int): ...

    @abstractmethod
    def get_screenshot(self, rect: Rect, format="png", quality=-1) -> bytearray: ...
