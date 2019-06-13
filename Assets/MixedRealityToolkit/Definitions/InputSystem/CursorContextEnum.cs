// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Enum for current cursor context
    /// </summary>
    public enum CursorContextEnum
    {
        /// <summary>
        /// Useful for releasing external override.
        /// See <c>CursorStateEnum.Contextual</c>
        /// </summary>
        None = -1,
        MoveEastWest,
        MoveNorthSouth,
        MoveNorthwestSoutheast,
        MoveNortheastSouthwest,
        MoveCross,
        RotateEastWest,
        RotateNorthSouth,
        /// <summary>
        /// Allows for external override
        /// </summary>
        Contextual
    }
}