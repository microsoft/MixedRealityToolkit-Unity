// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Allows for adding custom behaviors that can be assigned in the Editor and triggered when the scene is loaded. 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/TriggerOnStartup")]
    public class TriggerOnStartup : MonoBehaviour
    {
        [Tooltip("Event handler when the scene is loaded.")]
        [SerializeField]
        private UnityEvent OnSceneStart = null;

        private void Start()
        {
            OnSceneStart.Invoke();
        }
    }
}
