// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    public static class FlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="InputDeviceCharacteristics"/> value.</param>
        /// <param name="b"><see cref="InputDeviceCharacteristics"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this InputDeviceCharacteristics a, InputDeviceCharacteristics b)
        {
            return (a & b) == b;
        }

        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="TrackingOriginModeFlags"/> value.</param>
        /// <param name="b"><see cref="TrackingOriginModeFlags"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this TrackingOriginModeFlags a, TrackingOriginModeFlags b)
        {
            return (a & b) == b;
        }
    }
}
