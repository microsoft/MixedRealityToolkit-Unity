// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
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
        /// </remarks>
        Tracked
    }
}