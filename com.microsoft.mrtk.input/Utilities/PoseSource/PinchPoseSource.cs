// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A pose source which tries to obtain the pinch pose from a hand specified by the <see cref="HandBasedPoseSource.Hand"/> property.
    /// </summary>
    [Serializable]
    public class PinchPoseSource : HandBasedPoseSource
    {
        /// <summary>
        /// Tries to get the pinch pose of a specific hand.
        /// </summary>
        public override bool TryGetPose(out Pose pose)
        {
            XRNode? handNode = Hand.ToXRNode();
            if (handNode.HasValue
                && XRSubsystemHelpers.HandsAggregator != null
                && XRSubsystemHelpers.HandsAggregator.TryGetPinchingPoint(handNode.Value, out HandJointPose pinchPose))
            {
                pose.position = pinchPose.Position;
                pose.rotation = pinchPose.Rotation;
                return true;
            }

            pose = Pose.identity;
            return false;
        }
    }
}
