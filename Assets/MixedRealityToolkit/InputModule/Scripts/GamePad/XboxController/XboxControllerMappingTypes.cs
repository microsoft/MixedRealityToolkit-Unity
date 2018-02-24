// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.InputModule.GamePad
{
    /// <summary>
    /// Xbox Controller axis and button types.
    /// </summary>
    [Flags]
    public enum XboxControllerMappingTypes
    {
        None = 0,
        XboxLeftStickHorizontal = 1,
        XboxLeftStickVertical = 2,
        XboxRightStickHorizontal = 3,
        XboxRightStickVertical = 4,
        XboxDpadHorizontal = 5,
        XboxDpadVertical = 6,
        XboxLeftTrigger = 7,
        XboxRightTrigger = 8,
        XboxSharedTrigger = 9,
        XboxA = 10,
        XboxB = 11,
        XboxX = 12,
        XboxY = 13,
        XboxView = 14,
        XboxMenu = 15,
        XboxLeftBumper = 16,
        XboxRightBumper = 17,
        XboxLeftStickClick = 18,
        XboxRightStickClick = 19
    }
}