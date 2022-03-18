// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Gaze Provider for an Input Source.
    /// </summary>
    public interface IMixedRealityEyeGazeProvider : IMixedRealityGazeProvider
    {
        /// <summary>
        /// Whether eye tracking data is currently been used for gaze rather then head pose. Eye Tracking must be both enabled and have valid data.
        /// </summary>
        bool IsEyeTrackingEnabledAndValid { get; }

        /// <summary>
        /// Whether eye tracking data is available. It may be unavailable due to timeout or lack of tracking hardware or permissions.
        /// </summary>
        bool IsEyeTrackingDataValid { get; }

        /// <summary>
        /// Whether the user is eye calibrated. It returns 'null', if the value has not yet received data from the eye tracking system.
        /// </summary>
        bool? IsEyeCalibrationValid { get; }

        /// <summary>
        /// The most recent eye tracking ray
        /// </summary>
        Ray LatestEyeGaze { get; }

        /// <summary>
        /// If true, eye-based tracking will be used when available.
        /// </summary>
        /// <remarks>
        /// <para>The usage of eye-based tracking depends on having the Gaze Input permission set
        /// and user approved, along with proper device eye calibration. This will fallback to head-based
        /// gaze when eye-based tracking is not available.</para>
        /// </remarks>
        bool IsEyeTrackingEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when the signal was last updated.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Tells the eye gaze provider that eye gaze has updated.
        /// </summary>
        /// <param name="provider">The provider raising the event.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealityEyeGazeDataProvider"/> interface, not by application code.
        /// </remarks>
        void UpdateEyeGaze(IMixedRealityEyeGazeDataProvider provider, Ray eyeRay, DateTime timestamp);

        /// <summary>
        /// Tells the eye gaze provider about the eye tracking status (e.g., whether the user is calibrated);
        /// </summary>
        /// <param name="provider">The provider raising the event.</param>
        /// <param name="userIsEyeCalibrated">Boolean whether the user is eye calibrated or not.</param>
        /// <remarks>
        /// <para>Note that this function is not invoked when eye tracking is lost - use IsEyeTrackingAvailable
        /// to detect when eye tracking is lost.</para>
        /// </remarks>
        void UpdateEyeTrackingStatus(IMixedRealityEyeGazeDataProvider provider, bool userIsEyeCalibrated);
    }
}
