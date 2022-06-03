// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Simple script that makes the attached GameObject follow the specified joint.
    /// Internal for now; TODO possibly made public in the future.
    /// </summary>
    /// <remarks>
    /// Packaging-wise, this should be considered part of the Hands feature. This also does
    /// not depend on XRI.
    /// </remarks>
    internal class FollowJoint : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The hand on which to track the joint.")]
        private Handedness hand;

        /// <summary>
        /// The hand on which to track the joint.
        /// </summary>
        protected Handedness Hand { get => hand; set => hand = value; }

        [SerializeField]
        [Tooltip("The specific joint to track.")]
        private TrackedHandJoint joint;

        /// <summary>
        /// The specific joint to track.
        /// </summary>
        protected TrackedHandJoint Joint { get => joint; set => joint = value; }

        private HandsAggregatorSubsystem handsAggregator;

        /// <summary>
        /// Cached reference to hands aggregator for efficient per-frame use.
        /// </summary>
        protected HandsAggregatorSubsystem HandsAggregator
        {
            get
            {
                if (handsAggregator == null)
                {
                    handsAggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
                }
                return handsAggregator;
            }
        }

        void Update()
        {
            XRNode? node = Hand.ToXRNode();
            if (node.HasValue && HandsAggregator != null && HandsAggregator.TryGetJoint(joint, node.Value, out var jointPose))
            {
                transform.SetPositionAndRotation(jointPose.Position, jointPose.Rotation);
            }
            else
            {
                // If we have no valid tracked joint, reset to local zero.
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}