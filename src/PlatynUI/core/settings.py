from typing import Any, Callable
from weakref import WeakSet

__all__ = ["Settings"]


class Settings(dict):
    class Setting:
        def __init__(self, name: str, default_value: Any = None):
            self.default_value = default_value
            self.name = name
            self.owner = None
            try:
                setattr(Settings, name, self)
            except NameError:
                pass

        def __set_name__(self, owner, name):
            self.owner = owner
            try:
                if owner != Settings:
                    setattr(Settings, name, self)
            except NameError:
                pass
            self.name = name

        def __get__(self, instance, owner):
            if instance is None or self.name not in instance:
                return self.default_value
            return instance[self.name]

        def __set__(self, instance, value):
            instance[self.name] = value

        def __delete__(self, instance):
            if self.name in instance:
                del instance[self.name]

    wait_for_timeout = Setting("wait_for_timeout", 1.0)  # type: float
    wait_for_delay = Setting("wait_for_delay", 0.1)  # type: float

    ensure_timeout = Setting("ensure_timeout", 15)  # type: float
    ensure_delay = Setting("ensure_delay", 0.1)  # type: float
    exists_timeout = Setting("exists_timeout", 1.0)  # type: float

    window_close_timeout = Setting("window_close_timeout", 1.0)  # type: float

    input_after_input_delay = Setting("input_after_input_delay", 0.0)  # type: float

    mouse_before_next_click_delay_multiplicator = Setting(
        "mouse_before_next_click_delay_multiplicator", 1.5
    )  # type: float
    mouse_after_click_delay = Setting("mouse_after_click_delay", 0.010)  # type: float
    mouse_multi_click_delay_multiplicator = Setting("mouse_multi_click_delay_multiplicator", 0.5)  # type: float
    mouse_press_release_delay = Setting("mouse_press_release_delay", 0.010)  # type: float
    mouse_after_move_delay = Setting("mouse_after_move_delay", 0.010)  # type: float
    mouse_move_delay = Setting("mouse_move_delay", 0.01)  # type: float
    mouse_move_time = Setting("mouse_move_time", 0.4)  # type: float

    keyboard_after_press_key_delay = Setting("keyboard_after_press_key_delay", 0.001)  # type: float
    keyboard_after_release_key_delay = Setting("keyboard_after_release_key_delay", 0.005)  # type: float
    keyboard_after_press_release_delay = Setting("keyboard_after_press_release_delay", 0.05)  # type: float

    display_screenshot_format = Setting("display_screenshot_format", "png")  # type: str
    display_screenshot_quality = Setting("display_screenshot_quality", -1)  # type: float
    display_screenshot_basename = Setting("display_screenshot_basename", "screenshot")  # type: str

    element_highlight_time = Setting("element_highlight_time", 2)  # type: float
    element_highlight_ensure_timeout = Setting("element_highlight_ensure_timeout", 2)  # type: float

    @property
    def defaults(self):
        return {v.name: v.default_value for n, v in Settings.__dict__.items() if isinstance(v, Settings.Setting)}

    def __init__(self, **kwargs):

        if Settings.__current is not None:
            super().__init__(Settings.__current, **kwargs)
        else:
            new_values = self.defaults.copy()
            new_values.update(kwargs)

            super().__init__(**new_values)

    __current = None  # type: Settings

    def __setitem__(self, key, value):
        super().__setitem__(key, value)
        if self == Settings.__current:
            Settings._changed(key, self, value)

    def __delitem__(self, key):
        super().__delitem__(key)
        if self == Settings.__current:
            Settings._deleted(key, self)

    def __getattr__(self, item):
        raise AttributeError

    @staticmethod
    def current() -> "Settings":
        if Settings.__current is None:
            Settings.__current = Settings()
        return Settings.__current

    def __enter__(self):
        self.__old_settings = Settings.__current
        Settings.__current = self
        try:
            return Settings.__current
        finally:
            for i in self:
                if i in self.defaults and self.defaults[i] != self[i] or i not in self.defaults:
                    Settings._changed(i, self, self[i])

    def __exit__(self, exc_type, exc_val, exc_tb):
        Settings.__current = self.__old_settings
        for i in self:
            if i in self.defaults and self.defaults[i] != self[i] or i not in self.defaults:
                Settings._changed(i, self, Settings.Reset())

    _hooks = WeakSet()

    @classmethod
    def add_changed_hook(cls, hook: Callable[[str, "Settings", Any], None]):
        if hook not in cls._hooks:
            cls._hooks.add(hook)

    @classmethod
    def remove_changed_hook(cls, hook: Callable[[str, "Settings", Any], None]):
        if hook in cls._hooks:
            cls._hooks.remove(hook)

    @classmethod
    def _changed(cls, setting, instance, value):
        for h in cls._hooks:
            h(setting, instance, value)

    class Reset:
        pass

    __reset = Reset()

    @classmethod
    def _deleted(cls, setting, instance):
        for h in cls._hooks:
            h(setting, instance, Settings.__reset)
