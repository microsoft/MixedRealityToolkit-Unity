using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Experimental;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    [MixedRealityDataProvider(
    typeof(IMixedRealitySpatialAwarenessSystem),
    SupportedPlatforms.WindowsUniversal,
    "Spatial Awareness Surface Plane Observer",
    "Experimental/Profiles/DefaultSurfacePlaneObserverProfile.asset",
    "MixedRealityToolkit.SDK")]
    public class SpatialAwarenessSurfacePlaneObserver : BaseSpatialObserver
    {

        private SurfaceMeshesToPlanes meshesToPlanes;
     
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
        {
            ReadProfile();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Enable()
        {
            base.Enable();

            var go = new GameObject();
            GameObject.Instantiate(go);
            meshesToPlanes = go.AddComponent<SurfaceMeshesToPlanes>();
            meshesToPlanes.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
            
            meshesToPlanes.MakePlanes();
        }

        public override void Update()
        {
            base.Update();
        }

        // TODO: Read configuration from profile
        private void ReadProfile()
        {
            // TODO: ensure profile is correct type
        }

        private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
        {
            SetPlaneColors(SpatialAwarenessSurfaceTypes.Ceiling);
            SetPlaneColors(SpatialAwarenessSurfaceTypes.Floor);
            SetPlaneColors(SpatialAwarenessSurfaceTypes.Wall);
            SetPlaneColors(SpatialAwarenessSurfaceTypes.Platform);
        }

        private void SetPlaneColors(SpatialAwarenessSurfaceTypes type)
        {
            var planes = meshesToPlanes.GetActivePlanes(type);

            foreach (var plane in planes)
            {
                var renderer = plane.GetComponent<Renderer>();

                switch (type)
                {
                    case SpatialAwarenessSurfaceTypes.Ceiling:
                        renderer.material.color = Color.blue;
                        break;
                    case SpatialAwarenessSurfaceTypes.Floor:
                        renderer.material.color = Color.green;
                        break;
                    case SpatialAwarenessSurfaceTypes.Wall:
                        renderer.material.color = Color.red;
                        break;
                    case SpatialAwarenessSurfaceTypes.Platform:
                        renderer.material.color = Color.magenta;
                        break;
                    default:
                        break;
                }
                
            }
        }
    }

}
