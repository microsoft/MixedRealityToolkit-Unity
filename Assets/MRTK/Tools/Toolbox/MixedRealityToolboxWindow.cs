// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.EditModeTests")]
namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Inspector class that renders the MRTK toolbox for easy access to creating out-of-box UX prefabs in the current scene
    /// </summary>
    public class MixedRealityToolboxWindow : EditorWindow
    {
        private static readonly string SearchDisplaySearchFieldKey = "MixedRealityToolboxWindow.SearchField";
        private const string WindowTitle = "MRTK Toolbox";

        private Vector2 scrollPos;
        private string searchString;
        private const float ToolboxItemWidth = 125f;
        private const float ToolboxItemButtonWidth = 100f;
        private const float ToolboxItemHeight = 64f;

        private const string RequiresCanvas = "(Requires Canvas)";
        private const float ReloadButtonWidth = 60f;
        private static readonly GUIContent CanvasDropdownContent = new GUIContent("MRTK canvases", "Canvases in the scene with an MRTK Canvas Utility attached.\nComponent will be placed under the selected canvas in the hierarchy.");
        private static readonly GUIContent RefreshButtonContent = new GUIContent("Refresh");

        /// <summary>
        /// Represents a collection of categories, each containing specific prefabs to be displayed in the Toolbox.
        /// </summary>
        [Serializable]
        internal class ToolboxItemCollection
        {
            [SerializeField]
            private ToolboxCategory[] categories = null;

            /// <summary>
            /// The prefab type categories represented by this collection.
            /// </summary>
            public ToolboxCategory[] Categories => categories;
        }

        /// <summary>
        /// Represents a distinct categorization of prefabs for display in the toolbox window.
        /// </summary>
        [Serializable]
        internal class ToolboxCategory
        {
            [SerializeField]
            private string categoryName = string.Empty;

            /// <summary>
            /// The friendly name representing this category.
            /// </summary>
            public string CategoryName => categoryName;

            [SerializeField]
            private ToolboxItem[] items = null;

            /// <summary>
            /// The prefabs represented by this category.
            /// </summary>
            public ToolboxItem[] Items => items;
        }

        /// <summary>
        /// Represents a prefab and additional data about the prefab for the toolbox.
        /// </summary>
        [Serializable]
        internal class ToolboxItem
        {
            [SerializeField]
            private string name = string.Empty;

            /// <summary>
            /// The friendly name of this prefab.
            /// </summary>
            public string Name => name;

            [SerializeField]
            private string docURL = string.Empty;

            /// <summary>
            /// A link to documentation about this prefab.
            /// </summary>
            public string DocURL => docURL;

            [SerializeField]
            private string assetGUID = string.Empty;

            /// <summary>
            /// The path from the AssetDatabase to the prefab.
            /// </summary>
            public string AssetPath => AssetDatabase.GUIDToAssetPath(assetGUID);

            [SerializeField]
            private string iconGUID = string.Empty;

            /// <summary>
            /// The path from the AssetDatabase to the icon representing this prefab in the toolbox UI.
            /// </summary>
            public string IconPath => AssetDatabase.GUIDToAssetPath(iconGUID);

            /// <summary>
            /// The actual prefab GameObject represented by this item.
            /// </summary>
            public GameObject Prefab { get; protected set; }

            /// <summary>
            /// The icon representing this prefab in the toolbox UI.
            /// </summary>
            public Texture Icon { get; protected set; }

            /// <summary>
            /// Initialize this item by loading the prefab and icon from the AssetDatabase.
            /// </summary>
            public void Init()
            {
                Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
                Icon = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);
            }
        }

        private const string JSONDataGUID = "23b7f096c52f5d74a90b0b05c72ca03c";
        private string JSONDataPath => AssetDatabase.GUIDToAssetPath(JSONDataGUID);

        internal ToolboxItemCollection toolBoxCollection;
        internal ToolboxCategory[] ToolboxPrefabs
        {
            get
            {
                if (toolBoxCollection == null)
                {
                    try
                    {
                        toolBoxCollection = JsonUtility.FromJson<ToolboxItemCollection>(File.ReadAllText(JSONDataPath));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }

                    foreach (var bucket in toolBoxCollection.Categories)
                    {
                        foreach (var item in bucket.Items)
                        {
                            item.Init();
                        }
                    }
                }

                return toolBoxCollection.Categories;
            }

        }

        private GUIStyle centeredStyle;

        [MenuItem("Mixed Reality/Toolkit/Toolbox", false, 3)]
        internal static void ShowWindow()
        {
            var window = GetWindow<MixedRealityToolboxWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("Grid.BoxTool").image);
            window.Show();
        }

        internal static void HideWindow()
        {
            var window = GetWindow<MixedRealityToolboxWindow>(typeof(SceneView));
            window.Close();
        }

        private void OnEnable()
        {
            searchString = SessionState.GetString(SearchDisplaySearchFieldKey, string.Empty);
            FindAllMRTKCanvases();
        }

        private void OnGUI()
        {
            centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.UpperCenter,
                wordWrap = true
            };

            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Search: ", GUILayout.MaxWidth(70));
                searchString = EditorGUILayout.TextField(searchString, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.MaxWidth(50)))
                {
                    searchString = string.Empty;
                }

                SessionState.SetString(SearchDisplaySearchFieldKey, searchString);
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
                {
                    scrollPos = scrollView.scrollPosition;

                    foreach (var bucket in ToolboxPrefabs)
                    {
                        RenderSection(bucket);
                    }
                }
            }
        }

        private Input.Utilities.CanvasUtility[] canvasUtilities = Array.Empty<Input.Utilities.CanvasUtility>();
        private string[] dropdownValues = Array.Empty<string>();
        private int dropdownIndex = 0;

        private void RenderSection(ToolboxCategory bucket)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                string key = $"MixedRealityToolboxWindow_{bucket.CategoryName}";
                if (InspectorUIUtility.DrawSectionFoldoutWithKey(bucket.CategoryName, key, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
                {
                    InspectorUIUtility.DrawDivider();
                    EditorGUILayout.Space();

                    bool isCategoryNameSearchMatch = IsSearchMatch(bucket.CategoryName, searchString);

                    List<ToolboxItem> validItems = new List<ToolboxItem>();
                    foreach (var item in bucket.Items)
                    {
                        if (item != null && item.Prefab != null
                            && (isCategoryNameSearchMatch || IsSearchMatch(item, searchString)))
                        {
                            validItems.Add(item);
                        }
                    }

                    bool requiresCanvas = bucket.CategoryName.Contains(RequiresCanvas);
                    if (requiresCanvas)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            dropdownIndex = EditorGUILayout.Popup(CanvasDropdownContent, dropdownIndex, dropdownValues);

                            if (GUILayout.Button(RefreshButtonContent, EditorStyles.miniButton, GUILayout.Width(ReloadButtonWidth)))
                            {
                                FindAllMRTKCanvases();
                            }
                        }

                        if (canvasUtilities.Length == 0)
                        {
                            GUIStyle CanvasWarningStyle = new GUIStyle(EditorStyles.textField)
                            {
                                wordWrap = true
                            };
                            GUILayout.TextField("These MRTK components require an MRTK Canvas Utility on one of the Unity UI Canvases in the scene.\nNone were detected. Press refresh if you recently added any.\nTo create a MRTK Canvas: GameObject > UI > Canvas, and under the Canvas component in the inspector, click 'Convert to MRTK Canvas' button.", CanvasWarningStyle);
                        }
                    }

                    EditorGUILayout.Space();

                    // Render grid of toolbox items
                    int itemsPerRow = Mathf.Max((int)(position.width / ToolboxItemWidth), 1);

                    for (int row = 0; row <= validItems.Count / itemsPerRow; row++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            int startIndex = row * itemsPerRow;
                            for (int col = 0; col < itemsPerRow && startIndex + col < validItems.Count; col++)
                            {
                                var item = validItems[startIndex + col];
                                RenderToolboxItem(item, requiresCanvas);
                            }
                        }
                    }
                }
            }
        }

        private void RenderToolboxItem(ToolboxItem item, bool requiresCanvas = false)
        {
            if (item == null || item.Prefab == null)
            {
                return;
            }

            var buttonContent = new GUIContent()
            {
                image = item.Icon,
                text = string.Empty,
                tooltip = item.DocURL,
            };

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // If this component requires an MRTK canvas but none are present, disable the button.
                    using (new EditorGUI.DisabledGroupScope(requiresCanvas && canvasUtilities.Length == 0))
                    {
                        if (GUILayout.Button(buttonContent, GUILayout.MaxHeight(ToolboxItemHeight), GUILayout.Width(ToolboxItemButtonWidth)))
                        {
                            Selection.activeObject = !requiresCanvas ? Instantiate(item.Prefab) : Instantiate(item.Prefab, canvasUtilities[dropdownIndex].transform);
                            Undo.RegisterCreatedObjectUndo(Selection.activeObject, $"Create {item.Name} Object");
                        }
                    }
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (!string.IsNullOrEmpty(item.DocURL))
                    {
                        InspectorUIUtility.RenderDocumentationButton(item.DocURL, ToolboxItemButtonWidth);
                    }
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(item.Name, centeredStyle, GUILayout.MaxWidth(ToolboxItemWidth));
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.Space();
            }
        }

        private static bool IsSearchMatch(ToolboxItem item, string searchContent)
        {
            return IsSearchMatch(item.Name, searchContent);
        }

        private static bool IsSearchMatch(string field, string searchContent)
        {
            return field.IndexOf(searchContent, 0, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void FindAllMRTKCanvases()
        {
            canvasUtilities = FindObjectsOfType<Input.Utilities.CanvasUtility>();
            dropdownValues = new string[canvasUtilities.Length];

            for (int i = 0; i < canvasUtilities.Length; i++)
            {
                dropdownValues[i] = canvasUtilities[i].name;
            }
        }
    }
}
