// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.ViveKnuckles,
        new[] { Handedness.Left, Handedness.Right })]
    public class ViveKnucklesController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ViveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.AXIS_20),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.AXIS_22),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.AXIS_24),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.AXIS_26),
        };


        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.AXIS_21),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.AXIS_23),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.AXIS_25),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.AXIS_27),
        };
    }
}