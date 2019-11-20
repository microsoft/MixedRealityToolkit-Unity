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
        /// Cached reference to the active instance of the input system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityInputSystem InputSystem
        {
            get
            {
                return GetService<IMixedRealityInputSystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the boundary system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                return GetService<IMixedRealityBoundarySystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the spatial awareness system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                return GetService<IMixedRealitySpatialAwarenessSystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the teleport system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem
        {
            get
            {
                return GetService<IMixedRealityTeleportSystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the diagnostics system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                return GetService<IMixedRealityDiagnosticsSystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the camera system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityCameraSystem CameraSystem
        {
            get
            {
                return GetService<IMixedRealityCameraSystem>();
            }
        }

        /// <summary>
        /// Cached reference to the active instance of the camera system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySceneSystem SceneSystem
        {
            get
            {
                return GetService<IMixedRealitySceneSystem>();
            }
        }

        /// <summary>
        /// Resets all cached system references to null
        /// </summary>
        public static void ResetCacheReferences()
        {
            serviceCache.Clear();
        }

        /// <summary>
        /// Clears the cache of the reference with key of given type if present and applicable
        /// </summary>
        /// <param name="serviceType">interface of service to key against. Must be of type IMixedRealityService</param>
        /// <returns>true if succesfully cleared, false otherwise</returns>
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

        // We do not want to keep a service around so use WeakReference
        private static readonly Dictionary<Type, WeakReference<IMixedRealityService>> serviceCache = new Dictionary<Type, WeakReference<IMixedRealityService>>();

        private static T GetService<T>() where T : IMixedRealityService
        {
            Type serviceType = typeof(T);

            // See if we already have a WeakReference entry for this serivce type
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
