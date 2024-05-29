from abc import *

from ...core import StrategyBase

__all__ = ["Editable"]


class Editable(StrategyBase):
    strategy_name = "org.platynui.strategies.Editable"

    @property
    @abstractmethod
    def is_editable(self) -> bool: pass
