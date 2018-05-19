// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The InputType defines the types of input exposed by a controller or Input Source.
    /// </summary>
    public enum InputType
    {
        None = 0,
        Voice,
        Pointer,
        PointerPosition,
        PointerRotation,
        ButtonPress,
        ButtonTouch,
        ButtonNearTouch,
        Trigger,
        TriggerTouch,
        TriggerNearTouch,
        TriggerPress,
        Grip,
        GripTouch,
        GripNearTouch,
        GripPress,
        GripPosition,
        GripRotation,
        ThumbStick,
        ThumbStickPress,
        ThumbStickTouch,
        ThumbStickNearTouch,
        Touchpad,
        TouchpadTouch,
        TouchpadNearTouch,
        TouchpadPress,
        Select,
        Start,
        Menu,
        Thumb,
        IndexFinger,
        MiddleFinger,
        RingFinger,
        PinkyFinger,
    }
}