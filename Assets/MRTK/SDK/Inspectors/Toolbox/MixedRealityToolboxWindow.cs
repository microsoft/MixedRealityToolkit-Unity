// Copyright (c) Microsoft Corporation. All rights reserved.
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
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolboxWindow.SearchField";
        private const string WindowTitle = "MRTK Toolbox";

        private Vector2 scrollPos;
        private string searchString;
        private const float ToolboxItemWidth = 125f;
        private const float ToolboxItemButtonWidth = 100f;
        private const float ToolboxItemHeight = 64f;

        [Serializable]
        internal class ToolboxItemCollection
        {
            [SerializeField]
            private ToolboxCategory[] categories = null;
            public ToolboxCategory[] Categories => categories;
        }

        [Serializable]
        internal class ToolboxCategory
        {
            [SerializeField]
            private string categoryName = string.Empty;
            public string CategoryName => categoryName;

            [SerializeField]
            private ToolboxItem[] items = null;
            public ToolboxItem[] Items => items;
        }

        [Serializable]
        internal class ToolboxItem
        {
            [SerializeField]
            private string name = string.Empty;
            public string Name => name;

            [SerializeField]
            private string docURL = string.Empty;
            public string DocURL => docURL;

            [SerializeField]
            private string assetGUID = string.Empty;
            public string AssetPath => AssetDatabase.GUIDToAssetPath(assetGUID);

            [SerializeField]
            private string iconGUID = string.Empty;
            public string IconPath => AssetDatabase.GUIDToAssetPath(iconGUID);

            public GameObject Prefab { get; protected set; }

            public Texture Icon { get; protected set; }

            public void Init()
            {
                Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
                Icon = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);
            }
        }

        private const string RelativeJSONDataPath = @"Inspectors\Data\DefaultToolboxItems.json";
        private string JSONDataPath => MixedRealityToolkitFiles.MapRelativeFilePath(RelativeJSONDataPath);

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

        [MenuItem("Mixed Reality Toolkit/Toolbox", false, 3)]
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
            searchString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
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

                SessionState.SetString(searchDisplaySearchFieldKey, searchString);
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
                                RenderToolboxItem(item);
                            }
                        }
                    }
                }
            }
        }

        private void RenderToolboxItem(ToolboxItem item)
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
                    using (new EditorGUI.DisabledGroupScope(false))
                    {
                        if (GUILayout.Button(buttonContent, GUILayout.MaxHeight(ToolboxItemHeight), GUILayout.Width(ToolboxItemButtonWidth)))
                        {
                            Selection.activeObject = Instantiate(item.Prefab);
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
    }
}
