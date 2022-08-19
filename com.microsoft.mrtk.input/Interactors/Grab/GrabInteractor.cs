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
        private IPoseSource pinchPoseSource;

        /// <summary>
        /// Get near interaction point from hands aggregator.
        /// </summary>
        protected override bool TryGetInteractionPoint(out Pose jointPose)
        {
            return pinchPoseSource.TryGetPose(out jointPose);
        }
    }
}
