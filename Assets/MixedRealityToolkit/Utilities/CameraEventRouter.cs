// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A helper class to provide hooks into the Unity camera exclusive Lifecycle events
    /// </summary>
    public class CameraEventRouter : MonoBehaviour
    {
        /// <summary>
        /// A callback to act upon <see href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPreRender.html">MonoBehaviour.OnPreRender()</see>
        /// without a script needing to exist on a <see href="https://docs.unity3d.com/ScriptReference/Camera.html">Camera</see> component.
        /// </summary>
        public event Action<CameraEventRouter> OnCameraPreRender;

        private void OnPreRender()
        {
            OnCameraPreRender?.Invoke(this);
        }
    }
}