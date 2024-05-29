from abc import abstractmethod

from ...core import StrategyBase

__all__ = ["Activatable", "Deactivatable", "HasIsActive"]


class Activatable(StrategyBase):
    strategy_name = "org.platynui.strategies.Activatable"

    @abstractmethod
    def activate(self) -> None:
        pass


class Deactivatable(StrategyBase):
    strategy_name = "org.platynui.strategies.Deactivatable"

    @abstractmethod
    def deactivate(self) -> None:
        pass


class HasIsActive(StrategyBase):
    strategy_name = "org.platynui.strategies.HasIsActive"

    @property
    @abstractmethod
    def is_active(self) -> bool:
        pass
