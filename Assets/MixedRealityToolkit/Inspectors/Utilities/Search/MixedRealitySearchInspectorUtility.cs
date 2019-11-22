// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Search
{
    /// <summary>
    /// Utility class for drawing search interface.
    /// Draws a search field by default. When search is active, draws search results.
    /// Also handles the business of storing search configuration and results so searching objects don't have to.
    /// </summary>
    public static class MixedRealitySearchInspectorUtility
    {
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolkitInspector.SearchField";
        private static readonly string searchDisplayRequireAllKeywordsKey = "MixedRealityToolkitInspector.RequireAllKeywords";
        private static readonly string searchDisplayOptionsFoldoutKey = "MixedRealityToolkitInspector.SearchOptionsFoldout";
        private static readonly string searchDisplaySearchTooltipsKey = "MixedRealityToolkitInspector.SearchTooltips";
        private static readonly string searchDisplaySearchFieldContentKey = "MixedRealityToolkitInspector.SearchFieldContent";

        private static SearchConfig config;
        private static SearchConfig prevConfig;
        private static UnityEngine.Object activeTarget;
        private static List<ProfileSearchResult> searchResults = new List<ProfileSearchResult>();

        /// <summary>
        /// Draws a search field and (if results have been returned) search results.
        /// </summary>
        /// <returns>True if search results are being displayed.</returns>
        public static bool DrawSearchInterface(UnityEngine.Object target)
        {
            if (target == null)
            {
                return false;
            }

            bool drewSearchGUI = false;

            if (target != activeTarget)
            {
                activeTarget = target;
                config = new SearchConfig();
                prevConfig = new SearchConfig();
                searchResults.Clear();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Search For: ", GUILayout.MaxWidth(70));
                string searchString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
                config.SearchFieldString = EditorGUILayout.TextField(SessionState.GetString(searchDisplaySearchFieldKey, string.Empty), GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.MaxWidth(50)))
                {
                    config.SearchFieldString = string.Empty;
                }
            }

            config.RequireAllKeywords = SessionState.GetBool(searchDisplayRequireAllKeywordsKey, true);
            config.SearchTooltips = SessionState.GetBool(searchDisplaySearchTooltipsKey, true);
            config.SearchFieldContent = SessionState.GetBool(searchDisplaySearchFieldContentKey, false);

            if (!string.IsNullOrEmpty(config.SearchFieldString))
            {
                // If we're searching for something, draw the search GUI
                DrawSearchResultInterface(target);
                drewSearchGUI = true;
            }

            // Store search settings
            SessionState.SetString(searchDisplaySearchFieldKey, config.SearchFieldString);
            SessionState.SetBool(searchDisplayRequireAllKeywordsKey, config.RequireAllKeywords);
            SessionState.SetBool(searchDisplaySearchTooltipsKey, config.SearchTooltips);
            SessionState.SetBool(searchDisplaySearchFieldContentKey, config.SearchFieldContent);

            return drewSearchGUI;
        }

        private static void DrawSearchResultInterface(UnityEngine.Object target)
        {
            bool optionsFoldout = SessionState.GetBool(searchDisplayOptionsFoldoutKey, false);
            optionsFoldout = EditorGUILayout.Foldout(optionsFoldout, "Search Preferences", true);
            SessionState.SetBool(searchDisplayOptionsFoldoutKey, optionsFoldout);
            if (optionsFoldout)
            {
                config.RequireAllKeywords = EditorGUILayout.Toggle("Require All Keywords", config.RequireAllKeywords);
                config.SearchTooltips = EditorGUILayout.Toggle("Search Tooltips", config.SearchTooltips);
                config.SearchFieldContent = EditorGUILayout.Toggle("Search Field Content", config.SearchFieldContent);
            }

            if (!config.Equals(prevConfig) && !MixedRealitySearchUtility.Searching)
            {
                MixedRealitySearchUtility.StartProfileSearch(target, config, OnSearchComplete);
                searchResults.Clear();
                prevConfig = config;
            }

            #region display results

            using (new EditorGUILayout.VerticalScope())
            {
                if (searchResults.Count == 0)
                {
                    if (MixedRealitySearchUtility.Searching)
                    {
                        EditorGUILayout.HelpBox("Searching...", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No search results. Try selecting a subject or entering a keyword.", MessageType.Warning);
                    }
                }

                int numDisplayedSearchResults = 0;
                if (searchResults.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Results:");
                    foreach (ProfileSearchResult search in searchResults)
                    {
                        if (search.Fields.Count == 0)
                        {   // Don't display results with no fields
                            continue;
                        }

                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Fields found in: ", EditorStyles.boldLabel, GUILayout.MaxWidth(105));
                                EditorGUILayout.ObjectField(search.Profile, typeof(UnityEngine.Object), false, GUILayout.ExpandWidth(true));
                                if (GUILayout.Button("View Asset", GUILayout.MaxWidth(75)))
                                {
                                    Selection.activeObject = search.Profile;
                                    EditorGUIUtility.PingObject(search.Profile);
                                }
                            }

                            if (MixedRealityProjectPreferences.LockProfiles && !search.IsCustomProfile)
                            {
                                EditorGUILayout.HelpBox("Clone this profile to edit default properties.", MessageType.Warning);
                            }

                            using (new EditorGUI.DisabledGroupScope(MixedRealityProjectPreferences.LockProfiles && !search.IsCustomProfile))
                            {
                                using (new EditorGUI.IndentLevelScope(1))
                                {
                                    EditorGUILayout.Space();

                                    foreach (FieldSearchResult r in search.Fields)
                                    {
                                        numDisplayedSearchResults++;

                                        GUI.color = Color.white;
                                        EditorGUI.BeginChangeCheck();
                                        EditorGUILayout.PropertyField(r.Property, true);

                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            r.Property.serializedObject.ApplyModifiedProperties();
                                        }

                                        EditorGUILayout.Space();
                                    }
                                }
                            }
                        }

                        EditorGUILayout.Space();
                    }
                }
            }

            #endregion
        }

        private static void OnSearchComplete(bool cancelled, UnityEngine.Object target, IReadOnlyCollection<ProfileSearchResult> results)
        {
            searchResults.Clear();

            if (cancelled || target != MixedRealitySearchInspectorUtility.activeTarget)
            {   // We've started searching something else, ignore
                return;
            }

            searchResults.AddRange(results);
            // Force editors to repaint so the results are displayed
            foreach (UnityEditor.Editor e in Resources.FindObjectsOfTypeAll<UnityEditor.Editor>())
            {
                e.Repaint();
            }
        }
    }
}