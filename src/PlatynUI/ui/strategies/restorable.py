from abc import *

from ...core import StrategyBase

__all__ = ["Restorable"]


class Restorable(StrategyBase):
    strategy_name = "org.platynui.strategies.Restorable"

    @abstractmethod
    def restore(self):
        pass
