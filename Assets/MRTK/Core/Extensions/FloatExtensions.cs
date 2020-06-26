// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for the .Net Float struct
    /// </summary>
    public static class FloatExtensions
    {
        /// <summary>
        /// Checks if two numbers are approximately equal. Similar to <see href="https://docs.unity3d.com/ScriptReference/Mathf.Approximately.html">Mathf.Approximately(float, float)</see>, but the tolerance
        /// can be specified.
        /// </summary>
        /// <param name="number">One of the numbers to compare.</param>
        /// <param name="other">The other number to compare.</param>
        /// <param name="tolerance">The amount of tolerance to allow while still considering the numbers approximately equal.</param>
        /// <returns>True if the difference between the numbers is less than or equal to the tolerance, false otherwise.</returns>
        public static bool Approximately(this float number, float other, float tolerance)
        {
            return Mathf.Abs(number - other) <= tolerance;
        }
    }
}
