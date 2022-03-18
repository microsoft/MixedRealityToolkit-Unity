// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A scriptable object which contains data and UI functionality to swap object (asset) references within a scene.
    /// </summary>
    [CreateAssetMenu(fileName = "AssetSwapCollection", menuName = "Mixed Reality/Toolkit/AssetSwapCollection")]
    public class AssetSwapUtility : ScriptableObject
    {
        [System.Serializable]
        public class Theme
        {
            /// <summary>
            /// The name of the theme (for the user only).
            /// </summary>
            public string Name;

            /// <summary>
            /// The assets that represent the theme.
            /// </summary>
            public List<Object> Assets;
        }

        /// <summary>
        /// The list of themes within the collection. Must contain at least 2 items.
        /// </summary>
        public List<Theme> Themes = new List<Theme>();

        /// <summary>
        /// The number of assets in each theme. Must contain at least 1 item.
        /// </summary>
        public int AssetCount = 1;
    }

#if UNITY_EDITOR

    /// <summary>
    /// Editor which displays on a AssetSwapUtility scriptable object.
    /// </summary>
    [CustomEditor(typeof(AssetSwapUtility))]
    public class AssetSwapUtilityEditor : UnityEditor.Editor
    {
        // Table.
        private SerializedProperty themeCount;
        private SerializedProperty assetCount;
        private Vector2 scrollPosition;

        // Controls.
        private int selectedThemeIndex;

        private enum SelectionMode
        {
            WholeScene,
            SelectionWithChildren,
            SelectionWithoutChildren
        }

        private SelectionMode selectionMode;

        [MenuItem("Mixed Reality/Toolkit/Utilities/Create Asset Swap Collection")]
        public static void CreateAssetSwapCollection()
        {
            AssetSwapUtility utility = CreateInstance<AssetSwapUtility>();
            AssetDatabase.CreateAsset(utility, AssetDatabase.GenerateUniqueAssetPath("Assets/AssetSwapCollection.asset"));
            AssetDatabase.SaveAssets();

            Selection.activeObject = utility;
            EditorUtility.FocusProjectWindow();
        }

        void OnEnable()
        {
            themeCount = serializedObject.FindProperty($"{nameof(AssetSwapUtility.Themes)}.Array.size");
            assetCount = serializedObject.FindProperty("AssetCount");
        }

        public override void OnInspectorGUI()
        {
            DrawTable();

            AssetSwapUtility utility = serializedObject.targetObject as AssetSwapUtility;

            if (utility != null)
            {
                DrawControls(utility);
            }
        }

        private void DrawTable()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(themeCount, new GUIContent("Themes (Minimum of 2)"));

                    int value = themeCount.intValue;

                    if (GUILayout.Button("+"))
                    {
                        ++value;
                    }

                    if (GUILayout.Button("-"))
                    {
                        --value;
                    }

                    themeCount.intValue = Mathf.Max(value, 2);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(assetCount, new GUIContent("Assets (Minimum of 1)"));

                    int value = assetCount.intValue;

                    if (GUILayout.Button("+"))
                    {
                        ++value;
                    }

                    if (GUILayout.Button("-"))
                    {
                        --value;
                    }

                    assetCount.intValue = Mathf.Max(value, 1);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            const float minThemeWidth = 128.0f;
            const float minThemeHeight = 18.0f;
            int themeCountInt = themeCount.intValue;
            int assetCountInt = assetCount.intValue;

            EditorGUILayout.BeginHorizontal("Box");
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height((3 + assetCountInt) * minThemeHeight));
                {
                    EditorGUILayout.BeginHorizontal("Box");
                    {
                        for (int i = 0; i < themeCountInt; ++i)
                        {
                            SerializedProperty themeData = serializedObject.FindProperty($"{nameof(AssetSwapUtility.Themes)}.Array.data[{i}]");

                            EditorGUILayout.BeginVertical("Box");
                            {
                                SerializedProperty themeDataName = themeData.FindPropertyRelative(nameof(AssetSwapUtility.Theme.Name));
                                EditorGUILayout.PropertyField(themeDataName, GUIContent.none, false, GUILayout.MinWidth(minThemeWidth));

                                SerializedProperty themeDataAssets = themeData.FindPropertyRelative($"{nameof(AssetSwapUtility.Theme.Assets)}.Array");

                                SerializedProperty assetsCount = themeDataAssets.FindPropertyRelative("size");
                                assetsCount.intValue = assetCountInt;

                                for (int j = 0; j < assetCountInt; ++j)
                                {
                                    SerializedProperty assetsData = themeDataAssets.FindPropertyRelative($"data[{j}]");

                                    EditorGUILayout.PropertyField(assetsData, GUIContent.none, false, GUILayout.MinWidth(minThemeWidth));
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawControls(AssetSwapUtility utility)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                List<string> displayedOptions = new List<string>(utility.Themes.Count);

                for (int i = 0; i < utility.Themes.Count; ++i)
                {
                    var theme = utility.Themes[i];
                    displayedOptions.Add(GetThemeName(theme, i));
                }

                selectedThemeIndex = EditorGUILayout.Popup("Selected Theme", selectedThemeIndex, displayedOptions.ToArray());
                selectionMode = (SelectionMode)EditorGUILayout.EnumPopup("Selection Mode", selectionMode);

                string warning;
                GUI.enabled = ValidateThemeCollection(utility, out warning);

                GameObject[] gameObjects = null;

                if (GUI.enabled == true)
                {
                    GUI.enabled = ValidateSelection(selectionMode, out gameObjects, out warning);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Apply"))
                {
                    Apply(gameObjects,
                          utility,
                          utility.Themes[selectedThemeIndex],
                          selectionMode != SelectionMode.SelectionWithoutChildren);
                }

                if (GUI.enabled == false)
                {
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static bool ValidateThemeCollection(AssetSwapUtility utility, out string warning)
        {
            // Make sure all themes have valid assets and each row contains similar types.
            for (int i = 0; i < utility.Themes[0].Assets.Count; ++i)
            {
                System.Type type = null;

                for (int j = 0; j < utility.Themes.Count; ++j)
                {
                    if (utility.Themes[j].Assets[i] == null)
                    {
                        warning = $"Theme \"{GetThemeName(utility.Themes[j], j)}\" asset index {i} is null.";
                        return false;
                    }

                    System.Type currentType = utility.Themes[j].Assets[i].GetType();

                    if (j == 0)
                    {
                        type = currentType;
                    }
                    else if (currentType != type)
                    {
                        warning = $"Theme \"{GetThemeName(utility.Themes[j], j)}\" asset index {i} is of mismatched type. Expected \"{type}\" and got \"{currentType}\".";
                        return false;
                    }
                }
            }

            warning = string.Empty;
            return true;
        }

        private static bool ValidateSelection(SelectionMode selectionMode, out GameObject[] gameObjects, out string warning)
        {
            switch (selectionMode)
            {
                case SelectionMode.WholeScene:
                {
                    gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

                    if (gameObjects.Length == 0)
                    {
                        warning = "The scene is empty, please create at least one game object.";
                        return false;
                    }
                    break;
                }

                default:
                case SelectionMode.SelectionWithChildren:
                case SelectionMode.SelectionWithoutChildren:
                {
                    gameObjects = Selection.gameObjects;

                    if (gameObjects.Length == 0)
                    {
                        warning = "Please select at least one game object in the scene. Locking this panel may help in selection.";
                        return false;
                    }
                    break;
                }
            }

            warning = string.Empty;
            return true;
        }

        private static void Apply(GameObject[] gameObjects, AssetSwapUtility utility, AssetSwapUtility.Theme selectedTheme, bool recurse)
        {
            int progress = 0;
            foreach (var gameObject in gameObjects)
            {
                EditorUtility.DisplayProgressBar("Applying Theme Change", "Please wait...", (float)progress / gameObjects.Length);

                if (recurse)
                {
                    SwapObjectReferencesRecurse(gameObject, utility, selectedTheme);
                }
                else
                {
                    SwapObjectReferences(gameObject, utility, selectedTheme);
                }

                ++progress;
            }

            EditorUtility.ClearProgressBar();
        }

        private static void SwapObjectReferences(GameObject gameObject, AssetSwapUtility utility, AssetSwapUtility.Theme selectedTheme)
        {
            var components = gameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();
                bool modified = false;

                while (property.NextVisible(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference &&
                        property.objectReferenceValue != null)
                    {
                        Object currentAsset = property.objectReferenceValue;

                        // Does the current asset match any non-selected theme(s) asset(s)?
                        if (currentAsset != null)
                        {
                            foreach (var theme in utility.Themes)
                            {
                                if (theme == selectedTheme)
                                {
                                    continue;
                                }

                                int assetIndex = 0;

                                foreach (var asset in theme.Assets)
                                {
                                    if (asset == currentAsset)
                                    {
                                        property.objectReferenceValue = selectedTheme.Assets[assetIndex];
                                        modified = true;
                                    }

                                    ++assetIndex;
                                }
                            }
                        }
                    }
                }

                if (modified == true)
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private static void SwapObjectReferencesRecurse(GameObject gameObject, AssetSwapUtility utility, AssetSwapUtility.Theme selectedTheme)
        {
            SwapObjectReferences(gameObject, utility, selectedTheme);

            foreach (Transform child in gameObject.transform)
            {
                SwapObjectReferencesRecurse(child.gameObject, utility, selectedTheme);
            }
        }

        private static string GetThemeName(AssetSwapUtility.Theme theme, int index)
        {
            return string.IsNullOrEmpty(theme.Name) ? $"Unnamed {index}" : theme.Name;
        }
    }

#endif // UNITY_EDITOR
}
