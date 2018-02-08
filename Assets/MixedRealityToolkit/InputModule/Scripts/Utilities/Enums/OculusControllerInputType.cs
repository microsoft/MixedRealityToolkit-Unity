// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.InputModule
{
    [Flags]
    public enum OculusControllerInputType
    {
        None                           = 0,
        ButtonOnePress                 = 1 << 0,
        ButtonOneTouch                 = 1 << 1,
        ButtonTwoPress                 = 1 << 2,
        ButtonTwoTouch                 = 1 << 3,
        ButtonThreePress               = 1 << 4,
        ButtonThreeTouch               = 1 << 5,
        ButtonFourPress                = 1 << 6,
        ButtonFourTouch                = 1 << 7,
        ButtonStart                    = 1 << 8,
        PrimaryThumbstickPress         = 1 << 9,
        PrimaryThumbstickTouch         = 1 << 10,
        PrimaryThumbstickNearTouch     = 1 << 11,
        SecondaryThumbstickPress       = 1 << 12,
        SecondaryThumbstickTouch       = 1 << 13,
        SecondaryThumbstickNearTouch   = 1 << 14,
        PrimaryThumbRestTouch          = 1 << 15,
        PrimaryThumbRestNearTouch      = 1 << 16,
        SecondaryThumbRestTouch        = 1 << 17,
        SecondaryThumbRestNearTouch    = 1 << 18,
        PrimaryIndexTriggerTouch       = 1 << 19,
        PrimaryIndexTriggerNearTouch   = 1 << 20,
        PrimaryIndexTriggerSqueeze     = 1 << 21,
        SecondaryIndexTriggerTouch     = 1 << 22,
        SecondaryIndexTriggerNearTouch = 1 << 23,
        SecondaryIndexTriggerSqueeze   = 1 << 24,
        PrimaryThumbstickHorizontal    = 1 << 25,
        PrimaryThumbstickVertical      = 1 << 26,
        SecondaryThumbstickHorizontal  = 1 << 27,
        SecondaryThumbstickVertical    = 1 << 28,
        DpadUp                         = 1 << 29,
        DpadDown                       = 1 << 30,
        DpadLeft                       = 1 << 31,
        DpadRight                      = 1 << 32,
    }
}
