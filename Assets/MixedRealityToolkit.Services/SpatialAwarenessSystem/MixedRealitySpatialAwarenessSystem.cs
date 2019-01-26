// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Services.SpatialAwarenessSystem
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystem : BaseEventSystem, IMixedRealitySpatialAwarenessSystem
    {
        // TODO: replace with data providers collection

        //private IMixedRealitySpatialAwarenessObserver spatialAwarenessObserver = null;

        ///// <summary>
        ///// The <see cref="IMixedRealitySpatialAwarenessObserver"/>, if any, that is active on the current platform.
        ///// </summary>
        //private IMixedRealitySpatialAwarenessObserver SpatialAwarenessObserver => spatialAwarenessObserver ?? (spatialAwarenessObserver = MixedRealityToolkit.Instance.GetService<IMixedRealitySpatialAwarenessObserver>());

        #region IMixedRealityToolkit Implementation

        private MixedRealitySpatialAwarenessEventData meshEventData = null;
        private MixedRealitySpatialAwarenessEventData surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        /// <summary>
        /// Performs initialization tasks for the spatial awareness system.
        /// </summary>
        private void InitializeInternal()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData(EventSystem.current);
            surfaceFindingEventData = new MixedRealitySpatialAwarenessEventData(EventSystem.current);

            // TODO

            //// General settings
            //StartupBehavior = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.StartupBehavior;
            //ObservationExtents = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.ObservationExtents;
            //IsStationaryObserver = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.IsStationaryObserver;
            //UpdateInterval = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval;

            //// Mesh settings
            //UseMeshSystem = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UseMeshSystem;
            //MeshPhysicsLayer = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshPhysicsLayer;
            //MeshLevelOfDetail = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshLevelOfDetail;
            //MeshTrianglesPerCubicMeter = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshTrianglesPerCubicMeter;
            //MeshRecalculateNormals = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshRecalculateNormals;
            //MeshDisplayOption = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshDisplayOption;
            //MeshVisibleMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshVisibleMaterial;
            //MeshOcclusionMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshOcclusionMaterial;

            //// Surface finding settings
            //UseSurfaceFindingSystem = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UseSurfaceFindingSystem;
            //SurfacePhysicsLayer = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.SurfaceFindingPhysicsLayer;
            //SurfaceFindingMinimumArea = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.SurfaceFindingMinimumArea;
            //DisplayFloorSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayFloorSurfaces;
            //FloorSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.FloorSurfaceMaterial;
            //DisplayCeilingSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayCeilingSurface;
            //CeilingSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.CeilingSurfaceMaterial;
            //DisplayWallSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayWallSurface;
            //WallSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.WallSurfaceMaterial;
            //DisplayPlatformSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayPlatformSurfaces;
            //PlatformSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.PlatformSurfaceMaterial;
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            // todo: cleanup some objects but not the root scene items
            InitializeInternal();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                // todo: ensure this does the right thing wrt observers.

                // Detach the child objects and clean up the parent.
                if (spatialAwarenessObjectParent != null)
                {
                    spatialAwarenessObjectParent.transform.DetachChildren();
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(spatialAwarenessObjectParent);
                    }
                    else
                    {
                        Object.Destroy(spatialAwarenessObjectParent);
                    }
                    spatialAwarenessObjectParent = null;
                }
            }
        }

        #endregion IMixedRealityToolkit Implementation

        #region IMixedRealitySpatialAwarenessSystem Implementation

        private GameObject spatialAwarenessObjectParent = null;

        /// <inheritdoc />
        public GameObject SpatialAwarenessObjectParent => spatialAwarenessObjectParent != null ? spatialAwarenessObjectParent : (spatialAwarenessObjectParent = CreateSpatialAwarenessParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent => new GameObject("Spatial Awareness System");

        /// <inheritdoc />
        public GameObject CreateSpatialAwarenessObjectParent(string name)
        {
            GameObject objectParent = new GameObject(name);

            objectParent.transform.parent = SpatialAwarenessObjectParent.transform;

            return objectParent;
        }

        private uint nextSourceId = 0;

        /// <inheritdoc />
        public uint GenerateNewSourceId()
        {
            return nextSourceId++;
        }

        /// <inheritdoc />
        public IList<IMixedRealitySpatialAwarenessObserver> GetObservers()
        {
            // todo
            return new List<IMixedRealitySpatialAwarenessObserver>();
        }

        /// <inheritdoc />
        public IList<IMixedRealitySpatialAwarenessObserver> GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
            return new List<IMixedRealitySpatialAwarenessObserver>();
        }

        /// <inheritdoc />
        public IMixedRealitySpatialAwarenessObserver GetObserver(string name)
        {
            // todo
            return null;
        }

        /// <inheritdoc />
        public IMixedRealitySpatialAwarenessObserver GetObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
            return null;
        }

        /// <inheritdoc />
        public void ResumeObservers()
        {
            // todo
        }

        /// <inheritdoc />
        public void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
        }

        /// <inheritdoc />
        public void ResumeObserver<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
        }

        /// <inheritdoc />
        public void ResumeObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
        }

        /// <inheritdoc />
        public void SuspendObservers()
        {
            // todo
        }

        /// <inheritdoc />
        public void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
        }

        /// <inheritdoc />
        public void SuspendObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            // todo
        }

        // TODO: update

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, int meshId, GameObject mesh)
        {
            // TODO
            //// Parent the mesh object
            //mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(observer, meshId, mesh);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, int meshId, GameObject mesh)
        {
            // TODO
            //// Parent the mesh object
            //mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(observer, meshId, mesh);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshUpdated(spatialEventData);
            };


        /// <inheritdoc />
        public void RaiseMeshRemoved(IMixedRealitySpatialAwarenessObserver observer, int meshId)
        {
            meshEventData.Initialize(observer, meshId, null);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshRemoved(spatialEventData);
            };

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
