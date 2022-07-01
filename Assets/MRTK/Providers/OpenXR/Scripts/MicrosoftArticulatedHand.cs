// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Open XR + XR SDK implementation of
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_MSFT_hand_interaction">XR_MSFT_hand_interaction</see> and
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_tracking">XR_EXT_hand_tracking</see>.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right })]
    public class MicrosoftArticulatedHand : GenericXRSDKController, IMixedRealityHand
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MicrosoftArticulatedHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        {
            handDefinition = Definition as ArticulatedHandDefinition;
            handMeshProvider = controllerHandedness == Handedness.Left ? OpenXRHandMeshProvider.Left : OpenXRHandMeshProvider.Right;
            handMeshProvider?.SetInputSource(inputSource);
            handJointProvider = new OpenXRHandJointProvider(controllerHandedness);
        }

        private readonly ArticulatedHandDefinition handDefinition;
        private readonly OpenXRHandMeshProvider handMeshProvider;
        private readonly OpenXRHandJointProvider handJointProvider;

        protected MixedRealityPose[] unityJointPoses = null;

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        // The rotation offset between the reported grip pose of a hand and the palm joint orientation.
        // These values were calculated by comparing the platform's reported grip pose and palm pose.
        private static readonly Quaternion rightPalmOffset = Quaternion.Inverse(new Quaternion(Mathf.Sqrt(0.125f), Mathf.Sqrt(0.125f), -Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f));
        private static readonly Quaternion leftPalmOffset = Quaternion.Inverse(new Quaternion(Mathf.Sqrt(0.125f), -Mathf.Sqrt(0.125f), Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f));

        #region IMixedRealityHand Implementation

        /// <inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose)
        {
            if (unityJointPoses != null)
            {
                pose = unityJointPoses[(int)joint];
                return pose != default(MixedRealityPose);
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        #endregion IMixedRealityHand Implementation

        /// <inheritdoc/>
        public override bool IsInPointingPose => handDefinition.IsInPointingPose;

        protected bool IsPinching => handDefinition.IsPinching;

        // Pinch was also used as grab, we want to allow hand-curl grab not just pinch.
        // Determine pinch and grab separately
        protected bool IsGrabbing => handDefinition.IsGrabbing;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdateController");

        // This bool is used to track whether or not we are receiving device data from the platform itself
        // If we aren't we will attempt to infer some common input actions from the hand joint data (i.e. the pinch gesture, pointer positions etc)
        private bool receivingDeviceInputs = false;

        /// <summary>
        /// The OpenXR plug-in uses extensions to expose all possible data, which might be surfaced through multiple input devices.
        /// This method is overridden to account for multiple input devices.
        /// </summary>
        /// <param name="inputDevice">The current input device to grab data from.</param>
        public override void UpdateController(InputDevice inputDevice)
        {
            if (!Enabled) { return; }

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for {GetType().Name}");
                Enabled = false;
            }

            using (UpdateControllerPerfMarker.Auto())
            {
                if (inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out _))
                {
                    base.UpdateController(inputDevice);

                    // We've gotten device data from the platform, don't attempt to infer other input actions
                    // from the hand joint data
                    receivingDeviceInputs = true;
                }

                if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
                {
                    UpdateHandData(hand);

                    // Updating the Index finger pose right after getting the hand data
                    // regardless of whether device data is present
                    for (int i = 0; i < Interactions?.Length; i++)
                    {
                        var interactionMapping = Interactions[i];
                        switch (interactionMapping.InputType)
                        {
                            case DeviceInputType.IndexFinger:
                                handDefinition?.UpdateCurrentIndexPose(interactionMapping);
                                break;
                        }
                    }

                    // If we aren't getting device data, infer input actions, velocity, etc from hand joint data
                    if (!receivingDeviceInputs)
                    {
                        for (int i = 0; i < Interactions?.Length; i++)
                        {
                            var interactionMapping = Interactions[i];
                            switch (interactionMapping.InputType)
                            {
                                case DeviceInputType.SpatialGrip:
                                    if (TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose currentGripPose))
                                    {
                                        interactionMapping.PoseData = currentGripPose;

                                        if (interactionMapping.Changed)
                                        {
                                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentGripPose);

                                            // Spatial Grip is also used as the basis for the source pose when device data is not provided
                                            // We need to rotate it by an offset to properly represent the source pose.
                                            MixedRealityPose CurrentControllerPose = currentGripPose;
                                            CurrentControllerPose.Rotation *= (ControllerHandedness == Handedness.Left ? leftPalmOffset : rightPalmOffset);

                                            CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                                            IsPositionAvailable = IsRotationAvailable = true;
                                        }
                                    }
                                    break;
                                case DeviceInputType.Select:
                                case DeviceInputType.TriggerPress:
                                case DeviceInputType.GripPress:
                                    interactionMapping.BoolData = IsPinching || IsGrabbing;

                                    if (interactionMapping.Changed)
                                    {
                                        if (interactionMapping.BoolData)
                                        {
                                            CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                        }
                                        else
                                        {
                                            CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                        }
                                    }
                                    break;
                                case DeviceInputType.SpatialPointer:
                                    handDefinition?.UpdatePointerPose(interactionMapping);
                                    break;
                                // Gotta do this only for non-AR devices
                                case DeviceInputType.ThumbStick:
                                    handDefinition?.UpdateCurrentTeleportPose(interactionMapping);
                                    break;
                            }
                        }

                        // Update the controller velocity based on the hand definition's calculations
                        handDefinition?.UpdateVelocity();
                        Velocity = (handDefinition?.Velocity).Value;
                        AngularVelocity = (handDefinition?.AngularVelocity).Value;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdateSingleAxisData");

        /// <inheritdoc />
        protected override void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.GripPress:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripAmount))
                        {
                            interactionMapping.BoolData = Mathf.Approximately(gripAmount, 1.0f);
                        }
                        break;
                    default:
                        base.UpdateSingleAxisData(interactionMapping, inputDevice);
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise bool input system event if it's available
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

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdateButtonData");

        /// <inheritdoc />
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Select:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed))
                        {
                            interactionMapping.BoolData = buttonPressed;
                        }
                        else
                        {
                            base.UpdateButtonData(interactionMapping, inputDevice);
                            return;
                        }
                        break;
                    default:
                        base.UpdateButtonData(interactionMapping, inputDevice);
                        return;
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

        private static readonly ProfilerMarker UpdatePoseDataPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdatePoseData");

        /// <inheritdoc />
        protected override void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdatePoseDataPerfMarker.Auto())
            {
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        if (inputDevice.TryGetFeatureValue(CustomUsages.PointerPosition, out currentPointerPosition))
                        {
                            currentPointerPose.Position = MixedRealityPlayspace.TransformPoint(currentPointerPosition);
                        }

                        if (inputDevice.TryGetFeatureValue(CustomUsages.PointerRotation, out currentPointerRotation))
                        {
                            currentPointerPose.Rotation = MixedRealityPlayspace.Rotation * currentPointerRotation;
                        }

                        interactionMapping.PoseData = currentPointerPose;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system event if it's enabled
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
                        }
                        break;
                    // IndexFinger is handled in ArticulatedHandDefinition, so we can safely skip this case.
                    case DeviceInputType.IndexFinger:
                        break;
                    default:
                        base.UpdatePoseData(interactionMapping, inputDevice);
                        break;
                }
            }
        }

        private static readonly ProfilerMarker UpdateHandDataPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdateHandData");

        /// <summary>
        /// Update the hand data from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateHandData(Hand hand)
        {
            using (UpdateHandDataPerfMarker.Auto())
            {
                handMeshProvider?.UpdateHandMesh();
                handJointProvider?.UpdateHandJoints(hand, ref unityJointPoses);
                handDefinition?.UpdateHandJoints(unityJointPoses);
            }
        }
    }
}
