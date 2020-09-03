// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Quaternion struct.
    /// </summary>
    public static class QuaternionExtensions
    {
        public static bool IsValidRotation(this Quaternion rotation)
        {
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) &&
                   !float.IsInfinity(rotation.x) && !float.IsInfinity(rotation.y) && !float.IsInfinity(rotation.z) && !float.IsInfinity(rotation.w);
        }

        /// <summary>
        /// Determines if the angle between two quaternions is within a given tolerance.
        /// </summary>
        /// <param name="q1">The first quaternion.</param>
        /// <param name="q2">The second quaternion.</param>
        /// <param name="angleTolerance">The maximum angle that will cause this to return true.</param>
        /// <returns>True if the quaternions are aligned within the tolerance, false otherwise.</returns>
        public static bool AlignedEnough(Quaternion q1, Quaternion q2, float angleTolerance)
        {
            return Mathf.Abs(Quaternion.Angle(q1, q2)) < angleTolerance;
        }

    }
}