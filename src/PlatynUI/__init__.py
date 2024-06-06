# SPDX-FileCopyrightText: 2024-present Daniel Biehl <dbiehl@live.de>
#
# SPDX-License-Identifier: MIT
from robotlibcore import DynamicCore, keyword

from .ButtonKeywords import ButtonKeywords
from .TextKeywords import TextKeywords


class PlatynUI(DynamicCore):
    def __init__(self) -> None:
        super().__init__([ButtonKeywords(), TextKeywords()])

    @keyword
    def blah(self) -> None:
        print("blah")
