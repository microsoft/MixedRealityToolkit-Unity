// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A pose source which represents a hand ray. This hand ray is constructed by deriving it from the
    /// palm and knuckle positions
    /// </summary>
    [Serializable]
    public class PolyfillHandRayPoseSource : HandBasedPoseSource
    {
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
        /// Tries to get the pose of the hand ray in world space by deriving it from the
        /// palm and knuckle positions
        /// </summary>
        public override bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();

            bool poseRetrieved = handNode.HasValue;
            poseRetrieved &= HandsAggregator != null && HandsAggregator.TryGetJoint(TrackedHandJoint.IndexProximal, handNode.Value, out knuckle);
            poseRetrieved &= HandsAggregator != null && HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, handNode.Value, out palm);

            // Tick the hand ray generator function. Uses index knuckle for position.
            if(poseRetrieved)
            {
                handRay.Update(knuckle.Position, -palm.Up, Camera.main.transform, Hand);
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
