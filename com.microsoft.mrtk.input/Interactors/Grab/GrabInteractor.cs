// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An ArticulatedTouchInteractor that is driven by the
    /// pinching point on the specified hand, as defined by the
    /// HandsAggregatorSubsystem. This is typically the averaged pose
    /// between the index tip and the thumb tip.
    /// </summary>
    [UnityEngine.AddComponentMenu("MRTK/Input/Grab Interactor")]
    public class GrabInteractor : HandJointInteractor, IGrabInteractor
    {
        [SerializeReference]
        [InterfaceSelector]
        [Tooltip("The pose source representing the worldspace pose of the hand pinching point.")]
        private IPoseSource pinchPoseSource;

        /// <summary>
        /// Get near interaction point from hands aggregator.
        /// </summary>
        protected override bool TryGetInteractionPoint(out Pose pose)
        {
            pose = Pose.identity;
            return pinchPoseSource != null && pinchPoseSource.TryGetPose(out pose);
        }
    }
}
