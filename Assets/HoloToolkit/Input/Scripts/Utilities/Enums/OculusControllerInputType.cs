// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    [Flags]
    public enum OculusControllerInputType
    {
        None = 0,
        ButtonOnePress = 1 << 1,
        ButtonOneTouch = 1 << 2,
        ButtonTwoPress = 1 << 3,
        ButtonTwoTouch = 1 << 4,
        ButtonThreePress = 1 << 5,
        ButtonThreeTouch = 1 << 6,
        ButtonFourPress = 1 << 7,
        ButtonFourTouch = 1 << 8,
        ButtonStart = 1 << 9,
        PrimaryThumbstickPress = 1 << 10,
        PrimaryThumbstickTouch = 1 << 11,
        PrimaryThumbstickNearTouch = 1 << 12,
        SecondaryThumbstickPress = 1 << 13,
        SecondaryThumbstickTouch = 1 << 14,
        SecondaryThumbstickNearTouch = 1 << 15,
        PrimaryThumbRestTouch = 1 << 16,
        PrimaryThumbRestNearTouch = 1 << 17,
        SecondaryThumbRestTouch = 1 << 16,
        SecondaryThumbRestNearTouch = 1 << 17,
        PrimaryIndexTriggerTouch = 1 << 18,
        PrimaryIndexTriggerNearTouch = 1 << 19,
        PrimaryIndexTriggerSqueeze = 1 << 20,
        SecondaryIndexTriggerTouch = 1 << 21,
        SecondaryIndexTriggerNearTouch = 1 << 22,
        SecondaryIndexTriggerSqueeze = 1 << 23,
        PrimaryThumbstickHorizontal = 1 << 24,
        PrimaryThumbstickVertical = 1 << 25,
        SecondaryThumbstickHorizontal = 1 << 26,
        SecondaryThumbstickVertical = 1 << 27,
        DpadUp = 1 << 28,
        DpadDown = 1 << 29,
        DpadLeft = 1 << 30,
        DpadRight = 1 << 31,
    }
}
