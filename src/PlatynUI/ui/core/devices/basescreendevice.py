from abc import *

from ..core.types import *

__all__ = ["BaseScreenDevice"]


class BaseScreenDevice(metaclass=ABCMeta):
    @property
    @abstractmethod
    def rectangle(self) -> Rect:
        pass

    @abstractmethod
    def take_screen_shot(self, rect: Rect, filename: str, image_type: str) -> bool:
        pass
