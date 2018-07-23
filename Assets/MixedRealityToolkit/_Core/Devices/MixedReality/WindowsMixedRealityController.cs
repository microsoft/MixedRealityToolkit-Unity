// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    public class WindowsMixedRealityController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public WindowsMixedRealityController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The Windows Mixed Reality Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public static readonly MixedRealityInteractionMapping[] DefaultInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press",AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbstick Press ", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Trigger Pressed (Select)",AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trigger Touched", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Menu Pressed", AxisType.Digital, DeviceInputType.Menu, MixedRealityInputAction.None),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

#if UNITY_WSA

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Controller.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; private set; }

        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        private Vector3 currentGripPosition = Vector3.zero;
        private Quaternion currentGripRotation = Quaternion.identity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(InteractionSourceState interactionSourceState)
        {
            UpdateControllerData(interactionSourceState);

            Debug.Assert(Interactions != null, "No interaction configuration for controller");
            if (Interactions == null) { Enabled = false; }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdatePointerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                        UpdateTriggerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.ButtonPress:
                        UpdateGripData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                        UpdateMenuData(interactionSourceState, Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [WindowsMixedRealityController]");
                        Enabled = false;
                        break;
                }
            }

            LastSourceStateReading = interactionSourceState;
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            var lastState = TrackingState;
            var sourceKind = interactionSourceState.source.kind;

            if (sourceKind == InteractionSourceKind.Hand ||
               (sourceKind == InteractionSourceKind.Controller && interactionSourceState.source.supportsPointing))
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = interactionSourceState.sourcePose.TryGetPosition(out currentControllerPosition);

                if (IsPositionAvailable)
                {
                    IsPositionApproximate = (interactionSourceState.sourcePose.positionAccuracy == InteractionSourcePositionAccuracy.Approximate);
                }
                else
                {
                    IsPositionApproximate = false;
                }

                IsRotationAvailable = interactionSourceState.sourcePose.TryGetRotation(out currentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            lastControllerPose = currentControllerPose;
            currentControllerPose.Position = currentControllerPosition;
            currentControllerPose.Rotation = currentControllerRotation;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastControllerPose != currentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentControllerRotation);
                }
            }
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            interactionSourceState.sourcePose.TryGetPosition(out currentPointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out currentPointerRotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                currentPointerPose.Position = CameraCache.Main.transform.parent.TransformPoint(currentPointerPosition);
                currentPointerPose.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentPointerRotation.eulerAngles));
            }

            // Update the interaction data source
            interactionMapping.PoseData = currentPointerPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentPointerPose);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.AxisType)
            {
                case AxisType.SixDof:
                    {
                        interactionSourceState.sourcePose.TryGetPosition(out currentGripPosition, InteractionSourceNode.Grip);
                        interactionSourceState.sourcePose.TryGetRotation(out currentGripRotation, InteractionSourceNode.Grip);

                        if (CameraCache.Main.transform.parent != null)
                        {
                            currentGripPose.Position = CameraCache.Main.transform.parent.TransformPoint(currentGripPosition);
                            currentGripPose.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentGripRotation.eulerAngles));
                        }

                        // Update the interaction data source
                        interactionMapping.PoseData = currentGripPose;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentGripPose);
                        }
                    }
                    break;
                case AxisType.Digital:
                    {
                        //Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.grasped;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.grasped)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTouchPadData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        // Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.touchpadTouched;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.touchpadTouched)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        //Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.touchpadPressed;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.touchpadPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        // Update the interaction data source
                        interactionMapping.Vector2Data = interactionSourceState.touchpadPosition;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbStickData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        // Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.thumbstickPressed;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.thumbstickPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        // Update the interaction data source
                        interactionMapping.Vector2Data = interactionSourceState.thumbstickPosition;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.thumbstickPosition);
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTriggerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                case DeviceInputType.Select:
                    {
                        // Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.selectPressed;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.selectPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        // Update the interaction data source
                        interactionMapping.FloatData = interactionSourceState.selectPressedAmount;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.selectPressedAmount);
                        }
                        break;
                    }
                case DeviceInputType.TriggerTouch:
                    {
                        // Update the interaction data source
                        interactionMapping.BoolData = interactionSourceState.selectPressedAmount > 0;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system Event if it enabled
                            if (interactionSourceState.selectPressedAmount > 0)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Menu button state.
        /// </summary>
        /// <param name="interactionSourceState"></param>
        /// <param name="interactionMapping"></param>
        private void UpdateMenuData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            //Update the interaction data source
            interactionMapping.BoolData = interactionSourceState.menuPressed;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                if (interactionSourceState.menuPressed)
                {
                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Update data functions

#endif // UNITY_WSA
    }
}