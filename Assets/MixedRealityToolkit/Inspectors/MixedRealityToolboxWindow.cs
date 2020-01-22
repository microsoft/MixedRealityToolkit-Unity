// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class MixedRealityToolboxWindow : EditorWindow
    {
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolboxWindow.SearchField";
        private const string WindowTitle = "MRTK Toolbox";

        private Vector2 scrollPos;
        private string searchString;
        private const float ToolboxItemWidth = 250f;
        private const float ToolboxItemHeight = 64f;

        [Serializable]
        private class ToolboxItemCollection
        {
            [SerializeField]
            public ToolboxCategory[] Categories = null;
        }

        [Serializable]
        private class ToolboxCategory
        {
            [SerializeField]
            public string CategoryName = string.Empty;

            [SerializeField]
            public ToolboxItem[] Items = null;
        }

        [Serializable]
        private class ToolboxItem
        {
            public string Name = string.Empty;

            public string RelativeAssetPath = string.Empty;

            public MixedRealityToolkitModuleType AssetModule = MixedRealityToolkitModuleType.Core;

            public string AssetPath =>
                MixedRealityToolkitFiles.MapRelativeFilePath(AssetModule, RelativeAssetPath);

            public MixedRealityToolkitModuleType IconModule = MixedRealityToolkitModuleType.Core;

            public string RelativeIconPath = string.Empty;

            public string IconPath 
                => MixedRealityToolkitFiles.MapRelativeFilePath(IconModule, RelativeIconPath);

            public string DocURL = string.Empty;

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

        private ToolboxItemCollection toolBoxCollection;
        private ToolboxCategory[] ToolboxPrefabs
        {
            get
            {
                if (toolBoxCollection == null)
                {
                    try
                    {
                        toolBoxCollection = JsonUtility.FromJson<ToolboxItemCollection>(File.ReadAllText(JSONDataPath));
                    }catch (Exception ex)
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

        [MenuItem("Mixed Reality Toolkit/Toolbox Window", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<MixedRealityToolboxWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("d_EditCollider").image);
            window.Show();
        }

        private void OnEnable()
        {
            searchString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
        }

        private void OnGUI()
        {
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

                    // bucket categories?

                    foreach (var bucket in ToolboxPrefabs)
                    {
                        RenderSection(bucket);
                    }
                }
            }
        }

        private void RenderSection(ToolboxCategory bucket)
        {
            int itemsPerRow = (int)(position.width / ToolboxItemWidth);

            InspectorUIUtility.DrawTitle(bucket.CategoryName);

            // Render grid of toolbox items
            for (int row = 0; row <= bucket.Items.Length / itemsPerRow; row++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    int startIndex = row * itemsPerRow;
                    for (int col = 0; col < itemsPerRow && startIndex + col < bucket.Items.Length; col++)
                    {
                        var item = bucket.Items[startIndex + col];
                        if (IsSearchMatch(item, searchString))
                        {
                            RenderToolboxItem(item);
                        }
                    }
                }
            }
        }

        private void RenderToolboxItem(ToolboxItem item)
        {
            if (item == null || item.Prefab == null)
            {
                // TODO: Download SDK path? or other module*
                return;
            }

            var buttonContent = new GUIContent()
            {
                image = item.Icon,
                text = item.Name,
                //tooltip = docURL,
            };

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // The documentation button should always be enabled.
                    using (new EditorGUI.DisabledGroupScope(false))
                    {
                        if (GUILayout.Button(buttonContent, GUILayout.MaxHeight(ToolboxItemHeight), GUILayout.Width(ToolboxItemWidth)))
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
                        InspectorUIUtility.RenderDocumentationButton(item.DocURL);
                    }
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.Space();
            }
        }

        private static bool IsSearchMatch(ToolboxItem item, string searchContent)
        {
            return item.Name.IndexOf(searchContent, 0, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}