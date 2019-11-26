// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Enumeration indicating the available types of augmented reality tracking poses.
    /// </summary>
    public enum ArTrackedPose
    {
        /// <summary>
        /// The left eye of a head mounted device.
        /// </summary>
        LeftEye = 0,

        /// <summary>
        /// The left eye of a head mounted device.
        /// </summary>
        RightEye = 1,

        /// <summary>
        /// The center eye of a head mounted device, this is typically the default for most such devices.
        /// </summary>
        Center = 2,

        /// <summary>
        /// The "head" eye of a head mounted device, this location is often slightly above the center eye for most such devices.
        /// </summary>
        Head = 3,

        /// <summary>
        /// The left hand controller pose.
        /// </summary>
        LeftPose = 4,

        /// <summary>
        /// The right hand controller pose.
        /// </summary>
        RightPose = 5,

        /// <summary>
        /// The color camera of a mobile (ex: phone) device.
        /// </summary>
        ColorCamera = 6
    }
}
