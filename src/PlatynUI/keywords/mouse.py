# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

from robotlibcore import keyword

from ..ui.strategies import HasMouse
from .types import ElementDescriptor


class Mouse:
    @keyword
    def click(self, descriptor: ElementDescriptor[HasMouse]) -> None:
        descriptor().mouse.click()

    @keyword
    def double_click(self, descriptor: ElementDescriptor[HasMouse]) -> None:
        descriptor().mouse.double_click()
