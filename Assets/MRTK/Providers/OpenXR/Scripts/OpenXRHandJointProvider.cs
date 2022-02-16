// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    internal class OpenXRHandJointProvider
    {
        public OpenXRHandJointProvider(Utilities.Handedness handedness)
        {
#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
            handTracker = handedness == Utilities.Handedness.Left ? HandTracker.Left : HandTracker.Right;
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        }

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private static readonly HandJoint[] HandJoints = Enum.GetValues(typeof(HandJoint)) as HandJoint[];
        private readonly HandTracker handTracker = null;
        private readonly HandJointLocation[] locations = new HandJointLocation[HandTracker.JointCount];
#else
        private static readonly HandFinger[] HandFingers = Enum.GetValues(typeof(HandFinger)) as HandFinger[];
        private readonly List<Bone> fingerBones = new List<Bone>();
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)

        public void UpdateHandJoints(InputDevice inputDevice, Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
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

                    jointPoses[ConvertToTrackedHandJoint(handJoint)] = new MixedRealityPose(position, rotation);
                }
#else
            if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
            {
                foreach (HandFinger finger in HandFingers)
                {
                    if (hand.TryGetRootBone(out Bone rootBone) && TryReadHandJoint(rootBone, out MixedRealityPose rootPose))
                    {
                        jointPoses[TrackedHandJoint.Palm] = rootPose;
                    }

                    if (hand.TryGetFingerBones(finger, fingerBones))
                    {
                        for (int i = 0; i < fingerBones.Count; i++)
                        {
                            if (TryReadHandJoint(fingerBones[i], out MixedRealityPose pose))
                            {
                                jointPoses[ConvertToTrackedHandJoint(finger, i)] = pose;
                            }
                        }
                    }
                }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
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
        private bool TryReadHandJoint(Bone bone, out MixedRealityPose pose)
        {
            bool positionAvailable = bone.TryGetPosition(out Vector3 position);
            bool rotationAvailable = bone.TryGetRotation(out Quaternion rotation);

            if (positionAvailable && rotationAvailable)
            {
                // We want input sources to follow the playspace, so fold in the playspace transform here to
                // put the pose into world space.
                position = MixedRealityPlayspace.TransformPoint(position);
                rotation = MixedRealityPlayspace.Rotation * rotation;

                pose = new MixedRealityPose(position, rotation);
                return true;
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
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
                case HandFinger.Thumb: return (index == 0) ? TrackedHandJoint.Wrist : TrackedHandJoint.ThumbMetacarpalJoint + index - 1;
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
