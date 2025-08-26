# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

from enum import Enum, auto
from typing import Optional, Union

from robotlibcore import keyword

from ..ui.strategies import Element
from .assertable import assertable
from .types import ElementDescriptor


class PropertyName(Enum):
    BoundingRectangle = auto()
    Enabled = auto()
    Visible = auto()


class Properties:
    @keyword
    @assertable
    def get_property_value(self, descriptor: ElementDescriptor[Element], name: PropertyName) -> None:
        match name:
            case PropertyName.BoundingRectangle:
                return descriptor().bounding_rectangle
            case PropertyName.Enabled:
                return descriptor().is_enabled
            case PropertyName.Visible:
                return descriptor().is_visible
            case _:
                raise TypeError("Unknown property")
