// Copyright (c) Microsoft Corporation. All rights reserved.
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
    /// <see href="https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/MigrationWindow.html"/>This is an utility window for the MigrationTool. 
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

        private const string MigrationWindowURL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/MigrationWindow.html";
        private const string WindowTitle = "Migration Window";
        private const string WindowDescription = "This tool allows the migration of obsolete components into up-to-date versions.";

        private int selectedMigrationHandlerIndex;
        private string[] migrationHandlerTypeNames;
        private bool isMigrationEnabled;
        private Type selectedMigrationHandlerType;
        private string migrationLog;

        private readonly MigrationTool migrationTool = new MigrationTool();

        [MenuItem("Mixed Reality Toolkit/Utilities/Migration Window", false, 4)]
        private static void ShowWindow()
        {
            var window = GetWindow<MigrationWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(WindowTitle, EditorGUIUtility.IconContent("d_TimelineEditModeRippleOFF").image);
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
        }

        private void OnGUI()
        {
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
                            if (!migrationObject.Value.IsProcessed)
                            {
                                if (GUILayout.Button(removeIcon, GUILayout.Width(30)))
                                {
                                    migrationTool.RemoveObjectForMigration(migrationObject.Key);
                                    break;
                                }
                            }
                            else
                            {
                                GUIContent statusIcon;
                                string tooltip = "";

                                if (migrationObject.Value.Failures > 0)
                                {
                                    statusIcon = EditorGUIUtility.IconContent("vcs_delete");
                                    tooltip = "Object migration had some issues.\nClick for more details.";
                                }
                                else
                                {
                                    statusIcon = EditorGUIUtility.IconContent("vcs_check");
                                    tooltip = "Object migration was successful.\nClick for more details.";
                                }

                                if (GUILayout.Button(new GUIContent(statusIcon.image, tooltip), GUILayout.Width(30), GUILayout.Height(20)))
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
    }
}