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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class MixedRealitySpatialAwarenessSystem :
        BaseDataProviderAccessCoreSystem, 
        IMixedRealitySpatialAwarenessSystem, 
        IMixedRealityCapabilityCheck
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

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Spatial Awareness System";

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            foreach(var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                IMixedRealityCapabilityCheck capabilityChecker = observer as IMixedRealityCapabilityCheck;

                // If one of the running data providers supports the requested capability, 
                // the application has the needed support to leverage the desired functionality.
                if ((capabilityChecker != null) &&
                    capabilityChecker.CheckCapability(capability))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

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

            foreach (var provider in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                UnregisterDataProvider(provider);
            }
        }

        /// <inheritdoc/>
        public override void Enable()
        {
            MixedRealitySpatialAwarenessSystemProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessSystemProfile;

            if ((GetDataProviders<IMixedRealitySpatialAwarenessObserver>().Count == 0) && (profile != null))
            {
                // Register the spatial observers.
                for (int i = 0; i < profile.ObserverConfigurations.Length; i++)
                {
                    MixedRealitySpatialObserverConfiguration configuration = profile.ObserverConfigurations[i];
                    object[] args = { Registrar, this, configuration.ComponentName, configuration.Priority, configuration.ObserverProfile };

                    RegisterDataProvider<IMixedRealitySpatialAwarenessObserver>(
                        configuration.ComponentType.Type,
                        configuration.RuntimePlatform,
                        args);
                }
            }

            // Ensure data providers are enabled (performed by the base class)
            base.Enable();
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
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

            base.Destroy();
        }

        #endregion IMixedRealityToolkitService Implementation

        #region IMixedRealitySpatialAwarenessSystem Implementation

        /// <summary>
        /// The parent object, in the hierarchy, under which all observed game objects will be placed.
        /// </summary>
        private GameObject spatialAwarenessObjectParent = null;

        /// <inheritdoc />
        public GameObject SpatialAwarenessObjectParent => spatialAwarenessObjectParent != null ? spatialAwarenessObjectParent : (spatialAwarenessObjectParent = CreateSpatialAwarenessObjectParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessObjectParent
        {
            get
            {
                GameObject newParent = new GameObject("Spatial Awareness System");
                MixedRealityPlayspace.AddChild(newParent.transform);

                return newParent;
            }
        }

        /// <inheritdoc />
        public GameObject CreateSpatialAwarenessObservationParent(string name)
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

        private MixedRealitySpatialAwarenessSystemProfile spatialAwarenessSystemProfile = null;

        /// <inheritdoc/>
        public MixedRealitySpatialAwarenessSystemProfile SpatialAwarenessSystemProfile
        {
            get
            {
                if (spatialAwarenessSystemProfile == null)
                {
                    spatialAwarenessSystemProfile = ConfigurationProfile as MixedRealitySpatialAwarenessSystemProfile;
                }
                return spatialAwarenessSystemProfile;
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealitySpatialAwarenessObserver> GetObservers()
        {
            return GetDataProviders() as IReadOnlyList<IMixedRealitySpatialAwarenessObserver>;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            return GetDataProviders<T>();
        }

        /// <inheritdoc />
        public IMixedRealitySpatialAwarenessObserver GetObserver(string name)
        {
            return GetDataProvider(name) as IMixedRealitySpatialAwarenessObserver;
        }

        /// <inheritdoc />
        public T GetObserver<T>(string name = null) where T : IMixedRealitySpatialAwarenessObserver
        {
            return GetDataProvider<T>(name);
        }

        #region IMixedRealityDataProviderAccess Implementation

        /// <inheritdoc />
        public override IReadOnlyList<T> GetDataProviders<T>()
        {
            if (!typeof(IMixedRealitySpatialAwarenessObserver).IsAssignableFrom(typeof(T)))
            {
                return null;
            }

            return base.GetDataProviders<T>();
        }

        /// <inheritdoc />
        public override T GetDataProvider<T>(string name = null)
        {
            if (!typeof(IMixedRealitySpatialAwarenessObserver).IsAssignableFrom(typeof(T)))
            {
                return default(T);
            }

            return base.GetDataProvider<T>(name);
        }

        #endregion IMixedRealityDataProviderAccess Implementation

        /// <inheritdoc />
        public void ResumeObservers()
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                observer.Resume();
            }
        }

        /// <inheritdoc />
        public void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                if (observer is T)
                {
                    observer.Resume();
                }
            }
        }

        /// <inheritdoc />
        public void ResumeObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                if (observer is T && observer.Name == name)
                {
                    observer.Resume();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObservers()
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                observer.Suspend();
            }
        }

        /// <inheritdoc />
        public void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                if (observer is T)
                {
                    observer.Suspend();
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                if (observer is T && observer.Name == name)
                {
                    observer.Suspend();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void ClearObservations()
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                observer.ClearObservations();
            }
        }

        /// <inheritdoc />
        public void ClearObservations<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            T observer = GetObserver<T>(name);
            observer?.ClearObservations();
        }

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
