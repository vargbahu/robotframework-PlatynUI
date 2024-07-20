// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

namespace PlatynUI.Technology.UiAutomation.Client;

public enum WindowInteractionState
{
    WindowInteractionState_Running,
    WindowInteractionState_Closing,
    WindowInteractionState_ReadyForUserInteraction,
    WindowInteractionState_BlockedByModalWindow,
    WindowInteractionState_NotResponding,
}
