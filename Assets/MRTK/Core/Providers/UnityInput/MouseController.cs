// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    [MixedRealityController(SupportedControllerType.Mouse, new[] { Handedness.Any })]
    public class MouseController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState">The controller's tracking state.</param>
        /// <param name="controllerHandedness">The handedness (ex: right) of the controller.</param>
        /// <param name="inputSource">The controller's input source.</param>
        /// <param name="interactions">The set of interactions supported by this controller.</param>
        public MouseController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null) : base(trackingState, controllerHandedness, inputSource, interactions)
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInteractionMapping(2, "Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll, ControllerMappingLibrary.AXIS_3),
            new MixedRealityInteractionMapping(3, "Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse0),
            new MixedRealityInteractionMapping(4, "Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse1),
            new MixedRealityInteractionMapping(5, "Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse2),
            new MixedRealityInteractionMapping(6, "Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse3),
            new MixedRealityInteractionMapping(7, "Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse4),
            new MixedRealityInteractionMapping(8, "Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse5),
            new MixedRealityInteractionMapping(9, "Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.Mouse6),
        };

        private MixedRealityPose controllerPose = MixedRealityPose.ZeroIdentity;

        private IMixedRealityMouseDeviceManager mouseDeviceManager = null;

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] MouseController.Update");

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!UInput.mousePresent) { return; }

                if (mouseDeviceManager == null)
                {
                    mouseDeviceManager = CoreServices.GetInputSystemDataProvider<IMixedRealityMouseDeviceManager>();
                }

                // Bail early if our mouse isn't in our game window.
                if (UInput.mousePosition.x < 0 ||
                    UInput.mousePosition.y < 0 ||
                    UInput.mousePosition.x > Screen.width ||
                    UInput.mousePosition.y > Screen.height)
                {
                    return;
                }

                for (int i = 0; i < Interactions.Length; i++)
                {
                    if ((Interactions[i].InputType == DeviceInputType.SpatialPointer) ||
                        (Interactions[i].InputType == DeviceInputType.PointerPosition))
                    {
                        Vector3 mouseDelta = Vector3.zero;
                        mouseDelta.x = -UInput.GetAxis("Mouse Y");
                        mouseDelta.y = UInput.GetAxis("Mouse X");
                        if (mouseDeviceManager != null)
                        {
                            // Apply cursor speed.
                            mouseDelta *= mouseDeviceManager.CursorSpeed;
                        }

                        if (Interactions[i].InputType == DeviceInputType.SpatialPointer)
                        {
                            // Spatial pointer raises Pose events
                            controllerPose = MixedRealityPose.ZeroIdentity;
                            controllerPose.Rotation = Quaternion.Euler(mouseDelta);
                            Interactions[i].PoseData = controllerPose;

                            if (Interactions[i].Changed)
                            {
                                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].PoseData);
                            }
                        }
                        else
                        {
                            // Pointer position raises position events
                            Interactions[i].Vector2Data = mouseDelta;

                            if (Interactions[i].Changed)
                            {
                                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
                            }
                        }
                    }

                    if (Interactions[i].AxisType == AxisType.Digital)
                    {
                        var keyButton = UInput.GetKey(Interactions[i].KeyCode);

                        // Update the interaction data source
                        Interactions[i].BoolData = keyButton;

                        // If our value changed raise it.
                        if (Interactions[i].Changed)
                        {
                            // Raise input system event if it's enabled
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                    }

                    if (Interactions[i].InputType == DeviceInputType.Scroll)
                    {
                        Vector2 wheelDelta = UInput.mouseScrollDelta;
                        if (mouseDeviceManager != null)
                        {
                            // Apply wheel speed.
                            wheelDelta *= mouseDeviceManager.WheelSpeed;
                        }

                        Interactions[i].Vector2Data = wheelDelta;

                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
                        }
                    }
                }
            }
        }
    }
}
