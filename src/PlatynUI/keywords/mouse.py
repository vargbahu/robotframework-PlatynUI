# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

from robotlibcore import keyword

from ..ui.strategies import HasMouse
from .types import Element


class Mouse:
    @keyword
    def click(self, element: Element[HasMouse]) -> None:
        element.context.mouse.click()

    @keyword
    def double_click(self, element: Element[HasMouse]) -> None:
        element.context.mouse.double_click()
