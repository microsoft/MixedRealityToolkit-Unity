// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
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
        #region IMixedRealityToolkit Implementation

        private MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> meshEventData = null;

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
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>(EventSystem.current);
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            base.Disable();

            // Clear the collection of registered observers.
            observers.Clear();
        }

        /// <inheritdoc/>
        public override void Enable()
        {
            base.Enable();

            if (observers.Count != 0)
            {
                // todo: ensure this is a clean pattern
                Debug.LogWarning("The spatial awareness system is already enabled.");
                return;
            }

            // Get the collection of registered observers.
            List<Core.Interfaces.IMixedRealityService> services = MixedRealityToolkit.Instance.GetActiveServices(typeof(IMixedRealitySpatialAwarenessObserver));
            for (int i = 0; i < services.Count; i++)
            {
                observers.Add(services[i] as IMixedRealitySpatialAwarenessObserver);
            }
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            // todo: base Reset should likely call Disable, then Initialize
            InitializeInternal();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
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

        /// <summary>
        ///  The collection of registered spatial awareness observers.
        /// </summary>
        private List<IMixedRealitySpatialAwarenessObserver> observers = new List<IMixedRealitySpatialAwarenessObserver>();

        /// <summary>
        /// The parent object, in the hierarchy, under which all observed game objects will be placed.
        /// </summary>
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
        public IReadOnlyList<IMixedRealitySpatialAwarenessObserver> GetObservers()
        {
            return new List<IMixedRealitySpatialAwarenessObserver>(observers) as IReadOnlyList<IMixedRealitySpatialAwarenessObserver>;
        }
        
        /// <inheritdoc />
        public IReadOnlyList<T> GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            List<T> selected = new List<T>();

            for (int i = 0; i < observers.Count; i++)
            {
                if (observers[i] is T)
                {
                    selected.Add((T)observers[i]);
                }
            }

            return selected;
        }

        /// <inheritdoc />
        public IMixedRealitySpatialAwarenessObserver GetObserver(string name)
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if (observers[i].Name == name)
                {
                    return observers[i];
                }
            }

            return null;
        }

        /// <inheritdoc />
        public T GetObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if ((observers[i] is T) && (observers[i].Name == name))
                {
                    return (T)observers[i];
                }
            }
            
            return default(T);
        }

        /// <inheritdoc />
        public void ResumeObservers()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Resume();
            }
        }

        /// <inheritdoc />
        public void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if (observers[i] is T)
                {
                    observers[i].Resume();
                }
            }
        }

        /// <inheritdoc />
        public void ResumeObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if ((observers[i] is T) && (observers[i].Name == name))
                {
                    observers[i].Resume();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObservers()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].Suspend();
            }
        }

        /// <inheritdoc />
        public void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if (observers[i] is T)
                {
                    observers[i].Suspend();
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            for (int i = 0; i < observers.Count; i++)
            {
                if ((observers[i] is T) && (observers[i].Name == name))
                {
                    observers[i].Suspend();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, int meshId, SpatialAwarenessMeshObject meshObject)
        {
            meshEventData.Initialize(observer, meshId, meshObject);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                handler.OnObservationAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, int meshId, SpatialAwarenessMeshObject meshObject)
        {
            meshEventData.Initialize(observer, meshId, meshObject);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                handler.OnObservationUpdated(spatialEventData);
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
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                handler.OnObservationRemoved(spatialEventData);
            };

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
