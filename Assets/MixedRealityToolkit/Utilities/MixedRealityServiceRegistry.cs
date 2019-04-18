// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    // todo: collection

    /// <summary>
    /// Static class that represents the Mixed Reality Toolkit service registry.
    /// </summary>
    /// <remarks>
    /// The service registry is used to enable discovery of and access to active Mixed Reality Toolkit services at
    /// runtime without requiring direct code reference to a singleton style component.
    /// </remarks>
    public static class MixedRealityServiceRegistry
    {
        // todo: is this the data type we want?
        /// <summary>
        /// The service registry store where the key is the Type of the service interface and the value is
        /// a pair in which they key is the service instance and the value is the regisrar instance.
        /// </summary>
        private static Dictionary<Type, List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>>> registry = 
            new Dictionary<Type, List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>>>();

        /// <summary>
        /// Static constructor.
        /// </summary>
        static MixedRealityServiceRegistry()
        { }

        /// <summary>
        /// Adds an <see cref="IMixedRealityService"/> instance to the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being added.</typeparam>
        /// <param name="serviceInstance">Instance of the service to add.</param>
        /// <param name="registrar">Instance of the registrar manages the service.</param>
        /// <returns>
        /// True if the service was successfully added, false otherwise.
        /// </returns>
        public static bool AddService<T>(T serviceInstance, IMixedRealityServiceRegistrar registrar) where T : IMixedRealityService
        {
            T existingService;
            bool added = false;

            if (!TryGetService<T>(out existingService, serviceInstance.Name))
            {
                // add service
                // todo
                added = true;
            }

            return added;
        }

        /// <summary>
        /// Adds an <see cref="IMixedRealityService"/> instance to the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being removed.</typeparam>
        /// <param name="serviceInstance">Instance of the service to add.</param>
        /// <param name="registrar">Instance of the registrar manages the service.</param>
        /// <returns>
        /// True if the service was successfully added, false otherwise.
        /// </returns>
        public static bool RemoveService<T>(T serviceInstance, IMixedRealityServiceRegistrar registrar) where T : IMixedRealityService
        {
            return RemoveServiceInternal(typeof(T), serviceInstance, registrar);
        }

        public static bool RemoveService<T>(T serviceInstance)
        {
            // todo
            return false;
        }

        private static bool RemoveServiceInternal(
            Type interfaceType,
            IMixedRealityService serviceInstance,
            IMixedRealityServiceRegistrar registrar)
        {
            // todo
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceInstance"></param>
        /// <param name="registrar"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetService<T>(
            out T serviceInstance,
            out IMixedRealityServiceRegistrar registrar,
            string name = null)
        {
            Type interfaceType = typeof(T);

            if (!registry.ContainsKey(interfaceType))
            {
                serviceInstance = default(T);
                registrar = null;
                return false;
            }

            List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>> services = registry[interfaceType];
            Debug.Assert(services.Count > 0, $"Service registry returned 0 items. AddService appears to have failed for {interfaceType.Name}");

            int registryIndex = -1;
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Find the desired service by it's name.
                for (int i = 0; i < registry.Count; i++)
                {
                    if (services[i].Key.Name != name) { continue; }

                    registryIndex = i;
                    break;
                }

                if (registryIndex == -1)
                {
                    // Failed to find the requested service.
                    serviceInstance = default(T);
                    registrar = null;
                    return false;
                }
            }
            else
            {
                // Use the first service found
                registryIndex = 0;
            }

            IMixedRealityService tempService = services[registryIndex].Key;
            Debug.Assert(tempService is T, "The service in the registry does not match the expected type.");

            serviceInstance = (T)tempService;
            registrar = services[registryIndex].Value;
            return true;
        }

        /// <summary>
        /// Gets the instance of the requested service from the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being requested.</typeparam>
        /// <param name="serviceInstance">Output parameter to receive the requested service instance.</param>
        /// <param name="name">Optional name of the service.</param>
        /// <returns>
        /// True if the requested service is being returned, false otherwise.
        /// </returns>
        public static bool TryGetService<T>(
            out T serviceInstance,
            string name = null) where T : IMixedRealityService
        {
            IMixedRealityServiceRegistrar registrar;

            return TryGetService<T>(
                out serviceInstance,
                out registrar,
                name);
        }
    }
}
