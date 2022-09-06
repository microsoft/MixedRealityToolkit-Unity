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
    [AddComponentMenu("MRTK/Input/Grab Interactor")]
    public class GrabInteractor : HandJointInteractor, IGrabInteractor
    {
        [SerializeReference]
        [InterfaceSelector(true)]
        [Tooltip("The pose source representing the worldspace pose of the hand pinching point.")]
        private IPoseSource pinchPoseSource;

        /// <summary>
        /// The pose source representing the worldspace pose of the hand pinching point.
        /// </summary>
        protected IPoseSource PinchPoseSource { get => pinchPoseSource; set => pinchPoseSource = value; }

        /// <summary>
        /// Get near interaction point from hands aggregator.
        /// </summary>
        protected override bool TryGetInteractionPoint(out Pose pose)
        {
            pose = Pose.identity;
            return PinchPoseSource != null && PinchPoseSource.TryGetPose(out pose);
        }
    }
}
