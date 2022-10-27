// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    /// <summary>
    /// Utilities that are useful for all runtime tests. Mainly used for scene setup and controlling test flow.
    /// </summary>
    /// <remarks>
    /// Does not include utilities for input tests; see <see cref="RuntimeInputTestUtilities"/>
    /// for utilities for working with simulated input devices.
    /// </remarks>
    public static class RuntimeTestUtilities
    {
        // Unity's default scene name for a recently created scene
        const string playModeTestSceneName = "MixedRealityToolkit.PlayModeTestScene";

        /// <summary>
        /// Creates a play mode test scene, creates an MRTK instance, initializes playspace.
        /// </summary>
        /// <remarks>
        /// Takes an optional MixedRealityToolkitConfigurationProfile used to initialize the MRTK.
        /// </remarks>
        public static void SetupScene()
        {
            Debug.Assert(Application.isPlaying, "This setup method should only be used during play mode tests.");

            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene playModeTestScene = SceneManager.GetSceneAt(i);
                if (playModeTestScene.name == playModeTestSceneName && playModeTestScene.isLoaded)
                {
                    SceneManager.SetActiveScene(playModeTestScene);
                    sceneExists = true;
                }
            }

            if (!sceneExists)
            {
                Scene playModeTestScene = SceneManager.CreateScene(playModeTestSceneName);
                SceneManager.SetActiveScene(playModeTestScene);
            }
        }

        /// <summary>
        /// Destroys all objects in the play mode test scene, if it has been loaded.
        /// </summary>
        public static void TeardownScene()
        {
            Scene playModeTestScene = SceneManager.GetSceneByName(playModeTestSceneName);
            if (playModeTestScene.isLoaded)
            {
                foreach (GameObject gameObject in playModeTestScene.GetRootGameObjects())
                {
                    GameObject.Destroy(gameObject);
                }
            }

            // If we created a temporary untitled scene in edit mode to get us started, unload that now
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene editorScene = SceneManager.GetSceneAt(i);
                if (string.IsNullOrEmpty(editorScene.name))
                {   // We've found our editor scene. Unload it.
                    SceneManager.UnloadSceneAsync(editorScene);
                }
            }
        }

        /// <summary>
        /// Ensures TextMeshProEssentials are installed.
        /// </summary>
        public static void InstallTextMeshProEssentials()
        {
#if UNITY_EDITOR
            // Import the TMP Essential Resources package
            string packageFullPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
            if (Directory.Exists(packageFullPath))
            {
                AssetDatabase.ImportPackage(packageFullPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
            }
            else
            {
                Debug.LogError("Unable to locate the Text Mesh Pro package.");
            }
#endif
        }

        private static bool testPaused;
        /// <summary>
        /// Used for debugging. Pauses the test until the dialog is cleared.
        /// </summary>
        public static IEnumerator PauseTest()
        {
#if UNITY_EDITOR
            if(!Application.isBatchMode)
            {
                PauseDialogWindow.ShowWindow();
                while (testPaused)
                {
                    yield return null;
                };
            }
#endif
        }

        private class PauseDialogWindow : EditorWindow
        {
            public static void ShowWindow()
            {
                testPaused = true;
                var window = GetWindow(typeof(PauseDialogWindow));
                var position = window.position;
                position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
                window.position = position;
                window.Show();
            }

            void OnGUI()
            {
                GUILayout.Label("Test Paused for Debugging", EditorStyles.boldLabel);
                if (GUILayout.Button("Resume Test"))
                {
                    testPaused = false;
                    Close();
                }
            }

            // Make sure that we unpause the test if this window is closed
            void OnDestroy()
            {
                testPaused = false;
            }
        }

        /// <summary>
        /// Sometimes it take a few frames for XRI to process input events.
        /// This method waits for enough frames to pass so that any events
        /// raised actually have time to send to handlers.
        /// We set it fairly conservatively to ensure that after waiting
        /// all input events have been sent.
        /// </summary>
        public static IEnumerator WaitForUpdates(int frameCount = 10)
        {
            yield return new WaitForFixedUpdate();
            for (int i = 0; i < frameCount; i++)
            {
                yield return null;
            }
        }
    }
}
