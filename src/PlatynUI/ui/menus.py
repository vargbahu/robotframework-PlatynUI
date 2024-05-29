from ..core import context
from . import strategies
from .control import *
from .desktopbase import *
from .window import *

__all__ = ["Menu", "MenuBar", "MenuItem"]


@context
class Menu(Control):
    pass


@context
class MenuBar(Control):
    pass


@context
class MenuItem(Control):

    def activate(self):
        items = []
        p = self
        while p is not None and not isinstance(p, (Window, DesktopBase)):
            if isinstance(p, MenuItem):
                items.append(p)
            p = p.parent

        for i in reversed(items[1:]):
            if not self.parent_window_is_active:
                i.activate()

        self.ensure_that(self._parent_window_is_active, self._element_is_in_view)
        try:
            self.adapter.get_strategy(strategies.Activatable).activate()
        finally:
            self.ensure_that(self._application_is_ready, raise_exception=False)
