using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    public abstract class BaseSurfacePlaneObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessSurfacePlaneObserver
    {
        public BaseSurfacePlaneObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = 10,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        public SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }
        public int PhysicsLayer { get; set; }

        protected virtual void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError($"{Name} requires a configuration profile to run properly.");
                return;
            }

            SpatialAwarenessSurfacePlaneObserverProfile profile = ConfigurationProfile as SpatialAwarenessSurfacePlaneObserverProfile;
            if (profile == null)
            {
                Debug.LogError($"{Name}'s configuration profile must be a MixedRealitySpatialAwarenessMeshObserverProfile.");
                return;
            }

            //SurfaceTypes = profile.SurfaceTypes;
            PhysicsLayer = profile.PhysicsLayer;
        }
    }
}