// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
using Microsoft.MixedReality.OpenXR;
#else
using System.Collections.Generic;
#endif

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

        public void UpdateHandJoints(Hand hand, ref MixedRealityPose[] jointPoses)
        {
#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
            if (handTracker != null && handTracker.TryLocateHandJoints(FrameTime.OnUpdate, locations))
            {
                if (jointPoses == null)
                {
                    jointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
                }

                foreach (HandJoint handJoint in HandJoints)
                {
                    HandJointLocation handJointLocation = locations[(int)handJoint];

                    // We want input sources to follow the playspace, so fold in the playspace transform here to
                    // put the pose into world space.
                    Vector3 position = MixedRealityPlayspace.TransformPoint(handJointLocation.Pose.position);
                    Quaternion rotation = MixedRealityPlayspace.Rotation * handJointLocation.Pose.rotation;

                    jointPoses[ConvertToArrayIndex(handJoint)] = new MixedRealityPose(position, rotation);
                }
#else
            if (jointPoses == null)
            {
                jointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
            }

            foreach (HandFinger finger in HandFingers)
            {
                if (hand.TryGetRootBone(out Bone rootBone) && TryReadHandJoint(rootBone, out MixedRealityPose rootPose))
                {
                    jointPoses[(int)TrackedHandJoint.Palm] = rootPose;
                }

                if (hand.TryGetFingerBones(finger, fingerBones))
                {
                    for (int i = 0; i < fingerBones.Count; i++)
                    {
                        if (TryReadHandJoint(fingerBones[i], out MixedRealityPose pose))
                        {
                            jointPoses[ConvertToArrayIndex(finger, i)] = pose;
                        }
                    }
                }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
            }
        }

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private int ConvertToArrayIndex(HandJoint handJoint)
        {
            TrackedHandJoint trackedHandJoint;

            switch (handJoint)
            {
                case HandJoint.Palm: trackedHandJoint = TrackedHandJoint.Palm; break;
                case HandJoint.Wrist: trackedHandJoint = TrackedHandJoint.Wrist; break;

                case HandJoint.ThumbMetacarpal: trackedHandJoint = TrackedHandJoint.ThumbMetacarpalJoint; break;
                case HandJoint.ThumbProximal: trackedHandJoint = TrackedHandJoint.ThumbProximalJoint; break;
                case HandJoint.ThumbDistal: trackedHandJoint = TrackedHandJoint.ThumbDistalJoint; break;
                case HandJoint.ThumbTip: trackedHandJoint = TrackedHandJoint.ThumbTip; break;

                case HandJoint.IndexMetacarpal: trackedHandJoint = TrackedHandJoint.IndexMetacarpal; break;
                case HandJoint.IndexProximal: trackedHandJoint = TrackedHandJoint.IndexKnuckle; break;
                case HandJoint.IndexIntermediate: trackedHandJoint = TrackedHandJoint.IndexMiddleJoint; break;
                case HandJoint.IndexDistal: trackedHandJoint = TrackedHandJoint.IndexDistalJoint; break;
                case HandJoint.IndexTip: trackedHandJoint = TrackedHandJoint.IndexTip; break;

                case HandJoint.MiddleMetacarpal: trackedHandJoint = TrackedHandJoint.MiddleMetacarpal; break;
                case HandJoint.MiddleProximal: trackedHandJoint = TrackedHandJoint.MiddleKnuckle; break;
                case HandJoint.MiddleIntermediate: trackedHandJoint = TrackedHandJoint.MiddleMiddleJoint; break;
                case HandJoint.MiddleDistal: trackedHandJoint = TrackedHandJoint.MiddleDistalJoint; break;
                case HandJoint.MiddleTip: trackedHandJoint = TrackedHandJoint.MiddleTip; break;

                case HandJoint.RingMetacarpal: trackedHandJoint = TrackedHandJoint.RingMetacarpal; break;
                case HandJoint.RingProximal: trackedHandJoint = TrackedHandJoint.RingKnuckle; break;
                case HandJoint.RingIntermediate: trackedHandJoint = TrackedHandJoint.RingMiddleJoint; break;
                case HandJoint.RingDistal: trackedHandJoint = TrackedHandJoint.RingDistalJoint; break;
                case HandJoint.RingTip: trackedHandJoint = TrackedHandJoint.RingTip; break;

                case HandJoint.LittleMetacarpal: trackedHandJoint = TrackedHandJoint.PinkyMetacarpal; break;
                case HandJoint.LittleProximal: trackedHandJoint = TrackedHandJoint.PinkyKnuckle; break;
                case HandJoint.LittleIntermediate: trackedHandJoint = TrackedHandJoint.PinkyMiddleJoint; break;
                case HandJoint.LittleDistal: trackedHandJoint = TrackedHandJoint.PinkyDistalJoint; break;
                case HandJoint.LittleTip: trackedHandJoint = TrackedHandJoint.PinkyTip; break;

                default: trackedHandJoint = TrackedHandJoint.None; break;
            }

            return (int)trackedHandJoint;
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
        private int ConvertToArrayIndex(HandFinger finger, int index)
        {
            TrackedHandJoint trackedHandJoint;

            switch (finger)
            {
                case HandFinger.Thumb: trackedHandJoint = (index == 0) ? TrackedHandJoint.Wrist : TrackedHandJoint.ThumbMetacarpalJoint + index - 1; break;
                case HandFinger.Index: trackedHandJoint = TrackedHandJoint.IndexMetacarpal + index; break;
                case HandFinger.Middle: trackedHandJoint = TrackedHandJoint.MiddleMetacarpal + index; break;
                case HandFinger.Ring: trackedHandJoint = TrackedHandJoint.RingMetacarpal + index; break;
                case HandFinger.Pinky: trackedHandJoint = TrackedHandJoint.PinkyMetacarpal + index; break;
                default: trackedHandJoint = TrackedHandJoint.None; break;
            }

            return (int)trackedHandJoint;
        }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
    }
}
