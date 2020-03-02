using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for spatial awareness mesh observers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Surface Plane Observer Profile", fileName = "MixedRealitySurfacePlaneProfile", order = (int)CreateProfileMenuItemIndices.SurfacePlaneObserver)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySpatialAwarenessObserver))]
        public class SpatialAwarenessSurfacePlaneObserverProfile : BaseSpatialAwarenessObserverProfile
    {

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Physics layer on which to set understood planes.")]
        private int physicsLayer = 31;
        /// <summary>
        /// Physics layer on which to set understood planes
        /// </summary>
        public int PhysicsLayer => physicsLayer;

    }
}