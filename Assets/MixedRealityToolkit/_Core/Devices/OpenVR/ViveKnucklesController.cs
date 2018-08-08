// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public class ViveKnucklesController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public ViveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public new static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS9),
            new MixedRealityInteractionMapping(2, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.MIXEDREALITY_AXIS9),
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS11),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.MIXEDREALITY_AXIS1, ControllerMappingLibrary.MIXEDREALITY_AXIS2),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS20),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS22),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS24),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS26),
        };

        public new static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS10),
            new MixedRealityInteractionMapping(2, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.MIXEDREALITY_AXIS10),
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS12),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.MIXEDREALITY_AXIS4, ControllerMappingLibrary.MIXEDREALITY_AXIS5),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS21),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS23),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS25),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, ControllerMappingLibrary.MIXEDREALITY_AXIS27),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }
    }
}