from typing import ClassVar, Protocol, runtime_checkable

from ....core.strategybase import StrategyBase
from ....core.types import Rect

__all__ = ["BaseDisplayDevice"]


@runtime_checkable
class BaseDisplayDevice(StrategyBase, Protocol):
    strategy_name: ClassVar[str] = "org.platynui.devices.Display"

    @property
    def name(self) -> str: ...

    @property
    def bounding_rectangle(self) -> Rect: ...

    def highlight_rect(self, rect: Rect, timeout: int): ...

    def get_screenshot(self, rect: Rect, format="png", quality=-1) -> bytearray: ...
