// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly string searchDisplaySelectedTagsKey = "MixedRealityToolkitInspector.SelectedTags";
        private static readonly string searchDisplayRequireAllTagsKey = "MixedRealityToolkitInspector.RequireAllTags";
        private static readonly string searchDisplayRequireAllKeywordsKey = "MixedRealityToolkitInspector.RequireAllKeywords";
        private const int maxDisplayedSearchResults = 30;
        private const int maxSubjectButtonsPerLine = 5;
        private const int minSearchStringLength = 3;

        private static string prevSearchFieldString = string.Empty;
        private static SubjectTag prevSelectedTags = 0;
        private static bool prevRequireAllTags = false;
        private static bool prevRequireAllKeywords = false;

        // Raw list of search results
        private static List<MixedRealityProfileUtility.ProfileSearchResult> fieldSearchResults = new List<MixedRealityProfileUtility.ProfileSearchResult>();
        // Filtered list of profile-only search results
        private static List<MixedRealityProfileUtility.ProfileSearchResult> profileSearchResults = new List<MixedRealityProfileUtility.ProfileSearchResult>();

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

            SubjectTag selectedTags = (SubjectTag)SessionState.GetInt(searchDisplaySelectedTagsKey, 0);
            bool requireAllTags = SessionState.GetBool(searchDisplayRequireAllTagsKey, false);
            string searchFieldString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
            bool requireAllKeywords = SessionState.GetBool(searchDisplayRequireAllKeywordsKey, false);

            // Draw our subject tags in a clump
            EditorGUILayout.HelpBox("Select subjects below to search for profiles and fields tagged with that subject.", MessageType.Info);

            #region draw subjects and field input

            using (new EditorGUILayout.VerticalScope())
            {
                bool horizontalGroupOpen = false;
                int numButtons = maxSubjectButtonsPerLine;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.color = selectedTags == 0 ? MixedRealityInspectorUtility.SuccessColor : MixedRealityInspectorUtility.DisabledColor;
                    if (GUILayout.Button("(None)", EditorStyles.miniButton))
                    {
                        selectedTags = 0;
                    }

                    GUI.color = selectedTags == SubjectTag.All ? MixedRealityInspectorUtility.SuccessColor : MixedRealityInspectorUtility.DisabledColor;
                    if (GUILayout.Button("(All)", EditorStyles.miniButton))
                    {
                        selectedTags = SubjectTag.All;
                    }
                }

                foreach (SubjectTag tag in Enum.GetValues(typeof(SubjectTag)))
                {
                    if (tag == SubjectTag.All)
                    {
                        continue;
                    }

                    if (numButtons >= maxSubjectButtonsPerLine)
                    {
                        numButtons = 0;
                        EditorGUILayout.BeginHorizontal();
                        horizontalGroupOpen = true;
                    }


                    bool selected = (selectedTags & tag) != 0;
                    GUI.color = selected ? MixedRealityInspectorUtility.SuccessColor : MixedRealityInspectorUtility.DisabledColor;
                    if (GUILayout.Button(tag.ToString(), EditorStyles.miniButton))
                    {
                        if (selected)
                        {
                            selectedTags &= ~tag;
                        }
                        else
                        {
                            selectedTags |= tag;
                        }
                    }
                    GUI.color = Color.white;

                    numButtons++;
                    if (numButtons >= maxSubjectButtonsPerLine)
                    {
                        EditorGUILayout.EndHorizontal();
                        horizontalGroupOpen = false;
                    }
                }

                if (horizontalGroupOpen)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Enter keywords below to search field names and tooltips.\nIf any subjects are selected, those will be used to narrow your search. Select (None) to search all fields.", MessageType.Info);
            searchFieldString = EditorGUILayout.TextField(searchFieldString);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            /*using (new GUILayout.HorizontalScope())
            {
                requireAllKeywords = GUILayout.Toggle(requireAllKeywords, "Require All Keywords");
                requireAllTags = GUILayout.Toggle(requireAllTags, "Require All Tags");
            }*/

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            SessionState.SetString(searchDisplaySearchFieldKey, searchFieldString);
            SessionState.SetInt(searchDisplaySelectedTagsKey, (int)selectedTags);
            SessionState.SetBool(searchDisplayRequireAllTagsKey, requireAllTags);
            SessionState.SetBool(searchDisplayRequireAllKeywordsKey, requireAllKeywords);

            #region execute search

            if (prevSearchFieldString != searchFieldString || prevSelectedTags != selectedTags || prevRequireAllTags != requireAllTags || prevRequireAllKeywords != requireAllKeywords)
            {
                HashSet<string> searchFieldStrings = new HashSet<string>(searchFieldString.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries));
                searchFieldStrings.RemoveWhere(s => s.Length < minSearchStringLength);

                List<MixedRealityProfileUtility.ProfileSearchResult> allSearchResults = new List<MixedRealityProfileUtility.ProfileSearchResult>(
                    MixedRealityProfileUtility.SearchProfileFields(activeProfileObject, searchFieldStrings, selectedTags, requireAllTags, requireAllKeywords));

                fieldSearchResults.Clear();
                fieldSearchResults.AddRange(allSearchResults.Where(r => r.MaxFieldMatchStrength > 0));
                fieldSearchResults.Sort(delegate (MixedRealityProfileUtility.ProfileSearchResult r1, MixedRealityProfileUtility.ProfileSearchResult r2)
                {
                    if (r1.MaxFieldMatchStrength != r2.MaxFieldMatchStrength)
                    {
                        return r2.MaxFieldMatchStrength.CompareTo(r1.MaxFieldMatchStrength);
                    }
                    return r2.Profile.name.CompareTo(r1.Profile.name);
                });

                profileSearchResults.Clear();
                profileSearchResults.AddRange(allSearchResults.Where(r => r.ProfileMatchStrength > 0));
                profileSearchResults.Sort(delegate (MixedRealityProfileUtility.ProfileSearchResult r1, MixedRealityProfileUtility.ProfileSearchResult r2)
                {
                    if (r1.ProfileMatchStrength != r2.ProfileMatchStrength)
                    {
                        return r2.ProfileMatchStrength.CompareTo(r1.ProfileMatchStrength);
                    }
                    return r2.Profile.name.CompareTo(r1.Profile.name);
                });

                prevSearchFieldString = searchFieldString;
                prevSelectedTags = selectedTags;
                prevRequireAllTags = requireAllTags;
                prevRequireAllKeywords = requireAllKeywords;
            }

            #endregion

            #region display results

            using (new EditorGUILayout.VerticalScope())
            {
                if (fieldSearchResults.Count == 0 && profileSearchResults.Count == 0)
                {
                    EditorGUILayout.HelpBox("No search results. Try selecting a subject or entering a keyword.", MessageType.Warning);
                }

                if (profileSearchResults.Count > 0)
                {
                    EditorGUILayout.LabelField("Profiles:");
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        foreach (MixedRealityProfileUtility.ProfileSearchResult search in profileSearchResults)
                        {
                            EditorGUILayout.ObjectField(search.Profile, typeof(UnityEngine.Object), false);
                        }
                    }
                    EditorGUILayout.Space();
                }

                int numDisplayedSearchResults = 0;
                if (fieldSearchResults.Count > 0)
                {
                    EditorGUILayout.LabelField("Fields:");
                    foreach (MixedRealityProfileUtility.ProfileSearchResult search in fieldSearchResults)
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.ObjectField(search.Profile, typeof(UnityEngine.Object), false);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.Space();

                            foreach (MixedRealityProfileUtility.FieldResult r in search.Fields)
                            {
                                numDisplayedSearchResults++;

                                if (r.ToolTip != null)
                                {
                                    GUI.color = MixedRealityInspectorUtility.DisabledColor;
                                    EditorGUILayout.LabelField(r.ToolTip.tooltip + "(" + r.MatchStrength + ")", EditorStyles.wordWrappedMiniLabel);
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
