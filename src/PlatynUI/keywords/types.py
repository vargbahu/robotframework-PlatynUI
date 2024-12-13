# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

from typing import Any, Generic, Optional, Type, Union, cast

from robot.running.context import EXECUTION_CONTEXTS
from robot.utils import secs_to_timestr
from typing_extensions import TypeGuard, TypeVar

from PlatynUI.core.adapter import Adapter

from ..core.contextbase import ContextBase, ContextFactory, TContextBase
from ..core.locatorbase import LocatorBase
from ..ui import Locator

T = TypeVar("T", default=Any)

ROOT_ELEMENT_VAR = "PLATYNUI_ROOT_ELEMENT"


class Element(Generic[T]):
    def __init__(
        self,
        locator: Optional[LocatorBase] = None,
        context_type: Optional[Type[TContextBase]] = None,
        parent: Optional["Element"] = None,
        context: Optional[ContextBase] = None,
    ) -> None:
        self.__locator = locator
        self.__context_type = context_type
        self.__parent = parent
        self.__context = context
        self.__has_real_context = context is not None

    def __repr__(self) -> str:
        return f"<Proxy for {self.__context!r}>"

    @property
    def context(self) -> T:
        if self.__context is None:
            self.__context = ContextBase(
                self.__locator, cast(ContextBase, self.__parent.context) if self.__parent else None
            )

        if self.__context is not None and not self.__has_real_context:
            adapter = self.__context.get_adapter()

            if adapter is not None:
                context_type = ContextFactory.find_context_class_for(adapter, self.__context_type)
                self.__has_real_context = True

                self.__context = context_type(
                    self.__locator,
                    context_parent=cast(ContextBase, self.__parent.context) if self.__parent else None,
                    adapter=adapter,
                )

        return cast(T, self.__context)

    @staticmethod
    def convert(value: Union[str, ContextBase]) -> "Element[T]":
        if isinstance(value, ContextBase):
            return Element(context=value)
        return Element(Locator(path=value), parent=Element.get_root_element())

    @staticmethod
    def set_root_element(element: "Element[Any]") -> "Element[Any]":
        old = Element.get_root_element()

        EXECUTION_CONTEXTS.current.variables[f"${{{ROOT_ELEMENT_VAR}}}"] = element

        return old

    @staticmethod
    def get_root_element() -> "Element[Any]":
        return cast(
            Element[Any],
            (
                EXECUTION_CONTEXTS.current.variables[f"${{{ROOT_ELEMENT_VAR}}}"]
                if ROOT_ELEMENT_VAR in EXECUTION_CONTEXTS.current.variables
                else None
            ),
        )


class TimeSpan:
    def __init__(self, seconds: float) -> None:
        self.seconds = seconds

    def __str__(self) -> str:
        return str(secs_to_timestr(self.seconds))
