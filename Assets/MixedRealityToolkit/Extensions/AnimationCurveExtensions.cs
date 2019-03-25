// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's AnimationCurve class
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Returns the absolute duration of the curve from first to last key frame
        /// </summary>
        /// <param name="curve">The animation curve to check duration of.</param>
        /// <returns>Returns 0 if the curve is null or has less than 1 frame, otherwise returns time difference between first and last frame.</returns>
        public static float Duration(this AnimationCurve curve)
        {
            if (curve == null || curve.length <= 1)
            {
                return 0.0f;
            }

            return Mathf.Abs(curve[curve.length - 1].time - curve[0].time);
        }
    }
}
