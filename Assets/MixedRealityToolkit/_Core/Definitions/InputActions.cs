// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// InputActions defines the set of actions consumed internally within the MRTK.
    /// Denoting the available buttons / interactions that MRTK supports and exposed as events from the InputSystem.
    /// </summary>
    public enum InputActions
    {
        Raw = 0,
        LeftTrigger,
        LeftTriggerPress,
        LeftTriggerHold,
        RightTrigger,
        RightTriggerPressed,
        RightTriggerHold,
        LeftTouch,
        LeftTouchTouched,
        LeftTouchPressed,
        RightTouch,
        RightTouchTouched,
        RightTouchPressed,
        LeftThumbstick,
        LeftThumbstickPressed,
        RightThumbstick,
        RightThumbstickPressed,
        /// <summary>
        /// Grab
        /// </summary>
        ActionOne,
        /// <summary>
        /// Menu
        /// </summary>
        ActionTwo,
        /// <summary>
        /// Start
        /// </summary>
        ActionThree,
        ActionFour,
        ActionFive,
        ActionSix,
        ActionSeven,
        ActionEight,
        ActionNine,
        ActionTen,
        ActionEleven,
        ActionTwelve
    }
}