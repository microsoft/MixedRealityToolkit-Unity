// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// The Controller State defines how a controller or headset is currently being tracked.
    /// This enables developers to be able to handle non-tracked situations and react accordingly
    /// </summary>
    public enum ControllerState
    {
        /// <summary>
        /// No  controller state provided by the SDK.
        /// </summary>
        None = 0,
        /// <summary>
        /// Reserved, for systems that provide alternate tracking.
        /// </summary>
        Other,
        /// <summary>
        /// The controller is currently fully tracked and has accurate positioning.
        /// </summary>
        Tracked,
        /// <summary>
        /// The controller is currently not tracked.
        /// </summary>
        NotTracked,
        /// <summary>
        /// The controller is currently only returning position data.
        /// </summary>
        PositionOnly,
        /// <summary>
        /// The controller is currently only returning orientation data.
        /// </summary>
        OrientationOnly
    }
}