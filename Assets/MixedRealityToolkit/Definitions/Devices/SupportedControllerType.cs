// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    // todo: remove this.... it requires customization to add new device types
    
    /// <summary>
    /// The SDKType lists the XR SDKs that are supported by the Mixed Reality Toolkit.
    /// Initially, this lists proposed SDKs, not all may be implemented at this time (please see ReleaseNotes for more details)
    /// </summary>
    public enum SupportedControllerType
    {
        None = 0,
        GenericOpenVR,
        ViveWand,
        ViveKnuckles,
        OculusTouch,
        OculusRemote,
        WindowsMixedReality,
        GenericUnity,
        Xbox,
        TouchScreen,
        Mouse
    }
}
