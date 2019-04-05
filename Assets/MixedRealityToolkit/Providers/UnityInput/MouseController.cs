// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public MouseController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll, ControllerMappingLibrary.AXIS_3),
            new MixedRealityInteractionMapping(3, "Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse0),
            new MixedRealityInteractionMapping(4, "Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse1),
            new MixedRealityInteractionMapping(5, "Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse2),
            new MixedRealityInteractionMapping(6, "Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse3),
            new MixedRealityInteractionMapping(7, "Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse4),
            new MixedRealityInteractionMapping(8, "Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse5),
            new MixedRealityInteractionMapping(9, "Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.Mouse6),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private MixedRealityPose controllerPose = MixedRealityPose.ZeroIdentity;
        private Vector2 mouseDelta;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            if (!UInput.mousePresent) { return; }

            // Bail early if our mouse isn't in our game window.
            if (UInput.mousePosition.x < 0 ||
                UInput.mousePosition.y < 0 ||
                UInput.mousePosition.x > Screen.width ||
                UInput.mousePosition.y > Screen.height)
            {
                return;
            }

            if (InputSource.Pointers[0].BaseCursor != null)
            {
                controllerPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                controllerPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
            }

            mouseDelta.x = -UInput.GetAxis("Mouse Y");
            mouseDelta.y = UInput.GetAxis("Mouse X");
            MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, mouseDelta);
            MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, controllerPose);
            MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, UInput.mouseScrollDelta);

            for (int i = 0; i < Interactions.Length; i++)
            {
                if (Interactions[i].InputType == DeviceInputType.SpatialPointer)
                {
                    Interactions[i].PoseData = controllerPose;

                    if (Interactions[i].Changed)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].PoseData);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.PointerPosition)
                {
                    Interactions[i].Vector2Data = mouseDelta;

                    if (Interactions[i].Changed)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
                    }
                }

                if (Interactions[i].InputType == DeviceInputType.Scroll)
                {
                    Interactions[i].Vector2Data = UInput.mouseScrollDelta;

                    if (Interactions[i].Changed)
                    {
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, Interactions[i].Vector2Data);
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
                        // Raise input system Event if it enabled
                        if (Interactions[i].BoolData)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                        }
                    }
                }
            }
        }
    }
}
