// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class MixedRealityToolboxWindow : EditorWindow //UnityEditor.Editor //EditorWindow,
    {
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolboxWindow.SearchField";
        private const string WindowTitle = "MRTK Toolbox";

        private GUIStyle labelStyle;
        private Vector2 scrollPos;

        [Serializable]
        private class ToolboxItemCollection
        {
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
        private ToolboxItem[] ToolboxPrefabs
        {
            get
            {
                if (toolBoxCollection == null)
                {
                    Debug.Log("TEST");
                    toolBoxCollection = JsonUtility.FromJson<ToolboxItemCollection>(File.ReadAllText(JSONDataPath));

                    foreach (var item in toolBoxCollection.Items)
                    {
                        item.Init();
                    }
                }

                return toolBoxCollection.Items;
            }

        }

        [MenuItem("Mixed Reality Toolkit/Toolbox Window", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<MixedRealityToolboxWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("d_EditCollider").image);
            window.Show();
        }

        private void OnGUI()
        {
            labelStyle = new GUIStyle(EditorStyles.label);
            //labelStyle.wordWrap = true;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Search: ", GUILayout.MaxWidth(70));
                string searchString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
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
                    // TODO: Search bar*

                    /*
                    foreach (var item in ToolboxPrefabs)
                    {
                        RenderToolboxItem(item);
                    }
                    */
                    
                    int itemsPerRow = (int)(position.width / 250f);

                    // Render grid of toolbox items
                    for (int row = 0; row < ToolboxPrefabs.Length / itemsPerRow; row++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            int startIndex = row * itemsPerRow;
                            for (int col = 0; col < itemsPerRow; col++)
                            {
                                RenderToolboxItem(ToolboxPrefabs[startIndex + col]);
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
                        if (GUILayout.Button(buttonContent, GUILayout.MaxHeight(64f), GUILayout.Width(250f)))
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
            }

            /*
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // Use it in a button, this is sumple formatting for example
                    if (GUILayout.Button(new GUIContent(item.Icon), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        Selection.activeObject = Instantiate(item.Prefab);
                    }
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(item.Name, labelStyle);
                    GUILayout.FlexibleSpace();
                }

                if (!string.IsNullOrEmpty(item.DocURL))
                {
                    InspectorUIUtility.RenderDocumentationButton(item.DocURL);
                }
            }*/
        }
    }
}