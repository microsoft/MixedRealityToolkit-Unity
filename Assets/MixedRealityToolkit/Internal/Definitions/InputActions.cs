// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.Internal.Definitions
{
    /// <summary>
    /// InputActions defines the set of actions consumed internally within the MRTK.
    /// Denoting the available buttons / interactions that MRTK supports and exposed as events from the InputSystem.
    /// </summary>
    [System.Flags]
    public enum InputActions
    {
        LeftTrigger             = 0,
        LeftTriggerPress        = 1,
        LeftTriggerHold         = 2,
        RightTrigger            = 4,
        RightTriggerPressed     = 8,
        RightTriggerHold        = 16,
        LeftTouch               = 32,
        LeftTouchTouched        = 64,
        LeftTouchPressed        = 128,
        RightTouch              = 256,
        RightTouchTouched       = 512,
        RightTouchPressed       = 1024,
        LeftThumbstick          = 2048,
        LeftThumbstickPressed   = 4096,
        RightThumbstick         = 8192,
        RightThumbstickPressed  = 16384,
        /// <summary>
        /// Grab
        /// </summary>
        ActionOne               = 32768,
        /// <summary>
        /// Menu
        /// </summary>
        ActionTwo               = 65536,
        /// <summary>
        /// Start
        /// </summary>
        ActionThree             = 131072,
        ActionFour              = 262144,
        ActionFive              = 524288,
        ActionSix               = 1048576,
        ActionSeven             = 2097152,
        ActionEight             = 4194304,
        ActionNine              = 8388608,
        ActionTen               = 16777216,
        ActionEleven            = 33554432,
        ActionTwelve            = 67108864,
        Raw                     = 134217728,
    }
}