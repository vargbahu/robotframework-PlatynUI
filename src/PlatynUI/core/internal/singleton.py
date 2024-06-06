from abc import ABCMeta
from typing import Any, Dict

__all__ = ["SingletonMeta"]


class SingletonMeta(ABCMeta):
    _instances: Dict[Any, Any] = {}

    def __call__(cls, *args: Any, **kwargs: Any) -> Any:
        if cls not in cls._instances:
            cls._instances[cls] = super(SingletonMeta, cls).__call__(*args, **kwargs)
        return cls._instances[cls]
