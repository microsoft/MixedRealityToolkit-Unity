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
    /// <remarks>
    /// See <see href="https://docs.microsoft.com/windows/mixed-reality/distribute/3d-app-launcher-design-guidance">3D app launcher design guidance</see> for more information.
    /// </remarks>
    [Serializable]
    internal class MRTKBuildPreferences : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string AppLauncherPath = @"Assets\AppLauncherModel.glb";
        private const string AppLauncherDocsUrl = @"https://docs.microsoft.com/windows/mixed-reality/distribute/creating-3d-models-for-use-in-the-windows-mixed-reality-home";

        private static GUIContent appLauncherModelLabel = null;
        private static GUIContent buttonContent = null;
        private static UnityEditor.Editor gameObjectEditor = null;
        private static GUIStyle appLauncherPreviewBackgroundColor = null;
        private static bool isBuilding = false;

        // Arbitrary callback order, chosen to be larger so that it runs after other things that
        // a developer may have already.
        int IOrderedCallback.callbackOrder => 100;

        [SerializeField]
        private GameObject appLauncherModel;

        private static MRTKSettings Settings => settings != null ? settings : settings = MRTKSettings.GetOrCreateSettings();
        private static MRTKSettings settings = null;

        [SettingsProvider]
        private static SettingsProvider BuildPreferences()
        {
            var provider = new SettingsProvider("Project/MRTK3/Build Settings", SettingsScope.Project)
            {
                guiHandler = GUIHandler,
                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit", "Build" })
            };

            static void GUIHandler(string searchContext)
            {
                EditorGUILayout.HelpBox("These settings are serialized into MRTKSettings.asset in the MRTK.Generated folder.\nThis file can be checked into source control to maintain consistent settings across collaborators.", MessageType.Info);
                DrawAppLauncherModelField();
            }

            return provider;
        }

        public static void DrawAppLauncherModelField(bool showInteractivePreview = true)
        {
            appLauncherModelLabel ??= new GUIContent("3D App Launcher Model (UWP)", "Location of .glb model to use as a 3D App Launcher");
            buttonContent ??= new GUIContent(string.Empty, EditorGUIUtility.IconContent("_Help").image, "Click for documentation");

            using (new EditorGUILayout.HorizontalScope())
            {
                GameObject newGlbModel;
                bool appLauncherChanged = false;

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.HelpBox("This field will only accept .glb files. Any other file type will be silently rejected.", MessageType.Info);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(appLauncherModelLabel);

                        if (GUILayout.Button(buttonContent, EditorStyles.label))
                        {
                            Help.BrowseURL(AppLauncherDocsUrl);
                        }
                    }

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        newGlbModel = EditorGUILayout.ObjectField(Settings.BuildPreferences.appLauncherModel, typeof(GameObject), false, GUILayout.MaxWidth(256)) as GameObject;
                        string newAppLauncherModelLocation = AssetDatabase.GetAssetPath(newGlbModel);
                        if (check.changed
                            && (newAppLauncherModelLocation.EndsWith(".glb")
                                || string.IsNullOrWhiteSpace(newAppLauncherModelLocation)))
                        {
                            Undo.RecordObject(Settings, "Change app launcher model");
                            Settings.BuildPreferences.appLauncherModel = newGlbModel;
                            appLauncherChanged = true;
                            EditorUtility.SetDirty(Settings);
                        }
                    }
                }

                // The preview GUI has a problem during the build, so we don't render it
                if (showInteractivePreview && newGlbModel != null && !isBuilding)
                {
                    if (gameObjectEditor == null || appLauncherChanged)
                    {
                        gameObjectEditor = UnityEditor.Editor.CreateEditor(newGlbModel);
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

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platformGroup == BuildTargetGroup.WSA && Settings.BuildPreferences.appLauncherModel != null)
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
            if (report.summary.platformGroup == BuildTargetGroup.WSA && Settings.BuildPreferences.appLauncherModel != null)
            {
                string appxPath = $"{report.summary.outputPath}/{PlayerSettings.productName}";
                string assetPath = AssetDatabase.GetAssetPath(Settings.BuildPreferences.appLauncherModel);

                Debug.Log($"3D App Launcher: {assetPath}, Destination: {appxPath}/{AppLauncherPath}");

                FileUtil.ReplaceFile(assetPath, $"{appxPath}/{AppLauncherPath}");
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
