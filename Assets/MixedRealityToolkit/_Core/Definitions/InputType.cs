// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The InputType defines the types of input exposed by a controller.
    /// Denoting the available buttons / interactions that a controller supports.
    /// </summary>
    public enum InputType
    {
        None = 0,
        Pointer,
        PointerPosition,
        PointerRotation,
        ButtonPress,
        ButtonTouch,
        Trigger,
        TriggerTouch,
        TriggerPress,
        Grip,
        GripTouch,
        GripPress,
        GripPosition,
        GripRotation,
        ThumbStick,
        ThumbStickPress,
        Touchpad,
        TouchpadTouch,
        TouchpadPress,
        Select,
        Voice,
        Start,
        Menu,
    }
}