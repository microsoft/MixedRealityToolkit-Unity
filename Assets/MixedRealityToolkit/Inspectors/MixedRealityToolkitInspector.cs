// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor.Search;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkit))]
    public class MixedRealityToolkitInspector : UnityEditor.Editor
    {
        #region search

        private static readonly string[] searchDisplayToolbarButtons = new string[] { "Configure Active Profile", "Search Active Profile" };
        private static readonly string searchDisplayToolbarIndexKey = "MixedRealityToolkitInspector.SearchDisplayToolbarIndex";
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolkitInspector.SearchField";
        private static readonly string searchDisplayRequireAllKeywordsKey = "MixedRealityToolkitInspector.RequireAllKeywords";
        private static readonly string searchDisplayOptionsFoldoutKey = "MixedRealityToolkitInspector.SearchOptionsFoldout";
        private static readonly string searchDisplaySearchTooltipsKey = "MixedRealityToolkitInspector.SearchTooltips";
        private static readonly string searchDisplaySearchFieldObjectNamesKey = "MixedRealityToolkitInspector.SearchFieldObjectNames";
        private static readonly string searchDisplaySearchChildPropertiesKey = "MixedRealityToolkitInspector.SearchChildProperties";

        private const int maxDisplayedSearchResults = 30;
        private const int maxSubjectButtonsPerLine = 6;

        private SearchConfig prevConfig = new SearchConfig();
        private static List<ProfileSearchResult> fieldSearchResults = new List<ProfileSearchResult>();

        #endregion

        private SerializedProperty activeProfile;
        private int currentPickerWindow = -1;

        // Utility to show object picker for ActiveProfile property since Show command must be called in OnGUI()
        private static bool forceShowProfilePicker = false;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            currentPickerWindow = -1;
        }

        public override void OnInspectorGUI()
        {
            MixedRealityToolkit instance = (MixedRealityToolkit)target;

            if (MixedRealityToolkit.Instance == null && instance.isActiveAndEnabled)
            {   // See if an active instance exists at all. If it doesn't register this instance preemptively.
                MixedRealityToolkit.SetActiveInstance(instance);
            }

            if (!instance.IsActiveInstance)
            {
                EditorGUILayout.HelpBox("This instance of the toolkit is inactive. There can only be one active instance loaded at any time.", MessageType.Warning);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Select Active Instance"))
                    {
                        UnityEditor.Selection.activeGameObject = MixedRealityToolkit.Instance.gameObject;
                    }

                    if (GUILayout.Button("Make this the Active Instance"))
                    {
                        MixedRealityToolkit.SetActiveInstance(instance);
                    }
                }
                return;
            }

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);
            bool changed = EditorGUI.EndChangeCheck();
            string commandName = Event.current.commandName;

            // If not profile is assigned, then warn user
            if (activeProfile.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("MixedRealityToolkit cannot initialize unless an Active Profile is assigned!", MessageType.Error);

                if (GUILayout.Button("Assign MixedRealityToolkit Profile") || forceShowProfilePicker)
                {
                    forceShowProfilePicker = false;

                    var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>();

                    // Shows the list of MixedRealityToolkitConfigurationProfiles in our project,
                    // selecting the default profile by default (if it exists).
                    if (allConfigProfiles.Length > 0)
                    {
                        currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive);

                        var defaultMRTKProfile = MixedRealityInspectorUtility.GetDefaultConfigProfile(allConfigProfiles);
                        activeProfile.objectReferenceValue = defaultMRTKProfile;

                        EditorGUIUtility.ShowObjectPicker<MixedRealityToolkitConfigurationProfile>(defaultMRTKProfile, false, string.Empty, currentPickerWindow);
                    }
                    else
                    {
                        if (EditorUtility.DisplayDialog("Attention!", "No profiles were found for the Mixed Reality Toolkit.\n\n" +
                                                                      "Would you like to create one now?", "OK", "Later"))
                        {
                            ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                            profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles");
                            activeProfile.objectReferenceValue = profile;
                            EditorGUIUtility.PingObject(profile);
                        }
                    }
                }

            }

            // If user selects a new MRTK Active Profile, then update configuration
            if (EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
            {
                switch (commandName)
                {
                    case "ObjectSelectorUpdated":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        changed = true;
                        break;
                    case "ObjectSelectorClosed":
                        activeProfile.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        currentPickerWindow = -1;
                        changed = true;
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                MixedRealityToolkit.Instance.ResetConfiguration((MixedRealityToolkitConfigurationProfile)activeProfile.objectReferenceValue);
            }

            if (activeProfile.objectReferenceValue != null)
            {
                int searchDisplayToolbarIndex = SessionState.GetInt(searchDisplayToolbarIndexKey, 0);
                searchDisplayToolbarIndex = GUILayout.Toolbar(searchDisplayToolbarIndex, searchDisplayToolbarButtons);
                SessionState.SetInt(searchDisplayToolbarIndexKey, searchDisplayToolbarIndex);
                switch (searchDisplayToolbarIndex)
                {
                    case 0:
                    default:
                        // For configure, show the default inspector GUI
                        UnityEditor.Editor activeProfileEditor = CreateEditor(activeProfile.objectReferenceValue);
                        activeProfileEditor.OnInspectorGUI();
                        break;

                    case 1:
                        // For search, draw our custom search GUI
                        DrawSearchGUI(activeProfile.objectReferenceValue);
                        break;
                }
            }
        }

        #region search

        private void DrawSearchGUI(UnityEngine.Object activeProfileObject)
        {
            EditorGUILayout.Space();

            SearchConfig config = new SearchConfig()
            {
                SearchFieldString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty),
                RequireAllKeywords = SessionState.GetBool(searchDisplayRequireAllKeywordsKey, true),
                SearchTooltips = SessionState.GetBool(searchDisplaySearchTooltipsKey, true),
                SearchFieldObjectNames = SessionState.GetBool(searchDisplaySearchFieldObjectNamesKey, false),
                SearchChildProperties = SessionState.GetBool(searchDisplaySearchChildPropertiesKey, true)
            };

            #region draw subjects and field input

            EditorGUILayout.HelpBox("Search profiles for:", MessageType.Info);
            config.SearchFieldString = EditorGUILayout.TextField(config.SearchFieldString);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #endregion

            #region options

            bool optionsFoldout = SessionState.GetBool(searchDisplayOptionsFoldoutKey, false);
            optionsFoldout = EditorGUILayout.Foldout(optionsFoldout, "Saerch Preferences", true);
            if (optionsFoldout)
            {
                config.RequireAllKeywords = EditorGUILayout.Toggle("Require All Keywords", config.RequireAllKeywords);
                config.SearchTooltips = EditorGUILayout.Toggle("Search Tooltips", config.SearchTooltips);
                config.SearchFieldObjectNames = EditorGUILayout.Toggle("Search GameObject Names", config.SearchFieldObjectNames);
                config.SearchChildProperties = EditorGUILayout.Toggle("Search Child Properties", config.SearchChildProperties);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #endregion

            SessionState.SetString(searchDisplaySearchFieldKey, config.SearchFieldString);
            SessionState.SetBool(searchDisplayRequireAllKeywordsKey, config.RequireAllKeywords);
            SessionState.SetBool(searchDisplayOptionsFoldoutKey, optionsFoldout);
            SessionState.SetBool(searchDisplaySearchTooltipsKey, config.SearchTooltips);
            SessionState.SetBool(searchDisplaySearchFieldObjectNamesKey, config.SearchFieldObjectNames);
            SessionState.SetBool(searchDisplaySearchChildPropertiesKey, config.SearchChildProperties);

            #region execute search

            if (!config.Equals(prevConfig))
            {
                fieldSearchResults.Clear();
                fieldSearchResults.AddRange(MixedRealitySearchUtility.SearchProfileFields(activeProfileObject, config));
                fieldSearchResults.Sort(delegate (ProfileSearchResult r1, ProfileSearchResult r2)
                {
                    if (r1.MaxFieldMatchStrength != r2.MaxFieldMatchStrength)
                    {
                        return r2.MaxFieldMatchStrength.CompareTo(r1.MaxFieldMatchStrength);
                    }
                    else if (r1.ProfileMatchStrength != r2.ProfileMatchStrength)
                    {
                        return r2.ProfileMatchStrength.CompareTo(r1.ProfileMatchStrength);
                    }
                    else
                    {
                        return r2.Profile.name.CompareTo(r1.Profile.name);
                    }
                });
                prevConfig = config;
            }

            #endregion

            #region display results

            using (new EditorGUILayout.VerticalScope())
            {
                if (fieldSearchResults.Count == 0)
                {
                    EditorGUILayout.HelpBox("No search results. Try selecting a subject or entering a keyword.", MessageType.Warning);
                }

                int numDisplayedSearchResults = 0;
                if (fieldSearchResults.Count > 0)
                {
                    if (string.IsNullOrEmpty(config.SearchFieldString))
                    {   // If no keywords are used, just show the profile editor
                        EditorGUILayout.LabelField("Matching Profiles:");
                        foreach (ProfileSearchResult search in fieldSearchResults)
                        {
                            EditorGUILayout.ObjectField(search.Profile, typeof(ScriptableObject), false);
                        }
                    }
                    else
                    {   // Otherwise, show individual fields
                        EditorGUILayout.LabelField("Results:");
                        foreach (ProfileSearchResult search in fieldSearchResults)
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
                                }

                                EditorGUI.indentLevel++;
                                EditorGUILayout.Space();

                                foreach (FieldSearchResult r in search.Fields)
                                {
                                    numDisplayedSearchResults++;

                                    if (!string.IsNullOrEmpty(r.Property.tooltip))
                                    {
                                        GUI.color = MixedRealityInspectorUtility.DisabledColor;
                                        EditorGUILayout.LabelField(r.Property.tooltip + " (" + r.MatchStrength + ")", EditorStyles.wordWrappedMiniLabel);
                                    }

                                    GUI.color = Color.white;
                                    EditorGUILayout.PropertyField(r.Property, true);
                                    if (numDisplayedSearchResults >= maxDisplayedSearchResults)
                                    {
                                        break;
                                    }

                                    EditorGUILayout.Space();
                                }
                                EditorGUI.indentLevel--;
                            }

                            EditorGUILayout.Space();

                            if (numDisplayedSearchResults >= maxDisplayedSearchResults)
                            {
                                break;
                            }
                        }

                        if (numDisplayedSearchResults >= maxDisplayedSearchResults)
                        {
                            EditorGUILayout.HelpBox("Displaying the first " + maxDisplayedSearchResults + " results. Try narrowing your search criteria.", MessageType.Info);
                        }
                    }
                }
            }
            #endregion
        }

        #endregion

        [MenuItem("Mixed Reality Toolkit/Add to Scene and Configure...")]
        public static void CreateMixedRealityToolkitGameObject()
        {
            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene();
            Selection.activeObject = MixedRealityToolkit.Instance;
            EditorGUIUtility.PingObject(MixedRealityToolkit.Instance);

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
            {
                forceShowProfilePicker = true;
            }
        }
    }
}
