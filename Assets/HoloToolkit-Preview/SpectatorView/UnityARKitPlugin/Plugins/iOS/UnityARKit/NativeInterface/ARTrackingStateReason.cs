#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public enum ARTrackingStateReason
    {
        /** Tracking is not limited. */
        ARTrackingStateReasonNone,

        /** Tracking is limited due to initialization in progress. */
        ARTrackingStateReasonInitializing,

        /** Tracking is limited due to a excessive motion of the camera. */
        ARTrackingStateReasonExcessiveMotion,

        /** Tracking is limited due to a lack of features visible to the camera. */
        ARTrackingStateReasonInsufficientFeatures,
    }
}
#endif
