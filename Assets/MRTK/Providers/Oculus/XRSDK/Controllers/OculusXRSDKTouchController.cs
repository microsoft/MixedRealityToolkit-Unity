// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.```

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.XR;

#if OCULUS_ENABLED
using Unity.XR.Oculus;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusTouch,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/OculusControllersTouch")]
    public class OculusXRSDKTouchController : GenericXRSDKController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusXRSDKTouchController(TrackingState trackingState, Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(6, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(7, "Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch),
            new MixedRealityInteractionMapping(8, "Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
            new MixedRealityInteractionMapping(10, "Button.Three Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(11, "Button.Four Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(12, "Button.Three Touch", AxisType.Digital, DeviceInputType.PrimaryButtonTouch),
            new MixedRealityInteractionMapping(13, "Button.Four Touch", AxisType.Digital, DeviceInputType.SecondaryButtonTouch),
            new MixedRealityInteractionMapping(14, "Button.Start Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(15, "Touch.PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch),
            new MixedRealityInteractionMapping(16, "Touch.PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(2, "Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(3, "Axis1D.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch),
            new MixedRealityInteractionMapping(4, "Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(5, "Axis1D.SecondaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(6, "Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(7, "Button.SecondaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch),
            new MixedRealityInteractionMapping(8, "Button.SecondaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
            new MixedRealityInteractionMapping(9, "Button.SecondaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
            new MixedRealityInteractionMapping(10, "Button.One Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(11, "Button.Two Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(12, "Button.One Touch", AxisType.Digital, DeviceInputType.PrimaryButtonTouch),
            new MixedRealityInteractionMapping(13, "Button.Two Touch", AxisType.Digital, DeviceInputType.SecondaryButtonTouch),
            new MixedRealityInteractionMapping(14, "Touch.SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch),
            new MixedRealityInteractionMapping(15, "Touch.SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch)
        };


        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] OculusXRSDKController.UpdateButtonData");
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                InputFeatureUsage<bool> buttonUsage;
                bool usingOculusButtonData = false;

#if OCULUS_ENABLED
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerTouch:
                        buttonUsage = OculusUsages.indexTouch;
                        usingOculusButtonData = true;
                        break;
                    case DeviceInputType.TriggerNearTouch:
                        buttonUsage = OculusUsages.indexTouch;
                        usingOculusButtonData = true;
                        break;
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.ThumbNearTouch:
                        buttonUsage = OculusUsages.thumbrest;
                        usingOculusButtonData = true;
                        break;
                }
#endif

                if (!usingOculusButtonData)
                {
                    base.UpdateButtonData(interactionMapping, inputDevice);
                }
                else
                {
                    if (inputDevice.TryGetFeatureValue(buttonUsage, out bool buttonPressed))
                    {
                        interactionMapping.BoolData = buttonPressed;
                    }

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        if (interactionMapping.BoolData)
                        {
                            CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                }
            }
        }
    }
}
