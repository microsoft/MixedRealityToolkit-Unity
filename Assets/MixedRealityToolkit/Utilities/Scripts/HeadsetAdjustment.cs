// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MixedRealityToolkit.Utilities
{
    public class HeadsetAdjustment : MonoBehaviour, IPointerHandler, ISpeechHandler
    {
        public string NextSceneName;

        private void Start()
        {
            InputManager.AddGlobalListener(gameObject);
        }

        public void OnPointerUp(ClickEventData eventData) { }

        public void OnPointerDown(ClickEventData eventData) { }

        public void OnPointerClicked(ClickEventData eventData)
        {
            GotoNextScene();
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            GotoNextScene();
        }

        private void GotoNextScene()
        {
            InputManager.RemoveGlobalListener(gameObject);

            if (!string.IsNullOrEmpty(NextSceneName))
            {
                SceneManager.LoadScene(NextSceneName);
            }
            else
            {
                int sceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(sceneIndex + 1);
            }
        }
    }
}
