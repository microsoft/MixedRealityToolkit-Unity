// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class MixedRealityToolboxWindow : EditorWindow //UnityEditor.Editor //EditorWindow,
    {
        private const string WindowTitle = "MRTK Toolbox";

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

            public GameObject Prefab;
            public Texture Icon;

            public void Init()
            {
                Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetPath);
                Icon = AssetDatabase.LoadAssetAtPath<Texture>(IconPath);
            }
        }

        private ToolboxItem[] AssetPaths =
        {
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
            foreach (var item in AssetPaths)
            {
                item.Init();
            }
        }

        private void OnGUI()
        {
            // TODO: MRTK logo
            // TODO: scroll view
            // TODO: Search bar*
            // TODO: documentation button*

            // GUILayout.SelectionGrid
            for (int i = 0; i < AssetPaths.Length; i++)
            {
                var item = AssetPaths[i];

                if (i % 2 == 0)
                    EditorGUILayout.BeginHorizontal();

                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        // Use it in a button, this is sumple formatting for example
                        if (GUILayout.Button(new GUIContent(item.Icon), GUILayout.Width(100), GUILayout.Height(100)))
                        {
                            Selection.activeObject = Instantiate(item.Prefab);
                        }
                        GUILayout.FlexibleSpace();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(item.Name);
                        GUILayout.FlexibleSpace();
                    }
                }
                
                if (i % 2 != 0)
                    EditorGUILayout.EndHorizontal();
            }

            if (AssetPaths.Length % 2 != 0)
                EditorGUILayout.EndHorizontal();
        }
    }
}