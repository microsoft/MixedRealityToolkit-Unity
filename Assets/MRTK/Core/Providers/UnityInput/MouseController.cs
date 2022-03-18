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
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new MouseControllerDefinition())
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<MixedRealityInputActionMapping> definitionInteractions = Definition?.GetDefaultMappings(ControllerHandedness);
                MixedRealityInteractionMapping[] defaultInteractions = new MixedRealityInteractionMapping[definitionInteractions.Count];
                for (int i = 0; i < definitionInteractions.Count; i++)
                {
                    defaultInteractions[i] = new MixedRealityInteractionMapping((uint)i, definitionInteractions[i], LegacyInputSupport[i]);
                }
                return defaultInteractions;
            }
        }

        private static readonly MixedRealityInteractionMappingLegacyInput[] LegacyInputSupport = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Mouse Position
            new MixedRealityInteractionMappingLegacyInput(), // Mouse Delta Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_3), // Mouse Scroll Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse0), // Left Mouse Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse1), // Right Mouse Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse2), // Mouse Button 2
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse3), // Mouse Button 3
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse4), // Mouse Button 4
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse5), // Mouse Button 5
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.Mouse6), // Mouse Button 6
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
