// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
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

        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses)
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
            if (inputDevice.TryGetFeatureValue(CommonUsages.handData, out Hand hand))
            {
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
                }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
            }
        }

#if MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
        private int ConvertToArrayIndex(HandJoint handJoint)
        {
            switch (handJoint)
            {
                case HandJoint.Palm: return (int)TrackedHandJoint.Palm;
                case HandJoint.Wrist: return (int)TrackedHandJoint.Wrist;

                case HandJoint.ThumbMetacarpal: return (int)TrackedHandJoint.ThumbMetacarpalJoint;
                case HandJoint.ThumbProximal: return (int)TrackedHandJoint.ThumbProximalJoint;
                case HandJoint.ThumbDistal: return (int)TrackedHandJoint.ThumbDistalJoint;
                case HandJoint.ThumbTip: return (int)TrackedHandJoint.ThumbTip;

                case HandJoint.IndexMetacarpal: return (int)TrackedHandJoint.IndexMetacarpal;
                case HandJoint.IndexProximal: return (int)TrackedHandJoint.IndexKnuckle;
                case HandJoint.IndexIntermediate: return (int)TrackedHandJoint.IndexMiddleJoint;
                case HandJoint.IndexDistal: return (int)TrackedHandJoint.IndexDistalJoint;
                case HandJoint.IndexTip: return (int)TrackedHandJoint.IndexTip;

                case HandJoint.MiddleMetacarpal: return (int)TrackedHandJoint.MiddleMetacarpal;
                case HandJoint.MiddleProximal: return (int)TrackedHandJoint.MiddleKnuckle;
                case HandJoint.MiddleIntermediate: return (int)TrackedHandJoint.MiddleMiddleJoint;
                case HandJoint.MiddleDistal: return (int)TrackedHandJoint.MiddleDistalJoint;
                case HandJoint.MiddleTip: return (int)TrackedHandJoint.MiddleTip;

                case HandJoint.RingMetacarpal: return (int)TrackedHandJoint.RingMetacarpal;
                case HandJoint.RingProximal: return (int)TrackedHandJoint.RingKnuckle;
                case HandJoint.RingIntermediate: return (int)TrackedHandJoint.RingMiddleJoint;
                case HandJoint.RingDistal: return (int)TrackedHandJoint.RingDistalJoint;
                case HandJoint.RingTip: return (int)TrackedHandJoint.RingTip;

                case HandJoint.LittleMetacarpal: return (int)TrackedHandJoint.PinkyMetacarpal;
                case HandJoint.LittleProximal: return (int)TrackedHandJoint.PinkyKnuckle;
                case HandJoint.LittleIntermediate: return (int)TrackedHandJoint.PinkyMiddleJoint;
                case HandJoint.LittleDistal: return (int)TrackedHandJoint.PinkyDistalJoint;
                case HandJoint.LittleTip: return (int)TrackedHandJoint.PinkyTip;

                default: return (int)TrackedHandJoint.None;
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
        private int ConvertToArrayIndex(HandFinger finger, int index)
        {
            switch (finger)
            {
                case HandFinger.Thumb: return (index == 0) ? (int)TrackedHandJoint.Wrist : (int)TrackedHandJoint.ThumbMetacarpalJoint + index - 1;
                case HandFinger.Index: return (int)TrackedHandJoint.IndexMetacarpal + index;
                case HandFinger.Middle: return (int)TrackedHandJoint.MiddleMetacarpal + index;
                case HandFinger.Ring: return (int)TrackedHandJoint.RingMetacarpal + index;
                case HandFinger.Pinky: return (int)TrackedHandJoint.PinkyMetacarpal + index;
                default: return (int)TrackedHandJoint.None;
            }
        }
#endif // MSFT_OPENXR && (UNITY_STANDALONE_WIN || UNITY_WSA)
    }
}
