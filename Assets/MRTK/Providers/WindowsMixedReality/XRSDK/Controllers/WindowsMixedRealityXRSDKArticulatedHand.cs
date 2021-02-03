// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if WINDOWS_UWP
#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED
using Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// XR SDK implementation of Windows Mixed Reality articulated hands.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right })]
    public class WindowsMixedRealityXRSDKArticulatedHand : BaseWindowsMixedRealityXRSDKSource, IMixedRealityHand
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityXRSDKArticulatedHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        {
            handMeshProvider = new WindowsMixedRealityHandMeshProvider(this);
        }

        private ArticulatedHandDefinition handDefinition;
        private ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        private readonly Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private readonly WindowsMixedRealityHandMeshProvider handMeshProvider;

        private static readonly HandFinger[] handFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();

        // The rotation offset between the reported grip pose of a hand and the palm joint orientation.
        // These values were calculated by comparing the platform's reported grip pose and palm pose.
        private static readonly Quaternion rightPalmOffset = new Quaternion(Mathf.Sqrt(0.125f), Mathf.Sqrt(0.125f), -Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f);
        private static readonly Quaternion leftPalmOffset = new Quaternion(Mathf.Sqrt(0.125f), -Mathf.Sqrt(0.125f), Mathf.Sqrt(1.5f) / 2.0f, Mathf.Sqrt(1.5f) / 2.0f);

#if WINDOWS_UWP && WMR_ENABLED
        private readonly List<object> states = new List<object>();
#endif // WINDOWS_UWP && WMR_ENABLED

        #region IMixedRealityHand Implementation

        /// <inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, out MixedRealityPose pose) => unityJointPoses.TryGetValue(joint, out pose);

        #endregion IMixedRealityHand Implementation

        /// <inheritdoc/>
        public override bool IsInPointingPose => HandDefinition.IsInPointingPose;

        #region Update data functions

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityXRSDKArticulatedHand.UpdateController");

        /// <inheritdoc />
        public override void UpdateController(InputDevice inputDevice)
        {
            if (!Enabled) { return; }

            using (UpdateControllerPerfMarker.Auto())
            {
                base.UpdateController(inputDevice);

                UpdateHandData(inputDevice);

                for (int i = 0; i < Interactions?.Length; i++)
                {
                    switch (Interactions[i].InputType)
                    {
                        case DeviceInputType.IndexFinger:
                            HandDefinition?.UpdateCurrentIndexPose(Interactions[i]);
                            break;
                        case DeviceInputType.ThumbStick:
                            HandDefinition?.UpdateCurrentTeleportPose(Interactions[i]);
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateHandDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityXRSDKArticulatedHand.UpdateHandData");

        /// <summary>
        /// Update the hand data from the device.
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform.</param>
        private void UpdateHandData(InputDevice inputDevice)
        {
            using (UpdateHandDataPerfMarker.Auto())
            {
#if WINDOWS_UWP && WMR_ENABLED
                XRSubsystemHelpers.InputSubsystem?.GetCurrentSourceStates(states);

                foreach (SpatialInteractionSourceState sourceState in states)
                {
                    if (sourceState.Source.Handedness.ToMRTKHandedness() == ControllerHandedness)
                    {
                        handMeshProvider?.UpdateHandMesh(sourceState);
                        break;
                    }
                }
#endif // WINDOWS_UWP && WMR_ENABLED

                Hand hand;
                if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out hand))
                {
                    foreach (HandFinger finger in handFingers)
                    {
                        if (hand.TryGetFingerBones(finger, fingerBones))
                        {
                            for (int i = 0; i < fingerBones.Count; i++)
                            {
                                TrackedHandJoint trackedHandJoint = ConvertToTrackedHandJoint(finger, i);
                                Bone bone = fingerBones[i];

                                Vector3 position = Vector3.zero;
                                Quaternion rotation = Quaternion.identity;

                                bool positionAvailable = bone.TryGetPosition(out position);
                                bool rotationAvailable = bone.TryGetRotation(out rotation);

                                // If either position or rotation is available, use both pieces of data given.
                                // This might result in using a zeroed out position or rotation. Most likely,
                                // either both are available or both are unavailable.
                                if (positionAvailable || rotationAvailable)
                                {
                                    // We want input sources to follow the playspace, so fold in the playspace transform here to
                                    // put the controller pose into world space.
                                    position = MixedRealityPlayspace.TransformPoint(position);
                                    rotation = MixedRealityPlayspace.Rotation * rotation;

                                    unityJointPoses[trackedHandJoint] = new MixedRealityPose(position, rotation);
                                }
                            }

                            // Unity doesn't provide a palm joint, so we synthesize one here
                            MixedRealityPose palmPose = CurrentControllerPose;
                            palmPose.Rotation *= (ControllerHandedness == Handedness.Left ? leftPalmOffset : rightPalmOffset);
                            unityJointPoses[TrackedHandJoint.Palm] = palmPose;
                        }
                    }

                    HandDefinition?.UpdateHandJoints(unityJointPoses);
                }
            }
        }

        /// <summary>
        /// Converts a Unity finger bone into an MRTK hand joint.
        /// </summary>
        /// <remarks>
        /// <para>For HoloLens 2, Unity provides four joints per finger, in index order of metacarpal (0) to tip (4).
        /// The first joint for the thumb is the wrist joint. Palm joint is not provided.</para>
        /// </remarks>
        /// <param name="finger">The Unity classification of the current finger.</param>
        /// <param name="index">The Unity index of the current finger bone.</param>
        /// <returns>The current Unity finger bone converted into an MRTK joint.</returns>
        private TrackedHandJoint ConvertToTrackedHandJoint(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return (index == 0) ? TrackedHandJoint.Wrist : TrackedHandJoint.ThumbMetacarpalJoint + index - 1;
                case HandFinger.Index: return TrackedHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return TrackedHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return TrackedHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return TrackedHandJoint.PinkyMetacarpal + index;
                default: return TrackedHandJoint.None;
            }
        }

        #endregion Update data functions
    }
}
