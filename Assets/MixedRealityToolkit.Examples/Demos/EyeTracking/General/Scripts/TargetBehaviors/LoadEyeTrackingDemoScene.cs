// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// When the button is selected, it triggers starting the specified scene.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class LoadEyeTrackingDemoScene : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Name of the scene to be loaded when the button is selected.")]
        private string SceneToBeLoaded = "";

        [SerializeField]
        [Tooltip("Optional AudioClip which is played when the button is selected.")]
        private AudioClip audio_OnSelect = null;

        [SerializeField]
        [Tooltip("Timeout in seconds before new scene is loaded.")]
        private float waitTimeInSecBeforeLoading = 0.25f;

        public void LoadScene()
        {
            LoadScene(SceneToBeLoaded);
        }

        public async void LoadScene(string sceneName)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                AudioFeedbackPlayer.Instance.PlaySound(audio_OnSelect);
                // Load this as a 'single' scene - this will not destroy the manager scene
                await MixedRealityToolkit.SceneSystem.LoadContent(sceneName, LoadSceneMode.Single);
            }
            else
            {
                Debug.Log($"Unsupported scene name: {sceneName}");
            }
        }
    }
}