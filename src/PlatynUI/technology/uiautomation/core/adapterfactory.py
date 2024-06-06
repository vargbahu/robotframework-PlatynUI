from typing import List, Optional, Type

from PlatynUI.core import Adapter, AdapterFactory, AdapterProxyFactory, ContextBase, LocatorBase, TContextBase

from ..locator import Locator
from .loader import DotNetInterface
from .technology import UiaTechnology
from .uiaadapter import UiaAdapter


class UiaAdapterFactory(AdapterFactory):
    def __init__(self, technology: UiaTechnology) -> None:
        self._technology = technology

    def get_adapter(
        self,
        parent: Optional[ContextBase],
        context_type: Optional[Type[TContextBase]],
        locator: "LocatorBase",
        raise_error: bool = True,
    ) -> Optional["Adapter"]:

        if not isinstance(locator, Locator):
            return None

        uia_parent = parent.adapter.element if parent and isinstance(parent.adapter, UiaAdapter) else None
        path = locator.get_path(parent, context_type=context_type)

        try:
            result = DotNetInterface.finder().FindSingleElement(uia_parent, path, False)
        except:  # noqa: E722
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
