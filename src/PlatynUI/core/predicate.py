from typing import Any, Callable, Optional

from .internal.decorator import *

__all__ = ["predicate"]


class predicate(FuncDecorator):  # noqa: N801
    def __init__(self, message: Optional[str] = None, flags: Any = None) -> None:
        self.message = message
        self.flags = flags

    def decorate(self, func: Callable[..., Any], *decorator_args: Any, **decorator_kwargs: Any) -> Callable[..., Any]:
        func.message = self.message
        func.flags = self.flags
        return super().decorate(func, decorator_args, decorator_kwargs)
