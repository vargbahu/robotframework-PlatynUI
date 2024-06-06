import inspect
from typing import TYPE_CHECKING, Any, List, Optional, Protocol, Set, Type, overload

from PlatynUI.core import Adapter, Technology, TStrategyBase
from PlatynUI.core.adapterproxy import AdapterProxyFactory
from PlatynUI.core.exceptions import AdapterNotSupportsStrategyError, NotAStrategyTypeError
from PlatynUI.core.types import Point, Rect
from PlatynUI.technology.uiautomation.core.technology import UiaTechnology
from PlatynUI.ui.strategies import Control, Element

from .loader import DotNetInterface

if TYPE_CHECKING:
    from PlatynUI.Technology.UiAutomation.Client import IUIAutomationElement  # type: ignore


class UiaBase(Protocol):
    @property
    def element(self) -> "IUIAutomationElement": ...

    @property
    def valid(self) -> bool: ...

    def invalidate(self) -> None: ...

    @property
    def technology(self) -> UiaTechnology: ...


class UiaElement(Element, UiaBase):

    @property
    def is_readonly(self) -> bool:
        return DotNetInterface.adapter().IsReadOnly(self.element)

    @property
    def is_enabled(self) -> bool:
        return self.element.CurrentIsEnabled != 0

    @property
    def is_visible(self) -> bool:
        return not self.element.CurrentIsOffscreen == 0

    @property
    def is_in_view(self) -> bool:
        raise NotImplementedError("is_in_view")

    @property
    def toplevel_parent_is_active(self) -> bool:
        raise NotImplementedError("toplevel_parent_is_active")

    @property
    def bounding_rectangle(self) -> Rect:
        r = self.element.CurrentBoundingRectangle
        return Rect(r.left, r.top, r.right - r.left, r.bottom - r.top)

    @property
    def visible_rectangle(self) -> Rect:
        raise NotImplementedError("visible_rectangle")

    @property
    def default_click_position(self) -> Point:
        p = DotNetInterface.adapter().GetClickablePoint(self.element)
        return Point(p.X, p.Y)

    def try_ensure_visible(self) -> bool:
        # TODO: Implement this if parent toplevel is out of view
        return self.element.CurrentIsOffscreen == 0

    def try_ensure_application_is_ready(self) -> bool:
        raise NotImplementedError("try_ensure_application_is_ready")

    def try_ensure_toplevel_parent_is_active(self) -> bool:
        return DotNetInterface.adapter().TryEnsureTopLevelParentIsActive(self.element)

    def try_bring_into_view(self) -> bool:
        # TODO: Implement this if element toplevel is out of view and can be scrolled
        return True

    def top_level_parent(self) -> Optional[Adapter]:
        result = DotNetInterface.adapter().GetTopLevelParent(self.element)

        if result is None:
            return None

        return AdapterProxyFactory.find_proxy_for(UiaAdapter(result, self.technology))


class UiaAdapter(Adapter, UiaElement):
    def __init__(self, element: "IUIAutomationElement", technology: UiaTechnology) -> None:
        super().__init__()
        self._element: Optional["IUIAutomationElement"] = element
        self._technology = technology

    @property
    def element(self) -> "IUIAutomationElement":
        if not self.valid or self._element is None:
            raise RuntimeError("element is not valid")

        return self._element

    @property
    def valid(self) -> bool:
        if self._element is None:
            return False

        v = False
        try:
            v = self._element.CurrentProcessId != 0
        except BaseException:
            v = False

        if not v:
            self.invalidate()

        return self._element is not None

    def invalidate(self) -> None:
        self._element = None

    @property
    def id(self) -> str:
        raise NotImplementedError("id")

    @property
    def name(self) -> str:
        return self.element.CurrentName

    @property
    def class_name(self) -> str:
        return self.element.CurrentClassName

    @property
    def tag_name(self) -> str:
        return self.role

    @property
    def role(self) -> str:
        return DotNetInterface.adapter().GetRole(self.element)

    @property
    def supported_roles(self) -> Set[str]:
        return {self.role, "Control", "Element", "Adapter"}

    @property
    def type(self) -> str:
        return "element" if self.valid else ""

    @property
    def supported_types(self) -> Set[str]:
        return {"element", "adapter"} if self.valid else set()

    @property
    def framework_id(self) -> str:
        return self.element.CurrentFrameworkId

    @property
    def runtime_id(self) -> str:
        return DotNetInterface.adapter().GetRuntimeId(self.element)

    @property
    def supported_strategies(self) -> Set[str]:
        if self.valid:
            return {
                m.strategy_name
                for m in inspect.getmro(type(self))
                if hasattr(m, "strategy_name") and m.strategy_name is not None
            }

        return set()

    @overload
    def get_strategy(self, strategy_type: Type[TStrategyBase]) -> TStrategyBase: ...

    @overload
    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool) -> Optional[TStrategyBase]: ...

    def get_strategy(self, strategy_type: Type[TStrategyBase], raise_exception: bool = True) -> Optional[TStrategyBase]:
        if hasattr(strategy_type, "strategy_name"):
            strategy_name = getattr(strategy_type, "strategy_name")
            if strategy_name is not None:
                if isinstance(self, strategy_type):
                    return self

                if raise_exception:
                    raise AdapterNotSupportsStrategyError(
                        "adapter not supports the %s strategy" % strategy_name
                        if strategy_name is not None
                        else repr(strategy_type)
                    )
                return None

        if raise_exception:
            raise NotAStrategyTypeError("type %s is not a strategy type" % repr(strategy_type))
        return None

    @property
    def technology(self) -> Technology:
        return self._technology

    @property
    def parent(self) -> Optional["Adapter"]:
        raise NotImplementedError("parent")

    @property
    def children(self) -> List["Adapter"]:
        raise NotImplementedError("children")
