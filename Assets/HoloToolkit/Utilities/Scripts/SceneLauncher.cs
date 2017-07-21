// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
{
    public class SceneLauncher : MonoBehaviour
    {
        [Tooltip("Prefab used as a button for each scene.")]
        public SceneLauncherButton SceneButtonPrefab;
        [Tooltip("Location of the center of the grid of buttons in Unity space.")]
        public Vector3 ButtonCenterLocation = new Vector3(0, 0, 1);
        [Tooltip("Number of rows in the grid of buttons. As more scenes are added, they will spread out horizontally using this number of rows.")]
        public int MaxRows = 5;
        [Tooltip("Prefab that will continue running when another scene is launched, offering a command to return to the scene launcher.")]
        public KeywordManager ReturnToSceneLauncherPrefab;

        private Vector3 sceneButtonSize = Vector3.one;

        private void OnValidate()
        {
            Debug.Assert(SceneButtonPrefab != null, "SceneLauncher.SceneButtonPrefab is not set.");
            Debug.Assert(ReturnToSceneLauncherPrefab != null, "SceneLauncher.ReturnToSceneLauncherPrefab is not set.");
            if (ReturnToSceneLauncherPrefab != null)
            {
                Debug.Assert(ReturnToSceneLauncherPrefab.KeywordsAndResponses.Length > 0, "SceneLauncher.ReturnToSceneLauncherPrefab has a KeywordManager with no keywords.");
            }
        }

        private void Start()
        {
            if (SceneButtonPrefab == null)
            {
                return;
            }

            if (ReturnToSceneLauncherPrefab != null)
            {
                KeywordManager returnToSceneLauncher = Instantiate(ReturnToSceneLauncherPrefab);
                DontDestroyOnLoad(returnToSceneLauncher);
                if (returnToSceneLauncher.KeywordsAndResponses.Length > 0)
                {
                    // Set the response action of the first keyword to reload this scene.
                    int sceneLauncherBuildIndex = SceneManager.GetActiveScene().buildIndex;
                    UnityAction keywordAction = delegate
                    {
                        Debug.LogFormat("SceneLauncher: Returning to SceneLauncher scene {0}.", sceneLauncherBuildIndex);
                        SceneManager.LoadScene(sceneLauncherBuildIndex, LoadSceneMode.Single);
                        GameObject.Destroy(returnToSceneLauncher.gameObject);
                    };
                    returnToSceneLauncher.KeywordsAndResponses[0].Response.AddListener(keywordAction);
                }
            }

            // Determine the size of the buttons. Instantiate one of them so that we can check its bounds.
            SceneLauncherButton sceneButtonForSize = Instantiate(SceneButtonPrefab);
            Collider sceneButtonForSizeCollider = sceneButtonForSize.GetComponent<Collider>();
            if (sceneButtonForSizeCollider != null)
            {
                sceneButtonSize = sceneButtonForSizeCollider.bounds.size;
            }
            Destroy(sceneButtonForSize.gameObject);

            // Create an empty game object to serve as a parent for all the buttons we're about to create.
            GameObject buttonParent = new GameObject("Buttons");

            List<string> sceneNames = SceneList.Instance.GetSceneNames();
            for (int sceneIndex = 0; sceneIndex < sceneNames.Count; ++sceneIndex)
            {
                CreateSceneButton(buttonParent, sceneIndex, sceneNames);
            }
        }

        private void CreateSceneButton(GameObject buttonParent, int sceneIndex, List<string> sceneNames)
        {
            string sceneName = sceneNames[sceneIndex];
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            Debug.Assert(SceneManager.GetSceneByName(sceneName) == scene);

            SceneLauncherButton sceneButton = Instantiate(SceneButtonPrefab, GetButtonPosition(sceneIndex, sceneNames.Count), Quaternion.identity, buttonParent.transform);
            sceneButton.SceneIndex = sceneIndex;
            sceneButton.SceneName = sceneName;
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

            return ButtonCenterLocation + positionOffset;
        }
    }
}
