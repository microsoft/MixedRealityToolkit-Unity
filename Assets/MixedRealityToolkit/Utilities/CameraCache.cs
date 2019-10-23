// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
                    Debug.LogWarning("No main camera found. The Mixed Reality Toolkit requires one camera in the scene to be tagged as \"MainCamera\".");
                    Debug.Log("Searching for cameras in the scene.");

                    // If no main camera was found, try to determine one.
                    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
                    switch (cameras.Length)
                    {
                        case 0:
                            Debug.LogWarning("No cameras found. Creating a \"MainCamera\".");
                            mainCamera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();
                            break;

                        case 1:
                            Debug.LogWarning("Camera found. Tagging the camera as \"MainCamera\".");
                            cameras[0].tag = "MainCamera";
                            mainCamera = cameras[0];
                            break;

                        default:
                            Debug.LogError("More than one camera found. Please tag one of the cameras as \"MainCamera\".");
                            break;
                    }
                }

                // Cache the main camera
                cachedCamera = mainCamera;

                return cachedCamera;
            }
        }
    }
}
