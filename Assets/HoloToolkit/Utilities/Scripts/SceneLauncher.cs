// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
{
    public class SceneLauncher : MonoBehaviour
    {
        public Interactive SceneButtonPrefab;
        public Vector3 ButtonCenterLocation = new Vector3(0, 0, 1);
        public int MaxRows = 5;
        public KeywordManager ReturnToSceneLauncherPrefab;

        private Vector3 sceneButtonSize = Vector3.one;

        private void Start()
        {
            if (SceneButtonPrefab == null)
            {
                Debug.LogError("SceneLauncher.SceneButtonPrefab is not set.");
                return;
            }

            if (ReturnToSceneLauncherPrefab == null)
            {
                Debug.LogWarning("SceneLauncher.ReturnToSceneLauncherPrefab is not set. You won't be able to return to the scene launcher after loading a different scene.");
            }
            else
            {
                KeywordManager returnToSceneLauncher = Instantiate(ReturnToSceneLauncherPrefab);
                DontDestroyOnLoad(returnToSceneLauncher);
                if (returnToSceneLauncher.KeywordsAndResponses.Length == 0)
                {
                    Debug.LogWarning("SceneLauncher.ReturnToSceneLauncherPrefab has no keywords. You won't be able to return to the scene launcher after loading a different scene.");
                }
                else
                {
                    int sceneLauncherBuildIndex = SceneManager.GetActiveScene().buildIndex;
                    UnityAction action = delegate
                    {
                        Debug.LogFormat("SceneLauncher: Returning to SceneLauncher scene {0}.", sceneLauncherBuildIndex);
                        SceneManager.LoadScene(sceneLauncherBuildIndex, LoadSceneMode.Single);
                        GameObject.Destroy(returnToSceneLauncher);
                    };
                    returnToSceneLauncher.KeywordsAndResponses[0].Response.AddListener(action);
                }
            }

            List<string> sceneNames = SceneList.Instance.GetSceneNames();
            for (int iScene = 0; iScene < sceneNames.Count; ++iScene)
            {
                string sceneName = sceneNames[iScene];
                Scene scene = SceneManager.GetSceneByBuildIndex(iScene);
                Debug.Assert(SceneManager.GetSceneByName(sceneName) == scene);

                Interactive sceneButton = Instantiate<Interactive>(SceneButtonPrefab);
                if (iScene == 0)
                {
                    Collider sceneButtonCollider = sceneButton.GetComponent<Collider>();
                    if (sceneButtonCollider != null)
                    {
                        sceneButtonSize = sceneButtonCollider.bounds.size;
                    }
                }
                sceneButton.transform.position = GetButtonPosition(iScene, sceneNames.Count);
                sceneButton.IsEnabled = scene != SceneManager.GetActiveScene(); // Disable button to launch our own scene.
                int buildIndex = iScene;
                UnityAction action = delegate
                {
                    OnButtonSelect(buildIndex);
                };
                sceneButton.OnSelectEvents.AddListener(action);
                LabelTheme labelTheme = sceneButton.GetComponent<LabelTheme>();
                if (labelTheme != null)
                {
                    labelTheme.Default = sceneName;
                }
            }
        }

        private Vector3 GetButtonPosition(int iScene, int numberOfScenes)
        {
            int yCount = Mathf.Min(numberOfScenes, MaxRows);
            int xCount = (numberOfScenes - 1) / yCount + 1;
            int x = iScene % xCount;
            int y = iScene / xCount;
            Debug.Assert(x < xCount && y < yCount);

            // Center a grid of cells in a grid.
            // The top-left corner is shifted .5 cell widths for every row/column after the first one.
            Vector3 topLeft = new Vector3((xCount - 1) * -0.5f, (yCount - 1) * 0.5f, 0.0f);
            Vector3 cellFromTopLeft = new Vector3(x, -y, 0.0f);
            // Scale by size of the button.
            Vector3 positionOffset = Vector3.Scale(topLeft + cellFromTopLeft, new Vector3(sceneButtonSize.x, sceneButtonSize.y, 1.0f));

            return ButtonCenterLocation + positionOffset;
        }

        private void OnButtonSelect(int buildIndex)
        {
            Debug.LogFormat("SceneLauncher: Loading scene {0}: {1}", buildIndex, SceneList.Instance.GetSceneNames()[buildIndex]);
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }
    }
}
