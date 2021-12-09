// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Flags used to represent whether manipulation can be far, near or both
    /// </summary>
    [System.Flags]
    public enum ManipulationProximityFlags
    {
        Near = 1 << 0,
        Far = 1 << 1,
    }

    /// <summary>
    /// Extension methods specific to the <see cref="ManipulationProximityFlags"/> enum.
    /// </summary>
    public static class ManipulationProximityFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="ManipulationProximityFlags"/> value.</param>
        /// <param name="b"><see cref="ManipulationProximityFlags"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this ManipulationProximityFlags a, ManipulationProximityFlags b)
        {
            return (a & b) == b;
        }
    }
}