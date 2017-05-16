using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Switches scenes, next scene or previous scene
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        /// <summary>
        /// Load the next scene, if at the end of the list, load the first scene
        /// </summary>
        public void NextScene()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; ++i)
            {
                if (i < sceneCount - 1)
                {
                    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(i))
                    {
                        SceneManager.LoadScene(i + 1);
                        break;
                    }
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
            }
        }

        /// <summary>
        /// Load the previous scene, if at the beginning of the list, load the first scene
        /// </summary>
        public void PreviousScene()
        {
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; ++i)
            {
                if (i > 1)
                {
                    if (SceneManager.GetActiveScene() == SceneManager.GetSceneAt(i))
                    {
                        SceneManager.LoadScene(i - 1);
                        break;
                    }
                }
                else
                {
                    SceneManager.LoadScene(sceneCount - 1);
                }
            }
        }
    }
}
