from abc import *

from ...core import StrategyBase

__all__ = ["HasExpanded", "Expandable"]


class HasExpanded(StrategyBase):
    strategy_name = "org.platynui.strategies.HasExpanded"

    @property
    @abstractmethod
    def can_expand(self) -> bool:
        pass

    @property
    @abstractmethod
    def is_expanded(self) -> bool:
        pass


class Expandable(HasExpanded):
    strategy_name = "org.platynui.strategies.Expandable"

    @abstractmethod
    def expand(self):
        pass

    @abstractmethod
    def collapse(self):
        pass
