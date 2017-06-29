// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
{
    public class SceneLauncher : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of scenes in the build.  Updating this list, also updates the scene build settings!")]
        public List<string> SceneList;

        [SerializeField]
        [Tooltip("Location of the center of the grid of buttons in Unity space.")]
        public GameObject ButtonSpawnLocation;

        [SerializeField]
        [Tooltip("Prefab used as a button for each scene.")]
        public SceneLauncherButton SceneButtonPrefab;

        [SerializeField]
        [Tooltip("Number of rows in the grid of buttons. As more scenes are added, they will spread out horizontally using this number of rows.")]
        public int MaxRows = 5;

        public int SceneLauncherBuildIndex { get; private set; }
        private Vector3 sceneButtonSize = Vector3.one;

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

            for (int sceneIndex = 0; sceneIndex < SceneList.Count; ++sceneIndex)
            {
                CreateSceneButton(ButtonSpawnLocation, sceneIndex);
            }
            Destroy(sceneButtonForSize.gameObject);
        }

        private void CreateSceneButton(GameObject buttonParent, int sceneIndex)
        {
            string sceneName = SceneList[sceneIndex];
            sceneName = sceneName.Substring(sceneName.LastIndexOf("/", StringComparison.Ordinal) + 1);
            sceneName = sceneName.Replace(".unity", "");
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            Debug.Assert(SceneManager.GetSceneByName(sceneName) == scene);

            SceneLauncherButton sceneButton = Instantiate(SceneButtonPrefab, GetButtonPosition(sceneIndex, SceneList.Count), Quaternion.identity, buttonParent.transform);
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
            Vector3 topLeft = new Vector3((xCount - 1) * -0.5f, (yCount - 1) * 0.5f, 0.0f);
            Vector3 cellFromTopLeft = new Vector3(x, -y, 0.0f);
            // Scale by size of the button.
            Vector3 positionOffset = Vector3.Scale(topLeft + cellFromTopLeft, new Vector3(sceneButtonSize.x, sceneButtonSize.y, 1.0f));

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
