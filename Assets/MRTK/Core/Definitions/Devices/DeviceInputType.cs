// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// 6-DoF pointer with position and rotation.
        /// </summary>
        SpatialPointer,
        /// <summary>
        /// 3-DoF pointer with only position.
        /// </summary>
        PointerPosition,
        /// <summary>
        /// 3-DoF pointer with only rotation.
        /// </summary>
        PointerRotation,
        PointerClick,
        ButtonPress,
        ButtonTouch,
        ButtonNearTouch,
        Trigger,
        TriggerTouch,
        TriggerNearTouch,
        // TriggerPress, in some cases, maps to the grab/grasp gesture.
        TriggerPress,
        /// <summary>
        /// 6-DoF grip with position and rotation.
        /// </summary>
        SpatialGrip,
        /// <summary>
        /// 3-DoF grip with only position.
        /// </summary>
        GripPosition,
        /// <summary>
        /// 3-DoF grip with only rotation.
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
        /// <summary>
        /// Select, in some cases, maps to the pinch/airtap gesture.
        /// </summary>
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
        PrimaryButtonPress,
        PrimaryButtonTouch,
        PrimaryButtonNearTouch,
        SecondaryButtonPress,
        SecondaryButtonTouch,
        SecondaryButtonNearTouch,
        Grip,
        GripTouch,
        GripNearTouch,
        // GripPress, in some cases, maps to the grab/grasp gesture.
        GripPress,
    }
}