from robotlibcore import keyword

from ..core.contextbase import ContextBase


class Text:
    @keyword
    def mouse_click(self, locator: ContextBase, text: str) -> None:
        raise NotImplementedError
