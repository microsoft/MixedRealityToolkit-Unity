// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    /// <summary>
    /// The offset modes when the LeapControllerOrientation is Headset.  These offset modes can only be used if the 
    /// LeapServiceProvider within the LeapMotionDeviceManager.cs is the LeapXRServiceProvider.  These modes are only for the 
    /// offset of the Leap Controller while in VR and not while the controller is on the desk.
    /// </summary>
    public enum LeapVRDeviceOffsetMode
    {
        /// <summary>
        /// No change or offset will be applied to the Leap Controller while in this mode.
        /// </summary>
        Default = 0,

        /// <summary>
        /// This mode exposes the modification of 3 properties: LeapDeviceOffsetY, LeapDeviceOffsetZ and LeapDeviceOffsetTiltX.  These properties 
        /// have the same set range as the offset properties contained in the LeapXRServiceProvider component.
        /// </summary>
        ManualHeadOffset,

        /// <summary>
        /// Set a new transform as the origin of the Leap Controller while in VR.  Setting the origin of the Leap Controller will move the hands
        /// to the new transform.
        /// </summary>
        Transform
    }
}
