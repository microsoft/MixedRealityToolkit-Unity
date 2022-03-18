// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent the number of hands that can be used in manipulation
    /// </summary>
    [System.Flags]
    public enum ManipulationHandFlags
    {
        OneHanded = 1 << 0,
        TwoHanded = 1 << 1,
    }

    /// <summary>
    /// Extension methods specific to the <see cref="ManipulationHandFlags"/> enum.
    /// </summary>
    public static class ManipulationHandFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="ManipulationHandFlags"/> value.</param>
        /// <param name="b"><see cref="ManipulationHandFlags"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this ManipulationHandFlags a, ManipulationHandFlags b)
        {
            return (a & b) == b;
        }
    }
}