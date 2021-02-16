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

#if MSFT_OPENXR
using Microsoft.MixedReality.OpenXR;
using Preview = Microsoft.MixedReality.OpenXR.Preview;
#endif // MSFT_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Open XR + XR SDK implementation of
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_MSFT_hand_interaction">XR_MSFT_hand_interaction</see> and
    /// <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_hand_tracking">XR_EXT_hand_tracking</see>.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Utilities.Handedness.Left, Utilities.Handedness.Right })]
    public class MicrosoftArticulatedHand : GenericXRSDKController, IMixedRealityHand
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MicrosoftArticulatedHand(TrackingState trackingState, Utilities.Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        {
#if MSFT_OPENXR
            handTracker = new Preview.HandTracker(controllerHandedness == Utilities.Handedness.Left ? Preview.Handedness.Left : Preview.Handedness.Right, Preview.HandPoseType.Tracked);
#endif // MSFT_OPENXR
        }

        private ArticulatedHandDefinition handDefinition;
        private ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

#if MSFT_OPENXR
        private static readonly Preview.HandJoint[] HandJoints = Enum.GetValues(typeof(Preview.HandJoint)) as Preview.HandJoint[];
        private readonly Preview.HandTracker handTracker = null;
        private readonly Preview.HandJointLocation[] locations = new Preview.HandJointLocation[Preview.HandTracker.JointCount];
#else
        private static readonly HandFinger[] handFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
#endif // MSFT_OPENXR

        #region IMixedRealityHand Implementation

        /// <inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose) => unityJointPoses.TryGetValue(joint, out pose);

        #endregion IMixedRealityHand Implementation

        /// <inheritdoc/>
        public override bool IsInPointingPose => HandDefinition.IsInPointingPose;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRArticulatedHand.UpdateController");

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

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRArticulatedHand.UpdateSingleAxisData");

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

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRArticulatedHand.UpdateButtonData");

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

        private static readonly ProfilerMarker UpdatePoseDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRArticulatedHand.UpdatePoseData");

        /// <inheritdoc />
        protected override void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdatePoseDataPerfMarker.Auto())
            {
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.IndexFinger:
                        HandDefinition?.UpdateCurrentIndexPose(interactionMapping);
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

        private static readonly ProfilerMarker UpdateHandDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRArticulatedHand.UpdateHandData");

        /// <summary>
        /// Update the hand data from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateHandData(InputDevice inputDevice)
        {
            using (UpdateHandDataPerfMarker.Auto())
            {
#if MSFT_OPENXR
#if MSFT_OPENXR_PRE_013
                if (handTracker != null && handTracker.TryLocateHandJoints(Preview.FrameTime.OnUpdate, locations))
#else
                if (handTracker != null && handTracker.TryLocateHandJoints(FrameTime.OnUpdate, locations))
#endif // MSFT_OPENXR_PRE_013
                {
                    foreach (Preview.HandJoint handJoint in HandJoints)
                    {
                        Preview.HandJointLocation handJointLocation = locations[(int)handJoint];

                        // We want input sources to follow the playspace, so fold in the playspace transform here to
                        // put the pose into world space.
                        Vector3 position = MixedRealityPlayspace.TransformPoint(handJointLocation.Position);
                        Quaternion rotation = MixedRealityPlayspace.Rotation * handJointLocation.Rotation;

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
#endif // MSFT_OPENXR

                    HandDefinition?.UpdateHandJoints(unityJointPoses);
                }
            }
        }

#if MSFT_OPENXR
        private TrackedHandJoint ConvertToTrackedHandJoint(Preview.HandJoint handJoint)
        {
            switch (handJoint)
            {
                case Preview.HandJoint.Palm: return TrackedHandJoint.Palm;
                case Preview.HandJoint.Wrist: return TrackedHandJoint.Wrist;

                case Preview.HandJoint.ThumbMetacarpal: return TrackedHandJoint.ThumbMetacarpalJoint;
                case Preview.HandJoint.ThumbProximal: return TrackedHandJoint.ThumbProximalJoint;
                case Preview.HandJoint.ThumbDistal: return TrackedHandJoint.ThumbDistalJoint;
                case Preview.HandJoint.ThumbTip: return TrackedHandJoint.ThumbTip;

                case Preview.HandJoint.IndexMetacarpal: return TrackedHandJoint.IndexMetacarpal;
                case Preview.HandJoint.IndexProximal: return TrackedHandJoint.IndexKnuckle;
                case Preview.HandJoint.IndexIntermediate: return TrackedHandJoint.IndexMiddleJoint;
                case Preview.HandJoint.IndexDistal: return TrackedHandJoint.IndexDistalJoint;
                case Preview.HandJoint.IndexTip: return TrackedHandJoint.IndexTip;

                case Preview.HandJoint.MiddleMetacarpal: return TrackedHandJoint.MiddleMetacarpal;
                case Preview.HandJoint.MiddleProximal: return TrackedHandJoint.MiddleKnuckle;
                case Preview.HandJoint.MiddleIntermediate: return TrackedHandJoint.MiddleMiddleJoint;
                case Preview.HandJoint.MiddleDistal: return TrackedHandJoint.MiddleDistalJoint;
                case Preview.HandJoint.MiddleTip: return TrackedHandJoint.MiddleTip;

                case Preview.HandJoint.RingMetacarpal: return TrackedHandJoint.RingMetacarpal;
                case Preview.HandJoint.RingProximal: return TrackedHandJoint.RingKnuckle;
                case Preview.HandJoint.RingIntermediate: return TrackedHandJoint.RingMiddleJoint;
                case Preview.HandJoint.RingDistal: return TrackedHandJoint.RingDistalJoint;
                case Preview.HandJoint.RingTip: return TrackedHandJoint.RingTip;

                case Preview.HandJoint.LittleMetacarpal: return TrackedHandJoint.PinkyMetacarpal;
                case Preview.HandJoint.LittleProximal: return TrackedHandJoint.PinkyKnuckle;
                case Preview.HandJoint.LittleIntermediate: return TrackedHandJoint.PinkyMiddleJoint;
                case Preview.HandJoint.LittleDistal: return TrackedHandJoint.PinkyDistalJoint;
                case Preview.HandJoint.LittleTip: return TrackedHandJoint.PinkyTip;

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
#endif // MSFT_OPENXR
    }
}
