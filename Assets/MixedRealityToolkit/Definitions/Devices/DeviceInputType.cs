// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    // TODO - Expand input list for additional controller types and have a filter defined by the controller
    /// <summary>
    /// The InputType defines the types of input exposed by a controller.
    /// Denoting the available buttons / interactions that a controller supports.
    /// </summary>
    public enum DeviceInputType
    {
        None = 0,
        Gaze,
        Voice,
        /// <summary>
        /// 6 Dof Pointer with position and rotation.
        /// </summary>
        SpatialPointer,
        /// <summary>
        /// 3 Dof Pointer with only position.
        /// </summary>
        PointerPosition,
        /// <summary>
        /// 3 Dof Pointer with only rotation.
        /// </summary>
        PointerRotation,
        PointerClick,
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
        SpatialGrip,
        /// <summary>
        /// 3 DoF Grip with only position.
        /// </summary>
        GripPosition,
        /// <summary>
        /// 3 Dof Grip with only rotation.
        /// </summary>
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
        Hand,
        Thumb,
        ThumbTouch,
        ThumbNearTouch,
        ThumbPress,
        IndexFinger,
        IndexFingerTouch,
        IndexFingerNearTouch,
        IndexFingerPress,
        MiddleFinger,
        MiddleFingerTouch,
        MiddleFingerNearTouch,
        MiddleFingerPress,
        RingFinger,
        RingFingerTouch,
        RingFingerNearTouch,
        RingFingerPress,
        PinkyFinger,
        PinkyFingerTouch,
        PinkyFingerNearTouch,
        PinkyFingerPress,
        DirectionalPad,
        Scroll,
    }
}