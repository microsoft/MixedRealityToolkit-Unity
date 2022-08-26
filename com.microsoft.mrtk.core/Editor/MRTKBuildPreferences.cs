// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Settings provider for build-specific settings, like the 3D app launcher model for Windows builds.
    /// </summary>
    [Serializable]
    internal class MRTKBuildPreferences : ScriptableObject, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string AppLauncherPath = @"Assets\AppLauncherModel.glb";
        private static readonly GUIContent AppLauncherModelLabel = new GUIContent("3D App Launcher Model", "Location of .glb model to use as a 3D App Launcher");
        private static UnityEditor.Editor gameObjectEditor = null;
        private static GUIStyle appLauncherPreviewBackgroundColor = null;
        private static bool isBuilding = false;

        // Arbitrary callback order, chosen to be larger so that it runs after other things that
        // a developer may have already.
        int IOrderedCallback.callbackOrder => 100;

        /// <remarks>See <see href="https://docs.microsoft.com/windows/mixed-reality/distribute/3d-app-launcher-design-guidance">3D app launcher design guidance</see> for more information.</remarks>
        [SerializeField]
        private string appLauncherModelLocation;

        private static MRTKSettings Settings => settings != null ? settings : settings = MRTKSettings.GetOrCreateSettings();
        private static MRTKSettings settings = null;

        private static UnityEditor.Editor cachedSettingsEditor;

        [SettingsProvider]
        private static SettingsProvider BuildPreferences()
        {
            var provider = new SettingsProvider("Project/MRTK3/Build Settings", SettingsScope.Project)
            {
                guiHandler = GUIHandler,

                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit", "Build" })
            };

            if (Settings.BuildPreferences == null)
            {
                Settings.BuildPreferences = CreateInstance<MRTKBuildPreferences>();
                AssetDatabase.AddObjectToAsset(Settings.BuildPreferences, Settings);
            }

            void GUIHandler(string searchContext)
            {
                EditorGUILayout.HelpBox("These settings are serialized into MRTKSettings.asset in the MRTK-Generated folder.\nThis file can be checked into source control to maintain consistent settings across collaborators.", MessageType.Info);
                UnityEditor.Editor.CreateCachedEditor(Settings.BuildPreferences, null, ref cachedSettingsEditor);

                if (cachedSettingsEditor != null)
                {
                    cachedSettingsEditor.OnInspectorGUI();
                }
            }

            return provider;
        }

        [CustomEditor(typeof(MRTKBuildPreferences))]
        private class MRTKBuildPreferencesInspector : UnityEditor.Editor
        {
            private SerializedProperty appLauncherModelLocation;

            private void OnEnable()
            {
                appLauncherModelLocation = serializedObject.FindProperty(nameof(appLauncherModelLocation));
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                using (new EditorGUILayout.HorizontalScope())
                {
                    GameObject newGlbModel;
                    bool appLauncherChanged = false;

                    // 3D launcher model
                    var curGlbModel = AssetDatabase.LoadAssetAtPath(appLauncherModelLocation.stringValue, typeof(GameObject));

                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.HelpBox("This field will only accept .glb and .gltf files. Any other file type will be silently rejected.", MessageType.Info);

                        EditorGUILayout.LabelField(AppLauncherModelLabel);
                        using (var check = new EditorGUI.ChangeCheckScope())
                        {
                            newGlbModel = EditorGUILayout.ObjectField(curGlbModel, typeof(GameObject), false, GUILayout.MaxWidth(256)) as GameObject;
                            string newAppLauncherModelLocation = AssetDatabase.GetAssetPath(newGlbModel);
                            if (check.changed
                                && (newAppLauncherModelLocation.EndsWith(".glb")
                                    || newAppLauncherModelLocation.EndsWith(".gltf")
                                    || string.IsNullOrWhiteSpace(newAppLauncherModelLocation)))
                            {
                                appLauncherModelLocation.stringValue = newAppLauncherModelLocation;
                                appLauncherChanged = true;
                            }
                        }
                    }

                    // The preview GUI has a problem during the build, so we don't render it
                    if (newGlbModel != null && !isBuilding)
                    {
                        if (gameObjectEditor == null || appLauncherChanged)
                        {
                            gameObjectEditor = CreateEditor(newGlbModel);
                        }

                        if (appLauncherPreviewBackgroundColor == null)
                        {
                            appLauncherPreviewBackgroundColor = new GUIStyle();
                            appLauncherPreviewBackgroundColor.normal.background = EditorGUIUtility.whiteTexture;
                        }

                        gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(128, 128), appLauncherPreviewBackgroundColor);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup == BuildTargetGroup.WSA && !string.IsNullOrEmpty(appLauncherModelLocation))
            {
                isBuilding = true;
                // Sets the editor to null. On a build, Unity reloads the object preview
                // in a seemingly unexpected way, so it starts rendering a null texture.
                // This refreshes the preview window instead.
                gameObjectEditor = null;
            }
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup == BuildTargetGroup.WSA && !string.IsNullOrEmpty(appLauncherModelLocation))
            {

                string appxPath = $"{report.summary.outputPath}/{PlayerSettings.productName}";

                Debug.Log($"3D App Launcher: {appLauncherModelLocation}, Destination: {appxPath}/{AppLauncherPath}");

                FileUtil.ReplaceFile(appLauncherModelLocation, $"{appxPath}/{AppLauncherPath}");
                AddAppLauncherModelToProject($"{appxPath}/{PlayerSettings.productName}.vcxproj");
                AddAppLauncherModelToFilter($"{appxPath}/{PlayerSettings.productName}.vcxproj.filters");
                UpdateManifest($"{appxPath}/Package.appxmanifest");

                isBuilding = false;
            }
        }

        private static void AddAppLauncherModelToProject(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if model has already been added
            XmlNodeList nodes = root.SelectNodes($"//None[@Include = \"{AppLauncherPath}\"]");
            if (nodes.Count > 0)
            {
                return;
            }

            var newNodeDoc = new XmlDocument();
            newNodeDoc.LoadXml($"<None Include=\"{AppLauncherPath}\">" +
                            "<DeploymentContent>true</DeploymentContent>" +
                            "</None>");
            var newNode = doc.ImportNode(newNodeDoc.DocumentElement, true);
            var list = doc.GetElementsByTagName("ItemGroup");
            var items = list.Item(1);
            items.AppendChild(newNode);
            doc.Save(filePath);
        }

        private static void AddAppLauncherModelToFilter(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if model has already been added
            XmlNodeList nodes = root.SelectNodes($"//None[@Include = \"{AppLauncherPath}\"]");
            if (nodes.Count > 0)
            {
                return;
            }

            var newNodeDoc = new XmlDocument();
            newNodeDoc.LoadXml($"<None Include=\"{AppLauncherPath}\">" +
                            "<Filter>Assets</Filter>" +
                            "</None>");
            var newNode = doc.ImportNode(newNodeDoc.DocumentElement, true);
            var list = doc.GetElementsByTagName("ItemGroup");
            var items = list.Item(0);
            items.AppendChild(newNode);
            doc.Save(filePath);
        }

        private static void UpdateManifest(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if the element exists already
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("uap5", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
            XmlNodeList nodes = root.SelectNodes("//uap5:MixedRealityModel", nsmgr);
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes != null && node.Attributes["Path"].Value == AppLauncherPath)
                {
                    return;
                }
            }
            root.SetAttribute("xmlns:uap5", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");

            var ignoredValue = root.GetAttribute("IgnorableNamespaces");
            root.SetAttribute("IgnorableNamespaces", ignoredValue + " uap5");

            var newElement = doc.CreateElement("uap5", "MixedRealityModel", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
            newElement.SetAttribute("Path", AppLauncherPath);
            var list = doc.GetElementsByTagName("uap:DefaultTile");
            var items = list.Item(0);
            items.AppendChild(newElement);

            doc.Save(filePath);
        }
    }
}
