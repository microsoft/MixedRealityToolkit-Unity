// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        GGVHand = 1 << 11
    }
}
