// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Provides a list of names of scenes that were included in the Unity build settings.
    /// This is necessary because SceneManager doesn't provide the names of scenes before they are loaded
    /// and EditorBuildSettingsScene is only availabe in the editor.
    /// </summary>
    public class SceneList : Singleton<SceneList>
    {
#pragma warning disable 0414, 0649 // sceneNamesUpdatedByBuild isn't assigned to, but it is set via Unity serialization
        [SerializeField]
        [HideInInspector]
        private List<string> sceneNamesUpdatedByBuild;
#pragma warning restore 0414, 0649

        protected override void Awake()
        {
            base.Awake();
            LogSceneNames();
        }

        /// <summary>
        /// Print the list of scene names to Debug.Log.
        /// </summary>
        private void LogSceneNames()
        {
            List<string> sceneNames = new List<string>(GetSceneNames());
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("SceneList has {0} scenes: ", sceneNames.Count);
            for (int iScene = 0; iScene < sceneNames.Count; ++iScene)
            {
                string sceneName = sceneNames[iScene];
                if (iScene > 0)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(sceneName);
            }
            Debug.Log(stringBuilder.ToString());
        }

        /// <summary>
        /// Get the list of scene names. Can be called either in the editor or from the running app.
        /// </summary>
        /// <returns>The list of scene names in build index order.</returns>
        public List<string> GetSceneNames()
        {
#if UNITY_EDITOR
            // Use scene names from build settings directly from the editor.
            return GetSceneNamesFromEditor();
#else
            // Use the scene names serialized by OnPostProcessScene.
            return sceneNamesUpdatedByBuild;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get the list of scene names. Can only be called in the editor.
        /// </summary>
        /// <returns>The list of scene names in build index order.</returns>
        private static List<string> GetSceneNamesFromEditor()
        {
            List<string> result = new List<string>();
            foreach (UnityEditor.EditorBuildSettingsScene buildScene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (!buildScene.enabled)
                {
                    continue;
                }
                string name = Path.GetFileNameWithoutExtension(buildScene.path);
                result.Add(name);
            }
            return result;
        }

        /// <summary>
        /// Called by Unity when building the scene.
        /// Saves the list of scene names in sceneNamesUpdatedByBuild.
        /// </summary>
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            if (Application.isPlaying)
            {
                return;
            }

            SceneList sceneList = FindObjectOfType<SceneList>();
            if (sceneList == null)
            {
                return;
            }
            sceneList.sceneNamesUpdatedByBuild = new List<string>(GetSceneNamesFromEditor());
            sceneList.LogSceneNames();
        }
#endif
    }
}
