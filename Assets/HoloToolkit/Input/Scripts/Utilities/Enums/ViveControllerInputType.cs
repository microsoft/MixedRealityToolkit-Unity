// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    [Flags]
    public enum ViveControllerInputType
    {
        None = 0,
        LeftControllerMenuButton = 1 << 1,
        RightControllerMenuButton = 1 << 2,
        LeftControllerTrackpadPress = 1 << 3,
        LeftControllerTrackpadTouch = 1 << 4,
        RightControllerTrackpadPress = 1 << 3,
        RightControllerTrackpadTouch = 1 << 4,
        LeftControllerTrackpadHorizontal = 1 << 5,
        LeftControllerTrackpadVertical = 1 << 6,
        RightControllerTrackpadHorizontal = 1 << 5,
        RightControllerTrackpadVertical = 1 << 6,
        LeftControllerTriggerTouch = 1 << 7,
        RightControllerTriggerTouch = 1 << 8,
        LeftControllerTriggerSqueeze = 1 << 9,
        RightControllerTriggerSqueeze = 1 << 10,
        LeftControllerGripSqueeze = 1 << 11,
        RightControllerGripSqueeze = 1 << 12,
    }
}
