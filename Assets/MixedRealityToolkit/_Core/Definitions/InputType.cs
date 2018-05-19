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
        Voice,
        /// <summary>
        /// 6 Dof Pointer with position and rotation.
        /// </summary>
        Pointer,
        /// <summary>
        /// 3 Dof Pointer with only position.
        /// </summary>
        PointerPosition,
        /// <summary>
        /// 3 Dof Pointer with only rotation.
        /// </summary>
        PointerRotation,
        ButtonPress,
        ButtonTouch,
        ButtonNearTouch,
        Trigger,
        TriggerTouch,
        TriggerNearTouch,
        TriggerPress,
        /// <summary>
        /// 6 DoF Grip with position and rotation.
        /// </summary>
        Grip,
        /// <summary>
        /// 3 DoF Grip with only position.
        /// </summary>
        GripPosition,
        /// <summary>
        /// 3 Dof Grip with only rotation.
        /// </summary>
        GripRotation,
        GripPress,
        GripTouch,
        GripNearTouch,
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