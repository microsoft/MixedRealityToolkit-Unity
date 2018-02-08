// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.InputModule
{
    [Flags]
    public enum OpenVRControllerInputType
    {
        None                              = 0,
        LeftControllerMenuButton          = 1 << 0,
        RightControllerMenuButton         = 1 << 1,
        LeftControllerTrackpadPress       = 1 << 2,
        LeftControllerTrackpadTouch       = 1 << 3,
        RightControllerTrackpadPress      = 1 << 4,
        RightControllerTrackpadTouch      = 1 << 5,
        LeftControllerTrackpadHorizontal  = 1 << 6,
        LeftControllerTrackpadVertical    = 1 << 7,
        RightControllerTrackpadHorizontal = 1 << 8,
        RightControllerTrackpadVertical   = 1 << 9,
        LeftControllerTriggerTouch        = 1 << 10,
        RightControllerTriggerTouch       = 1 << 11,
        LeftControllerTriggerSqueeze      = 1 << 12,
        RightControllerTriggerSqueeze     = 1 << 13,
        LeftControllerGripSqueeze         = 1 << 14,
        RightControllerGripSqueeze        = 1 << 15,
    }
}
