// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

namespace PlatynUI.Technology.UiAutomation.Client;

public enum TextEditChangeType
{
    TextEditChangeType_None,
    TextEditChangeType_AutoCorrect,
    TextEditChangeType_Composition,
    TextEditChangeType_CompositionFinalized,
    TextEditChangeType_AutoComplete,
}
