// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Provides several utility functions for smoothing and lerping.
    /// </summary>
    internal class Smoothing
    {

        /// <summary>
        /// Smooths from source to goal, provided lerptime and a deltaTime.
        /// </summary>
        /// <param name="source">Current value</param>
        /// <param name="goal">"goal" value which will be lerped to</param>
        /// <param name="lerpTime">Smoothing/lerp amount. Smoothing of 0 means no smoothing, and max value means no change at all.</param>
        /// <param name="deltaTime">Delta time. Usually would be set to Time.deltaTime</param>
        /// <returns>Smoothed value</returns>
        internal static Vector3 SmoothTo(Vector3 source, Vector3 goal, float lerpTime, float deltaTime)
        {
            return Vector3.Lerp(source, goal, (lerpTime == 0f) ? 1f : 1f - Mathf.Pow(lerpTime, deltaTime));
        }

        /// <summary>
        /// Smooths from source to goal, provided slerptime and a deltaTime.
        /// </summary>
        /// <param name="source">Current value</param>
        /// <param name="goal">"goal" value which will be lerped to</param>
        /// <param name="slerpTime">Smoothing/lerp amount. Smoothing of 0 means no smoothing, and max value means no change at all.</param>
        /// <param name="deltaTime">Delta time. Usually would be set to Time.deltaTime</param>
        /// <returns>Smoothed value</returns>
        internal static Quaternion SmoothTo(Quaternion source, Quaternion goal, float slerpTime, float deltaTime)
        {
            return Quaternion.Slerp(source, goal, (slerpTime == 0f) ? 1f : 1f - Mathf.Pow(slerpTime, deltaTime));
        }
    }
}