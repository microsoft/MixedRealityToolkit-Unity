// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Enum for current cursor context
    /// </summary>
    public enum CursorContextEnum
    {
        None = -1,
        MoveEastWest,
        MoveNorthSouth,
        MoveNorthwestSoutheast,
        MoveNortheastSouthwest,
        MoveCross,
        RotateEastWest,
        RotateNorthSouth,
        Contextual
    }
}