from abc import ABCMeta, abstractmethod
from typing import TYPE_CHECKING, List, Optional, Type

if TYPE_CHECKING:
    from .adapter import Adapter
    from .contextbase import ContextBase, TContextBase
    from .locatorbase import LocatorBase

__all__ = ["AdapterFactory"]


class AdapterFactory(metaclass=ABCMeta):

    @abstractmethod
    def get_adapter(
        self,
        parent: Optional["ContextBase"],
        context_type: Optional[Type["TContextBase"]],
        locator: "LocatorBase",
        raise_error: bool = True,
    ) -> Optional["Adapter"]:
        pass

    @abstractmethod
    def get_children_adapters(
        self,
        parent: Optional["ContextBase"],
        context_type: Optional[Type["TContextBase"]],
        locator: "LocatorBase",
        raise_error: bool = True,
    ) -> List["Adapter"]:
        pass
