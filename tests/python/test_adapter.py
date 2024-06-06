from PlatynUI.core.contextbase import ContextFactory
from PlatynUI.core.settings import Settings
from PlatynUI.technology.uiautomation import Desktop, Locator, locator
from PlatynUI.ui import Button, Edit, Element, Pane, Window


@locator(ClassName="Shell_TrayWnd", role="Pane")
class TaskBar(Window):
    @property
    @locator(name="Start")
    def start(self) -> Button: ...


def test_first() -> None:
    e = Element(Locator(path="/."))

    assert e.exists(5, raise_exception=True) is True


def test_second() -> None:
    e = Pane(Locator(name="Taskleiste"))
    assert e.exists(5, raise_exception=True) is True


def test_third() -> None:
    context = ContextFactory.create_context(Locator(path="/."))
    assert isinstance(context, Pane)


def test_fourth() -> None:
    assert Desktop().exists()

    win = TaskBar()
    assert win.start.exists()


def test_fifth() -> None:
    context = ContextFactory.create_context(Locator(path="/Pane[@Name='Taskleiste']//Button[@Name='Start']"))

    assert isinstance(context, Button)

    desktop = Desktop()

    with Settings(mouse_move_time=0.1):
        for i in range(5):

            context.activate()
            desktop.mouse.move_to(desktop.bounding_rectangle.CENTER)
