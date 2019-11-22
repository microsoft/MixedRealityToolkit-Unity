// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    [MixedRealityController(
        SupportedControllerType.GenericUnity,
        new[] { Handedness.None },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class GenericJoystickController : BaseController
    {
        public GenericJoystickController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The pointer's offset angle.
        /// </summary>
        public float PointerOffsetAngle { get; protected set; } = 0f;

        private Vector2 dualAxisPosition = Vector2.zero;
        private MixedRealityPose pointerOffsetPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The current position of this controller.
        /// </summary>
        protected Vector3 CurrentControllerPosition = Vector3.zero;

        /// <summary>
        /// The current rotation of this controller.
        /// </summary>
        protected Quaternion CurrentControllerRotation = Quaternion.identity;

        /// <summary>
        /// The previous pose of this controller.
        /// </summary>
        protected MixedRealityPose LastControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The current pose of this controller.
        /// </summary>
        protected MixedRealityPose CurrentControllerPose = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            // Generic Unity controllers will not have default interactions
        }

        /// <summary>
        /// Update the controller data from Unity's Input Manager
        /// </summary>
        public virtual void UpdateController()
        {
            if (!Enabled) { return; }

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for {GetType().Name}");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].AxisType)
                {
                    case AxisType.None:
                        break;
                    case AxisType.Digital:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case AxisType.SingleAxis:
                        UpdateSingleAxisData(Interactions[i]);
                        break;
                    case AxisType.DualAxis:
                        UpdateDualAxisData(Interactions[i]);
                        break;
                    case AxisType.SixDof:
                        UpdatePoseData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [{GetType().Name}]");
                        break;
                }
            }
        }

        /// <summary>
        /// Update an Interaction Bool data type from a Bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// Also raises a "Pressed" event while pressed
        /// </remarks>
        protected void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                    interactionMapping.BoolData = UInput.GetAxisRaw(interactionMapping.AxisCodeX).Equals(1);
                    break;
                case DeviceInputType.TriggerNearTouch:
                case DeviceInputType.ThumbNearTouch:
                case DeviceInputType.IndexFingerNearTouch:
                case DeviceInputType.MiddleFingerNearTouch:
                case DeviceInputType.RingFingerNearTouch:
                case DeviceInputType.PinkyFingerNearTouch:
                    interactionMapping.BoolData = !UInput.GetAxisRaw(interactionMapping.AxisCodeX).Equals(0);
                    break;
                default:
                    interactionMapping.BoolData = UInput.GetKey(interactionMapping.KeyCode);
                    break;
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

        /// <summary>
        /// Update an Interaction Float data type from a SingleAxis (float) input 
        /// </summary>
        /// <remarks>
        /// Raises a Float Input Changed event when the float data changes
        /// </remarks>
        protected void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            var singleAxisValue = UInput.GetAxisRaw(interactionMapping.AxisCodeX);

            if (interactionMapping.InputType == DeviceInputType.TriggerPress)
            {
                interactionMapping.BoolData = singleAxisValue.Equals(1);

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
            else
            {
                // Update the interaction data source
                interactionMapping.FloatData = singleAxisValue;

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                }
            }
        }

        /// <summary>
        /// Update the Touchpad / Thumbstick input from the device (in OpenVR, touchpad and thumbstick are the same input control)
        /// </summary>
        protected void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            dualAxisPosition.x = UInput.GetAxisRaw(interactionMapping.AxisCodeX);
            dualAxisPosition.y = UInput.GetAxisRaw(interactionMapping.AxisCodeY);

            // Update the interaction data source
            interactionMapping.Vector2Data = dualAxisPosition;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
            }
        }

        /// <summary>
        /// Update Spatial Pointer Data.
        /// </summary>
        protected void UpdatePoseData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            if (interactionMapping.InputType == DeviceInputType.SpatialPointer)
            {
                pointerOffsetPose.Position = CurrentControllerPose.Position;
                pointerOffsetPose.Rotation = CurrentControllerPose.Rotation * Quaternion.AngleAxis(PointerOffsetAngle, Vector3.left);

                // Update the interaction data source
                interactionMapping.PoseData = pointerOffsetPose;
            }
            else if (interactionMapping.InputType == DeviceInputType.SpatialGrip)
            {
                // Update the interaction data source
                interactionMapping.PoseData = CurrentControllerPose;
            }
            else
            {
                Debug.LogWarning("Unhandled Interaction");
                return;
            }

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }
    }
}
