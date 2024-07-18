// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

﻿namespace PlatynUI.Technology.UiAutomation.Client;

public enum StructureChangeType
{
    StructureChangeType_ChildAdded,
    StructureChangeType_ChildRemoved,
    StructureChangeType_ChildrenInvalidated,
    StructureChangeType_ChildrenBulkAdded,
    StructureChangeType_ChildrenBulkRemoved,
    StructureChangeType_ChildrenReordered,
}
