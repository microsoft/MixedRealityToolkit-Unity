// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Alerts the user to duplicate scene names and provides a way to resolve them.
    /// </summary>
    public class ResolveDuplicateScenesWindow : EditorWindow
    {
        public static bool IsOpen { get { return instance != null; } }
        public static ResolveDuplicateScenesWindow Instance { get { return instance; } }

        private static ResolveDuplicateScenesWindow instance;

        private Dictionary<string, List<int>> duplicates;
        private List<SceneInfo> currentScenes;
        private List<SceneInfo> newScenes;

        public void ResolveDuplicates(Dictionary<string, List<int>> duplicates, List<SceneInfo> scenes)
        {
            this.duplicates = new Dictionary<string, List<int>>(duplicates);
            this.currentScenes = new List<SceneInfo>(scenes);
            this.newScenes = new List<SceneInfo>(scenes);
        }

        private void OnGUI()
        {
            if (instance == null || duplicates == null || currentScenes == null || newScenes == null)
            {   // Probably caused by recompilation
                Close();
                return;
            }

            GUI.color = Color.white;
            EditorGUILayout.HelpBox("Some scenes in your build settings have duplicate names. This can cause problems when attempting to load scenes by name. Please ensure the highlighted scenes have unique names.", MessageType.Warning);

            bool readyToApply = true;

            for (int i = 0; i < currentScenes.Count; i++)
            {
                bool isDuplicate = duplicates[currentScenes[i].Name].Count > 1;
                GUI.color = isDuplicate ? Color.Lerp(Color.red, Color.white, 0.5f) : Color.white;

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(currentScenes[i].BuildIndex.ToString(), GUILayout.MaxWidth(25));
                if (GUILayout.Button(currentScenes[i].Name, EditorStyles.miniButton))
                {
                    EditorGUIUtility.PingObject(currentScenes[i].Asset);
                }

                GUI.color = Color.white;
                if (isDuplicate)
                {
                    SceneInfo newSceneName = newScenes[i];
                    newSceneName.Name = EditorGUILayout.TextField(newSceneName.Name);
                    newScenes[i] = newSceneName;
                    readyToApply &= ValidateNewSceneName(i, newSceneName.Name);
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            GUI.color = readyToApply ? Color.white : Color.Lerp(Color.white, Color.clear, 0.5f);
            if (GUILayout.Button("Rename Scene Assets") && readyToApply)
            {
                // Change the names of the assets
                for (int i = 0; i < currentScenes.Count; i++)
                {
                    if (currentScenes[i].Name != newScenes[i].Name)
                    {   // We've renamed something
                        // Rename the asset now
                        AssetDatabase.RenameAsset(newScenes[i].Path, newScenes[i].Name);
                    }
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
                Close();
            }
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void OnDisable()
        {
            instance = null;
        }

        private bool ValidateNewSceneName(int index, string newSceneName)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                return false;
            }

            for (int i = 0; i < newScenes.Count; i++)
            {
                if (i == index) { continue; }

                if (newScenes[i].Name == newSceneName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif
