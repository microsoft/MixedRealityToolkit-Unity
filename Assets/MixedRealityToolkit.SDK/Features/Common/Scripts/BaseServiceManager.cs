// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    public class BaseServiceManager : MonoBehaviour, IMixedRealityServiceRegistrar
    {
        // todo: remove
        //protected IMixedRealityService service = null;
        // todo: rename to services (when done)
        protected Dictionary<Type, IMixedRealityService> registeredServices = new Dictionary<Type, IMixedRealityService>();

        /// <summary>
        /// The collection of registered data providers.
        /// </summary>
        private List<IMixedRealityDataProvider> dataProviders = new List<IMixedRealityDataProvider>();

        #region MonoBehaviour implementation

        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                IReadOnlyList<IMixedRealityService> serviceSnapshot = new List<IMixedRealityService>(registeredServices.Values);
                for (int i = 0; i < serviceSnapshot.Count; i++)
                {
                    serviceSnapshot[i]?.Update();
                }

                IMixedRealityDataProvider[] providers = dataProviders.ToArray();
                for (int i = 0; i < providers.Length; i++)
                {
                    providers[i]?.Update();
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (Application.isPlaying)
            {
                IReadOnlyList<IMixedRealityService> serviceSnapshot = new List<IMixedRealityService>(registeredServices.Values);
                for (int i = 0; i < serviceSnapshot.Count; i++)
                {
                    serviceSnapshot[i]?.Enable();
                }

                IMixedRealityDataProvider[] providers = dataProviders.ToArray();
                for (int i = 0; i < providers.Length; i++)
                {
                    providers[i]?.Enable();
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                IMixedRealityDataProvider[] providers = dataProviders.ToArray();
                for (int i = 0; i < providers.Length; i++)
                {
                    providers[i]?.Disable();
                }

                IReadOnlyList<IMixedRealityService> serviceSnapshot = new List<IMixedRealityService>(registeredServices.Values);
                for (int i = 0; i < serviceSnapshot.Count; i++)
                {
                    serviceSnapshot[i]?.Disable();
                }
            }
        }

        protected virtual void OnDestroy()
        {
            IMixedRealityDataProvider[] providers = dataProviders.ToArray();
            for (int i = 0; i < providers.Length; i++)
            {
                providers[i]?.Disable(); // Disable before destroy to ensure the data provider has time to get in a good state.
                providers[i]?.Destroy();
            }
            // Clear the actual collection
            dataProviders.Clear();

            IReadOnlyList<IMixedRealityService> serviceSnapshot = new List<IMixedRealityService>(registeredServices.Values);
            for (int i = 0; i < serviceSnapshot.Count; i++)
            {
                serviceSnapshot[i].Disable(); // Disable before destroy to ensure the service has time to get in a good state.
                serviceSnapshot[i].Destroy();
            }
            // Clear the actual collection
            registeredServices.Clear();
        }

        #endregion MonoBehaviour implementation

        #region IMixedRealityServiceRegistrar implementation

        /// <inheritdoc />
        public T GetDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            Type interfaceType = typeof(T);
            T provider = default(T);

            IMixedRealityDataProvider[] providers = dataProviders.ToArray();
            for (int i = 0; i < providers.Length; i++)
            {
                // Check for null and mismatched type.
                if ((providers[i] == null) || !interfaceType.IsAssignableFrom(providers[i].GetType())) { continue; }

                // Check to see if name is valid (not null or whitespace) and if it matches the provider's name.
                if (!string.IsNullOrWhiteSpace(name) && string.Equals(providers[i].Name, name))
                {
                    provider = (T)providers[i];
                }
                // If name is invalid, the first instance of a matching provider type indicates that a registration has occurred.
                else
                {
                    provider = (T)providers[i];
                }
            }

            return provider;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetDataProviders<T>(string name = null) where T : IMixedRealityDataProvider
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Debug.LogError("This registrar does not support requesting multiple data providers of the same interface type and name.");
                return new List<T>(); ;
            }

            Type interfaceType = typeof(T);
            List<T> matchingProviders = new List<T>();

            IMixedRealityDataProvider[] providers = dataProviders.ToArray();
            for (int i = 0; i < providers.Length; i++)
            {
                if (interfaceType.IsAssignableFrom(providers[i].GetType()))
                {
                    matchingProviders.Add((T)providers[i]);
                }
            }

            return matchingProviders;
        }

        /// <inheritdoc />
        public T GetService<T>(string name = null, bool showLogs = true) where T : IMixedRealityService
        {
            T serviceInstance = FindService<T>(name);

            if (showLogs && (serviceInstance == null))
            {
                Debug.Log("Failed to get the requested service.");
            }

            return serviceInstance;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetServices<T>(string name = null) where T : IMixedRealityService
        {
            List<T> services = new List<T>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                Debug.LogError("This registrar does not support requesting multiple services of the same interface type and name.");
                return services;
            }

            T serviceInstance = FindService<T>();
            if (serviceInstance != null)
            {
                services.Add(serviceInstance);
            }
            return services;
        }

        /// <inheritdoc />
        public bool IsDataProviderRegistered<T>(string name = null) where T : IMixedRealityDataProvider
        {
            return (GetDataProvider<T>(name) != null);
        }

        /// <inheritdoc />
        public bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService
        {
            return (GetService<T>(name) != null);
        }

        /// <inheritdoc />
        public virtual bool RegisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            if ((dataProviderInstance == null) || (dataProviders.Contains(dataProviderInstance))) { return false; }

            dataProviders.Add(dataProviderInstance);

            return true;
        }

        /// <inheritdoc />
        public bool RegisterDataProvider<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityDataProvider
        {
            T serviceInstance = ActivateInstance<T>(concreteType, supportedPlatforms, args);
            if (serviceInstance == null) { return false; }

            return RegisterDataProvider<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool RegisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);

            if (registeredServices.ContainsKey(interfaceType))
            {
                Debug.LogError("This registrar does not support registering multiple services of the same interface type.");
                return false;
            }

            bool registered = MixedRealityServiceRegistry.AddService<T>(serviceInstance, this);
            if (registered)
            {
                registeredServices.Add(interfaceType, serviceInstance);
            }

            return registered;
        }

        /// <inheritdoc />
        public bool RegisterService<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityService
        {
            T serviceInstance = ActivateInstance<T>(concreteType, supportedPlatforms, args);
            if (serviceInstance == null)
            {
                return false;
            }

            return RegisterService<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool UnregisterDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            T dataProviderInstance = GetDataProvider<T>(name);
            return UnregisterDataProvider<T>(dataProviderInstance);
        }

        /// <inheritdoc />
        public bool UnregisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            if ((dataProviderInstance == null) || (!dataProviders.Contains(dataProviderInstance))) { return false; }

            dataProviders.Remove(dataProviderInstance);
            return true;
        }

        /// <inheritdoc />
        public bool UnregisterService<T>(string name = null) where T : IMixedRealityService
        {
            T serviceInstance = FindService<T>(name);

            if (serviceInstance == null) { return false; }

            return UnregisterService<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool UnregisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            if (serviceInstance == null) { return false; }

            Type interfaceType = typeof(T);
            if (!registeredServices.ContainsKey(interfaceType)) { return false; }

            registeredServices.Remove(interfaceType);
            return MixedRealityServiceRegistry.RemoveService<T>(serviceInstance, this);
        }

        /// <summary>
        /// Activates an instance of the specified concrete type using the provided argument collection.
        /// </summary>
        /// <typeparam name="T">The interface which must be implemented by the concrete type.</typeparam>
        /// <param name="concreteType">The type of object to be instantiated.</param>
        /// <param name="supportedPlatforms">The platform(s) on which the concrete type is supported.</param>
        /// <param name="args">Collection of arguments to provide to the concrete type's constructor.</param>
        /// <returns>An instance of the concrete type. Returns a default value of T (typically null) in the event of a failure.</returns>
        private T ActivateInstance<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityService
        {
            if (concreteType == null) { return default(T); }

#if UNITY_EDITOR
            if (!UnityEditor.EditorUserBuildSettings.activeBuildTarget.IsPlatformSupported(supportedPlatforms))
#else
            if (!Application.platform.IsPlatformSupported(supportedPlatforms))
#endif
            {
                return default(T);
            }

            if (!typeof(T).IsAssignableFrom(concreteType))
            {
                Debug.LogError($"Error: {concreteType.Name} service must implement {typeof(T)}.");
                return default(T);
            }

            try
            {
                T serviceInstance = (T)Activator.CreateInstance(concreteType, args);
                return serviceInstance;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: Failed to instantiate {concreteType.Name}: {e.GetType()} - {e.Message}");
                return default(T);
            }
        }

        #endregion IMixedRealityServiceRegistrar implementation

        protected virtual void Initialize<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityService
        {
            RegisterService<T>(concreteType, supportedPlatforms, args);
            T serviceInstance = FindService<T>();
            serviceInstance?.Initialize();
        }

        protected virtual void Uninitialize<T>() where T : IMixedRealityService
        {
            T serviceInstance = FindService<T>();

            if (serviceInstance != null)
            {
                registeredServices.Remove(typeof(T));
                MixedRealityServiceRegistry.RemoveService<T>(serviceInstance, this);
            }
        }

        private T FindService<T>(string name = null) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);

            if (!registeredServices.ContainsKey(interfaceType)) { return default(T); }

            return (T)registeredServices[interfaceType];
         }
    }
}