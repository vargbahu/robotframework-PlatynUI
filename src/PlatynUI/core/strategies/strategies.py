from typing import Any, ClassVar, List, Protocol, runtime_checkable

from ..strategybase import StrategyBase

__all__ = ["Properties", "NativeProperties"]


@runtime_checkable
class Properties(StrategyBase, Protocol):
    strategy_name: ClassVar[str] = "io.platynui.strategies.Properties"

    def get_property_names(self) -> List[str]: ...

    def get_property_value(self, name: str) -> Any: ...


@runtime_checkable
class NativeProperties(StrategyBase, Protocol):
    strategy_name: ClassVar[str] = "io.platynui.strategies.NativeProperties"

    def get_native_property_names(self) -> List[str]: ...

    def get_native_property_value(self, name: str) -> Any: ...
