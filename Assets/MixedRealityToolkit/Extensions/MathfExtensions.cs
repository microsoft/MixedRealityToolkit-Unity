// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods and helper functions for various math data
    /// </summary>
    public static class MathExtensions
    {
        public static int MostSignificantBit(this int x)
        {
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);

            return x & ~(x >> 1);
        }

        public static int PowerOfTwoGreaterThanOrEqualTo(this int v)
        {
            return Mathf.IsPowerOfTwo(v) ? v : Mathf.NextPowerOfTwo(v);
        }

        public static Vector3Int PowerOfTwoGreaterThanOrEqualTo(this Vector3Int v)
        {
            return new Vector3Int(PowerOfTwoGreaterThanOrEqualTo(v.x),
                                  PowerOfTwoGreaterThanOrEqualTo(v.y),
                                  PowerOfTwoGreaterThanOrEqualTo(v.z));
        }

        public static int CubicToLinearIndex(Vector3Int ndx, Vector3Int size)
        {
            return (ndx.x) +
                   (ndx.y * size.x) +
                   (ndx.z * size.x * size.y);
        }

        public static Vector3Int LinearToCubicIndex(int linearIndex, Vector3Int size)
        {
            return new Vector3Int((linearIndex / 1) % size.x,
                                  (linearIndex / size.x) % size.y,
                                  (linearIndex / (size.x * size.y)) % size.z);
        }

        public static Vector3 ClampComponentWise(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x),
                               Mathf.Clamp(value.y, min.y, max.y),
                               Mathf.Clamp(value.z, min.z, max.z));
        }

        /// <summary>
        /// Sets the value to zero if greater than the specified amount.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static int ResetIfGreaterThan(this int value, int amount)
        {
            if (value > amount)
            {
                value = 0;
            }

            return value;
        }
    }
}
