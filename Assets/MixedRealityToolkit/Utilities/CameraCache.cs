// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The purpose of this class is to provide a cached reference to the main camera.
    /// If the camera system is enabled, the camera cache will return a reference to that system's camera.
    /// Otherwise a default camera will be created.
    /// </summary>
    public static class CameraCache
    {
        private static Camera cachedCamera;
        private static IMixedRealityCameraSystem cameraSystem;

        /// <summary>
        /// Returns a cached reference to the main camera. Uses the camera system's main camera, if available. Otherwise finds or creates a main camera.
        /// </summary>
        public static Camera Main
        {
            get
            {
                if (MixedRealityToolkit.IsCameraSystemEnabled)
                {
                    if (cameraSystem == null)
                    {
                        cameraSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityCameraSystem>();
                    }
                    return cameraSystem.Main;
                }

                if (cachedCamera != null)
                {
                    if (cachedCamera.gameObject.activeInHierarchy)
                    {   // If the cached camera is active, return it
                        // Otherwise, our playspace may have been disabled
                        // We'll have to search for the next available
                        return cachedCamera;
                    }
                }

                // If the cached camera is null, search for main
                var mainCamera = Camera.main;

                if (mainCamera == null)
                {   // If no main camera was found, create it now
                    Debug.LogWarning("No main camera found. The Mixed Reality Toolkit requires at least one camera in the scene. One will be generated now.");
                    mainCamera = new GameObject("Main Camera", typeof(Camera)) { tag = "MainCamera" }.GetComponent<Camera>();
                }

                // Cache the main camera
                cachedCamera = mainCamera;

                return cachedCamera;
            }
        }
    }
}
