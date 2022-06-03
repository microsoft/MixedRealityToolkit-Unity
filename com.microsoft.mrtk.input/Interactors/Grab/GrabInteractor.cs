// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An ArticulatedTouchInteractor that is driven by the
    /// pinching point on the specfied hand, as defined by the
    /// HandsAggregatorSubsystem. This is typically the averaged pose
    /// between the index tip and the thumb tip.
    /// </summary>
    [UnityEngine.AddComponentMenu("Scripts/Microsoft/MRTK/Input/Grab Interactor")]
    public class GrabInteractor : HandJointInteractor, IGrabInteractor
    {
        /// <summary>
        /// Get near interaction point from hands aggregator.
        /// </summary>
        protected override bool TryGetInteractionPoint(out HandJointPose jointPose)
        {
            return HandsAggregator.TryGetPinchingPoint(HandNode, out jointPose);
        }
    }
}