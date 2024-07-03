from PlatynUI.technology.uiautomation import Desktop, locator
from PlatynUI.ui import Button, Text, Window


@locator(name="Rechner")
class Calculator(Window):
    @property
    @locator(AutomationId="num0Button")
    def n0(self) -> Button: ...
    @property
    @locator(AutomationId="num1Button")
    def n1(self) -> Button: ...
    @property
    @locator(AutomationId="num2Button")
    def n2(self) -> Button: ...
    @property
    @locator(AutomationId="num3Button")
    def n3(self) -> Button: ...
    @property
    @locator(AutomationId="num4Button")
    def n4(self) -> Button: ...
    @property
    @locator(AutomationId="num5Button")
    def n5(self) -> Button: ...
    @property
    @locator(AutomationId="num6Button")
    def n6(self) -> Button: ...
    @property
    @locator(AutomationId="num7Button")
    def n7(self) -> Button: ...
    @property
    @locator(AutomationId="num8Button")
    def n8(self) -> Button: ...
    @property
    @locator(AutomationId="num9Button")
    def n9(self) -> Button: ...

    @property
    @locator(AutomationId="clearEntryButton")
    def clear_entry(self) -> Button: ...

    @property
    @locator(AutomationId="clearButton")
    def clear(self) -> Button: ...

    @property
    @locator(AutomationId="equalButton")
    def equal(self) -> Button: ...

    @property
    @locator(AutomationId="plusButton")
    def plus(self) -> Button: ...

    @property
    @locator(AutomationId="minusButton")
    def minus(self) -> Button: ...

    @property
    @locator(AutomationId="multiplyButton")
    def multiply(self) -> Button: ...

    @property
    @locator(AutomationId="divideButton")
    def divide(self) -> Button: ...

    @property
    @locator(AutomationId="NormalOutput")
    def results(self) -> Text: ...


def test_seventh() -> None:
    calc = Calculator()
    desktop = Desktop()
    desktop.mouse.move_to(x=calc.n1.bounding_rectangle.left, y=calc.n1.bounding_rectangle.top)
    desktop.mouse.move_to(x=calc.n2.bounding_rectangle.left, y=calc.n2.bounding_rectangle.top)
    desktop.mouse.move_to(x=calc.n3.bounding_rectangle.left, y=calc.n3.bounding_rectangle.top)

    desktop.mouse.move_to(x=calc.n1.bounding_rectangle.left, y=calc.n1.bounding_rectangle.top)
    desktop.mouse.move_to(x=calc.n2.bounding_rectangle.left, y=calc.n2.bounding_rectangle.top)
    desktop.mouse.move_to(x=calc.n3.bounding_rectangle.left, y=calc.n3.bounding_rectangle.top)

    calc.clear.activate()

    calc.n1.mouse.move_to()
    calc.n2.mouse.move_to()
    calc.n3.mouse.move_to()
    calc.n4.mouse.move_to()

    calc.clear.activate()
    calc.n1.activate()
    calc.n2.activate()
    calc.n3.activate()
    calc.n4.activate()
    calc.n5.activate()
    calc.n6.activate()
    calc.n7.activate()
    calc.n8.activate()
    calc.n9.activate()
    calc.n0.activate()

    print(calc.results.text)
    # calc.keyboard.type_keys("1234")


test_seventh()
