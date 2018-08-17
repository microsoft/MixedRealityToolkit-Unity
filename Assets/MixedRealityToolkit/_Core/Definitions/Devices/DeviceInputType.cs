// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    // TODO - Expand input list for additional controller types and have a filter defined by the controller
    /// <summary>
    /// The InputType defines the types of input exposed by a controller.
    /// Denoting the available buttons / interactions that a controller supports.
    /// </summary>
    public enum DeviceInputType
    {
        //Physical Input Type       -       Recommended Axis Type
        None = 0,
        Gaze,
        Voice,
        /// <summary>
        /// 6 Dof Pointer with position and rotation.
        /// </summary>
        SpatialPointer,                     // SixDof Axis
        /// <summary>
        /// 3 Dof Pointer with only position.
        /// </summary>
        PointerPosition,                    // ThreeDofPosition Axis
        /// <summary>
        /// 3 Dof Pointer with only rotation.
        /// </summary>
        PointerRotation,                    // ThreeDofRotation Axis
        PointerClick,                       // Digital
        ButtonPress,                        // Digital
        ButtonTouch,                        // Digital
        ButtonNearTouch,                    // Digital
        Trigger,                            // Single Axis
        TriggerTouch,                       // Digital
        TriggerNearTouch,                   // Digital
        TriggerPress,                       // Digital
        /// <summary>
        /// 6 DoF Grip with position and rotation.
        /// </summary>
        SpatialGrip,                        // SixDof Axis
        /// <summary>
        /// 3 DoF Grip with only position.
        /// </summary>
        GripPosition,                       // ThreeDofPosition Axis
        /// <summary>
        /// 3 Dof Grip with only rotation.
        /// </summary>
        GripRotation,                       // ThreeDofRotation Axis
        ThumbStick,                         // Dual Axis
        ThumbStickPress,                    // Digital
        ThumbStickTouch,                    // Digital
        ThumbStickNearTouch,                // Digital
        Touchpad,                           // Dual Axis
        TouchpadTouch,                      // Digital
        TouchpadNearTouch,                  // Digital
        TouchpadPress,                      // Digital
        Select,                             // Digital
        Start,                              // Digital
        Menu,                               // Digital
        Hand,
        Thumb,                              // Single Axis
        ThumbTouch,                         // Digital
        ThumbNearTouch,                     // Digital
        ThumbPress,                         // Digital
        IndexFinger,
        IndexFingerTouch,                   // Digital
        IndexFingerNearTouch,               // Digital
        IndexFingerPress,                   // Digital
        MiddleFinger,                       // Single Axis
        MiddleFingerTouch,                  // Digital
        MiddleFingerNearTouch,              // Digital
        MiddleFingerPress,                  // Digital
        RingFinger,                         // Single Axis
        RingFingerTouch,                    // Digital
        RingFingerNearTouch,                // Digital
        RingFingerPress,                    // Digital
        PinkyFinger,                        // Single Axis
        PinkyFingerTouch,                   // Digital
        PinkyFingerNearTouch,               // Digital
        PinkyFingerPress,                   // Digital
        DPad,                               // Dual Axis
    }
}