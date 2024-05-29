from ..core import context
from . import strategies
from .control import Control
from .togglestate import ToggleState

__all__ = ["AbstractButton", "Button", "Link", "ToggleState", "CheckBox", "RadioButton"]


class AbstractButton(Control):
    @property
    def text(self) -> str:
        self.ensure_that(self._application_is_ready)

        return self.adapter.get_strategy(strategies.Text).text


@context
class Button(AbstractButton):
    def activate(self) -> None:
        self.ensure_that(self._parent_window_is_active, self._element_is_in_view, self._element_is_enabled)

        self.adapter.get_strategy(strategies.Activatable).activate()

        self.ensure_that(self._application_is_ready, raise_exception=False)


@context
class Link(Button):
    pass


@context
class CheckBox(AbstractButton):
    @property
    def state(self) -> ToggleState:
        self.ensure_that(self._application_is_ready)

        return self.adapter.get_strategy(strategies.HasToggleState).state

    @state.setter
    def state(self, value: ToggleState) -> None:
        self.set_state(value)

    def set_state(self, state):
        # noinspection PyTypeChecker
        for _ in ToggleState:
            if self.state == state:
                break
            self.toggle()

    def activate(self):
        self.set_state(ToggleState.Checked)

    def check(self):
        self.set_state(ToggleState.Checked)

    @property
    def is_checked(self):
        return self.state == ToggleState.Checked

    @property
    def is_unchecked(self):
        return self.state == ToggleState.Unchecked

    def uncheck(self):
        self.set_state(ToggleState.Unchecked)

    def toggle(self):
        self.ensure_that(
            self._parent_window_is_active,
            self._element_is_in_view,
            self._element_is_enabled,
            self._element_is_not_readonly,
        )

        self.adapter.get_strategy(strategies.Toggleable).toggle()

        self.ensure_that(self._application_is_ready, raise_exception=False)


@context
class RadioButton(AbstractButton):
    pass
