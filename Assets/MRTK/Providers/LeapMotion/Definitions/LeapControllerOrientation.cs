// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Input
{
    /// <summary>
    /// The location of the leap motion controller. LeapControllerOrientation.Headset indicates the controller is mounted on a headset. 
    /// LeapControllerOrientation.Desk indicates the controller is placed flat on desk. The default value is set to LeapControllerOrientation.Headset.
    /// </summary>
    public enum LeapControllerOrientation
    {
        // The leap controller is mounted on a headset
        Headset,
        // The leap controller is on a desk
        Desk
    }
}
