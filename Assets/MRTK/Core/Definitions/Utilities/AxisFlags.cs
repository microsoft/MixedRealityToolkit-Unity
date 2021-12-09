// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent a set of 3D axes
    /// </summary>
    [System.Flags]
    public enum AxisFlags
    {
        XAxis = 1 << 0,
        YAxis = 1 << 1,
        ZAxis = 1 << 2
    }

    /// <summary>
    /// Extension methods specific to the <see cref="AxisFlags"/> enum.
    /// </summary>
    public static class AxisFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="AxisFlags"/> value.</param>
        /// <param name="b"><see cref="AxisFlags"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this AxisFlags a, AxisFlags b)
        {
            return (a & b) == b;
        }
    }
}