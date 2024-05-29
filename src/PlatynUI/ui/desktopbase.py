from ..core import *
from .core.devices import *
from .core.devices.keyboardproxy import *
from .core.devices.mouseproxy import *
from .element import *

__all__ = ["DesktopBase"]


class DesktopBase(Element):
    default_role = "Desktop"

    class __DesktopMouseProxy(MouseProxy):
        def __init__(self, element: "DesktopBase", mouse_device: MouseDevice):
            super().__init__(mouse_device)
            self._element = element

        @property
        def base_point(self) -> Point:
            return self._element.bounding_rectangle.top_left

        @property
        def base_rect(self) -> Rect:
            return self._element.bounding_rectangle

        @property
        def default_click_position(self) -> Point:
            return self._element.default_click_position

        def before_action(self, action: MouseProxy.Action):
            self._element.ensure_that(self._element._adapter_exists)
            self.mouse_device.add_context(self._element)

        def after_action(self, action: MouseProxy.Action):
            self.mouse_device.remove_context(self._element)

    def _create_mouse_proxy(self, mouse_device: MouseDevice) -> MouseProxy:
        return self.__DesktopMouseProxy(self, mouse_device)

    class __DesktopKeyboardProxy(KeyboardProxy):
        def __init__(self, element: "DesktopBase", keyboard_device: KeyboardDevice):
            super().__init__(keyboard_device)
            self._element = element

        def before_action(self, action: KeyboardProxy.Action):
            self._element.ensure_that(self._element._adapter_exists)
            self.keyboard_device.add_context(self._element)

        def after_action(self, action: KeyboardProxy.Action):
            self.keyboard_device.remove_context(self._element)

    def _create_keyboard_proxy(self, keyboard_device: KeyboardDevice) -> KeyboardProxy:
        return self.__DesktopKeyboardProxy(self, keyboard_device)

    def _before_get_screenshot(self):
        pass
