// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Gaze;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MixedRealityToolkit.Utilities
{
    public class SceneLauncherButton : MonoBehaviour, IInputClickHandler
    {
        public int SceneIndex { get; set; }

        public string SceneName
        {
            set
            {
                gameObject.name = value;
                textMesh.text = value;
            }
        }

        public Color HighlightedTextColor;

        public GameObject MenuReference;

        public bool EnableDebug;

        private TextMesh textMesh;
        private Color originalTextColor;

        private void Awake()
        {
            textMesh = GetComponentInChildren<TextMesh>();
            Debug.Assert(textMesh != null, "SceneLauncherButton must contain a TextMesh.");
            originalTextColor = textMesh.color;
        }

        private void Update()
        {
            IsHighlighted = GazeManager.Instance.HitObject == gameObject;
        }

        private bool IsHighlighted
        {
            set
            {
                textMesh.color = value ? HighlightedTextColor : originalTextColor;
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (EnableDebug)
            {
                Debug.LogFormat("SceneLauncher: Loading scene {0}: {1}", SceneIndex, SceneManager.GetSceneAt(SceneIndex).name);
            }

            MenuReference.SetActive(false);
            SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Single);
        }
    }
}
