from PlatynUI.technology.uiautomation.locator import locator
from PlatynUI.ui.buttons import Button
from PlatynUI.ui.window import Window

# mypy: disable-error-code="empty-body"


@locator(name="Rechner")
class CalculatorWindow(Window):
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


calculator = CalculatorWindow()

print(calculator.n1)