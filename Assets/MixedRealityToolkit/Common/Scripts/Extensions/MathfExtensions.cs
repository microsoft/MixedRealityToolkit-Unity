// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Common.Extensions
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
            if (Mathf.IsPowerOfTwo(v))
            {
                return v;
            }

            return Mathf.NextPowerOfTwo(v);
        }

        public static Int3 PowerOfTwoGreaterThanOrEqualTo(this Int3 v)
        {
            return new Int3(PowerOfTwoGreaterThanOrEqualTo(v.x),
                            PowerOfTwoGreaterThanOrEqualTo(v.y),
                            PowerOfTwoGreaterThanOrEqualTo(v.z));
        }

        public static int CubicToLinearIndex(Int3 ndx, Int3 size)
        {
            return (ndx.x) +
                   (ndx.y * size.x) +
                   (ndx.z * size.x * size.y);
        }

        public static Int3 LinearToCubicIndex(int linearIndex, Int3 size)
        {
            return new Int3((linearIndex / 1) % size.x,
                            (linearIndex / size.x) % size.y,
                            (linearIndex / (size.x * size.y)) % size.z);
        }

        public static Vector3 ClampComponentwise(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x),
                               Mathf.Clamp(value.y, min.y, max.y),
                               Mathf.Clamp(value.z, min.z, max.z));
        }
    }
}