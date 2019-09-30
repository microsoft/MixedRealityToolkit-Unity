// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkit))]
    public class MixedRealityToolkitInspector : UnityEditor.Editor
    {
        private static readonly string[] searchDisplayToolbarButtons = new string[] { "Configure Active Profile", "Search Active Profile" };
        private static readonly string searchDisplayToolbarIndexKey = "MixedRealityToolkitInspector.SearchDisplayToolbarIndex";
        private static readonly string searchDisplaySearchFieldKey = "MixedRealityToolkitInspector.SearchField";
        private static readonly string searchDisplaySelectedTagsKey = "MixedRealityToolkitInspector.SelectedTags";
        private const int maxDisplayedSearchResults = 30;

        private static string prevSearchFieldString = string.Empty;
        private static SubjectTag prevSelectedTags = 0;
        private static List<MixedRealityProfileUtility.ProfileSearchResult> searchResults = new List<MixedRealityProfileUtility.ProfileSearchResult>();

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

        private void DrawSearchGUI(UnityEngine.Object activeProfileObject)
        {
            EditorGUILayout.Space();

            string searchFieldString = SessionState.GetString(searchDisplaySearchFieldKey, string.Empty);
            searchFieldString = EditorGUILayout.TextField("Search Fields:", searchFieldString);
            SessionState.SetString(searchDisplaySearchFieldKey, searchFieldString);
            SubjectTag selectedTags = (SubjectTag)SessionState.GetInt(searchDisplaySelectedTagsKey, (int)SubjectTag.All);

            EditorGUILayout.Space();

            // Draw our subject tags in a clump
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Search by Subject:");

                bool horizontalGroupOpen = false;
                int numButtons = 5;
                int maxButtonsPerLine = 5;

                foreach (SubjectTag tag in Enum.GetValues(typeof(SubjectTag)))
                {
                    if (tag == SubjectTag.All)
                    {
                        continue;
                    }

                    if (numButtons >= maxButtonsPerLine)
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
                    if (numButtons >= maxButtonsPerLine)
                    {
                        EditorGUILayout.EndHorizontal();
                        horizontalGroupOpen = false;
                    }
                }

                if (horizontalGroupOpen)
                {
                    EditorGUILayout.EndHorizontal();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("All", EditorStyles.miniButton))
                    {
                        selectedTags = SubjectTag.All;
                    }
                    if (GUILayout.Button("None", EditorStyles.miniButton))
                    {
                        selectedTags = 0;
                    }
                }

                SessionState.SetInt(searchDisplaySelectedTagsKey, (int)selectedTags);
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Results:");

                if (prevSearchFieldString != searchFieldString || prevSelectedTags != selectedTags)
                {
                    HashSet<string> searchFieldStrings = new HashSet<string>(searchFieldString.Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries));
                    searchResults.Clear();
                    searchResults.AddRange(MixedRealityProfileUtility.SearchProfileFields(activeProfileObject, searchFieldStrings, selectedTags));
                    prevSearchFieldString = searchFieldString;
                    prevSelectedTags = selectedTags;
                }

                int numDisplayedSearchResults = 0;
                foreach (MixedRealityProfileUtility.ProfileSearchResult result in searchResults)
                {
                    EditorGUILayout.ObjectField(result.profile, typeof(UnityEngine.Object), false);
                    EditorGUI.indentLevel++;
                    foreach (SerializedProperty property in result.properties)
                    {
                        numDisplayedSearchResults++;
                        EditorGUILayout.PropertyField(property, true);
                        if (numDisplayedSearchResults >= maxDisplayedSearchResults)
                        {
                            break;
                        }
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();
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
