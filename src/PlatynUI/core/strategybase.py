from token import OP
from typing import ClassVar, Optional, Protocol, runtime_checkable

__all__ = ["StrategyBase"]


@runtime_checkable
class StrategyBase(Protocol):
    strategy_name: ClassVar[Optional[str]] = None
