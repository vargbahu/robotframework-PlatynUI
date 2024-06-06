from PlatynUI.core.contextbase import ContextFactory
from PlatynUI.technology.uiautomation import Desktop, Locator
from PlatynUI.ui import Button

context = ContextFactory.create_context(Locator(path="/Pane[@Name='Taskleiste']//Button[@Name='Start']"))
# context.activate()
assert isinstance(context, Button)
print(context.framework_id)
print(context.runtime_id)
print(context.name)
Desktop().mouse.click()
