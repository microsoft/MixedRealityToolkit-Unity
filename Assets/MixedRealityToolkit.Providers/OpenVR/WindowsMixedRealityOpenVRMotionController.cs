// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Providers.OpenVR
{
    /// <summary>
    /// Open VR Implementation of the Windows Mixed Reality Motion Controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/MotionController")]
    public class WindowsMixedRealityOpenVRMotionController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public WindowsMixedRealityOpenVRMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            PointerOffsetAngle = -30f;
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(3, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(4, "Trigger Touch", AxisType.SingleAxis, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(5, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(6, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_17, ControllerMappingLibrary.AXIS_18, false, true),
            new MixedRealityInteractionMapping(7, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(8, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(9, "Menu Press", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(10, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, false, true),
            new MixedRealityInteractionMapping(11, "Thumbstick Press", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton18),
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press", AxisType.SingleAxis, DeviceInputType.ButtonPress, ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping(3, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(4, "Trigger Touch", AxisType.SingleAxis, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(5, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(6, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_19, ControllerMappingLibrary.AXIS_20, false, true),
            new MixedRealityInteractionMapping(7, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(8, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(9, "Menu Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(10, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, false, true),
            new MixedRealityInteractionMapping(11, "Thumbstick Press", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton19),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }
    }
}