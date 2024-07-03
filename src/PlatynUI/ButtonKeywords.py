from robotlibcore import keyword

from PlatynUI.core.contextbase import ContextBase


class ButtonKeywords:
    @keyword
    def activate(self, locator: ContextBase) -> None:
        locator.activate()
