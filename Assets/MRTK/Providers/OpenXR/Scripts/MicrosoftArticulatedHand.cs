// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System.Collections.Generic;
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

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        #region IMixedRealityHand Implementation

        /// <inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose) => unityJointPoses.TryGetValue(joint, out pose);

        #endregion IMixedRealityHand Implementation

        /// <inheritdoc/>
        public override bool IsInPointingPose => handDefinition.IsInPointingPose;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] MicrosoftArticulatedHand.UpdateController");

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
                if (inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 _))
                {
                    base.UpdateController(inputDevice);
                }
                else
                {
                    UpdateHandData(inputDevice);

                    for (int i = 0; i < Interactions?.Length; i++)
                    {
                        switch (Interactions[i].AxisType)
                        {
                            case AxisType.SixDof:
                                UpdatePoseData(Interactions[i], inputDevice);
                                break;
                        }
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
                    case DeviceInputType.IndexFinger:
                        handDefinition?.UpdateCurrentIndexPose(interactionMapping);
                        break;
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
        private void UpdateHandData(InputDevice inputDevice)
        {
            using (UpdateHandDataPerfMarker.Auto())
            {
                handMeshProvider?.UpdateHandMesh();
                handJointProvider?.UpdateHandJoints(inputDevice, unityJointPoses);
                handDefinition?.UpdateHandJoints(unityJointPoses);
            }
        }
    }
}
