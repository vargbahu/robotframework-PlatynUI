from abc import *

from ...core import StrategyBase

__all__ = ["Value"]


class Value(StrategyBase):
    strategy_name = "org.platynui.strategies.Value"

    @property
    @abstractmethod
    def value(self) -> any:
        pass
