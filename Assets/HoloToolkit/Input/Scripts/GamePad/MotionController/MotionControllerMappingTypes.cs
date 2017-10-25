// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Motion Controller axis and button types.
    /// </summary>
    [Flags]
    public enum MotionControllerMappingTypes
    {
        None = 0,
        LeftStickHorizontal = 1,
        LeftStickVertical = 2,
        LeftTouchPadHorizontal = 3,
        LeftTouchPadVertical = 4,
        RightStickHorizontal = 5,
        RightStickVertical = 6,
        RightTouchPadHorizontal = 7,
        RightTouchPadVertical = 8,
        LeftTrigger = 9,
        LeftTriggerPressed = 10,
        LeftTriggerPartiallyPressed = 11,
        RightTrigger = 12,
        RightTriggerPressed = 13,
        RightTriggerPartiallyPressed = 14,
        LeftMenu = 15,
        RightMenu = 16,
        LeftGrip = 17,
        RightGrip = 18,
        LeftStickClick = 19,
        RightStickClick = 20,
        LeftTouchPadTouched = 21,
        RightTouchPadTouched = 22,
        LeftTouchPadPressed = 23,
        RightTouchPadPressed = 24
    }
}
