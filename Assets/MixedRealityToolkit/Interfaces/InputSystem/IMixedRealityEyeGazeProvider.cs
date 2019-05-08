// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// Whether eye gaze is valid. It may be invalid due to timeout or lack of tracking hardware or permissions.
        /// </summary>
        bool IsEyeGazeValid { get; }

        /// <summary>
        /// If true, eye-based tracking will be used when available.
        /// </summary>
        /// <remarks>
        /// The usage of eye-based tracking depends on having the Gaze Input permission set
        /// and user approved, along with proper device eye calibration. This will fallback to head-based
        /// gaze when eye-based tracking is not available.
        /// </remarks>
        bool UseEyeTracking { get; set; }

        /// <summary>
        /// DateTime in UTC when the signal was last updated.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Tells the eye gaze provider that eye gaze has updated.
        /// </summary>
        /// <param name="provider">The provider raising the event.</param>
        /// <param name="eyeRay"></param>
        /// <param name="timestamp"></param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealityEyeGazeDataProvider"/> interface, not by application code.
        /// </remarks>
        void UpdateEyeGaze(IMixedRealityEyeGazeDataProvider provider, Ray eyeRay, DateTime timestamp);
    }
}
