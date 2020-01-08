// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// This tool allows the migration of game object obsolete components into up-to-date versions.
    /// Requires Implementation of IMigrationhandler.
    /// </summary>
    public class MigrationWindow : EditorWindow
    {
        private enum ToolbarOption 
        { 
            Objects = 0, 
            Scenes = 1, 
            Project = 2 
        };

        private static readonly string[] toolbarTitles =
{
            "Objects",
            "Scenes",
            "Full Project"
        };

        private ToolbarOption selectedToolbar = ToolbarOption.Objects;
        private Vector2 scrollPosition = Vector2.zero;

        private const string DependencyWindowURL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/MigrationWindow.html";
        private const string WindowTitle = "Migration Window";
        private const string WindowDescription = "This tool allows the migration of obsolete components into up-to-date versions. Component-specific implementation of the MigrationHandler Interface is required.";

        private const string SceneExtension = ".unity";
        private const string PrefabExtension = ".prefab";

        private Dictionary<string, Type> migrationTypesMap = new Dictionary<string, Type>();
        private String[] migrationTypeNames;
        private int selectedMigrationHandlerIndex;
        private List<Object> migrationObjects = new List<Object>();
        private Object selection;

        private dynamic migrationTypeInstance
        {
            get
            {
                var selectedMigrationTypeName = migrationTypeNames[selectedMigrationHandlerIndex];
                var migrationHandlerType = migrationTypesMap[selectedMigrationTypeName];
                return Activator.CreateInstance(migrationHandlerType) as IMigrationHandler;
            }
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Migration Window", false, 4)]
        private static void ShowWindow()
        {
            var window = GetWindow<MigrationWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("d_TimelineEditModeRippleOFF").image);
            window.minSize = new Vector2(400.0f, 300.0f);
            window.Show();
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                GUI.enabled = false;
            }
            DrawHeader();
            DrawMigrationTypeSelector();
            DrawMigrationToolbars();
        }

        private void OnFocus()
        {
            RefreshAvailableTypes();
        }

        private void RefreshAvailableTypes()
        {
            var type = typeof(IMigrationHandler);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

            foreach (var migrationHandlerType in types)
            {
                if (!migrationTypesMap.ContainsKey(migrationHandlerType.Name))
                {
                    migrationTypesMap.Add(migrationHandlerType.Name, migrationHandlerType);
                }
            }
            migrationTypeNames = new String[migrationTypesMap.Count + 1];

            for (int i = 0; i < migrationTypesMap.Count; i++)
            {
                migrationTypeNames[i + 1] = migrationTypesMap.ElementAt(i).Key;
            }
        }

        private void DrawHeader()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Migration Window", EditorStyles.boldLabel);
                InspectorUIUtility.RenderDocumentationButton(DependencyWindowURL);
            }
            EditorGUILayout.LabelField(WindowDescription, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void DrawMigrationTypeSelector()
        {
            using (var horizontal = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Migration Handler Selection", EditorStyles.boldLabel);
                selectedMigrationHandlerIndex = EditorGUILayout.Popup(selectedMigrationHandlerIndex, migrationTypeNames, GUILayout.Width(400));
            }

            if (selectedMigrationHandlerIndex == 0)
            {
                GUI.enabled = false;
            }
        }

        private void DrawMigrationToolbars()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.Space();

                ToolbarOption previousSelectedToolbar = selectedToolbar;
                selectedToolbar = (ToolbarOption)GUILayout.Toolbar((int)selectedToolbar, toolbarTitles);

                if (previousSelectedToolbar != selectedToolbar)
                {
                    scrollPosition = Vector2.zero;
                    migrationObjects.Clear();
                }

                if (selectedToolbar == ToolbarOption.Project)
                {
                    DrawProjectToolbar();
                }
                else
                {
                    DrawObjectsToolbar();
                }
            }
        }

        private void DrawObjectsToolbar()
        {
            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                string tooltip = $"Drag and Drop {toolbarTitles[(int)selectedToolbar]} for Migration.";
                EditorGUILayout.LabelField(new GUIContent($"{toolbarTitles[(int)selectedToolbar]} Selection", InspectorUIUtility.InfoIcon, tooltip));

                var selectionType = selectedToolbar == ToolbarOption.Objects ? typeof(GameObject) : typeof(Object);
                var allowSceneObjects = selectedToolbar == ToolbarOption.Objects;

                selection = EditorGUILayout.ObjectField(null, selectionType, allowSceneObjects);
                if (selection)
                {
                    AddSelection();
                }
            }
            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawMigrationObjectsList();
            }
        }

        private void DrawProjectToolbar()
        {
            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (GUILayout.Button("Migrate"))
                    {                        
                        MigrateAllPrefabs();
                        MigrateAllScenes();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();

                        string tooltip = "All Scenes and Prefabs Selected";
                        EditorGUILayout.LabelField(new GUIContent(tooltip, InspectorUIUtility.WarningIcon));
                    }
                }
            }
        }

        private void DrawMigrationObjectsList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var migrationObject in migrationObjects)
            {
                using (var horizontal = new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledGroupScope(true))
                    {
                        EditorGUILayout.ObjectField(migrationObject, typeof(Object), false);
                    }
                    var icon = EditorGUIUtility.IconContent("winbtn_win_min_h");

                    if (GUILayout.Button(icon, GUILayout.Width(30)))
                    {
                        migrationObjects.Remove(migrationObject);
                        break;
                    }
                }
            }
            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (migrationObjects.Count == 0)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Migrate"))
                {
                    MigrateSelection();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (migrationObjects.Count > 0)
                    {
                        GUILayout.FlexibleSpace();

                        string tooltip = $"{migrationObjects.Count} {toolbarTitles[(int)selectedToolbar]} selected for migration";
                        EditorGUILayout.LabelField(new GUIContent(tooltip, InspectorUIUtility.WarningIcon));
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void AddSelection()
        {
            if (selectedToolbar == ToolbarOption.Objects)
            {
                if (!migrationObjects.Contains(selection))
                {
                    migrationObjects.Add(selection);
                }
            }
            else
            {
                if (!migrationObjects.Contains(selection) && IsExtension(AssetDatabase.GetAssetPath(selection), SceneExtension))
                {
                    migrationObjects.Add(selection);
                }
            }
        }

        private void MigrateAllScenes()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                 
            var previousScenePath = EditorSceneManager.GetActiveScene().path;

            String[] scenePaths = FindAllAssets(SceneExtension);

            for (int i = 0; i < scenePaths.Length; i++)
            {
                MigrateScene(scenePaths[i]);
            }
        }

        private void MigrateScene(String path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            var previousScenePath = EditorSceneManager.GetActiveScene().path;
            Scene currentScene = EditorSceneManager.OpenScene(path);

            foreach (var parent in currentScene.GetRootGameObjects())
            {
                MigrateGameObjectHierarchy(parent);
            }
            EditorSceneManager.SaveScene(currentScene);
            EditorSceneManager.OpenScene(Directory.GetCurrentDirectory() + "/" + previousScenePath);
        }

        private void MigrateAllPrefabs()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            foreach (var path in FindAllAssets(PrefabExtension))
            {
                using (var editScope = new EditPrefabAsset(path))
                {
                    MigrateGameObjectHierarchy(editScope.root);
                }
            }
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        private void MigrateSelection()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            foreach (var migrationObject in migrationObjects)
            {
                string path = AssetDatabase.GetAssetPath(migrationObject);

                if (IsAsset(path))
                {
                    if (IsExtension(path, PrefabExtension))
                    {
                        // Modify and save Prefab Asset
                        using (var editScope = new EditPrefabAsset(path))
                        {
                            GameObject parent = editScope.root;
                            MigrateGameObjectHierarchy(parent);
                        }
                    }
                    else if (IsExtension(path, SceneExtension))
                    {
                        MigrateScene(path);
                    }
                }
                else
                {
                    MigrateGameObjectHierarchy((GameObject)migrationObject);
                }
            }
            migrationObjects.Clear();
        }

        private void MigrateGameObjectHierarchy(GameObject gameObject)
        {
            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (migrationTypeInstance.CanMigrate(child.gameObject))
                {
                    migrationTypeInstance.Migrate(child.gameObject);
                }
            }
        }

        private string[] FindAllAssets(string extension)
        {

            string[] scenePaths = Directory.GetFiles(Application.dataPath, "*" + extension, SearchOption.AllDirectories);

            for (int i = 0; i < scenePaths.Length; ++i)
            {

                var scenePath = scenePaths[i];
                var path = scenePath.Substring(0, scenePath.Length - extension.Length);

                if (!IsAsset(path))
                {
                    continue;
                }
            }
            return scenePaths;
        }

        private bool IsAsset(string path)
        {
            return File.Exists(path);
        }

        private bool IsExtension(string path, string extension)
        {
            return Path.GetExtension(path).ToLowerInvariant().Equals(extension);
        }

        private class EditPrefabAsset : IDisposable
        {
            public readonly string path;
            public readonly GameObject root;

            public EditPrefabAsset(string path)
            {
                this.path = path;
                root = PrefabUtility.LoadPrefabContents(path);
            }

            public void Dispose()
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}

