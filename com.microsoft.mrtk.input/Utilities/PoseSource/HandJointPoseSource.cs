// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    /// <summary>
    /// A pose source which tracks a specific hand joint on a specific hand
    /// </summary>
    public class HandJointPoseSource : HandBasedPoseSource
    {
        [SerializeField]
        [Tooltip("The specific joint whose pose we are retrieving.")]
        private TrackedHandJoint joint;

        /// <summary>
        /// The specific joint whose pose we are retrieving.
        /// </summary>
        public TrackedHandJoint Joint { get => joint; set => joint = value; }

        /// <summary>
        /// Tries to get the pose of a specific hand joint on a specific hand in worldspace.
        /// </summary>
        public override bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();
            if (handNode.HasValue
                && XRSubsystemHelpers.HandsAggregator != null
                && XRSubsystemHelpers.HandsAggregator.TryGetJoint(Joint, handNode.Value, out HandJointPose handJointPose))
            {
                // Hand Joints are already returned by the subsystem in worldspace, we don't have to do any transformations
                pose.position = handJointPose.Position;
                pose.rotation = handJointPose.Rotation;
                return true;
            }

            pose = Pose.identity;
            return false;
        }
    }
}
