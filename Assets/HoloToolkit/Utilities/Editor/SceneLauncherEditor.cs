using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(SceneLauncher))]
    public class SceneLauncherEditor : Editor
    {
        private GUIContent removeButtonContent = new GUIContent("Remove Scene");
        private GUIContent addButtonContent = new GUIContent("Add Scene");
        private GUILayoutOption miniButtonWidth = GUILayout.Width(120.0f);

        private SceneLauncher sceneLauncher;
        private SerializedProperty sceneListProperty;
        private SerializedProperty buttonSpawnLocationProperty;
        private SerializedProperty buttonPrefabProperty;
        private SerializedProperty buttonRowMaxProperty;

        private void OnEnable()
        {
            sceneLauncher = (SceneLauncher)target;
            sceneListProperty = serializedObject.FindProperty("SceneList");
            buttonSpawnLocationProperty = serializedObject.FindProperty("ButtonSpawnLocation");
            buttonPrefabProperty = serializedObject.FindProperty("SceneButtonPrefab");
            buttonRowMaxProperty = serializedObject.FindProperty("MaxRows");

            serializedObject.Update();
            CheckBuildScenes(sceneListProperty);
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowList(sceneListProperty);
            EditorGUILayout.PropertyField(buttonSpawnLocationProperty);
            EditorGUILayout.PropertyField(buttonPrefabProperty);
            EditorGUILayout.IntSlider(buttonRowMaxProperty, 1, 10);
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowList(SerializedProperty list)
        {
            bool updateBuildScenes = false;

            // property name with expansion widget
            EditorGUILayout.PropertyField(list);

            if (EditorBuildSettings.scenes.Length == 0)
            {
                list.ClearArray();
                int index = list.arraySize;
                list.InsertArrayElementAtIndex(index);
                list.GetArrayElementAtIndex(index).stringValue = SceneManager.GetActiveScene().path;
                updateBuildScenes = true;
            }

            if (list.isExpanded)
            {
                EditorGUI.indentLevel++;

                // add button row
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // the add element button
                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton, miniButtonWidth))
                {
                    var index = list.arraySize;
                    list.InsertArrayElementAtIndex(index);
                    list.GetArrayElementAtIndex(index).stringValue = string.Empty;
                }

                EditorGUILayout.EndHorizontal();

                // Scene rows
                for (int index = 0; index < list.arraySize; index++)
                {
                    string path = list.GetArrayElementAtIndex(index).stringValue;
                    var sceneAsset = AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)) as SceneAsset;

                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = index != sceneLauncher.SceneLauncherBuildIndex;

                    // the element
                    var sceneObj = EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);

                    if (GUI.changed)
                    {
                        path = sceneObj != null ? AssetDatabase.GetAssetPath(sceneObj) : string.Empty;
                        updateBuildScenes = true;
                    }

                    list.GetArrayElementAtIndex(index).stringValue = path;

                    // the remove element button
                    if (GUILayout.Button(removeButtonContent, EditorStyles.miniButton, miniButtonWidth))
                    {
                        list.DeleteArrayElementAtIndex(index);
                        updateBuildScenes = true;
                    }

                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            if (updateBuildScenes && list.arraySize > 0)
            {
                UpdateBuildSettings(list);
            }

            CheckBuildScenes(list);
        }

        private void CheckBuildScenes(SerializedProperty list)
        {
            if (list.arraySize != EditorBuildSettings.scenes.Length)
            {
                list.ClearArray();
                for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    list.InsertArrayElementAtIndex(i);
                    list.GetArrayElementAtIndex(i).stringValue = EditorBuildSettings.scenes[i].path;
                }
            }
        }

        private void UpdateBuildSettings(SerializedProperty list)
        {
            var sceneList = new List<string>();
            for (int index = 0; index < list.arraySize; index++)
            {
                string path = list.GetArrayElementAtIndex(index).stringValue;
                if (!string.IsNullOrEmpty(path))
                {
                    sceneList.Add(path);
                }
            }

            var buildSceneList = new EditorBuildSettingsScene[sceneList.Count];
            for (var i = 0; i < buildSceneList.Length; i++)
            {
                buildSceneList[i] = new EditorBuildSettingsScene(sceneList[i], true);
            }

            EditorBuildSettings.scenes = buildSceneList;
        }
    }
}
