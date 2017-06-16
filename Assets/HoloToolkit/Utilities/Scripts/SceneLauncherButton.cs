// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
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
            IsHighlighted = GazeManager.Instance.HitObject == this.gameObject;
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
            Debug.LogFormat("SceneLauncher: Loading scene {0}: {1}", SceneIndex, SceneList.Instance.GetSceneNames()[SceneIndex]);
            SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
        }
    }
}
