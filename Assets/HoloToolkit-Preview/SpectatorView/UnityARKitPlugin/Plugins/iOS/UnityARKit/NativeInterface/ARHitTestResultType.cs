#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    [Flags]
    public enum ARHitTestResultType : long
    {
        /** Result type from intersecting the nearest feature point. */
        ARHitTestResultTypeFeaturePoint     = (1 << 0),

        /** Result type from detecting and intersecting a new horizontal plane. */
        ARHitTestResultTypeHorizontalPlane  = (1 << 1),

        /** Result type from detecting and intersecting a new vertical plane. */
        ARHitTestResultTypeVerticalPlane    = (1 << 2),

        /** Result type from intersecting with an existing plane anchor. */
        ARHitTestResultTypeExistingPlane    = (1 << 3),

        /** Result type from intersecting with an existing plane anchor, taking into account the plane's extent. */
        ARHitTestResultTypeExistingPlaneUsingExtent  = ( 1 << 4)

    }
}
#endif
