from abc import ABCMeta, abstractmethod
from typing import Any, List, Optional, Set, Type, TypeVar, overload

from .strategybase import StrategyBase
from .technology import Technology

__all__ = ["Adapter", "TStrategyBase"]

TStrategyBase = TypeVar("TStrategyBase", bound=StrategyBase)


class Adapter(StrategyBase, metaclass=ABCMeta):
    """
    the base adapter
    """

    strategy_name = "io.platynui.Adapter"

    @property
    @abstractmethod
    def valid(self) -> bool:
        """
        get true if the adapter is valid at this time
        :return:
        """

    @property
    @abstractmethod
    def id(self) -> str:
        pass

    @property
    @abstractmethod
    def name(self) -> str:
        pass

    @property
    @abstractmethod
    def class_name(self) -> str:
        pass

    @property
    def tag_name(self) -> str:
        return ""

    @property
    @abstractmethod
    def role(self) -> str:
        pass

    @property
    @abstractmethod
    def supported_roles(self) -> Set[str]:
        pass

    @property
    @abstractmethod
    def type(self) -> str:
        pass

    @property
    @abstractmethod
    def supported_types(self) -> Set[str]:
        pass

    @property
    @abstractmethod
    def framework_id(self) -> str:
        pass

    @property
    @abstractmethod
    def runtime_id(self) -> str:
        pass

    @property
    @abstractmethod
    def supported_strategies(self) -> Set[str]:
        pass

    @overload
    @abstractmethod
    def get_strategy(self, strategy_type: Type[TStrategyBase]) -> TStrategyBase: ...

    @overload
    @abstractmethod
    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool) -> Optional[TStrategyBase]: ...

    @abstractmethod
    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool = True) -> Optional[TStrategyBase]:
        pass

    def supports_strategy(self, strategy_type: Type[TStrategyBase]) -> bool:
        return self.get_strategy(strategy_type, False) is not None

    @property
    @abstractmethod
    def technology(self) -> Technology:
        pass

    @property
    @abstractmethod
    def parent(self) -> Optional["Adapter"]:
        pass

    @property
    @abstractmethod
    def children(self) -> List["Adapter"]:
        pass

    def __eq__(self, other: Any) -> bool:
        if isinstance(other, Adapter):
            return self.runtime_id == other.runtime_id

        raise NotImplementedError
