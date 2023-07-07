// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods and helper functions for various math data
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Get the most significant bit from the input value. 
        /// </summary> 
        /// <param name="x">The input value to examine.</param>
        /// <returns>
        /// Integer value that is equal to the most significant bit within the input value. 
        /// </returns>
        public static int MostSignificantBit(this int x)
        {
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);

            return x & ~(x >> 1);
        }

        /// <summary>
        /// Get the value that is the next power of two at or after the input value.
        /// </summary>
        /// <param name="input">The value to test.</param>
        /// <returns>
        /// If the input value is a power of two, the input value is returned. 
        /// Otherwise the next power of two, after the input value, is returned.
        /// </returns>
        public static int PowerOfTwoGreaterThanOrEqualTo(this int input)
        {
            return Mathf.IsPowerOfTwo(input) ? input : Mathf.NextPowerOfTwo(input);
        }

        /// <summary>
        /// For each component in a `Vector3Int`, get the component value that is the next power of two at or
        /// after the input component value.
        /// </summary>
        /// <param name="inputVector">The vector containing the component values to test.</param>
        /// <returns>
        /// A Vector3Int containing components that are a power of two. If an input's component value is a power of two, 
        /// than that component value is in the returned in the corresponding vector position. Otherwise the next 
        /// power of two, after the component value,  is returned in the corresponding vector position.
        /// </returns>
        public static Vector3Int PowerOfTwoGreaterThanOrEqualTo(this Vector3Int inputVector)
        {
            return new Vector3Int(PowerOfTwoGreaterThanOrEqualTo(inputVector.x),
                                  PowerOfTwoGreaterThanOrEqualTo(inputVector.y),
                                  PowerOfTwoGreaterThanOrEqualTo(inputVector.z));
        }

        /// <summary>
        /// Convert a 3D texture's cubic index to its linear representation.
        /// </summary>
        /// <param name="cubicIndex">A cubic index into a 3D texture.</param>
        /// <param name="size">The 3D texture's dimensions or size.</param>
        /// <returns>
        /// The linear index for the given cubic index and size.
        /// </returns>
        public static int CubicToLinearIndex(Vector3Int cubicIndex, Vector3Int size)
        {
            return (cubicIndex.x) +
                   (cubicIndex.y * size.x) +
                   (cubicIndex.z * size.x * size.y);
        }

        /// <summary>
        /// Convert a 3D texture's linear index to its cubic representation.
        /// </summary>
        /// <param name="linearIndex">A linear index into a 3D texture.</param>
        /// <param name="size">The 3D texture's dimensions or size.</param>
        /// <returns>
        /// The cubic index for the given linear index and size.
        /// </returns>
        public static Vector3Int LinearToCubicIndex(int linearIndex, Vector3Int size)
        {
            return new Vector3Int((linearIndex / 1) % size.x,
                                  (linearIndex / size.x) % size.y,
                                  (linearIndex / (size.x * size.y)) % size.z);
        }

        /// <summary>
        /// Preform a component wise clamp on a given vector. Each input component will be clamped by its 
        /// corresponding minimum and maximum component as specified by the `min` and `max` input vectors.
        /// </summary>
        /// <param name="value">The vector whose components will be clamped by the given minimum and maximum vectors.</param>
        /// <param name="min">The vector whose components define the minimum value for the corresponding input vector component.</param>
        /// <param name="max">The vector whose components define the maximum value for the corresponding input vector component.</param>
        /// <returns>
        /// Returns a new vector whose components have been clamped by the specified minimum and maximum values.
        /// </returns>
        public static Vector3 ClampComponentWise(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x),
                               Mathf.Clamp(value.y, min.y, max.y),
                               Mathf.Clamp(value.z, min.z, max.z));
        }

        /// <summary>
        /// Sets the value to zero if greater than the specified amount.
        /// </summary>
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
