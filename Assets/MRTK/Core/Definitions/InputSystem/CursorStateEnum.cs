// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
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
        /// Not IsHandDetected OR HasTeleportIntent
        /// </summary>
        Observe,
        /// <summary>
        /// Not IsHandDetected AND not IsPointerDown AND TargetedObject exists OR HasTeleportIntent AND Teleport Surface IsValid
        /// </summary>
        ObserveHover,
        /// <summary>
        /// IsHandDetected AND not IsPointerDown AND TargetedObject is NULL
        /// </summary>
        Interact,
        /// <summary>
        /// IsHandDetected AND not IsPointerDown AND TargetedObject exists
        /// </summary>
        InteractHover,
        /// <summary>
        /// IsHandDetected AND IsPointerDown
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