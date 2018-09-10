// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    /// <summary>
    /// The Tracking State defines how a device is currently being tracked.
    /// This enables developers to be able to handle non-tracked situations and react accordingly.
    /// </summary>
    /// <remarks>
    /// Tracking is being defined as receiving sensor (positional and/or rotational) data from the device.
    /// </remarks>
    public enum TrackingState
    {
        /// <summary>
        /// The device does not support tracking (ex: a traditional game controller).
        /// </summary>
        NotApplicable = 0,
        /// <summary>
        /// The device is not tracked.
        /// </summary>
        NotTracked,
        /// <summary>
        /// The device is tracked (positionally and/or rotationally).
        /// </summary>
        /// <remarks>
        /// Some devices provide additional details regarding the accuracy of the tracking.
        /// <See cref="Definitions.Devices.TrackingAccuracy"/> for more information.
        /// </remarks>
        Tracked
    }
}