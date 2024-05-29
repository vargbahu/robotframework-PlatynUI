from PlatynUI.ui import Element


def test_first() -> None:
    e = Element(Locator("."))
    assert e.exists() is True
