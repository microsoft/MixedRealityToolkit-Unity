// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public static class MixedRealityBuildPreferences
    {
        private static readonly GUIContent AppLauncherModelLabel = new GUIContent("3D App Launcher Model", "Location of .glb model to use as a 3D App Launcher");
        private static UnityEditor.Editor gameObjectEditor = null;
        private static GUIStyle appLauncherPreviewBackgroundColor = null;

        [SettingsProvider]
        private static SettingsProvider BuildPreferences()
        {
            var provider = new SettingsProvider("Project/Mixed Reality Toolkit/Build Settings", SettingsScope.Project)
            {
                guiHandler = GUIHandler,

                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit", "Build" })
            };

            void GUIHandler(string searchContext)
            {
                EditorGUILayout.HelpBox("These settings are serialized into ProjectPreferences.asset in the MixedRealityToolkit-Generated folder.\nThis file can be checked into source control to maintain consistent settings across collaborators.", MessageType.Info);
                DrawAppLauncherModelField();
            }

            return provider;
        }

        public static void DrawAppLauncherModelField()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GltfAsset newGlbModel;
                bool appLauncherChanged = false;

                // 3D launcher model
                string curAppLauncherModelLocation = BuildDeployPreferences.AppLauncherModelLocation;
                var curGlbModel = AssetDatabase.LoadAssetAtPath(curAppLauncherModelLocation, typeof(GltfAsset));

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField(AppLauncherModelLabel);
                    newGlbModel = EditorGUILayout.ObjectField(curGlbModel, typeof(GltfAsset), false, GUILayout.MaxWidth(256)) as GltfAsset;
                    string newAppLauncherModelLocation = AssetDatabase.GetAssetPath(newGlbModel);
                    if (newAppLauncherModelLocation != curAppLauncherModelLocation)
                    {
                        BuildDeployPreferences.AppLauncherModelLocation = newAppLauncherModelLocation;
                        appLauncherChanged = true;
                    }
                }

                if (newGlbModel != null)
                {
                    if (gameObjectEditor == null || appLauncherChanged)
                    {
                        gameObjectEditor = UnityEditor.Editor.CreateEditor(newGlbModel.Model);
                    }

                    if (appLauncherPreviewBackgroundColor == null)
                    {
                        appLauncherPreviewBackgroundColor = new GUIStyle();
                        appLauncherPreviewBackgroundColor.normal.background = EditorGUIUtility.whiteTexture;
                    }

                    gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(128, 128), appLauncherPreviewBackgroundColor);
                }
            }
        }
    }
}