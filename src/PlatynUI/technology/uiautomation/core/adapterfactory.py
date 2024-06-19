from typing import List, Optional, Type, overload

from PlatynUI.core import Adapter, AdapterFactory, AdapterProxyFactory, ContextBase, LocatorBase, TContextBase
from PlatynUI.core.exceptions import AdapterNotFoundError

from ..locator import Locator
from .loader import DotNetInterface
from .proxies.window_pattern import WindowPatternProxy
from .technology import UiaTechnology
from .uiaadapter import UiaAdapter


class UiaAdapterFactory(AdapterFactory):
    def __init__(self, technology: UiaTechnology) -> None:
        self._technology = technology

    @overload
    def get_adapter(
        self,
        locator: "LocatorBase",
        parent: Optional["ContextBase"] = None,
        context_type: Optional[Type["ContextBase"]] = None,
    ) -> Adapter:
        pass

    @overload
    def get_adapter(
        self,
        locator: "LocatorBase",
        parent: Optional["ContextBase"] = None,
        context_type: Optional[Type["ContextBase"]] = None,
        raise_error: bool = True,
    ) -> Optional["Adapter"]:
        pass

    def get_adapter(
        self,
        locator: "LocatorBase",
        parent: Optional["ContextBase"] = None,
        context_type: Optional[Type["ContextBase"]] = None,
        raise_error: bool = True,
    ) -> Optional["Adapter"]:

        if not isinstance(locator, Locator):
            return None

        uia_parent = parent.adapter.element if parent and isinstance(parent.adapter, UiaAdapter) else None
        path = locator.get_path(parent, context_type=context_type)

        result = None
        try:
            result = DotNetInterface.finder().FindSingleElement(uia_parent, path, False)
            if result is None and raise_error:
                raise AdapterNotFoundError(
                    f"Element for '{path}' not found"
                    if parent is None
                    else f"Element for {path:r} not found in {parent}"
                )
        except BaseException:
            if raise_error:
                raise

        if result is None:
            return None

        return AdapterProxyFactory.find_proxy_for(UiaAdapter(result, self._technology))

    def get_children_adapters(
        self,
        parent: Optional[ContextBase],
        context_type: Optional[Type[TContextBase]],
        locator: LocatorBase,
        raise_error: Optional[bool] = True,
    ) -> List["Adapter"]:
        # TODO
        return []
