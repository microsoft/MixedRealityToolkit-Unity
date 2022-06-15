// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        private static IMixedRealityBoundarySystem boundarySystem;

        /// <summary>
        /// Cached reference to the active instance of the boundary system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem => boundarySystem ?? (boundarySystem = GetService<IMixedRealityBoundarySystem>());

        private static IMixedRealityCameraSystem cameraSystem;

        /// <summary>
        /// Cached reference to the active instance of the camera system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityCameraSystem CameraSystem => cameraSystem ?? (cameraSystem = GetService<IMixedRealityCameraSystem>());

        private static IMixedRealityDiagnosticsSystem diagnosticsSystem;

        /// <summary>
        /// Cached reference to the active instance of the diagnostics system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem => diagnosticsSystem ?? (diagnosticsSystem = GetService<IMixedRealityDiagnosticsSystem>());

        private static IMixedRealityFocusProvider focusProvider;

        /// <summary>
        /// Cached reference to the active instance of the focus provider.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityFocusProvider FocusProvider => focusProvider ?? (focusProvider = GetService<IMixedRealityFocusProvider>());

        private static IMixedRealityInputSystem inputSystem;

        /// <summary>
        /// Cached reference to the active instance of the input system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = GetService<IMixedRealityInputSystem>());

        private static IMixedRealityRaycastProvider raycastProvider;

        /// <summary>
        /// Cached reference to the active instance of the raycast provider.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityRaycastProvider RaycastProvider => raycastProvider ?? (raycastProvider = GetService<IMixedRealityRaycastProvider>());

        private static IMixedRealitySceneSystem sceneSystem;

        /// <summary>
        /// Cached reference to the active instance of the scene system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySceneSystem SceneSystem => sceneSystem ?? (sceneSystem = GetService<IMixedRealitySceneSystem>());

        private static IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;

        /// <summary>
        /// Cached reference to the active instance of the spatial awareness system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => spatialAwarenessSystem ?? (spatialAwarenessSystem = GetService<IMixedRealitySpatialAwarenessSystem>());

        private static IMixedRealityTeleportSystem teleportSystem;

        /// <summary>
        /// Cached reference to the active instance of the teleport system.
        /// If system is destroyed, reference will be invalid. Please use ResetCacheReferences() 
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem => teleportSystem ?? (teleportSystem = GetService<IMixedRealityTeleportSystem>());

        /// <summary>
        /// Resets all cached system references to null
        /// </summary>
        public static void ResetCacheReferences()
        {
            serviceCache.Clear();
            boundarySystem = null;
            cameraSystem = null;
            diagnosticsSystem = null;
            focusProvider = null;
            inputSystem = null;
            raycastProvider = null;
            sceneSystem = null;
            spatialAwarenessSystem = null;
            teleportSystem = null;
        } 

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
                    ResetCacheReferenceFromType(serviceType);
                    return true;
                }
            }
            else
            {
                Debug.Log("Cache only contains types that implement IMixedRealityService");
            }

            return false;
        }

        private static void ResetCacheReferenceFromType(Type serviceType)
        {
            if (typeof(IMixedRealityBoundarySystem).IsAssignableFrom(serviceType))
            {
                boundarySystem = null;
            }
            else if (typeof(IMixedRealityCameraSystem).IsAssignableFrom(serviceType))
            {
                cameraSystem = null;
            }
            else if (typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(serviceType))
            {
                diagnosticsSystem = null;
            }
            else if (typeof(IMixedRealityFocusProvider).IsAssignableFrom(serviceType))
            {
                focusProvider = null;
            }
            else if (typeof(IMixedRealityInputSystem).IsAssignableFrom(serviceType))
            {
                inputSystem = null;
            }
            else if (typeof(IMixedRealityRaycastProvider).IsAssignableFrom(serviceType))
            {
                raycastProvider = null;
            }
            else if (typeof(IMixedRealitySceneSystem).IsAssignableFrom(serviceType))
            {
                sceneSystem = null;
            }
            else if (typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(serviceType))
            {
                sceneSystem = null;
            }
            else if (typeof(IMixedRealityTeleportSystem).IsAssignableFrom(serviceType))
            {
                teleportSystem = null;
            }
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
        /// Gets first matching <see cref="Microsoft.MixedReality.Toolkit.CameraSystem.IMixedRealityCameraSettingsProvider"/> or extension thereof for CoreServices.CameraSystem
        /// </summary>
        public static T GetCameraSystemDataProvider<T>() where T : IMixedRealityCameraSettingsProvider
        {
            return GetDataProvider<T>(CameraSystem);
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
            if (service is IMixedRealityDataProviderAccess dataProviderAccess)
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
            if (serviceCache.TryGetValue(serviceType, out WeakReference<IMixedRealityService> weakService))
            {
                IMixedRealityService svc;
                // If our reference object is still alive, return it
                if (weakService.TryGetTarget(out svc))
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

            serviceCache.Add(serviceType, new WeakReference<IMixedRealityService>(service, false));
            return service;
        }
    }
}
