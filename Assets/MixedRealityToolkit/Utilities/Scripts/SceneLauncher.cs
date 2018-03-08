// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MixedRealityToolkit.Utilities
{
    public class SceneLauncher : Singleton<SceneLauncher>
    {
        [Serializable]
        private class SceneMapping
        {
            public string ScenePath = string.Empty;

            [Tooltip("This toggle enables or disables the generation of a button for this specific scene.")]
            public bool IsButtonEnabled = false;
        }

        [SerializeField]
        [Tooltip("The button scene mapping to keep track of which scenes are enabled in the scene launcher.  This list of scenes is generated from the build window's active scenes.")]
        private SceneMapping[] sceneMapping = null;

        [Tooltip("Location of the center of the grid of buttons in Unity space.")]
        public GameObject ButtonSpawnLocation = null;

        [Tooltip("Prefab used as a button for each scene.")]
        public SceneLauncherButton SceneButtonPrefab = null;

        [Tooltip("Number of rows in the grid of buttons. As more scenes are added, they will spread out horizontally using this number of rows.")]
        public int MaxRows = 5;

        private int SceneLauncherBuildIndex { get; set; }
        private Vector3 sceneButtonSize = Vector3.one;

        private void OnValidate()
        {
            Debug.Assert(SceneButtonPrefab != null, "SceneLauncher.SceneButtonPrefab is not set.");
        }

        protected override void Awake()
        {
            // If we have already initialized,
            // then we've created a second one.
            if (IsInitialized)
            {
                Destroy(gameObject);
            }
            else
            {
                base.Awake();
            }
        }

        private void Start()
        {
            if (SceneButtonPrefab == null)
            {
                return;
            }

            SceneLauncherBuildIndex = SceneManager.GetActiveScene().buildIndex;

            // Determine the size of the buttons. Instantiate one of them so that we can check its bounds.
            SceneLauncherButton sceneButtonForSize = Instantiate(SceneButtonPrefab);
            var sceneButtonForSizeCollider = sceneButtonForSize.GetComponent<Collider>();

            if (sceneButtonForSizeCollider != null)
            {
                sceneButtonSize = sceneButtonForSizeCollider.bounds.size;
            }

            for (int sceneIndex = 0; sceneIndex < sceneMapping.Length; ++sceneIndex)
            {
                if (sceneMapping[sceneIndex].IsButtonEnabled)
                {
                    CreateSceneButton(ButtonSpawnLocation, sceneIndex);
                }
            }

            Destroy(sceneButtonForSize.gameObject);
        }

        private void CreateSceneButton(GameObject buttonParent, int sceneIndex)
        {
            string sceneName = sceneMapping[sceneIndex].ScenePath;
            sceneName = sceneName.Substring(sceneName.LastIndexOf("/", StringComparison.Ordinal) + 1);
            sceneName = sceneName.Replace(".unity", "");
            var scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            Debug.Assert(SceneManager.GetSceneByName(sceneName) == scene);

            SceneLauncherButton sceneButton = Instantiate(SceneButtonPrefab, GetButtonPosition(sceneIndex, sceneMapping.Length), Quaternion.identity, buttonParent.transform);
            sceneButton.SceneIndex = sceneIndex;
            sceneButton.SceneName = sceneName;
            sceneButton.MenuReference = ButtonSpawnLocation;
        }

        private Vector3 GetButtonPosition(int sceneIndex, int numberOfScenes)
        {
            int yCount = Mathf.Min(numberOfScenes, MaxRows);
            int xCount = (numberOfScenes - 1) / yCount + 1;
            int x = sceneIndex % xCount;
            int y = sceneIndex / xCount;
            Debug.Assert(x < xCount && y < yCount);

            // Center a grid of cells in a grid.
            // The top-left corner is shifted .5 cell widths for every row/column after the first one.
            var topLeft = new Vector3((xCount - 1) * -0.5f, (yCount - 1) * 0.5f, 0.0f);
            var cellFromTopLeft = new Vector3(x, -y, 0.0f);
            // Scale by size of the button.
            var positionOffset = Vector3.Scale(topLeft + cellFromTopLeft, new Vector3(sceneButtonSize.x, sceneButtonSize.y, 1.0f));

            return ButtonSpawnLocation.transform.position + positionOffset;
        }

        public void LaunchSceneLoader()
        {
            Debug.LogFormat("SceneLauncher: Returning to SceneLauncher scene {0}.", SceneLauncherBuildIndex);
            SceneManager.LoadSceneAsync(SceneLauncherBuildIndex, LoadSceneMode.Single);
            ButtonSpawnLocation.SetActive(true);
        }
    }
}
