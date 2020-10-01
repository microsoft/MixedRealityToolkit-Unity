// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Static class that represents the Mixed Reality Toolkit service registry.
    /// </summary>
    /// <remarks>
    /// The service registry is used to enable discovery of and access to active Mixed Reality Toolkit services at
    /// runtime without requiring direct code reference to a singleton style component.
    /// </remarks>
    public static class MixedRealityServiceRegistry
    {
        /// <summary>
        /// The service registry store where the key is the Type of the service interface and the value is
        /// a pair in which they key is the service instance and the value is the registrar instance.
        /// </summary>
        private static Dictionary<Type, List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>>> registry =
            new Dictionary<Type, List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>>>();

        /// <summary>
        /// A cache used to power <seealso cref="GetAllServices(IMixedRealityServiceRegistrar)"/>
        /// </summary>
        /// <remarks>
        /// Lists are sorted in ascending priority order (i.e. services with a smaller priority
        /// value are first in the list).
        /// </remarks>
        private static Dictionary<IMixedRealityServiceRegistrar, List<IMixedRealityService>> allServicesByRegistrar =
            new Dictionary<IMixedRealityServiceRegistrar, List<IMixedRealityService>>();

        /// <summary>
        /// A cache used to power <seealso cref="GetAllServices"/>
        /// </summary>
        /// <remarks>
        /// The list is sorted in ascending priority order (i.e. services with a smaller priority
        /// value are first in the list).
        /// </remarks>
        private static List<IMixedRealityService> allServices = new List<IMixedRealityService>();

        /// <summary>
        /// A comparer used to sort the allServices and allServiceByRegistrar lists in-place.
        /// </summary>
        private static readonly Comparer<IMixedRealityService> ascendingOrderComparer =
            Comparer<IMixedRealityService>.Create((i1, i2) => i1.Priority.CompareTo(i2.Priority));

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
            if (serviceInstance == null)
            {
                // Adding a null service instance is not supported.
                return false;
            }

            if (serviceInstance is IMixedRealityDataProvider)
            {
                // Data providers are generally not used by application code. Services that intend for clients to
                // directly communicate with their data providers will expose a GetDataProvider or similarly named
                // method.
                return false;
            }

            Type interfaceType = typeof(T);
            T existingService;

            if (TryGetService<T>(out existingService, serviceInstance.Name))
            {
                return false;
            }

            // Ensure we have a place to put our newly registered service.
            if (!registry.ContainsKey(interfaceType))
            {
                registry.Add(interfaceType, new List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>>());
            }

            List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>> services = registry[interfaceType];
            services.Add(new KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>(serviceInstance, registrar));
            AddServiceToCache(serviceInstance, registrar);
            return true;
        }

        /// <summary>
        /// Removes an <see cref="IMixedRealityService"/> instance from the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being removed.</typeparam>
        /// <param name="serviceInstance">Instance of the service to remove.</param>
        /// <param name="registrar">Instance of the registrar manages the service.</param>
        /// <returns>
        /// True if the service was successfully removed, false otherwise.
        /// </returns>
        public static bool RemoveService<T>(T serviceInstance, IMixedRealityServiceRegistrar registrar) where T : IMixedRealityService
        {
            return RemoveServiceInternal(typeof(T), serviceInstance, registrar);
        }

        /// <summary>
        /// Removes an <see cref="IMixedRealityService"/> instance from the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being removed.</typeparam>
        /// <param name="serviceInstance">Instance of the service to remove.</param>
        /// <returns>
        /// True if the service was successfully removed, false otherwise.
        /// </returns>
        public static bool RemoveService<T>(T serviceInstance) where T : IMixedRealityService
        {
            T tempService;
            IMixedRealityServiceRegistrar registrar;

            if (!TryGetService<T>(out tempService, out registrar))
            {
                return false;
            }

            if (!object.ReferenceEquals(serviceInstance, tempService))
            {
                return false;
            }

            return RemoveServiceInternal(typeof(T), serviceInstance, registrar);
        }

        /// <summary>
        /// Removes an <see cref="IMixedRealityService"/> instance from the registry.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being removed.</typeparam>
        /// <param name="name">The friendly name of the service to remove.</param>
        /// <returns>
        /// True if the service was successfully removed, false otherwise.
        /// </returns>
        public static bool RemoveService<T>(string name) where T : IMixedRealityService
        {
            T tempService;
            IMixedRealityServiceRegistrar registrar;

            if (!TryGetService<T>(out tempService, out registrar, name))
            {
                return false;
            }

            return RemoveServiceInternal(typeof(T), tempService, registrar);
        }

        /// <summary>
        /// Removes an <see cref="IMixedRealityService"/> instance from the registry.
        /// </summary>
        /// <param name="interfaceType">The interface type of the service being removed.</param>
        /// <param name="serviceInstance">Instance of the service to remove.</param>
        /// <param name="registrar">Instance of the registrar manages the service.</param>
        /// <returns>
        /// True if the service was successfully removed, false otherwise.
        /// </returns>
        private static bool RemoveServiceInternal(
            Type interfaceType,
            IMixedRealityService serviceInstance,
            IMixedRealityServiceRegistrar registrar)
        {
            if (!registry.ContainsKey(interfaceType)) { return false; }

            List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>> services = registry[interfaceType];

            bool removed = services.Remove(new KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>(serviceInstance, registrar));

            if (services.Count == 0)
            {
                // If the last service was removed, the key can be removed.
                registry.Remove(interfaceType);
            }

            RemoveServiceFromCache(serviceInstance, registrar);

            return removed;
        }

        /// <summary>
        /// Adds the given service/registrar combination to the GetAllServices cache
        /// </summary>
        private static void AddServiceToCache(
            IMixedRealityService service,
            IMixedRealityServiceRegistrar registrar)
        {
            // Services are stored in ascending priority order - adding them to the
            // list requires that we re-enforce that order. This must happen
            // in both the allServices and allServicesByRegistrar data structures.
            allServices.Add(service);
            allServices.Sort(ascendingOrderComparer);

            if (!allServicesByRegistrar.ContainsKey(registrar))
            {
                allServicesByRegistrar.Add(registrar, new List<IMixedRealityService>());
            }

            allServicesByRegistrar[registrar].Add(service);
            allServicesByRegistrar[registrar].Sort(ascendingOrderComparer);
        }

        /// <summary>
        /// Removes the given service/registrar combination from the GetAllServices cache
        /// </summary>
        private static void RemoveServiceFromCache(
            IMixedRealityService service,
            IMixedRealityServiceRegistrar registrar)
        {
            // Removing from the sorted list keeps sort order, so re-sorting isn't necessary
            allServices.Remove(service);
            if (allServicesByRegistrar.ContainsKey(registrar))
            {
                allServicesByRegistrar[registrar].Remove(service);
                if (allServicesByRegistrar[registrar].Count == 0)
                {
                    allServicesByRegistrar.Remove(registrar);
                }
            }
        }

        /// <summary>
        /// Gets the first instance of the requested service from the registry that matches the given query.
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
            return TryGetService<T>(
                out serviceInstance,
                out _,                  // The registrar out param is not used, it can be discarded.
                name);
        }

        /// <summary>
        /// Gets the first instance of the requested service from the registry that matches the given query.
        /// </summary>
        /// <typeparam name="T">The interface type of the service being requested.</typeparam>
        /// <param name="serviceInstance">Output parameter to receive the requested service instance.</param>
        /// <param name="registrar">Output parameter to receive the registrar that loaded the service instance.</param>
        /// <param name="name">Optional name of the service.</param>
        /// <returns>
        /// True if the requested service is being returned, false otherwise.
        /// </returns>
        public static bool TryGetService<T>(
            out T serviceInstance,
            out IMixedRealityServiceRegistrar registrar,
            string name = null) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);

            IMixedRealityService tempService;
            if (TryGetServiceInternal(interfaceType, out tempService, out registrar, name))
            {
                Debug.Assert(tempService is T, "The service in the registry does not match the expected type.");
                serviceInstance = (T)tempService;
                return true;
            }

            serviceInstance = default(T);
            registrar = null;
            return false;
        }

        /// <summary>
        /// Gets the first instance of the requested service from the registry that matches the given query.
        /// </summary>
        /// <param name="interfaceType">The interface type of the service being requested.</param>
        /// <param name="serviceInstance">Output parameter to receive the requested service instance.</param>
        /// <param name="registrar">Output parameter to receive the registrar that loaded the service instance.</param>
        /// <param name="name">Optional name of the service.</param>
        /// <returns>
        /// True if the requested service is being returned, false otherwise.
        /// </returns>
        public static bool TryGetService(Type interfaceType,
            out IMixedRealityService serviceInstance,
            out IMixedRealityServiceRegistrar registrar,
            string name = null)
        {
            if (!typeof(IMixedRealityService).IsAssignableFrom(interfaceType))
            {
                Debug.LogWarning($"Cannot find type {interfaceType.Name} since it does not extend IMixedRealityService");
                serviceInstance = null;
                registrar = null;
                return false;
            }

            return TryGetServiceInternal(interfaceType, out serviceInstance, out registrar, name);
        }

        private static readonly ProfilerMarker TryGetServiceInternalPerfMarker = new ProfilerMarker("[MRTK] MixedRealityServiceRegistry.TryGetServiceInternal");

        private static bool TryGetServiceInternal(Type interfaceType,
            out IMixedRealityService serviceInstance,
            out IMixedRealityServiceRegistrar registrar,
            string name = null)
        {
            using (TryGetServiceInternalPerfMarker.Auto())
            {
                // Assume failed and return null unless proven otherwise
                serviceInstance = null;
                registrar = null;

                // If there is an entry for the interface key provided, search that small list first
                if (registry.ContainsKey(interfaceType))
                {
                    if (FindEntry(registry[interfaceType], interfaceType, name, out serviceInstance, out registrar))
                    {
                        return true;
                    }
                }

                // Either there is no entry for the interface type, or it was not placed in that list. 
                // Services can have multiple supported interfaces thus they may match the requested query but be placed in a different registry bin
                // Thus, search all bins until a match is found
                foreach (var list in registry.Values)
                {
                    if (FindEntry(list, interfaceType, name, out serviceInstance, out registrar))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static readonly ProfilerMarker FindEntryPerfMarker = new ProfilerMarker("[MRTK] MixedRealityServiceRegistry.FindEntry");

        /// <summary>
        /// Helper method to search list of IMixedRealityService/IMixedRealityServiceRegistrar pairs to find first service that matches name and interface type query
        /// </summary>
        /// <param name="serviceList">list of IMixedRealityService/IMixedRealityServiceRegistrar pairs to search</param>
        /// <param name="interfaceType">type of interface to check</param>
        /// <param name="name">name of service to check. Wildcard if null or empty</param>
        /// <param name="serviceInstance">reference to IMixedRealityService matching query, null otherwise</param>
        /// <param name="registrar">reference to IMixedRealityServiceRegistrar matching query, null otherwise</param>
        /// <returns>true if found first entry to match query, false otherwise</returns>
        private static bool FindEntry(List<KeyValuePair<IMixedRealityService, IMixedRealityServiceRegistrar>> serviceList,
            Type interfaceType,
            string name,
            out IMixedRealityService serviceInstance,
            out IMixedRealityServiceRegistrar registrar)
        {
            using (FindEntryPerfMarker.Auto())
            {
                // Assume failed and return null unless proven otherwise
                serviceInstance = null;
                registrar = null;

                for (int i = 0; i < serviceList.Count; ++i)
                {
                    var svc = serviceList[i].Key;
                    if ((string.IsNullOrEmpty(name) || svc.Name == name) && interfaceType.IsAssignableFrom(svc.GetType()))
                    {
                        serviceInstance = svc;
                        registrar = serviceList[i].Value;

                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Clears the registry cache of all services
        /// </summary>
        public static void ClearAllServices()
        {
            if (registry != null)
            {
                registry.Clear();
                allServices.Clear();
                allServicesByRegistrar.Clear();
            }
        }

        /// <summary>
        /// Returns readonly list of all services registered
        /// </summary>
        /// <remarks>
        /// The list is sorted in ascending priority order.
        /// </remarks>
        public static IReadOnlyList<IMixedRealityService> GetAllServices()
        {
            return allServices;
        }

        /// <summary>
        /// Returns readonly list of all services registered for given registrar
        /// </summary>
        /// <param name="registrar">Registrar object to filter services by</param>
        /// <remarks>
        /// The list is sorted in ascending priority order.
        /// </remarks>
        /// <returns>Readonly list of all services registered for given registrar, all services if parameter null.
        /// If given a registrar that the registry is not aware of, returns null.
        /// </returns>
        public static IReadOnlyCollection<IMixedRealityService> GetAllServices(IMixedRealityServiceRegistrar registrar)
        {
            if (registrar == null)
            {
                return GetAllServices();
            }
            if (allServicesByRegistrar.TryGetValue(registrar, out List<IMixedRealityService> services))
            {
                return services;
            }
            return null;
        }
    }
}