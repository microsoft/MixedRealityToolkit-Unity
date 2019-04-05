// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystem : BaseCoreSystem, IMixedRealitySpatialAwarenessSystem
    {
        public MixedRealitySpatialAwarenessSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySpatialAwarenessSystemProfile profile) : base(registrar, profile)
        {
            if (registrar == null)
            {
                Debug.LogError("The MixedRealitySpatialAwarenessSystem object requires a valid IMixedRealityServiceRegistrar instance.");
            }
        }

        #region IMixedRealityToolkitService Implementation

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

#if UNITY_EDITOR
            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.SpatialPerception))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.SpatialPerception, true);
            }
#endif // UNITY_EDITOR
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            base.Disable();

            if (observers.Count > 0)
            {
                // Unregister the spatial observers
                for (int i = 0; i < observers.Count; i++)
                {
                    if (observers[i] != null)
                    {
                        Registrar.UnregisterDataProvider<IMixedRealitySpatialAwarenessObserver>(observers[i]);
                    }
                }
            }
            observers.Clear();
        }

        /// <inheritdoc/>
        public override void Enable()
        {
            base.Enable();

            MixedRealitySpatialAwarenessSystemProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessSystemProfile;

            if ((observers.Count == 0) && (profile != null))
            {
                // Register the spatial observers.
                for (int i = 0; i < profile.ObserverConfigurations.Length; i++)
                {
                    MixedRealitySpatialObserverConfiguration configuration = profile.ObserverConfigurations[i];
                    object[] args = { Registrar, this, configuration.ComponentName, configuration.Priority, configuration.ObserverProfile };

                    if (Registrar.RegisterDataProvider<IMixedRealitySpatialAwarenessObserver>(
                        configuration.ComponentType.Type,
                        configuration.RuntimePlatform,
                        args))
                    {
                        observers.Add(Registrar.GetDataProvider<IMixedRealitySpatialAwarenessObserver>(configuration.ComponentName));
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            Disable();
            Initialize();
            Enable();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
#if UNITY_EDITOR
            if (UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.SpatialPerception))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.SpatialPerception, false);
            }
#endif // UNITY_EDITOR

            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                // Detach the child objects and clean up the parent.
                if (spatialAwarenessObjectParent != null)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(spatialAwarenessObjectParent);
                    }
                    else
                    {
                        spatialAwarenessObjectParent.transform.DetachChildren();
                        Object.Destroy(spatialAwarenessObjectParent);
                    }
                    spatialAwarenessObjectParent = null;
                }
            }
        }

        #endregion IMixedRealityToolkitService Implementation

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
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to which spatial awareness created objects will be parented.
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
