// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

        private class ToolboxItem
        {
            public string Name;
            public string RelativeAssetPath;
            public MixedRealityToolkitModuleType AssetModule = MixedRealityToolkitModuleType.Core;

            public string AssetPath =>
                MixedRealityToolkitFiles.MapRelativeFilePath(AssetModule, RelativeAssetPath);

            public MixedRealityToolkitModuleType IconModule = MixedRealityToolkitModuleType.Core;
            public string RelativeIconPath;
            public string IconPath 
                => MixedRealityToolkitFiles.MapRelativeFilePath(IconModule, RelativeIconPath);

            public string DocURL;

            public GameObject Prefab { get; protected set; }
            public Texture Icon { get; protected set; }

            public void Init()
            {
                Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
                Icon = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);
            }
        }

        //IndeterminateLoader 
        //ProgressIndicatorRotatingOrbs 
        //ProgressIndicatorLoadingBar 
        //ProgressIndicatorRotatingObject
        
        private ToolboxItem[] ToolboxPrefabs =
        {
            new ToolboxItem()
            {
                Name = "PressableButton HoloLens2",
                AssetModule = MixedRealityToolkitModuleType.SDK,
                RelativeAssetPath = @"Features\UX\Interactable\Prefabs\PressableButtonHoloLens2.prefab",
                IconModule = MixedRealityToolkitModuleType.SDK,
                RelativeIconPath = @"StandardAssets\Textures\IconRefresh.png",
                DocURL = "www.google.com",
            },

            new ToolboxItem()
            {
                Name = "Progress Indicator LoadingBar",
                AssetModule = MixedRealityToolkitModuleType.SDK,
                RelativeAssetPath = @"Features\UX\Interactable\Prefabs\PressableButtonHoloLens2.prefab",
                IconModule = MixedRealityToolkitModuleType.Core,
                RelativeIconPath = @"progress-bar.gif",
            },

            new ToolboxItem()
            {
                Name = "PressableButton HoloLens2",
                AssetModule = MixedRealityToolkitModuleType.SDK,
                RelativeAssetPath = @"Features\UX\Interactable\Prefabs\PressableButtonHoloLens2.prefab",
                IconModule = MixedRealityToolkitModuleType.SDK,
                RelativeIconPath = @"StandardAssets\Textures\IconRefresh.png",
            },

            new ToolboxItem()
            {
                Name = "PressableButton HoloLens2",
                AssetModule = MixedRealityToolkitModuleType.SDK,
                RelativeAssetPath = @"Features\UX\Interactable\Prefabs\PressableButtonHoloLens2.prefab",
                IconModule = MixedRealityToolkitModuleType.SDK,
                RelativeIconPath = @"StandardAssets\Textures\IconRefresh.png",
            },
        };

        [MenuItem("Mixed Reality Toolkit/Toolbox Window", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<MixedRealityToolboxWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("d_EditCollider").image);
            window.Show();
        }

        private void OnEnable()
        {
            foreach (var item in ToolboxPrefabs)
            {
                item.Init();
            }
        }

        private void OnGUI()
        {
            labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;
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
                    // TODO: Download SDK path? or other module*
                    // TODO: Search bar*

                    int itemsPerRow = (int)(position.width / 128.0f);

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
            }
        }
    }
}