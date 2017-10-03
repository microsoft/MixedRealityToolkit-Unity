// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Examples
{
    public class LevelManager : MonoBehaviour
    {
        public void LoadNextScene()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                sceneIndex = 0;
            }

            SceneManager.LoadScene(sceneIndex);
        }
    }
}
