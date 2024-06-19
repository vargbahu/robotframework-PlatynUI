from typing import TYPE_CHECKING

from PlatynUI.ui.strategies.minimizeable import Minimizable

if TYPE_CHECKING:
    from PlatynUI.Technology.UiAutomation.Client import IUIAutomationElement


class WindowPatternProxy(Minimizable):
    def __init__(self, element: "IUIAutomationElement"):
        self.element = element

    @property
    def can_minimize(self) -> bool:
        raise NotImplementedError

    @property
    def is_minimized(self) -> bool:
        raise NotImplementedError

    def minimize(self) -> None:
        raise NotImplementedError
