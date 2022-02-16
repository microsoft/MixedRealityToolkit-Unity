// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    // todo: remove this.... it requires customization to add new device types

    /// <summary>
    /// The SDKType lists the XR SDKs that are supported by the Mixed Reality Toolkit.
    /// Initially, this lists proposed SDKs, not all may be implemented at this time (please see ReleaseNotes for more details)
    /// </summary>
    [Flags]
    public enum SupportedControllerType
    {
        GenericOpenVR = 1 << 0,
        ViveWand = 1 << 1,
        ViveKnuckles = 1 << 2,
        OculusTouch = 1 << 3,
        OculusRemote = 1 << 4,
        WindowsMixedReality = 1 << 5,
        GenericUnity = 1 << 6,
        Xbox = 1 << 7,
        TouchScreen = 1 << 8,
        Mouse = 1 << 9,
        ArticulatedHand = 1 << 10,
        GGVHand = 1 << 11,
        HPMotionController = 1 << 12
    }

    /// <summary>
    /// Extension methods specific to the <see cref="SupportedControllerType"/> enum.
    /// </summary>
    public static class SupportedControllerTypeExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="SupportedControllerType"/> value.</param>
        /// <param name="b"><see cref="SupportedControllerType"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this SupportedControllerType a, SupportedControllerType b)
        {
            return (a & b) == b;
        }
    }
}
