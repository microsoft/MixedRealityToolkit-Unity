// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]

    /// <summary>
    /// A pose source which tracks a specific hand joint on a specific hand
    /// </summary>
    public class HandJointPoseSource : IPoseSource
    {
        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        protected HandsAggregatorSubsystem HandsAggregator => handsAggregator ??= HandsUtils.GetSubsystem();
        private HandsAggregatorSubsystem handsAggregator;

        [SerializeField]
        [Tooltip("The hand which the joint belongs to joint.")]
        private Handedness hand;

        /// <summary>
        /// The hand which the joint belongs to joint.
        /// </summary>
        internal Handedness Hand { get => hand; set => hand = value; }

        [SerializeField]
        [Tooltip("The specific joint whose pose we are retrieving.")]
        private TrackedHandJoint joint;

        /// <summary>
        /// The specific joint whose pose we are retrieving.
        /// </summary>
        internal TrackedHandJoint Joint { get => joint; set => joint = value; }

        /// <summary>
        /// A cache of the hand joint pose returned by the hands aggregator
        /// </summary>
        private HandJointPose cachedHandJointPose;

        /// <summary>
        /// Tries to get the pose of a specific hand joint on a specific hand
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();
            bool poseRetrieved = handNode.HasValue && HandsAggregator != null && HandsAggregator.TryGetJoint(Joint, handNode.Value, out cachedHandJointPose);
            if (poseRetrieved)
            {
                pose.position = cachedHandJointPose.Position;
                pose.rotation = cachedHandJointPose.Rotation;
            }
            else
            {
                pose = Pose.identity;
            }

            return poseRetrieved;
        }
    }
}
