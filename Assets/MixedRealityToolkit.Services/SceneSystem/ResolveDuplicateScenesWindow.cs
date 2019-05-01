// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
        private List<string> currentSceneNames;
        private List<string> newSceneNames;

        public void ResolveDuplicates(Dictionary<string, List<int>> duplicates, List<string> scenes)
        {
            this.duplicates = new Dictionary<string, List<int>>(duplicates);
            this.currentSceneNames = new List<string>(scenes);
            this.newSceneNames = new List<string>(scenes);
        }

        private void OnGUI()
        {
            if (instance == null || duplicates == null || currentSceneNames == null || newSceneNames == null)
            {   // Probably caused by recompilation
                Close();
                return;
            }

            GUI.color = Color.white;
            EditorGUILayout.HelpBox("Some scenes in your build settings have duplicate names. This can cause problems when attempting to load scenes by name. Please ensure the highlighted sceens have unique names.", MessageType.Warning);

            bool readyToApply = true;

            for (int i = 0; i < currentSceneNames.Count; i++)
            {
                bool isDuplicate = duplicates[currentSceneNames[i]].Count > 1;
                GUI.color = isDuplicate ? Color.Lerp(Color.red, Color.white, 0.5f) : Color.white;

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(25));
                GUILayout.Button(currentSceneNames[i], EditorStyles.toolbarButton);

                GUI.color = Color.white;
                if (isDuplicate)
                {
                    newSceneNames[i] = EditorGUILayout.TextField(newSceneNames[i]);
                    readyToApply &= ValidateNewSceneName(i, newSceneNames[i]);
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            GUI.color = readyToApply ? Color.white : Color.Lerp(Color.white, Color.clear, 0.5f);
            if (GUILayout.Button("Rename Scene Assets") && readyToApply)
            {
                // Change the names of the assets
                for (int i = 0; i < currentSceneNames.Count; i++)
                {
                    if (currentSceneNames[i] != newSceneNames[i])
                    {   // We've renamed something
                        // Get the path of the scene at this index
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        string newScenePath = scenePath.Replace(scenePath, newSceneNames[i]);
                        AssetDatabase.RenameAsset(scenePath, newScenePath);
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

            for (int i = 0; i < newSceneNames.Count; i++)
            {
                if (i == index) { continue; }

                if (newSceneNames[i] == newSceneName)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif