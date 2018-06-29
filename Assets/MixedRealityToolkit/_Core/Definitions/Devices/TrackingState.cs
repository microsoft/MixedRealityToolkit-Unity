// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// The Tracking State defines how a controller or headset is currently being tracked.
    /// This enables developers to be able to handle non-tracked situations and react accordingly
    /// </summary>
    public enum TrackingState
    {
        /// <summary>
        /// The controller is currently not tracked.
        /// </summary>
        NotTracked = 0,
        /// <summary>
        /// The controller is tracked, but has approximate positioning.
        /// </summary>
        Approximate,
        /// <summary>
        /// The controller is currently fully tracked and has accurate positioning.
        /// </summary>
        Tracked,
        /// <summary>
        /// Reserved, for systems that provide alternate tracking.
        /// </summary>
        Other,
    }
}