// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The purpose of this class is to provide a cached reference to the main camera. Calling Camera.main
    /// executes a FindByTag on the scene, which will get worse and worse with more tagged objects.
    /// </summary>
    public static class CameraCache
    {
        private static Camera cachedCamera;

        /// <summary>
        /// Returns a cached reference to the main camera and uses Camera.main if it hasn't been cached yet.
        /// </summary>
        public static Camera Main
        {
            get
            {
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
                {
                    Debug.Log("No main camera found. Searching for cameras in the scene.");

                    // If no main camera was found, try to determine one.
                    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    if (cameras.Length == 0)
                    {
                        Debug.LogWarning("No cameras found. Creating a \"MainCamera\".");
                        mainCamera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();
                    }
                    else
                    {
                        Debug.LogWarning("The Mixed Reality Toolkit requires one camera in the scene to be tagged as \"MainCamera\". Please ensure the application's main camera is tagged.");
                    }
                }

                // Cache the main camera
                cachedCamera = mainCamera;
                return cachedCamera;
            }
        }

        /// <summary>
        /// Manually update the cached main camera 
        /// </summary>
        public static void UpdateCachedMainCamera(Camera camera)
        {
            cachedCamera = camera;
        }
    }
}
