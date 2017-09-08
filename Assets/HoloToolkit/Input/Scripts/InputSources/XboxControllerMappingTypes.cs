// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    [Flags]
    public enum XboxControllerMappingTypes
    {
        XboxLeftStickHorizontal = 0,
        XboxLeftStickVertical = 1,
        XboxRightStickHorizontal = 2,
        XboxRightStickVertical = 3,
        XboxDpadHorizontal = 4,
        XboxDpadVertical = 5,
        XboxLeftTrigger = 6,
        XboxRightTrigger = 7,
        XboxSharedTrigger = 8,
        XboxA = 9,
        XboxB = 10,
        XboxX = 11,
        XboxY = 12,
        XboxView = 13,
        XboxMenu = 14,
        XboxLeftBumper = 15,
        XboxRightBumper = 16,
        XboxLeftStickClick = 17,
        XboxRightStickClick = 18,
    }
}