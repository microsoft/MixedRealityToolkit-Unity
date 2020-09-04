using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Experimental;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    [MixedRealityDataProvider(
    typeof(IMixedRealitySpatialAwarenessSystem),
    SupportedPlatforms.WindowsUniversal,
    "Spatial Awareness Surface Plane Observer",
    "Experimental/Profiles/DefaultSurfacePlaneObserverProfile.asset",
    "MixedRealityToolkit.SDK")]
    public class SpatialAwarenessSurfacePlaneObserver :
        BaseSurfacePlaneObserver,
        IMixedRealityCapabilityCheck
    {

        private SurfaceMeshesToPlanes meshesToPlanes;
        private GameObject ScriptContainer;
     
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public SpatialAwarenessSurfacePlaneObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        public override void Enable()
        {
            base.Enable();

            ScriptContainer = new GameObject();
            ScriptContainer.name = "SurfaceMeshesToPlanes";
            meshesToPlanes = ScriptContainer.AddComponent<SurfaceMeshesToPlanes>();
            ConfigureMeshesToPlanes(meshesToPlanes);

            meshesToPlanes.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
            meshesToPlanes.MakePlanes();
        }

        public override void Disable()
        {
            base.Disable();
            Object.Destroy(ScriptContainer);
        }

        private void ConfigureMeshesToPlanes(SurfaceMeshesToPlanes meshesToPlanes)
        {
            meshesToPlanes.PlanesParent = ScriptContainer;
            meshesToPlanes.PhysicsLayer = PhysicsLayer;
            meshesToPlanes.DefaultMaterial = DefaultMaterial;
            meshesToPlanes.drawPlanesMask = SurfaceTypes;
            meshesToPlanes.PlaneThickness = PlaneThickness;
        }

        private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
        {
            // TODO: Emit Observation Added Events
        }

        public bool CheckCapability(MixedRealityCapability capability)
        {
            // TODO: Verify this is the correct capability
            return capability == MixedRealityCapability.SpatialAwarenessMesh;
        }
    }

}
