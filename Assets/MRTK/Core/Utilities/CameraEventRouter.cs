// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A helper class to provide hooks into the Unity camera exclusive Lifecycle events
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/CameraEventRouter")]
    public class CameraEventRouter : MonoBehaviour
    {
        /// <summary>
        /// A callback to act upon MonoBehaviour.OnPreRender() without a script needing to exist on a Camera component
        /// </summary>
        public event Action<CameraEventRouter> OnCameraPreRender;

        private void OnPreRender()
        {
            OnCameraPreRender?.Invoke(this);
        }
    }
}