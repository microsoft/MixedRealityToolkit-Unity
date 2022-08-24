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
    /// A pose source which represents a hand ray. This hand ray is constructed by deriving it from the
    /// palm and knuckle positions
    /// </summary>
    public class PolyfillHandRayPoseSource : IPoseSource
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

        // The Hand Ray used to calculate the polyfill.
        private HandRay handRay = new HandRay();

        /// <summary>
        /// A cache of the knuckle joint pose returned by the hands aggregator.
        /// </summary>
        private HandJointPose knuckle;

        /// <summary>
        /// A cache of the knuckle joint pose returned by the hands aggregator.
        /// </summary>
        private HandJointPose palm;

        /// <summary>
        /// Tries to get the pose of the hand ray by deriving it from the
        /// palm and knuckle positions
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();

            bool poseRetrieved = handNode.HasValue;
            poseRetrieved &= HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, handNode.Value, out knuckle);
            poseRetrieved &= HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, handNode.Value, out palm);

            // Tick the hand ray generator function. Uses index knuckle for position.
            if(poseRetrieved)
            {
                handRay.Update(knuckle.Position, -palm.Up, CameraCache.Main.transform, Hand);
                Ray ray = handRay.Ray;

                // controllerState is in rig-local space, our ray generator works in worldspace!
                pose.position = PlayspaceUtilities.ReferenceTransform.InverseTransformPoint(ray.origin);
                pose.rotation = Quaternion.LookRotation(PlayspaceUtilities.ReferenceTransform.InverseTransformVector(ray.direction),
                                                                   PlayspaceUtilities.ReferenceTransform.InverseTransformVector(palm.Up));
            }
            else
            {
                pose = Pose.identity;
            }

            return poseRetrieved;
        }
    }
}
