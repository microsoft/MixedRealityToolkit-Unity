// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Abstract class for core MRTK system with functionality defined for managing and accessing IMixedRealityDataProviders
    /// </summary>
    public abstract class BaseDataProviderAccessCoreSystem : BaseCoreSystem, IMixedRealityDataProviderAccess
    {
        private readonly List<IMixedRealityDataProvider> dataProviders = new List<IMixedRealityDataProvider>();

        public override void Reset()
        {
            base.Reset();

            foreach (var provider in dataProviders)
            {
                provider.Reset();
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            foreach (var provider in dataProviders)
            {
                provider.Enable();
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] BaseDataProviderAccessCoreSystem.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

                foreach (var provider in dataProviders)
                {
                    provider.Update();
                }
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker = new ProfilerMarker("[MRTK] BaseDataProviderAccessCoreSystem.LateUpdate");

        /// <inheritdoc />
        public override void LateUpdate()
        {
            using (LateUpdatePerfMarker.Auto())
            {
                base.LateUpdate();

                foreach (var provider in dataProviders)
                {
                    provider.LateUpdate();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseDataProviderAccessCoreSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : this(profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        protected BaseDataProviderAccessCoreSystem(
            BaseMixedRealityProfile profile = null) : base(profile)
        { }

        #region IMixedRealityDataProviderAccess Implementation

        /// <inheritdoc />
        public virtual IReadOnlyList<IMixedRealityDataProvider> GetDataProviders()
        {
            return dataProviders.AsReadOnly();
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<T> GetDataProviders<T>() where T : IMixedRealityDataProvider
        {
            List<T> selected = new List<T>();

            foreach (var provider in dataProviders)
            {
                if (provider is T)
                {
                    selected.Add((T)provider);
                }
            }

            return selected;
        }

        /// <inheritdoc />
        public virtual IMixedRealityDataProvider GetDataProvider(string name)
        {
            foreach (var provider in dataProviders)
            {
                if (provider.Name == name)
                {
                    return provider;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public virtual T GetDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            foreach (var provider in dataProviders)
            {
                if (provider is T)
                {
                    if (name == null || provider.Name == name)
                    {
                        return (T)provider;
                    }
                }
            }

            return default(T);
        }

        #endregion IMixedRealityDataProviderAccess Implementation

        /// <summary>
        /// Registers a data provider of the specified type.
        /// </summary>
        protected bool RegisterDataProvider<T>(
            Type concreteType,
            string providerName,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityDataProvider
        {
            return RegisterDataProviderInternal<T>(
                true, // Retry with an added IMixedRealityService parameter
                concreteType,
                providerName,
                supportedPlatforms,
                args);
        }

        /// <summary>
        /// Registers a data provider of the specified type.
        /// </summary>
        [Obsolete("RegisterDataProvider<T>(Type, SupportedPlatforms, param object[]) is obsolete and will be removed from a future version of MRTK\n" +
            "Please use RegisterDataProvider<T>(Type, string, SupportedPlatforms, params object[])")]
        protected bool RegisterDataProvider<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityDataProvider
        {
            return RegisterDataProvider<T>(
                concreteType,
                string.Empty,
                supportedPlatforms,
                args);
        }

        /// <summary>
        /// Internal method that creates an instance of the specified concrete type and registers the provider.
        /// </summary>
        private bool RegisterDataProviderInternal<T>(
            bool retryWithRegistrar,
            Type concreteType,
            string providerName,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityDataProvider
        {
            if (!PlatformUtility.IsPlatformSupported(supportedPlatforms))
            {
                DebugUtilities.LogVerboseFormat(
                    "Not registering data provider of type {0} with name {1} because the current platform is not in supported platforms {2}",
                    concreteType,
                    providerName,
                    supportedPlatforms);
                return false;
            }

            if (concreteType == null)
            {
                Debug.LogError($"Unable to register {typeof(T).Name} data provider ({(!string.IsNullOrWhiteSpace(providerName) ? providerName : "unknown")}) because the value of concreteType is null.\n" +
                    "This may be caused by code being stripped during linking. The link.xml file in the MixedRealityToolkit.Generated folder is used to control code preservation.\n" +
                    "More information can be found at https://docs.unity3d.com/Manual/ManagedCodeStripping.html.");
                return false;
            }

            if (!typeof(IMixedRealityDataProvider).IsAssignableFrom(concreteType))
            {
                Debug.LogError($"Unable to register the {concreteType.Name} data provider. It does not implement {typeof(IMixedRealityDataProvider)}.");
                return false;
            }

            T dataProviderInstance;

            try
            {
                dataProviderInstance = (T)Activator.CreateInstance(concreteType, args);
            }
            catch (Exception e)
            {
                if (retryWithRegistrar && (e is MissingMethodException))
                {
                    Debug.LogWarning($"Failed to find an appropriate constructor for the {concreteType.Name} data provider. Adding the Registrar instance and re-attempting registration.");
#pragma warning disable 0618
                    List<object> updatedArgs = new List<object>();
                    updatedArgs.Add(Registrar);
                    if (args != null)
                    {
                        updatedArgs.AddRange(args);
                    }
                    return RegisterDataProviderInternal<T>(
                        false, // Do NOT retry, we have already added the configured IMIxedRealityServiceRegistrar
                        concreteType,
                        providerName,
                        supportedPlatforms,
                        updatedArgs.ToArray());
#pragma warning restore 0618
                }

                Debug.LogError($"Failed to register the {concreteType.Name} data provider: {e.GetType()} - {e.Message}");

                // Failures to create the concrete type generally surface as nested exceptions - just logging
                // the top level exception itself may not be helpful. If there is a nested exception (for example,
                // null reference in the constructor of the object itself), it's helpful to also surface those here.
                if (e.InnerException != null)
                {
                    Debug.LogError("Underlying exception information: " + e.InnerException);
                }
                return false;
            }

            return RegisterDataProvider(dataProviderInstance);
        }

        /// <summary>
        /// Registers a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be registered.</typeparam>
        /// <param name="dataProviderInstance">An instance of the data provider to be registered.</param>
        protected bool RegisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            if (dataProviderInstance == null)
            {
                Debug.LogWarning($"Unable to add a {dataProviderInstance.Name} data provider with a null instance.");
                return false;
            }

            dataProviders.Add(dataProviderInstance);
            dataProviderInstance.Initialize();

            return true;
        }

        /// <summary>
        /// Unregisters a data provider of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be unregistered.</typeparam>
        /// <param name="name">The name of the data provider to unregister.</param>
        /// <returns>True if the data provider was successfully unregistered, false otherwise.</returns>
        /// <remarks>If the name argument is not specified, the first instance will be unregistered</remarks>
        protected bool UnregisterDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            T dataProviderInstance = GetDataProvider<T>(name);

            if (dataProviderInstance == null) { return false; }

            return UnregisterDataProvider(dataProviderInstance);
        }

        /// <summary>
        /// Unregisters a data provider.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be unregistered.</typeparam>
        /// <param name="service">The specific data provider instance to unregister.</param>
        /// <returns>True if the data provider was successfully unregistered, false otherwise.</returns>
        protected bool UnregisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            if (dataProviderInstance == null)
            {
                return false;
            }

            if (dataProviders.Contains(dataProviderInstance))
            {
                dataProviders.Remove(dataProviderInstance);

                dataProviderInstance.Disable();
                dataProviderInstance.Destroy();

                return true;
            }

            return false;
        }

    }
}
