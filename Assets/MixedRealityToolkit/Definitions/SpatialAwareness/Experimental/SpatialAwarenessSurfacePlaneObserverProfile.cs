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

        #region IMixedRealityOnDemandObserver settings

        [SerializeField]
        [Tooltip("Observer will update once after initialization then require manual update thereafter.")]
        private bool updateOnceOnLoad = false;
        /// <summary>
        /// Observer will update once after initialization then require manual update thereafter. Uses <see cref="FirstUpdateDelay"/> to determine when.
        /// </summary>
        public bool UpdateOnceOnLoad => updateOnceOnLoad;

        [SerializeField]
        [Tooltip("Should the observer update on an interval or on demand?")]
        private bool autoUpdate = false;

        /// <summary>
        /// Indicates if the observer is to start immediately or wait for manual startup.
        /// </summary>
        /// <remarks>
        /// When false, the observer will only update when UpdateOnDemand() is called.
        /// </remarks>
        public bool AutoUpdate => autoUpdate;

        #endregion IMixedRealityOnDemandObserver settings

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Physics layer on which to set understood planes.")]
        private int physicsLayer = 31;
        /// <summary>
        /// Physics layer on which to set understood planes
        /// </summary>
        public int PhysicsLayer => physicsLayer;

        [EnumFlags]
        [SerializeField]
        [Tooltip("Which plane types the observer should generate.")]
        private SpatialAwarenessSurfaceTypes surfaceTypes = SpatialAwarenessSurfaceTypes.Floor | SpatialAwarenessSurfaceTypes.Ceiling | SpatialAwarenessSurfaceTypes.Wall | SpatialAwarenessSurfaceTypes.Platform;
        /// <summary>
        /// Which plane types the observer should generate.
        /// </summary>
        public SpatialAwarenessSurfaceTypes SurfaceTypes => surfaceTypes;

        [SerializeField]
        [Tooltip("Material to use when displaying understood planes and meshes")]
        private Material defaultMaterial = null;
        /// <summary>
        /// The material to be used when displaying understood planes.
        /// </summary>
        public Material DefaultMaterial => defaultMaterial;

        [SerializeField]
        [Tooltip("The amount of delay before the scene is updated the first time")]
        private float firstUpdateDelay = 1.0f;
        /// <summary>
        /// The amount of delay before the scene us updated the first time
        /// </summary>
        public float FirstUpdateDelay => firstUpdateDelay;

        [SerializeField]
        [Tooltip("Thickness of rendered plane game objects")]
        private float planeThickness = 0.01f;
        /// <summary>
        /// Thickness of rendered plane game objects
        /// </summary>
        public float PlaneThickness => planeThickness;
    }
}