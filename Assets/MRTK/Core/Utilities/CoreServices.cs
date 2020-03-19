// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Teleport;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utility class to easily access references to core runtime Mixed Reality Toolkit Services
    /// If deallocating and re-allocating a new system at runtime, ResetCacheReferences() should be used to get a proper reference
    /// </summary>
    public static class CoreServices
    {
        /// <summary>
        /// Cached reference to the active instance of the boundary system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem => GetService<IMixedRealityBoundarySystem>();

        /// <summary>
        /// Cached reference to the active instance of the camera system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityCameraSystem CameraSystem => GetService<IMixedRealityCameraSystem>();

        /// <summary>
        /// Cached reference to the active instance of the diagnostics system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem => GetService<IMixedRealityDiagnosticsSystem>();

        /// <summary>
        /// Cached reference to the active instance of the focus provider.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityFocusProvider FocusProvider => GetService<IMixedRealityFocusProvider>();

        /// <summary>
        /// Cached reference to the active instance of the input system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => GetService<IMixedRealityInputSystem>();

        /// <summary>
        /// Cached reference to the active instance of the raycast provider.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityRaycastProvider RaycastProvider => GetService<IMixedRealityRaycastProvider>();

        /// <summary>
        /// Cached reference to the active instance of the scene system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySceneSystem SceneSystem => GetService<IMixedRealitySceneSystem>();

        /// <summary>
        /// Cached reference to the active instance of the spatial awareness system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => GetService<IMixedRealitySpatialAwarenessSystem>();

        /// <summary>
        /// Cached reference to the active instance of the teleport system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem => GetService<IMixedRealityTeleportSystem>();

        /// <summary>
        /// Resets all cached system references to null
        /// </summary>
        public static void ResetCacheReferences() => serviceCache.Clear();

        /// <summary>
        /// Clears the cache of the reference with key of given type if present and applicable
        /// </summary>
        /// <param name="serviceType">interface of service to key against. Must be of type IMixedRealityService</param>
        /// <returns>true if successfully cleared, false otherwise</returns>
        public static bool ResetCacheReference(Type serviceType)
        {
            if (typeof(IMixedRealityService).IsAssignableFrom(serviceType))
            {
                if (serviceCache.ContainsKey(serviceType))
                {
                    serviceCache.Remove(serviceType);
                    return true;
                }
            }
            else
            {
                Debug.Log("Cache only contains types that implement IMixedRealityService");
            }

            return false;
        }

        /// <summary>
        /// Gets first matching <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputDeviceManager"/> or extension thereof for CoreServices.InputSystem
        /// </summary>
        public static T GetInputSystemDataProvider<T>() where T : IMixedRealityInputDeviceManager
        {
            return GetDataProvider<T>(InputSystem);
        }

        /// <summary>
        /// Gets first matching <see cref="Microsoft.MixedReality.Toolkit.SpatialAwareness.IMixedRealitySpatialAwarenessObserver"/> or extension thereof for CoreServices.SpatialAwarenessSystem
        /// </summary>
        public static T GetSpatialAwarenessSystemDataProvider<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            return GetDataProvider<T>(SpatialAwarenessSystem);
        }

        /// <summary>
        /// Gets first matching data provider of provided type T registered to the provided mixed reality service.
        /// </summary>
        /// <typeparam name="T">Type of data provider to return. Must implement and/or extend from <see cref="Microsoft.MixedReality.Toolkit.IMixedRealityDataProvider" /></typeparam>
        /// <param name="service">This function will attempt to get first available data provider registered to this service.</param>
        /// <remarks>
        /// Service parameter is expected to implement <see cref="Microsoft.MixedReality.Toolkit.IMixedRealityDataProviderAccess" />. If not, then will return default(T)
        /// </remarks>
        public static T GetDataProvider<T>(IMixedRealityService service) where T : IMixedRealityDataProvider
        {
            var dataProviderAccess = service as IMixedRealityDataProviderAccess;
            if (dataProviderAccess != null)
            {
                return dataProviderAccess.GetDataProvider<T>();
            }

            return default(T);
        }

        // We do not want to keep a service around so use WeakReference
        private static readonly Dictionary<Type, WeakReference<IMixedRealityService>> serviceCache = new Dictionary<Type, WeakReference<IMixedRealityService>>();

        private static T GetService<T>() where T : IMixedRealityService
        {
            Type serviceType = typeof(T);

            // See if we already have a WeakReference entry for this service type
            if (serviceCache.ContainsKey(serviceType))
            {
                IMixedRealityService svc;
                // If our reference object is still alive, return it
                if (serviceCache[serviceType].TryGetTarget(out svc))
                {
                    return (T)svc;
                }

                // Our reference object has been collected by the GC. Try to get the latest service if available
                serviceCache.Remove(serviceType);
            }

            // This is the first request for the given service type. See if it is available and if so, add entry
            T service;
            if (!MixedRealityServiceRegistry.TryGetService(out service))
            {
                return default(T);
            }

            serviceCache.Add(typeof(T), new WeakReference<IMixedRealityService>(service, false));
            return service;
        }
    }
}
