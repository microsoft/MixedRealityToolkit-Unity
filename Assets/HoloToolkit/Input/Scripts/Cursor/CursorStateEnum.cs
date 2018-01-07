// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
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
        /// Not IsSourceDetected
        /// </summary>
        Observe,
        /// <summary>
        /// Not IsSourceDetected AND not IsPointerDown AND TargetedObject exists
        /// </summary>
        ObserveHover,
        /// <summary>
        /// IsSourceDetected AND not IsPointerDown AND TargetedObject is NULL
        /// </summary>
        Interact,
        /// <summary>
        /// IsSourceDetected AND not IsPointerDown AND TargetedObject exists
        /// </summary>
        InteractHover,
        /// <summary>
        /// IsSourceDetected AND IsPointerDown
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