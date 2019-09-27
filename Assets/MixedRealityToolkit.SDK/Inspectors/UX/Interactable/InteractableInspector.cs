// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(Interactable))]
    public class InteractableInspector : UnityEditor.Editor
    {
        protected Interactable instance;

        protected SerializedProperty profileList;
        protected SerializedProperty statesProperty;
        protected SerializedProperty enabledProperty;
        protected SerializedProperty voiceCommands;
        protected SerializedProperty actionId;
        protected SerializedProperty isGlobal;
        protected SerializedProperty canSelect;
        protected SerializedProperty canDeselect;
        protected SerializedProperty startDimensionIndex;
        protected SerializedProperty dimensionIndex;
        protected SerializedProperty dimensions;

        protected const string ShowProfilesPrefKey = "InteractableInspectorProfiles";
        protected const string ShowEventsPrefKey = "InteractableInspectorProfiles_ShowEvents";
        protected bool enabled = false;

        protected string[] inputActionOptions = null;
        protected string[] speechKeywordOptions = null;

        private static readonly GUIContent InputActionsLabel = new GUIContent("Input Actions", "The input action filter");
        private static readonly GUIContent selectionModeLabel = new GUIContent("Selection Mode", "How the Interactable should react to input");
        private static readonly GUIContent dimensionsLabel = new GUIContent("Dimensions", "The amount of theme layers for sequence button functionality (3-9)");
        private static readonly GUIContent startDimensionLabel = new GUIContent("Start Dimension Index", "The dimensionIndex value to set on start.");
        private static readonly GUIContent CurrentDimensionLabel = new GUIContent("Dimension Index", "The dimensionIndex value at runtime.");
        private static readonly GUIContent isToggledLabel = new GUIContent("Is Toggled", "The toggled value to set on start.");
        private static readonly GUIContent CreateThemeLabel = new GUIContent("Create and Assign New Theme", "Create a new theme");
        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("+ Add Theme Property", "Add Theme Property");
        private static readonly GUIContent SpeechComamndsLabel = new GUIContent("Speech Command", "Speech Commands to use with Interactable, pulled from MRTK/Input/Speech Commands Profile");

        protected virtual void OnEnable()
        {
            instance = (Interactable)target;

            profileList = serializedObject.FindProperty("profiles");
            statesProperty = serializedObject.FindProperty("states");
            enabledProperty = serializedObject.FindProperty("Enabled");
            voiceCommands = serializedObject.FindProperty("VoiceCommand");
            actionId = serializedObject.FindProperty("InputActionId");
            isGlobal = serializedObject.FindProperty("IsGlobal");
            canSelect = serializedObject.FindProperty("CanSelect");
            canDeselect = serializedObject.FindProperty("CanDeselect");
            startDimensionIndex = serializedObject.FindProperty("StartDimensionIndex");
            dimensionIndex = serializedObject.FindProperty("dimensionIndex");
            dimensions = serializedObject.FindProperty("Dimensions");

            enabled = true;
        }

        protected virtual void RenderBaseInspector()
        {
            base.OnInspectorGUI();
        }

        /// <remarks>
        /// There is a check in here that verifies whether or not we can get InputActions, if we can't we show an error help box; otherwise we get them.
        /// This method is sealed, if you wish to override <see cref="OnInspectorGUI"/>, then override <see cref="RenderCustomInspector"/> method instead.
        /// </remarks>
        public sealed override void OnInspectorGUI()
        {
            if ((inputActionOptions == null && !Interactable.TryGetInputActions(out inputActionOptions)) 
                || (speechKeywordOptions == null && !Interactable.TryGetSpeechKeywords(out speechKeywordOptions)))
            {
                EditorGUILayout.HelpBox("Mixed Reality Toolkit is missing, configure it by invoking the 'Mixed Reality Toolkit > Add to Scene and Configure...' menu", MessageType.Error);
            }
            
            RenderCustomInspector();
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            RenderGeneralSettings();

            EditorGUILayout.Space();

            RenderProfileSettings();

            EditorGUILayout.Space();

            RenderEventSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderProfileSettings()
        {
            if (profileList.arraySize < 1)
            {
                AddProfile(0);
            }

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Profiles", ShowProfilesPrefKey, MixedRealityStylesUtility.TitleFoldoutStyle))
            {
                // Render all profile items. Profiles are per GameObject/ThemeContainer
                for (int i = 0; i < profileList.arraySize; i++)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        SerializedProperty profileItem = profileList.GetArrayElementAtIndex(i);
                        SerializedProperty hostGameObject = profileItem.FindPropertyRelative("Target");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(hostGameObject, new GUIContent("Target", "Target gameObject for this theme properties to manipulate"));
                            if (InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove Profile"), i, RemoveProfile))
                            {
                                // Profile removed via RemoveProfile callback
                                continue;
                            }
                        }

                        if (hostGameObject.objectReferenceValue == null)
                        {
                            InspectorUIUtility.DrawError("Assign a GameObject to apply visual effects");
                            if (GUILayout.Button("Assign Self"))
                            {
                                hostGameObject.objectReferenceValue = instance.gameObject;
                            }
                        }

                        EditorGUILayout.Space();

                        SerializedProperty themes = profileItem.FindPropertyRelative("Themes");
                        ValidateThemesForDimensions(dimensions, themes);

                        // Render all themes for current target
                        for (int t = 0; t < themes.arraySize; t++)
                        {
                            SerializedProperty themeItem = themes.GetArrayElementAtIndex(t);
                            string themeLabel = BuildThemeTitle(dimensions.intValue, t);

                            if (themeItem.objectReferenceValue != null)
                            {
                                bool showThemeSettings = false;
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    string prefKey = themeItem.objectReferenceValue.name + "Profiles" + i + "_Theme" + t + "_Edit";
                                    showThemeSettings = InspectorUIUtility.DrawSectionFoldoutWithKey(themeLabel, prefKey);
                                    EditorGUILayout.PropertyField(themeItem, new GUIContent(string.Empty, "Theme properties for interaction feedback"));
                                }

                                if (themeItem.objectReferenceValue != null)
                                {
                                    // TODO: Odd bug where themeStates below is null when it shouldn't be. Use instance object as workaround atm
                                    //SerializedProperty themeStates = themeItem.FindPropertyRelative("States");
                                    var themeInstance = themeItem.objectReferenceValue as Theme;
                                    if (statesProperty.objectReferenceValue != themeInstance.States)
                                    {
                                        InspectorUIUtility.DrawWarning($"{themeInstance.name}'s States property does not match Interactable's States property");
                                    }

                                    if (showThemeSettings)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            UnityEditor.Editor themeEditor = UnityEditor.Editor.CreateEditor(themeItem.objectReferenceValue);
                                            themeEditor.OnInspectorGUI();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(themeItem, new GUIContent(themeLabel, "Theme properties for interaction feedback"));

                                InspectorUIUtility.DrawError("Assign a Theme to add visual effects");
                                if (GUILayout.Button(CreateThemeLabel))
                                {
                                    themeItem.objectReferenceValue = CreateThemeAsset(hostGameObject.objectReferenceValue.name);
                                    return;
                                }
                            }

                            EditorGUILayout.Space();
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent("Add Profile")))
                {
                    AddProfile(profileList.arraySize);
                }
            }
        }

        private void RenderEventSettings()
        {
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Events", ShowEventsPrefKey, MixedRealityStylesUtility.TitleFoldoutStyle))
            {
                EditorGUILayout.Space();

                SerializedProperty onClick = serializedObject.FindProperty("OnClick");
                EditorGUILayout.PropertyField(onClick, new GUIContent("OnClick"));

                SerializedProperty events = serializedObject.FindProperty("Events");
                GUI.enabled = !isPlayMode;
                for (int i = 0; i < events.arraySize; i++)
                {
                    SerializedProperty eventItem = events.GetArrayElementAtIndex(i);
                    if (InteractableEventInspector.RenderEvent(eventItem))
                    {
                        events.DeleteArrayElementAtIndex(i);
                        // If removed, skip rendering rest of list till next redraw
                        break;
                    }

                    EditorGUILayout.Space();
                }
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("Add Event")))
                {
                    AddEvent(events.arraySize);
                }
            }
        }

        protected void RenderGeneralSettings()
        {
            Rect position;
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;

            using (new EditorGUILayout.HorizontalScope())
            {
                InspectorUIUtility.DrawLabel("General", InspectorUIUtility.TitleFontSize, InspectorUIUtility.ColorTint10);

                if (target != null)
                {
                    var helpURL = target.GetType().GetCustomAttribute<HelpURLAttribute>();
                    if (helpURL != null)
                    {
                        InspectorUIUtility.RenderDocumentationButton(helpURL.URL);
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // If states value is not provided, try to use Default states type
                if (statesProperty.objectReferenceValue == null)
                {
                    statesProperty.objectReferenceValue = GetDefaultInteractableStatesFile();
                }

                GUI.enabled = !isPlayMode;
                EditorGUILayout.PropertyField(statesProperty, new GUIContent("States", "The States this Interactable is based on"));
                GUI.enabled = true;

                if (statesProperty.objectReferenceValue == null)
                {
                    InspectorUIUtility.DrawError("Please assign a States object!");
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                EditorGUILayout.PropertyField(enabledProperty, new GUIContent("Enabled", "Is this Interactable Enabled?"));

                // Input Actions
                bool validActionOptions = inputActionOptions != null;
                GUI.enabled = validActionOptions && !isPlayMode;

                var actionOptions = validActionOptions ? inputActionOptions : new string[] { "Missing Mixed Reality Toolkit" };
                DrawDropDownProperty(EditorGUILayout.GetControlRect(), actionId, actionOptions, InputActionsLabel);

                GUI.enabled = true;

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(isGlobal, new GUIContent("Is Global", "Like a modal, does not require focus"));
                }

                // Speech keywords
                bool validSpeechKeywords = speechKeywordOptions != null;
                GUI.enabled = validSpeechKeywords && !isPlayMode;

                string[] keywordOptions = validSpeechKeywords ? speechKeywordOptions : new string[] { "Missing Speech Commands" };
                int currentIndex = validSpeechKeywords ? SpeechKeywordLookup(voiceCommands.stringValue, speechKeywordOptions) : 0;
                position = EditorGUILayout.GetControlRect();

                //BeginProperty allows tracking of serialized properties for bolding prefab changes etc
                using (new EditorGUI.PropertyScope(position, SpeechComamndsLabel, voiceCommands))
                {
                    currentIndex = EditorGUI.Popup(position, SpeechComamndsLabel.text, currentIndex, keywordOptions);
                    if (validSpeechKeywords)
                    {
                        voiceCommands.stringValue = currentIndex > 0 ? speechKeywordOptions[currentIndex] : string.Empty;
                    }
                }
                GUI.enabled = true;

                // show requires gaze because voice command has a value
                if (!string.IsNullOrEmpty(voiceCommands.stringValue))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        SerializedProperty requireGaze = serializedObject.FindProperty("RequiresFocus");
                        EditorGUILayout.PropertyField(requireGaze, new GUIContent("Requires Focus", "Does the voice command require gazing at this interactable?"));
                    }
                }

                // should be 1 or more
                dimensions.intValue = Mathf.Clamp(dimensions.intValue, 1, 9);
                string[] selectionModeNames = Enum.GetNames(typeof(SelectionModes));
                // clamp to values in the enum
                int selectionModeIndex = Mathf.Clamp(dimensions.intValue, 1, selectionModeNames.Length) - 1;

                // user-friendly dimension settings
                SelectionModes selectionMode = SelectionModes.Button;
                position = EditorGUILayout.GetControlRect();
                GUI.enabled = !isPlayMode;
                using (new EditorGUI.PropertyScope(position, selectionModeLabel, dimensions))
                {
                    selectionMode = (SelectionModes)EditorGUI.EnumPopup(position, selectionModeLabel, (SelectionModes)(selectionModeIndex));

                    switch (selectionMode)
                    {
                        case SelectionModes.Button:
                            dimensions.intValue = 1;
                            break;
                        case SelectionModes.Toggle:
                            dimensions.intValue = 2;
                            break;
                        case SelectionModes.MultiDimension:
                            // multi dimension mode - set min value to 3
                            dimensions.intValue = Mathf.Max(3, dimensions.intValue);
                            position = EditorGUILayout.GetControlRect();
                            dimensions.intValue = EditorGUI.IntField(position, dimensionsLabel, dimensions.intValue);
                            break;
                        default:
                            break;
                    }
                }

                if (dimensions.intValue > 1)
                {
                    // toggle or multi dimensional button
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(canSelect, new GUIContent("Can Select", "The user can toggle this button"));
                        EditorGUILayout.PropertyField(canDeselect, new GUIContent("Can Deselect", "The user can untoggle this button, set false for a radial interaction."));

                        position = EditorGUILayout.GetControlRect();
                        using (new EditorGUI.PropertyScope(position, startDimensionLabel, startDimensionIndex))
                        {
                            if (dimensions.intValue >= selectionModeNames.Length)
                            {
                                // multi dimensions
                                if (!isPlayMode)
                                {
                                    startDimensionIndex.intValue = EditorGUI.IntField(position, startDimensionLabel, startDimensionIndex.intValue);
                                }
                                else
                                {
                                    EditorGUI.IntField(position, CurrentDimensionLabel, dimensionIndex.intValue);
                                }
                            }
                            else if (dimensions.intValue == (int)SelectionModes.Toggle + 1)
                            {
                                if (!isPlayMode)
                                {
                                    bool isToggled = EditorGUI.Toggle(position, isToggledLabel, startDimensionIndex.intValue > 0);
                                    startDimensionIndex.intValue = isToggled ? 1 : 0;
                                }
                                else
                                {
                                    bool isToggled = EditorGUI.Toggle(position, isToggledLabel, dimensionIndex.intValue > 0);
                                }
                            }

                            startDimensionIndex.intValue = Mathf.Clamp(startDimensionIndex.intValue, 0, dimensions.intValue - 1);
                        }
                    }
                }
                GUI.enabled = true;
            }
        }

        public static States GetDefaultInteractableStatesFile()
        {
            AssetDatabase.Refresh();
            string[] stateLocations = AssetDatabase.FindAssets("DefaultInteractableStates");
            if (stateLocations.Length > 0)
            {
                for (int i = 0; i < stateLocations.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(stateLocations[i]);
                    States defaultStates = (States)AssetDatabase.LoadAssetAtPath(path, typeof(States));
                    if (defaultStates != null)
                    {
                        return defaultStates;
                    }
                }
            }

            return null;
        }

        private static string BuildThemeTitle(int dimensions, int themeIndex)
        {
            if (dimensions == 2)
            {
                return "Theme " + (themeIndex % 2 == 0 ? "(Deselected)" : "(Selected)");
            }
            else if (dimensions >= 3)
            {
                return "Theme " + (themeIndex + 1);
            }

            return "Theme";
        }

        #region Profiles

        protected void AddProfile(int index)
        {
            profileList.InsertArrayElementAtIndex(profileList.arraySize);
            SerializedProperty newProfile = profileList.GetArrayElementAtIndex(profileList.arraySize - 1);

            SerializedProperty newTarget = newProfile.FindPropertyRelative("Target");
            SerializedProperty themes = newProfile.FindPropertyRelative("Themes");
            newTarget.objectReferenceValue = null;

            themes.ClearArray();
        }

        protected void RemoveProfile(int index, SerializedProperty prop = null)
        {
            profileList.DeleteArrayElementAtIndex(index);
        }

        #endregion Profiles

        #region Themes

        protected static Theme CreateThemeAsset(string themeName = null)
        {
            string themeFileName = (string.IsNullOrEmpty(themeName) ? "New " : themeName) +"Theme.asset";

            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Theme",
                themeFileName,
                "asset",
                "Create a name and select a location for this theme");

            if (path.Length != 0)
            {
                Theme newTheme = ScriptableObject.CreateInstance<Theme>();
                newTheme.States = GetDefaultInteractableStatesFile();
                newTheme.Definitions = new List<ThemeDefinition>();
                AssetDatabase.CreateAsset(newTheme, path);
                return newTheme;
            }

            return null;
        }

        /// <summary>
        ///  Ensure the number of theme containers is equal to the number of dimensions
        /// </summary>
        /// <param name="dimensions">dimensions property of interactable</param>
        /// <param name="themes">List of ThemeContainers in Interactable profile</param>
        private static void ValidateThemesForDimensions(SerializedProperty dimensions, SerializedProperty themes)
        {
            int numOfDimensions = dimensions.intValue;
            if (themes.arraySize < numOfDimensions)
            {
                for (int index = themes.arraySize; index < numOfDimensions; index++)
                {
                    themes.InsertArrayElementAtIndex(themes.arraySize);

                    SerializedProperty newTheme = themes.GetArrayElementAtIndex(themes.arraySize - 1);
                    newTheme.objectReferenceValue = null;
                }
            }
            else
            {
                for (int index = themes.arraySize - 1; index > numOfDimensions - 1; index--)
                {
                    themes.DeleteArrayElementAtIndex(index);
                }
            }
        }
        
        #endregion Themes

        #region Events
        /*
         * EVENTS
         */

        protected void RemoveEvent(int index, SerializedProperty prop = null)
        {
            SerializedProperty events = serializedObject.FindProperty("Events");
            if (events.arraySize > index)
            {
                events.DeleteArrayElementAtIndex(index);
            }
        }

        protected void AddEvent(int index)
        {
            SerializedProperty events = serializedObject.FindProperty("Events");
            events.InsertArrayElementAtIndex(events.arraySize);
        }

        #endregion Events

        #region PopupUtilities
        /// <summary>
        /// Get the index of the speech keyword array item based on its name, pop-up field helper
        /// Skips the first item in the array (internal added blank value to turn feature off)
        /// and returns a 0 if no match is found for the blank value
        /// </summary>
        protected int SpeechKeywordLookup(string option, string[] options)
        {
            // starting on 1 to skip the blank value
            for (int i = 1; i < options.Length; i++)
            {
                if (options[i] == option)
                {
                    return i;
                }
            }
            return 0;
        }	
        
        /// <summary>
        /// Draws a popup UI with PropertyField type features.
        /// Displays prefab pending updates
        /// </summary>
        protected void DrawDropDownProperty(Rect position, SerializedProperty prop, string[] options, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);
            {
                prop.intValue = EditorGUI.Popup(position, label.text, prop.intValue, options);
            }
            EditorGUI.EndProperty();
        }
    }
    #endregion KeywordUtilities
}
