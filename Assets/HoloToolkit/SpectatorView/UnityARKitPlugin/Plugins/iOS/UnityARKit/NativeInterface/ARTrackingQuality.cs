#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public enum ARTrackingQuality : long
    {
        /** The tracking quality is not available. */
        ARTrackingQualityNotAvailable,

        /** The tracking quality is limited, relying only on the device's motion. */
        ARTrackingQualityLimited,

        /** The tracking quality is poor. */
        ARTrackingQualityPoor,

        /** The tracking quality is good. */
        ARTrackingQualityGood

    }
}
#endif
