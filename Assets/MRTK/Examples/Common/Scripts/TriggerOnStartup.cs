// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
