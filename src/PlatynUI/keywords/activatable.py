# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0


from typing import Any

from robotlibcore import keyword

from ..ui.strategies import Activatable, Deactivatable, HasIsActive
from .assertable import assertable
from .types import Element

__all__ = ["ActivatableKeywords"]


class ActivatableKeywords:
    @keyword
    def activate(self, element: Element[Activatable]) -> None:
        element.context.activate()

    @keyword
    def deactivate(self, element: Element[Deactivatable]) -> None:
        element.context.deactivate()

    @keyword
    @assertable
    def is_active(self, element: Element[HasIsActive]) -> bool:
        return element.context.is_active
