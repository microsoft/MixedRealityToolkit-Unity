// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental
{
    /// <summary>
    /// Base class providing service registration and management functionality. This class can be used to implement a
    /// custom service management component for one or more services, similar to the MixedRealityToolkit object.
    /// </summary>
    public class BaseServiceManager : MonoBehaviour, IMixedRealityServiceRegistrar
    {
        /// <summary>
        /// The collection of registered services.
        /// </summary>
        protected Dictionary<Type, IMixedRealityService> registeredServices = new Dictionary<Type, IMixedRealityService>();

        #region MonoBehaviour implementation

        protected virtual void Update()
        {
            if (Application.isPlaying)
            {
                foreach (IMixedRealityService service in registeredServices.Values)
                {
                    service.Update();
                }
            }
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                if (Application.isPlaying)
                {
                    foreach (IMixedRealityService service in registeredServices.Values)
                    {
                        service.LateUpdate();
                    }
                }
            }
        }


        protected virtual void OnEnable()
        {
            if (Application.isPlaying)
            {
                foreach (IMixedRealityService service in registeredServices.Values)
                {
                    service.Enable();
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                foreach (IMixedRealityService service in registeredServices.Values)
                {
                    service.Disable();
                }
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (IMixedRealityService service in registeredServices.Values)
            {
                service.Disable(); // Disable before destroy to ensure the service has time to get in a good state.
                service.Destroy();
            }
            registeredServices.Clear();
        }

        #endregion MonoBehaviour implementation

        #region IMixedRealityServiceRegistrar implementation

        /// <inheritdoc />
        public T GetService<T>(string name = null, bool showLogs = true) where T : IMixedRealityService
        {
            T serviceInstance = FindService<T>(name);

            if (showLogs && (serviceInstance == null))
            {
                Debug.LogError($"Failed to get the requested service of type {typeof(T)}.");
            }

            return serviceInstance;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetServices<T>(string name = null) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);
            List<T> matchingServices = new List<T>();

            foreach(IMixedRealityService service in registeredServices.Values)
            {
                if (!interfaceType.IsAssignableFrom(service.GetType())) { continue; }

                // If a name has been provided and if it matches the services's name, add the service.
                if (!string.IsNullOrWhiteSpace(name) && string.Equals(service.Name, name))
                {
                    matchingServices.Add((T)service);
                }
                // If no name has been specified, always add the service.
                else
                {
                    matchingServices.Add((T)service);
                }
            }

            return matchingServices;
        }

        /// <inheritdoc />
        public bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService
        {
            return (GetService<T>(name) != null);
        }

        /// <inheritdoc />
        public bool RegisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);

            if (registeredServices.ContainsKey(interfaceType))
            {
                Debug.LogError($"Failed to register {serviceInstance} service. There is already a registered service implementing {interfaceType}");
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

        /// <summary>
        /// Initialize a service.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be initialized.</typeparam>
        /// <param name="concreteType">The concrete type of the service to initialize.</param>
        /// <param name="supportedPlatforms">The platform(s) on which the service is supported.</param>
        /// <param name="args">Arguments to provide to the service class constructor.</param>
        protected virtual void Initialize<T>(Type concreteType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args) where T : IMixedRealityService
        {
            if (!RegisterService<T>(concreteType, supportedPlatforms, args))
            {
                Debug.LogError($"Failed to register the {concreteType.Name} service.");
            }

            T serviceInstance = FindService<T>();
            serviceInstance?.Initialize();
        }

        /// <summary>
        /// Uninitialize a service.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to uninitialize.</typeparam>
        protected virtual void Uninitialize<T>() where T : IMixedRealityService
        {
            T serviceInstance = FindService<T>();

            if (serviceInstance != null)
            {
                registeredServices.Remove(typeof(T));
                MixedRealityServiceRegistry.RemoveService<T>(serviceInstance, this);
            }
        }

        /// <summary>
        /// Locates a service instance in the registry,
        /// </summary>
        /// <typeparam name="T">The interface type of the service to locate.</typeparam>
        /// <param name="name">The name of the desired service.</param>
        /// <returns>Instance of the interface type, or null if not found.</returns>
        private T FindService<T>(string name = null) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);
            IMixedRealityService serviceInstance;

            if (!registeredServices.TryGetValue(interfaceType, out serviceInstance)) { return default(T);  }

            return (T)serviceInstance;
         }
    }
}