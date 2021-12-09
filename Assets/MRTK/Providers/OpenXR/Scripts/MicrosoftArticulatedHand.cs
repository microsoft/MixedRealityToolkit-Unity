// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

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

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
            handTracker = controllerHandedness == Handedness.Left ? HandTracker.Left : HandTracker.Right;
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        }

        private readonly ArticulatedHandDefinition handDefinition;
        private readonly OpenXRHandMeshProvider handMeshProvider;

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private static readonly HandJoint[] HandJoints = Enum.GetValues(typeof(HandJoint)) as HandJoint[];
        private readonly HandTracker handTracker = null;
        private readonly HandJointLocation[] locations = new HandJointLocation[HandTracker.JointCount];
#else
        private static readonly HandFinger[] handFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

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

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
                if (handTracker != null && handTracker.TryLocateHandJoints(FrameTime.OnUpdate, locations))
                {
                    foreach (HandJoint handJoint in HandJoints)
                    {
                        HandJointLocation handJointLocation = locations[(int)handJoint];

                        // We want input sources to follow the playspace, so fold in the playspace transform here to
                        // put the pose into world space.
                        Vector3 position = MixedRealityPlayspace.TransformPoint(handJointLocation.Pose.position);
                        Quaternion rotation = MixedRealityPlayspace.Rotation * handJointLocation.Pose.rotation;

                        unityJointPoses[ConvertToTrackedHandJoint(handJoint)] = new MixedRealityPose(position, rotation);
                    }
#else
                if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
                {
                    foreach (HandFinger finger in handFingers)
                    {
                        if (hand.TryGetRootBone(out Bone rootBone))
                        {
                            ReadHandJoint(TrackedHandJoint.Wrist, rootBone);
                        }

                        if (hand.TryGetFingerBones(finger, fingerBones))
                        {
                            for (int i = 0; i < fingerBones.Count; i++)
                            {
                                ReadHandJoint(ConvertToTrackedHandJoint(finger, i), fingerBones[i]);
                            }
                        }
                    }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

                    handDefinition?.UpdateHandJoints(unityJointPoses);
                }
            }
        }

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private TrackedHandJoint ConvertToTrackedHandJoint(HandJoint handJoint)
        {
            switch (handJoint)
            {
                case HandJoint.Palm: return TrackedHandJoint.Palm;
                case HandJoint.Wrist: return TrackedHandJoint.Wrist;

                case HandJoint.ThumbMetacarpal: return TrackedHandJoint.ThumbMetacarpalJoint;
                case HandJoint.ThumbProximal: return TrackedHandJoint.ThumbProximalJoint;
                case HandJoint.ThumbDistal: return TrackedHandJoint.ThumbDistalJoint;
                case HandJoint.ThumbTip: return TrackedHandJoint.ThumbTip;

                case HandJoint.IndexMetacarpal: return TrackedHandJoint.IndexMetacarpal;
                case HandJoint.IndexProximal: return TrackedHandJoint.IndexKnuckle;
                case HandJoint.IndexIntermediate: return TrackedHandJoint.IndexMiddleJoint;
                case HandJoint.IndexDistal: return TrackedHandJoint.IndexDistalJoint;
                case HandJoint.IndexTip: return TrackedHandJoint.IndexTip;

                case HandJoint.MiddleMetacarpal: return TrackedHandJoint.MiddleMetacarpal;
                case HandJoint.MiddleProximal: return TrackedHandJoint.MiddleKnuckle;
                case HandJoint.MiddleIntermediate: return TrackedHandJoint.MiddleMiddleJoint;
                case HandJoint.MiddleDistal: return TrackedHandJoint.MiddleDistalJoint;
                case HandJoint.MiddleTip: return TrackedHandJoint.MiddleTip;

                case HandJoint.RingMetacarpal: return TrackedHandJoint.RingMetacarpal;
                case HandJoint.RingProximal: return TrackedHandJoint.RingKnuckle;
                case HandJoint.RingIntermediate: return TrackedHandJoint.RingMiddleJoint;
                case HandJoint.RingDistal: return TrackedHandJoint.RingDistalJoint;
                case HandJoint.RingTip: return TrackedHandJoint.RingTip;

                case HandJoint.LittleMetacarpal: return TrackedHandJoint.PinkyMetacarpal;
                case HandJoint.LittleProximal: return TrackedHandJoint.PinkyKnuckle;
                case HandJoint.LittleIntermediate: return TrackedHandJoint.PinkyMiddleJoint;
                case HandJoint.LittleDistal: return TrackedHandJoint.PinkyDistalJoint;
                case HandJoint.LittleTip: return TrackedHandJoint.PinkyTip;

                default: return TrackedHandJoint.None;
            }
        }
#else
        private void ReadHandJoint(TrackedHandJoint trackedHandJoint, Bone bone)
        {
            bool positionAvailable = bone.TryGetPosition(out Vector3 position);
            bool rotationAvailable = bone.TryGetRotation(out Quaternion rotation);

            if (positionAvailable && rotationAvailable)
            {
                // We want input sources to follow the playspace, so fold in the playspace transform here to
                // put the pose into world space.
                position = MixedRealityPlayspace.TransformPoint(position);
                rotation = MixedRealityPlayspace.Rotation * rotation;

                unityJointPoses[trackedHandJoint] = new MixedRealityPose(position, rotation);
            }
        }

        /// <summary>
        /// Converts a Unity finger bone into an MRTK hand joint.
        /// </summary>
        /// <remarks>
        /// For HoloLens 2, Unity provides four joints for the thumb and five joints for other fingers, in index order of metacarpal (0) to tip (4).
        /// The wrist joint is provided as the hand root bone.
        /// </remarks>
        /// <param name="finger">The Unity classification of the current finger.</param>
        /// <param name="index">The Unity index of the current finger bone.</param>
        /// <returns>The current Unity finger bone converted into an MRTK joint.</returns>
        private TrackedHandJoint ConvertToTrackedHandJoint(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return TrackedHandJoint.ThumbMetacarpalJoint + index;
                case HandFinger.Index: return TrackedHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return TrackedHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return TrackedHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return TrackedHandJoint.PinkyMetacarpal + index;
                default: return TrackedHandJoint.None;
            }
        }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
    }
}
