using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    public abstract class BaseSurfacePlaneObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessSurfacePlaneObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public BaseSurfacePlaneObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        {
            ReadProfile();
        }

        public int PhysicsLayer { get; set; }
        public Material DefaultMaterial { get; set; }
        public SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }
        public float PlaneThickness { get; set; }

        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                return;
            }

            SpatialAwarenessSurfacePlaneObserverProfile profile = ConfigurationProfile as SpatialAwarenessSurfacePlaneObserverProfile;
            if (profile == null)
            {
                Debug.LogError("Spatial Awareness Surface Plane Observer's configuration profile must be a SpatialAwarenessSurfacePlaneObserverProfile.");
                return;
            }

            PhysicsLayer = profile.PhysicsLayer;
            DefaultMaterial = profile.DefaultMaterial;
            SurfaceTypes = profile.SurfaceTypes;
            PlaneThickness = profile.PlaneThickness;
        }
    }
}