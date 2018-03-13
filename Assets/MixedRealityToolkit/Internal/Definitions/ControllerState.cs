// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Internal.Definitons
{
    /// <summary>
    /// The Controller State defines whether a controller or headset is currently being tracker or not.
    /// This enables developers to be able to handle non-tracked situations and react accordingly
    /// </summary>
    public enum ControllerState
    {
        /// <summary>
        /// No  controller state provided by the SDK.
        /// </summary>
        None,
        /// <summary>
        /// The controller is currently fully tracked and has accurate positioning.
        /// </summary>
        Tracked,
        /// <summary>
        /// The controller is currently not tracked and has relative positioning.
        /// </summary>
        NotTracked,
        /// <summary>
        /// Reserved, for systems that provide alternate tracking.
        /// </summary>
        Other
    }
}