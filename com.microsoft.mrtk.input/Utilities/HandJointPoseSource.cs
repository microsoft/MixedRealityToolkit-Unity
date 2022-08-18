// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public class HandJointPoseSource : IPoseSource
    {
        private HandsAggregatorSubsystem handsAggregator;

        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        protected HandsAggregatorSubsystem HandsAggregator => handsAggregator ??= HandsUtils.GetSubsystem();

        [SerializeField]
        [Tooltip("The hand on which to track the joint.")]
        private Handedness hand;

        /// <summary>
        /// The hand on which to track the joint.
        /// </summary>
        internal Handedness Hand { get => hand; set => hand = value; }

        [SerializeField]
        [Tooltip("The specific joint to track.")]
        private TrackedHandJoint joint;

        /// <summary>
        /// The specific joint to track.
        /// </summary>
        internal TrackedHandJoint Joint { get => joint; set => joint = value; }

        /// <summary>
        /// A cache of the hand joint pose returned by the hands aggregator
        /// </summary>
        private HandJointPose cachedHandJointPose;

        public bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = hand.ToXRNode();
            bool poseRetrieved = handNode.HasValue && HandsAggregator != null && HandsAggregator.TryGetJoint(joint, handNode.Value, out cachedHandJointPose);
            pose.position = cachedHandJointPose.Position;
            pose.rotation = cachedHandJointPose.Rotation;

            return poseRetrieved;
        }
    }
}
