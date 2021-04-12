// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// <see href="https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/tools/migration-window"/>This is an utility window for the MigrationTool. 
    /// </summary>
    public class MigrationWindow : EditorWindow
    {
        private enum ToolbarOption
        {
            GameObjects = 0,
            Scenes = 1,
            Project = 2
        };

        private static readonly string[] toolbarTitles =
        {
            "Game Objects",
            "Scenes",
            "Full Project"
        };

        private ToolbarOption selectedToolbar = ToolbarOption.GameObjects;
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 logScrollPosition = Vector2.zero;

        private const string MigrationWindowURL = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/tools/migration-window";
        private const string WindowTitle = "Migration Window";
        private const string WindowDescription = "This tool allows the migration of obsolete components into up-to-date versions.";

        private int selectedMigrationHandlerIndex;
        private string[] migrationHandlerTypeNames;
        private bool isMigrationEnabled;
        private Type selectedMigrationHandlerType;
        private string migrationLog;

        // Assets/MRTK/Tools/MigrationWindow/Icons/IconMigrationTabDark.png
        private const string darkTabIconGUID = "b2681195a786ce54e97b2126f1164e1f";
        // Assets/MRTK/Tools/MigrationWindow/Icons/IconMigrationTabLight.png
        private const string lightTabIconGUID = "2064c6a354f93c74397e7795dfbfc111";
        // Assets/MRTK/Tools/MigrationWindow/Icons/IconMigrationPass.png
        private const string passIconGUID = "f3b1d57b0d86d29419b5737244fed8a9";
        // Assets/MRTK/Tools/MigrationWindow/Icons/IconMigrationFail.png
        private const string failIconGUID = "5f1c0a610cc7c1841a1e9b43045c3f05";

        private static Texture passIcon;
        private static Texture failIcon;
        private static Texture lightTabIcon;
        private static Texture darkTabIcon;

        private static bool isEditorProSkin;

        private readonly MigrationTool migrationTool = new MigrationTool();

        [MenuItem("Mixed Reality/Toolkit/Utilities/Migration Window", false, 4)]
        private static void ShowWindow()
        {
            var window = GetWindow<MigrationWindow>(typeof(SceneView));

            isEditorProSkin = EditorGUIUtility.isProSkin;
            var windowIcon = isEditorProSkin ? lightTabIcon : darkTabIcon;

            window.titleContent = new GUIContent(WindowTitle, windowIcon);
            window.minSize = new Vector2(400.0f, 300.0f);
            window.Show();
        }

        private void OnEnable()
        {
            isMigrationEnabled = false;
            migrationTool.ClearMigrationList();

            // Adds empty as first option for MigrationHandler selector 
            var migrationTypeNamesList = new List<string> { "" };
            migrationTypeNamesList.AddRange(migrationTool.MigrationHandlerTypes
                                  .Select(x => x.FullName)
                                  .ToList());
            migrationHandlerTypeNames = migrationTypeNamesList.ToArray();

            selectedMigrationHandlerIndex = 0;

            failIcon = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(failIconGUID), typeof(Texture));
            passIcon = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(passIconGUID), typeof(Texture));
            lightTabIcon = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(lightTabIconGUID), typeof(Texture));
            darkTabIcon = (Texture)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(darkTabIconGUID), typeof(Texture));
        }

        private void OnGUI()
        {
            CheckEditorSkinChange();

            using (new EditorGUI.DisabledGroupScope(EditorApplication.isPlaying || EditorApplication.isPaused))
            {
                DrawHeader();
                DrawMigrationTypeSelector();

                using (new EditorGUI.DisabledGroupScope(!isMigrationEnabled))
                {
                    DrawMigrationToolbars();

                    if (isMigrationEnabled)
                    {
                        DrawObjectSelection();
                        DrawObjectsForMigration();
                    }
                }
            }
        }

        private void DrawHeader()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Migration Window", EditorStyles.boldLabel);
                InspectorUIUtility.RenderDocumentationButton(MigrationWindowURL);
            }
            EditorGUILayout.LabelField(WindowDescription, EditorStyles.wordWrappedLabel);

            // show a warning as long as we're not handling dependencies
            EditorGUILayout.HelpBox("Note that the migration window currently doesn't do dependency checks, " +
                "so overrides on prefab instances might be lost after choosing to upgrade. Create a local copy " +
                "or make sure to use version control before using the tool on game objects or scenes that are " +
                "using prefab overrides.", MessageType.Warning);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void DrawMigrationTypeSelector()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Migration Handler Selection", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                selectedMigrationHandlerIndex = EditorGUILayout.Popup(selectedMigrationHandlerIndex, migrationHandlerTypeNames, GUILayout.Width(500));
                if (EditorGUI.EndChangeCheck())
                {
                    migrationTool.ClearMigrationList();
                    SetMigrationHandlerType();
                }
            }
        }

        private void DrawMigrationToolbars()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.Space();

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    selectedToolbar = (ToolbarOption)GUILayout.Toolbar((int)selectedToolbar, toolbarTitles);
                    if (check.changed)
                    {
                        scrollPosition = Vector2.zero;
                        migrationTool.ClearMigrationList();
                        migrationLog = "";
                    }
                }
            }
        }

        private void DrawObjectSelection()
        {
            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                if (selectedToolbar == ToolbarOption.Project)
                {
                    if (GUILayout.Button("Add full project for migration"))
                    {
                        migrationTool.TryAddProjectForMigration(selectedMigrationHandlerType);
                    }
                    return;
                }
                else
                {
                    string tooltip = $"Drag and drop {toolbarTitles[(int)selectedToolbar]} for migration.";
                    EditorGUILayout.LabelField(new GUIContent($"{toolbarTitles[(int)selectedToolbar]} Selection", InspectorUIUtility.InfoIcon, tooltip));

                    var allowSceneObjects = selectedToolbar == ToolbarOption.GameObjects;
                    var selectionType = allowSceneObjects ? typeof(GameObject) : typeof(SceneAsset);

                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var selection = EditorGUILayout.ObjectField(null, selectionType, allowSceneObjects);

                        if (check.changed && selection)
                        {
                            migrationTool.TryAddObjectForMigration(selectedMigrationHandlerType, selection);
                        }
                    }
                }
            }
            EditorGUILayout.Space();
        }

        private void DrawObjectsForMigration()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scrollView.scrollPosition;
                    var migrationObjects = migrationTool.MigrationObjects;
                    foreach (var migrationObject in migrationObjects)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.ObjectField(migrationObject.Key, typeof(Object), false);
                            }

                            var removeIcon = EditorGUIUtility.IconContent("winbtn_win_min_h");
                            var removeTooltip = "Remove object.";
                            if (!migrationObject.Value.IsProcessed)
                            {
                                if (GUILayout.Button(new GUIContent(removeIcon.image, removeTooltip), GUILayout.Width(20), GUILayout.Height(15)))
                                {
                                    migrationTool.RemoveObjectForMigration(migrationObject.Key);
                                    break;
                                }
                            }
                            else
                            {
                                Texture statusIcon;
                                string tooltip = "";

                                if (migrationObject.Value.Failures > 0)
                                {
                                    statusIcon = failIcon;
                                    tooltip = "Object migration had some issues.\nClick for more details.";
                                }
                                else
                                {
                                    statusIcon = passIcon;
                                    tooltip = "Object migration was successful.\nClick for more details.";
                                }

                                if (GUILayout.Button(new GUIContent(statusIcon, tooltip), GUILayout.Width(20), GUILayout.Height(15)))
                                {
                                    migrationLog = migrationObject.Value.Log;
                                    break;
                                }
                            }
                        }
                    }
                    EditorGUILayout.Space();

                    if (migrationTool.MigrationState == MigrationTool.MigrationToolState.PreMigration)
                    {
                        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            using (new EditorGUI.DisabledGroupScope(migrationObjects.Count == 0))
                            {

                                if (GUILayout.Button("Migrate"))
                                {
                                    migrationTool.MigrateSelection(selectedMigrationHandlerType, true);
                                    migrationLog = "";
                                }

                                if (migrationObjects.Count > 0)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.FlexibleSpace();

                                        string tooltip = $"{migrationObjects.Count} Objects selected for migration";
                                        EditorGUILayout.LabelField(new GUIContent(tooltip, InspectorUIUtility.WarningIcon));
                                    }
                                }
                            }
                        }
                    }

                    else if (migrationTool.MigrationState == MigrationTool.MigrationToolState.PostMigration && !String.IsNullOrEmpty(migrationLog))
                    {
                        using (var logScrollView = new EditorGUILayout.ScrollViewScope(logScrollPosition))
                        {
                            logScrollPosition = logScrollView.scrollPosition;
                            GUILayout.TextArea(migrationLog);
                        }
                    }
                }
            }
        }

        private void SetMigrationHandlerType()
        {
            selectedMigrationHandlerType = AppDomain.CurrentDomain.GetAssemblies()
                                           .SelectMany(x => x.GetLoadableTypes())
                                           .Where(x => x.FullName == migrationHandlerTypeNames[selectedMigrationHandlerIndex]).First();
            if (selectedMigrationHandlerType == null)
            {
                Debug.LogError("Unable to load type for migration");
                isMigrationEnabled = false;
            }
            isMigrationEnabled = true;
        }

        private void CheckEditorSkinChange()
        {
            if (isEditorProSkin != EditorGUIUtility.isProSkin)
            {
                isEditorProSkin = EditorGUIUtility.isProSkin;

                var window = GetWindow<MigrationWindow>(typeof(SceneView));
                window.titleContent.image = isEditorProSkin ? lightTabIcon : darkTabIcon;
            }
        }
    }
}