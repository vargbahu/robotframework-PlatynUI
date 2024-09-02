# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

from PlatynUI.ui.runtime.core.dotnet_interface import DotNetInterface


def test_finder_should_find_desktop() -> None:
    result = DotNetInterface.finder().FindSingleNode(None, "/.", False)
    assert result is not None


def test_finder_should_find_one_node() -> None:
    result = DotNetInterface.finder().FindSingleNode(None, "/*", False)
    assert result is not None
