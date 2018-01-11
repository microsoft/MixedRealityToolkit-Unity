// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.InputModule.Cursor
{
    /// <summary>
    /// Enum for current cursor state
    /// </summary>
    public enum CursorStateEnum
    {
        /// <summary>
        /// Useful for releasing external override.
        /// See <c>CursorStateEnum.Contextual</c>
        /// </summary>
        None = -1,
        /// <summary>
        /// Not IsHandVisible
        /// </summary>
        Observe,
        /// <summary>
        /// Not IsHandVisible AND not IsInputSourceDown AND TargetedObject exists
        /// </summary>
        ObserveHover,
        /// <summary>
        /// IsHandVisible AND not IsInputSourceDown AND TargetedObject is NULL
        /// </summary>
        Interact,
        /// <summary>
        /// IsHandVisible AND not IsInputSourceDown AND TargetedObject exists
        /// </summary>
        InteractHover,
        /// <summary>
        /// IsHandVisible AND IsInputSourceDown
        /// </summary>
        Select,
        /// <summary>
        /// Available for use by classes that extend Cursor.
        /// No logic for setting Release state exists in the base Cursor class.
        /// </summary>
        Release,
        /// <summary>
        /// Allows for external override
        /// </summary>
        Contextual
    }
}