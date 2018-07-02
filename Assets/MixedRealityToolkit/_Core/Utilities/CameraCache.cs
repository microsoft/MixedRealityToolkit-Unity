// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
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
        public static Camera Main => cachedCamera == null ? Refresh(Camera.main) : cachedCamera;

        /// <summary>
        /// Set the cached camera to a new reference and return it
        /// </summary>
        /// <param name="newMain">New main camera to cache</param>
        public static Camera Refresh(Camera newMain)
        {
            Debug.Assert(newMain != null, "No camera with MainCamera tag found in scene.  Did you forget to tag your camera?");
            return cachedCamera = newMain;
        }
    }
}
