from abc import abstractmethod
from typing import ClassVar, Optional, Protocol, runtime_checkable

from ...core import Adapter, Point, Rect, StrategyBase

__all__ = ["Element", "TopLevelElement"]


@runtime_checkable
class Element(StrategyBase, Protocol):
    strategy_name: ClassVar[str] = "org.platynui.strategies.Element"

    @property
    def is_readonly(self) -> bool: ...

    @property
    def is_enabled(self) -> bool: ...

    @property
    def is_visible(self) -> bool: ...

    @property
    def is_in_view(self) -> bool: ...

    @property
    def parent_window_is_active(self) -> bool: ...

    @property
    def bounding_rectangle(self) -> Rect: ...

    @property
    def visible_rectangle(self) -> Rect: ...

    @property
    def default_click_position(self) -> Point: ...

    def try_ensure_visible(self) -> bool: ...

    def try_ensure_application_is_ready(self) -> bool: ...

    def try_ensure_parent_window_is_active(self) -> bool: ...

    def try_bring_into_view(self) -> bool: ...

    def top_level_parent(self) -> Optional[Adapter]: ...


class TopLevelElement(StrategyBase):
    strategy_name = "org.platynui.strategies.TopLevelElement"

    @property
    @abstractmethod
    def is_top_level_element(self) -> bool:
        pass
