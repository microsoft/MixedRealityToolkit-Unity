// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// The Tracking State defines how a device is currently being tracked.
    /// This enables developers to be able to handle non-tracked situations and react accordingly
    /// </summary>
    public enum TrackingAccuracy
    {
        /// <summary>
        /// There is no accuracy data for this device.
        /// </summary>
        /// <remarks>
        /// This indicates that the device is either not tracked or does not support tracking. The level
        /// of accuracy is defined by the device's platform or driver and may vary by, for example, the 
        /// distance between values returned (ex: degrees of rotation or physical distance of position).
        /// </remarks>
        None = 0,
        /// <summary>
        /// The device is returning approximate data.
        /// </summary>
        /// <remarks>
        /// Approximate accuracy generally implies that the device is not
        /// visible to sensors and that the system is inferring the data by
        /// other means.
        /// </remarks>
        Approximate,
        /// <summary>
        /// The device is returning a low level of accuracy.
        /// </summary>
        Low,
        /// <summary>
        /// The device is returning a medium level of accuracy.
        /// </summary>
        Medium,
        /// <summary>
        /// The device is returning a high level of accuracy.
        /// </summary>
        High
    }
}