#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public enum ARTrackingState
    {
        /** Tracking is not available. */
        ARTrackingStateNotAvailable,

        /** Tracking is limited. See tracking reason for details. */
        ARTrackingStateLimited,

        /** Tracking is Normal. */
        ARTrackingStateNormal,
    }
}
#endif
