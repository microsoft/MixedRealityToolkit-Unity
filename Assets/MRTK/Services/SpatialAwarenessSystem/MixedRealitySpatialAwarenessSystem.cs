// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
    public class MixedRealitySpatialAwarenessSystem :
        BaseDataProviderAccessCoreSystem,
        IMixedRealitySpatialAwarenessSystem,
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public MixedRealitySpatialAwarenessSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySpatialAwarenessSystemProfile profile) : this(profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        public MixedRealitySpatialAwarenessSystem(
            MixedRealitySpatialAwarenessSystemProfile profile) : base(profile)
        { }

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Spatial Awareness System";

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
            {
                // If one of the running data providers supports the requested capability, 
                // the application has the needed support to leverage the desired functionality.
                if (observer is IMixedRealityCapabilityCheck capabilityChecker &&
                    capabilityChecker.CheckCapability(capability))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityToolkitService Implementation

        /// <inheritdoc/>
        public override void Initialize()
        {
            // Mark not initialized early so observers can use this state in their own initialization
            IsInitialized = false;
            InitializeInternal();
            base.Initialize();
        }

        /// <summary>
        /// Performs initialization tasks for the spatial awareness system.
        /// </summary>
        private void InitializeInternal()
        {
            MixedRealitySpatialAwarenessSystemProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessSystemProfile;

            if (profile != null && GetDataProviders<IMixedRealitySpatialAwarenessObserver>().Count == 0)
            {
                // Register the spatial observers.
                for (int i = 0; i < profile.ObserverConfigurations.Length; i++)
                {
                    MixedRealitySpatialObserverConfiguration configuration = profile.ObserverConfigurations[i];
                    object[] args = { this, configuration.ComponentName, configuration.Priority, configuration.ObserverProfile };

                    RegisterDataProvider<IMixedRealitySpatialAwarenessObserver>(
                        configuration.ComponentType.Type,
                        configuration.ComponentName,
                        configuration.RuntimePlatform,
                        args);
                }
            }
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
            InitializeInternal();

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
                /// Preserve local transform when attaching to playspace.
                newParent.transform.SetParent(MixedRealityPlayspace.Transform, false);

                return newParent;
            }
        }

        /// <inheritdoc />
        public GameObject CreateSpatialAwarenessObservationParent(string name)
        {
            GameObject objectParent = new GameObject(name);

            /// Preserve local transform when attaching to SA object parent.
            objectParent.transform.SetParent(SpatialAwarenessObjectParent.transform, false);

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

        private static readonly ProfilerMarker GetObserversPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetObservers");

        /// <inheritdoc />
        public IReadOnlyList<IMixedRealitySpatialAwarenessObserver> GetObservers()
        {
            using (GetObserversPerfMarker.Auto())
            {
                return GetDataProviders() as IReadOnlyList<IMixedRealitySpatialAwarenessObserver>;
            }
        }

        private static readonly ProfilerMarker GetObserversTPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetObservers<T>");

        /// <inheritdoc />
        public IReadOnlyList<T> GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            using (GetObserversTPerfMarker.Auto())
            {
                return GetDataProviders<T>();
            }
        }

        private static readonly ProfilerMarker GetObserverPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetObserver");

        /// <inheritdoc />
        public IMixedRealitySpatialAwarenessObserver GetObserver(string name)
        {
            using (GetObserverPerfMarker.Auto())
            {
                return GetDataProvider(name) as IMixedRealitySpatialAwarenessObserver;
            }
        }

        private static readonly ProfilerMarker GetObserverTPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetObserver<T>");

        /// <inheritdoc />
        public T GetObserver<T>(string name = null) where T : IMixedRealitySpatialAwarenessObserver
        {
            using (GetObserverTPerfMarker.Auto())
            {
                return GetDataProvider<T>(name);
            }
        }

        #region IMixedRealityDataProviderAccess Implementation

        private static readonly ProfilerMarker GetDataProvidersPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetDataProviders");

        /// <inheritdoc />
        public override IReadOnlyList<T> GetDataProviders<T>()
        {
            using (GetDataProvidersPerfMarker.Auto())
            {
                if (!typeof(IMixedRealitySpatialAwarenessObserver).IsAssignableFrom(typeof(T)))
                {
                    return null;
                }

                return base.GetDataProviders<T>();
            }
        }

        private static readonly ProfilerMarker GetDataProviderPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.GetDataProvider");

        /// <inheritdoc />
        public override T GetDataProvider<T>(string name = null)
        {
            using (GetDataProviderPerfMarker.Auto())
            {
                if (!typeof(IMixedRealitySpatialAwarenessObserver).IsAssignableFrom(typeof(T)))
                {
                    return default(T);
                }

                return base.GetDataProvider<T>(name);
            }
        }

        #endregion IMixedRealityDataProviderAccess Implementation

        private static readonly ProfilerMarker ResumeObserversPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.ResumeObservers");

        /// <inheritdoc />
        public void ResumeObservers()
        {
            using (ResumeObserversPerfMarker.Auto())
            {
                foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
                {
                    observer.Resume();
                }
            }
        }

        private static readonly ProfilerMarker ResumeObserversTPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.ResumeObservers<T>");

        /// <inheritdoc />
        public void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            using (ResumeObserversTPerfMarker.Auto())
            {
                foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
                {
                    if (observer is T)
                    {
                        observer.Resume();
                    }
                }
            }
        }

        private static readonly ProfilerMarker ResumeObserverPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.ResumeObserver");

        /// <inheritdoc />
        public void ResumeObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            using (ResumeObserverPerfMarker.Auto())
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
        }

        private static readonly ProfilerMarker SuspendObserversPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.SuspendObservers");

        /// <inheritdoc />
        public void SuspendObservers()
        {
            using (SuspendObserversPerfMarker.Auto())
            {
                foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
                {
                    observer.Suspend();
                }
            }
        }

        private static readonly ProfilerMarker SuspendObserversTPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.SuspendObservers<T>");

        /// <inheritdoc />
        public void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            using (SuspendObserversTPerfMarker.Auto())
            {
                foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
                {
                    if (observer is T)
                    {
                        observer.Suspend();
                    }
                }
            }
        }

        private static readonly ProfilerMarker SuspendObserverPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.SuspendObserver");

        /// <inheritdoc />
        public void SuspendObserver<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            using (SuspendObserverPerfMarker.Auto())
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
        }

        private static readonly ProfilerMarker ClearObservationsPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.ClearObservations");

        /// <inheritdoc />
        public void ClearObservations()
        {
            using (ClearObservationsPerfMarker.Auto())
            {
                foreach (var observer in GetDataProviders<IMixedRealitySpatialAwarenessObserver>())
                {
                    observer.ClearObservations();
                }
            }
        }

        private static readonly ProfilerMarker ClearObservationsTPerfMarker = new ProfilerMarker("[MRTK] MixedRealitySpatialAwarenessSystem.ClearObservations<T>");

        /// <inheritdoc />
        public void ClearObservations<T>(string name) where T : IMixedRealitySpatialAwarenessObserver
        {
            using (ClearObservationsTPerfMarker.Auto())
            {
                T observer = GetObserver<T>(name);
                observer?.ClearObservations();
            }
        }

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
