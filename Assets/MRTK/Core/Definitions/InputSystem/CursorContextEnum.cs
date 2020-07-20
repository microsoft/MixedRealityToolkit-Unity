// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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