// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public class ViveWandController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public ViveWandController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        //TODO - Update defaults
        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public new static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, "VIVE_TRIGGER_LEFT_CONTROLLER"),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, "VIVE_TRIGGER_LEFT_CONTROLLER"),
            new MixedRealityInteractionMapping(5, "Grip Press", AxisType.SingleAxis, DeviceInputType.Grip, MixedRealityInputAction.None, KeyCode.None, "VIVE_GRIP_LEFT_CONTROLLER"),
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(13, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
        };

        public new static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, "VIVE_TRIGGER_RIGHT_CONTROLLER"),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, "VIVE_TRIGGER_RIGHT_CONTROLLER"),
            new MixedRealityInteractionMapping(5, "Grip Press", AxisType.SingleAxis, DeviceInputType.Grip, MixedRealityInputAction.None, KeyCode.None, "VIVE_GRIP_RIGHT_CONTROLLER"),
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(13, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerMappingLibrary.GetInputManagerMappings(GetType().FullName);

        #endregion Base override configuration
    }
}
