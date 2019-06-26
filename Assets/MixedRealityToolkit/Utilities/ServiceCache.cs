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

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utility class to easily access references to runtime Mixed Reality Toolkit Services
    /// If deallocating and re-allocating a new system at runtime, ResetCacheReferences() should be used to get a proper reference
    /// </summary>
    public static class ServiceCache
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

        private static Dictionary<Type, IMixedRealityService> serviceCache = new Dictionary<Type, IMixedRealityService>();

        private static T GetService<T>() where T : IMixedRealityService
        {
            Type serviceType = typeof(T);

            if (serviceCache.ContainsKey(serviceType))
            {
                return (T)serviceCache[serviceType];
            }
            else
            {
                T service;
                MixedRealityServiceRegistry.TryGetService<T>(out service);
                if (service == null)
                {
                    return service;
                }

                serviceCache.Add(typeof(T), service as IMixedRealityService);
                return service;
            }
        }
    }
}
