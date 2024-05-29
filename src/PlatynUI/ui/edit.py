from ..core import context
from . import strategies
from .text import Text

__all__ = ["Edit"]


@context
class Edit(Text):

    @Text.text.setter
    def text(self, value: str):
        self.set_text(value)

    def set_text(self, value):
        self.ensure_that(
            self._parent_window_is_active,
            self._element_is_in_view,
            self._element_is_enabled,
            self._element_is_not_readonly,
            self._control_has_focus,
        )
        try:
            self.adapter.get_strategy(strategies.EditableText).set_text(value)
        finally:
            self.ensure_that(self._application_is_ready, raise_exception=False)

    def clear(self):
        self.ensure_that(
            self._parent_window_is_active,
            self._element_is_in_view,
            self._element_is_enabled,
            self._element_is_not_readonly,
            self._control_has_focus,
        )
        try:
            self.adapter.get_strategy(strategies.Clearable).clear()
        finally:
            self.ensure_that(self._application_is_ready, raise_exception=False)

    def is_multiline(self):
        self.ensure_that(self._application_is_ready)

        return self.adapter.get_strategy(strategies.HasMultiLine).is_multi_line
