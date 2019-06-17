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
        protected IMixedRealityService service = null;

        #region MonoBehaviour implementation

        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                service?.Update();
            }
        }

        protected virtual void OnEnable()
        {
            if (Application.isPlaying)
            {
                service?.Enable();
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                service?.Disable();
            }
        }

        protected virtual void OnDestroy()
        {
            if (service != null)
            {
                service.Disable(); // Disable before destroy to ensure the service has time to get in a good state.
                service.Destroy();
            }
        }

        #endregion MonoBehaviour implementation

        #region IMixedRealityServiceRegistrar implementation

        /// <summary>
        /// The collection of registered data providers.
        /// </summary>
        private List<IMixedRealityDataProvider> dataProviders = new List<IMixedRealityDataProvider>();

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
            if (!ConfirmService<T>(name))
            {
                if (showLogs)
                {
                    Debug.Log("Failed to get the requested service.");
                }
                return default(T);
            }

            return (T)service;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetServices<T>(string name = null) where T : IMixedRealityService
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Debug.LogError("This registrar does not support requesting multiple services of the same interface type and name.");
                return new List<T>();
            }

            if (!ConfirmService<T>(name)) { return new List<T>(); }

            List<T> services = new List<T>();
            services.Add((T)service);
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
        public bool RegisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
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
            bool registered = MixedRealityServiceRegistry.AddService<T>(serviceInstance, this);
            if (registered) { service = serviceInstance; }

            return registered;
        }

        /// <inheritdoc />
        public bool RegisterService<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityService
        {
            T serviceInstance = ActivateInstance<T>(concreteType, supportedPlatforms, args);
            if (serviceInstance == null) { return false; }

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
            if (!ConfirmService<T>(name)) { return false; }

            return UnregisterService<T>((T)service);
        }

        /// <inheritdoc />
        public bool UnregisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            if (serviceInstance == null) { return false; }

            // Only remove services that were registered by this registrar instance.
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
            service?.Initialize();
        }

        protected virtual void Uninitialize<T>() where T : IMixedRealityService
        {
            MixedRealityServiceRegistry.RemoveService<T>((T)service, this);
        }

        private bool ConfirmService<T>(string name)
        {
            if ((service == null) || !typeof(T).IsAssignableFrom(service.GetType())) { return false; }
            if (!string.IsNullOrWhiteSpace(name) && !name.Equals(service.Name)) { return false; }

            return true;
        }
    }
}