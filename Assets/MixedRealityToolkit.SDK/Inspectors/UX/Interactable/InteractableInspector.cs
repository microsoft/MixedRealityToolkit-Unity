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
        protected List<InteractableEvent> eventList;

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

        protected static bool showProfiles;
        protected static bool showEvents;
        protected const string ShowProfilesPrefKey = "InteractableInspectorProfiles";
        protected const string ShowEventsPrefKey = "InteractableInspectorProfiles_ShowEvents";
        protected bool enabled = false;

        protected InteractableTypesContainer eventOptions;
        protected InteractableTypesContainer themeOptions;
        protected string[] shaderOptions;

        protected string[] inputActionOptions = null;
        protected string[] speechKeywordOptions = null;
        protected static bool ProfilesSetup = false;

        protected bool hasProfileLayout;

        protected GUIStyle boxStyle;
        private const int ThemePropertiesBoxMargin = 30;

        private static readonly GUIContent InputActionsLabel = new GUIContent("Input Actions", "The input action filter");
        private static readonly GUIContent selectionModeLabel = new GUIContent("Selection Mode", "How the Interactable should react to input");
        private static readonly GUIContent dimensionsLabel = new GUIContent("Dimensions", "The amount of theme layers for sequence button functionality (3-9)");
        private static readonly GUIContent startDimensionLabel = new GUIContent("Start Dimension Index", "The dimensionIndex value to set on start.");
        private static readonly GUIContent CurrentDimensionLabel = new GUIContent("Dimension Index", "The dimensionIndex value at runtime.");
        private static readonly GUIContent isToggledLabel = new GUIContent("Is Toggled", "The toggled value to set on start.");
        private static readonly GUIContent CreateThemeLabel = new GUIContent("Create Theme", "Create a new theme");
        private static readonly GUIContent AddThemePropertyLabel = new GUIContent("+ Add Theme Property", "Add Theme Property");
        private static readonly GUIContent SpeechComamndsLabel = new GUIContent("Speech Command", "Speech Commands to use with Interactable, pulled from MRTK/Input/Speech Commands Profile");

        protected virtual void OnEnable()
        {
            instance = (Interactable)target;
            eventList = instance.Events;

            profileList = serializedObject.FindProperty("Profiles");
            statesProperty = serializedObject.FindProperty("States");
            enabledProperty = serializedObject.FindProperty("Enabled");
            voiceCommands = serializedObject.FindProperty("VoiceCommand");
            actionId = serializedObject.FindProperty("InputActionId");
            isGlobal = serializedObject.FindProperty("IsGlobal");
            canSelect = serializedObject.FindProperty("CanSelect");
            canDeselect = serializedObject.FindProperty("CanDeselect");
            startDimensionIndex = serializedObject.FindProperty("StartDimensionIndex");
            dimensionIndex = serializedObject.FindProperty("dimensionIndex");
            dimensions = serializedObject.FindProperty("Dimensions");

            showProfiles = EditorPrefs.GetBool(ShowProfilesPrefKey, showProfiles);

            SetupEventOptions();
            SetupThemeOptions();

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
            if (!ProfilesSetup && !showProfiles)
            {
                InspectorUIUtility.DrawWarning("Profiles (Optional) have not been set up or has errors.");
            }

            bool isProfilesOpen = InspectorUIUtility.DrawSectionFoldout("Profiles", showProfiles, FontStyle.Bold, InspectorUIUtility.TitleFontSize);
            if (showProfiles != isProfilesOpen)
            {
                showProfiles = isProfilesOpen;
                EditorPrefs.SetBool(ShowProfilesPrefKey, showProfiles);
            }

            if (profileList.arraySize < 1)
            {
                AddProfile(0);
            }

            int validProfileCnt = 0;
            int themeCnt = 0;

            if (showProfiles)
            {
                // Render all profile items
                for (int i = 0; i < profileList.arraySize; i++)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        SerializedProperty profileItem = profileList.GetArrayElementAtIndex(i);
                        SerializedProperty gameObject = profileItem.FindPropertyRelative("Target");

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(gameObject, new GUIContent("Target", "Target gameObject for this theme properties to manipulate"));
                            if (InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove Profile"), i, RemoveProfile))
                            {
                                continue;
                            }
                        }

                        SerializedProperty themes = profileItem.FindPropertyRelative("Themes");
                        ValidateThemes(dimensions, themes);

                        // Render all themes for current target
                        for (int t = 0; t < themes.arraySize; t++)
                        {
                            SerializedProperty themeItem = themes.GetArrayElementAtIndex(t);
                            string themeLabel = BuildThemeTitle(dimensions.intValue, t);
                            EditorGUILayout.PropertyField(themeItem, new GUIContent(themeLabel, "Theme properties for interaction feedback"));

                            if (themeItem.objectReferenceValue != null && gameObject.objectReferenceValue)
                            {
                                RenderDefaultThemeWarning(profileItem, themeItem);

                                SerializedProperty hadDefault = profileItem.FindPropertyRelative("HadDefaultTheme");
                                hadDefault.boolValue = true;

                                string prefKey = themeItem.objectReferenceValue.name + "Profiles" + i + "_Theme" + t + "_Edit";
                                bool showSettingsPref = EditorPrefs.GetBool(prefKey, true);
                                bool show = InspectorUIUtility.DrawSectionFoldout(themeItem.objectReferenceValue.name + " (Click to edit)", showSettingsPref, FontStyle.Normal);
                                if (show != showSettingsPref)
                                {
                                    EditorPrefs.SetBool(prefKey, show);
                                }

                                if (show)
                                {
                                    SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
                                    SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");

                                    GUILayout.Space(5);

                                    if (InspectorUIUtility.FlexButton(AddThemePropertyLabel))
                                    {
                                        AddThemeProperty(profileItem, themeItem);
                                    }

                                    State[] states = GetStates();

                                    themeObj.Update();
                                    ThemeInspector.RenderThemeSettings(themeObjSettings, themeOptions, gameObject, states, ThemePropertiesBoxMargin);
                                    ThemeInspector.RenderThemeStates(themeObjSettings, states, ThemePropertiesBoxMargin);
                                    themeObj.ApplyModifiedProperties();
                                }

                                validProfileCnt++;
                            }
                            else
                            {
                                // show message about profile setup
                                const string themeMsg = "Assign a Target and/or Theme above to add visual effects";
                                SerializedProperty hadDefault = profileItem.FindPropertyRelative("HadDefaultTheme");

                                if (!hadDefault.boolValue && t == 0)
                                {
                                    string[] themeLocations = AssetDatabase.FindAssets("DefaultTheme");
                                    if (themeLocations.Length > 0)
                                    {
                                        for (int j = 0; j < themeLocations.Length; j++)
                                        {
                                            string path = AssetDatabase.GUIDToAssetPath(themeLocations[0]);
                                            Theme defaultTheme = (Theme)AssetDatabase.LoadAssetAtPath(path, typeof(Theme));
                                            if (defaultTheme != null)
                                            {
                                                themeItem.objectReferenceValue = defaultTheme;
                                                break;
                                            }
                                        }

                                        if (themeItem.objectReferenceValue != null)
                                        {
                                            hadDefault.boolValue = true;
                                        }
                                    }
                                    else
                                    {
                                        InspectorUIUtility.DrawError("DefaultTheme missing from project!");
                                    }
                                }
                                InspectorUIUtility.DrawError(themeMsg);
                            }

                            themeCnt += themes.arraySize;
                        }
                    }
                }// profile for loop

                if (GUILayout.Button(new GUIContent("Add Profile")))
                {
                    AddProfile(profileList.arraySize);
                }
            }
            else
            {
                // make sure profiles are setup if closed by default
                for (int i = 0; i < profileList.arraySize; i++)
                {
                    SerializedProperty sItem = profileList.GetArrayElementAtIndex(i);
                    SerializedProperty gameObject = sItem.FindPropertyRelative("Target");
                    SerializedProperty themes = sItem.FindPropertyRelative("Themes");

                    if (gameObject.objectReferenceValue != null)
                    {
                        validProfileCnt++;
                    }

                    for (int t = 0; t < themes.arraySize; t++)
                    {
                        SerializedProperty themeItem = themes.GetArrayElementAtIndex(themes.arraySize - 1);
                        if (themeItem.objectReferenceValue != null && gameObject.objectReferenceValue)
                        {
                            validProfileCnt++;
                            SerializedProperty hadDefault = sItem.FindPropertyRelative("HadDefaultTheme");
                            hadDefault.boolValue = true;
                        }
                    }

                    themeCnt += themes.arraySize;
                }
            }

            ProfilesSetup = validProfileCnt == profileList.arraySize + themeCnt;
        }

        private void RenderDefaultThemeWarning(SerializedProperty profileItem, SerializedProperty themeItem)
        {
            if (themeItem.objectReferenceValue.name == "DefaultTheme")
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    InspectorUIUtility.DrawWarning("DefaultTheme should not be edited.  ");

                    if (InspectorUIUtility.FlexButton(CreateThemeLabel))
                    {
                        CreateTheme(profileItem, themeItem);
                    }
                }
            }
        }

        private void RenderEventSettings()
        {
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            bool isEventsOpen = InspectorUIUtility.DrawSectionFoldout("Events", showEvents, FontStyle.Bold, InspectorUIUtility.TitleFontSize);
            if (showEvents != isEventsOpen)
            {
                showEvents = isEventsOpen;
                EditorPrefs.SetBool(ShowEventsPrefKey, showEvents);
            }

            EditorGUILayout.Space();

            if (showEvents)
            {
                SerializedProperty onClick = serializedObject.FindProperty("OnClick");
                EditorGUILayout.PropertyField(onClick, new GUIContent("OnClick"));

                SerializedProperty events = serializedObject.FindProperty("Events");
                GUI.enabled = !isPlayMode;
                for (int i = 0; i < events.arraySize; i++)
                {
                    SerializedProperty eventItem = events.GetArrayElementAtIndex(i);
                    if (InteractableReceiverListInspector.RenderEventSettings(eventItem, i, eventOptions, ChangeEvent, RemoveEvent))
                    {
                        // If removed, skip rendering rest of list till next redraw
                        break;
                    }
                }
                GUI.enabled = true;

                if (eventOptions.ClassNames.Length > 1)
                {
                    if (GUILayout.Button(new GUIContent("Add Event")))
                    {
                        AddEvent(events.arraySize);
                    }
                }
            }
        }

        protected void RenderGeneralSettings()
        {
            Rect position;
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;

            using (new EditorGUILayout.HorizontalScope())
            {
                InspectorUIUtility.DrawTitle("General");

                if (target != null)
                {
                    var helpURL = target.GetType().GetCustomAttribute<HelpURLAttribute>();
                    if (helpURL != null)
                    {
                        InspectorUIUtility.RenderDocumentationButton(helpURL.URL);
                    }
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // States
            // If states value is not provided, try to use Default states type
            if (statesProperty.objectReferenceValue == null)
            {
                statesProperty.objectReferenceValue = ThemeInspector.GetDefaultInteractableStates();
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
            EditorGUI.BeginProperty(position, SpeechComamndsLabel, voiceCommands);
            {
                currentIndex = EditorGUI.Popup(position, SpeechComamndsLabel.text, currentIndex, keywordOptions);
                if (validSpeechKeywords)
                {
                    voiceCommands.stringValue = currentIndex > 0 ? speechKeywordOptions[currentIndex] : string.Empty;
                }
            }
            EditorGUI.EndProperty();
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
            EditorGUI.BeginProperty(position, selectionModeLabel, dimensions);
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
            EditorGUI.EndProperty();

            if (dimensions.intValue > 1)
            {
                // toggle or multi dimensional button
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(canSelect, new GUIContent("Can Select", "The user can toggle this button"));
                    EditorGUILayout.PropertyField(canDeselect, new GUIContent("Can Deselect", "The user can untoggle this button, set false for a radial interaction."));

                    position = EditorGUILayout.GetControlRect();
                    EditorGUI.BeginProperty(position, startDimensionLabel, startDimensionIndex);
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
                    EditorGUI.EndProperty();
                }
            }
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private static void ValidateThemes(SerializedProperty dimensions, SerializedProperty themes)
        {
            // make sure there are enough themes as dimensions
            if (themes.arraySize > dimensions.intValue)
            {
                // make sure there are not more themes than dimensions
                int cnt = themes.arraySize - 1;
                for (int j = cnt; j > dimensions.intValue - 1; j--)
                {
                    themes.DeleteArrayElementAtIndex(j);
                }
            }

            // add themes when increasing dimensions
            if (themes.arraySize < dimensions.intValue)
            {
                int cnt = themes.arraySize;
                for (int j = cnt; j < dimensions.intValue; j++)
                {
                    themes.InsertArrayElementAtIndex(themes.arraySize);
                    SerializedProperty theme = themes.GetArrayElementAtIndex(themes.arraySize - 1);

                    string[] themeLocations = AssetDatabase.FindAssets("DefaultTheme");
                    if (themeLocations.Length > 0)
                    {
                        for (int k = 0; k < themeLocations.Length; k++)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(themeLocations[k]);
                            Theme defaultTheme = (Theme)AssetDatabase.LoadAssetAtPath(path, typeof(Theme));
                            if (defaultTheme != null)
                            {
                                theme.objectReferenceValue = defaultTheme;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static string BuildThemeTitle(int dimensions, int themeIndex)
        {
            if (dimensions == 2)
            {
                return "Theme " + (themeIndex % 2 == 0 ? "(Deselected)" : "(Selected)");
            }
            else if (dimensions > 3)
            {
                return "Theme " + (themeIndex + 1);
            }

            return "Theme";
        }

        #region Profiles
        /*
         * PROFILES
         */
        protected void AddProfile(int index)
        {
            profileList.InsertArrayElementAtIndex(profileList.arraySize);
            SerializedProperty newItem = profileList.GetArrayElementAtIndex(profileList.arraySize - 1);

            SerializedProperty newTarget = newItem.FindPropertyRelative("Target");
            SerializedProperty themes = newItem.FindPropertyRelative("Themes");
            newTarget.objectReferenceValue = null;

            themes.ClearArray();
        }

        protected void RemoveProfile(int index, SerializedProperty prop = null)
        {
            profileList.DeleteArrayElementAtIndex(index);
        }

        #endregion Profiles

        #region Themes
        /*
         * THEMES
         */

        protected void SetupThemeOptions()
        {
            themeOptions = InteractableProfileItem.GetThemeTypes();
        }

        protected virtual void AddThemeProperty(SerializedProperty profileItem, SerializedProperty themeItem)
        {
            SerializedProperty serializedTarget = profileItem.FindPropertyRelative("Target");

            SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
            themeObj.Update();

            SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
            themeObjSettings.InsertArrayElementAtIndex(themeObjSettings.arraySize);

            SerializedProperty settingsItem = themeObjSettings.GetArrayElementAtIndex(themeObjSettings.arraySize - 1);
            SerializedProperty className = settingsItem.FindPropertyRelative("Name");
            SerializedProperty assemblyQualifiedName = settingsItem.FindPropertyRelative("AssemblyQualifiedName");
            if (themeObjSettings.arraySize == 1)
            {
                className.stringValue = "ScaleOffsetColorTheme";
                assemblyQualifiedName.stringValue = typeof(ScaleOffsetColorTheme).AssemblyQualifiedName;
            }
            else
            {
                className.stringValue = themeOptions.ClassNames[0];
                assemblyQualifiedName.stringValue = themeOptions.AssemblyQualifiedNames[0];
            }

            SerializedProperty easing = settingsItem.FindPropertyRelative("Easing");

            SerializedProperty time = easing.FindPropertyRelative("LerpTime");
            SerializedProperty curve = easing.FindPropertyRelative("Curve");
            time.floatValue = 0.5f;
            curve.animationCurveValue = AnimationCurve.Linear(0, 1, 1, 1);

            themeObjSettings = ThemeInspector.ChangeThemeProperty(themeObjSettings.arraySize - 1, themeObjSettings, serializedTarget, GetStates(), true);

            themeObj.ApplyModifiedProperties();
        }

        protected virtual void RemoveThemeProperty(int[] arr)
        {
            int profile = arr[0];
            int theme = arr[1];
            int index = arr[2];

            SerializedProperty sItem = profileList.GetArrayElementAtIndex(profile);
            SerializedProperty themes = sItem.FindPropertyRelative("Themes");

            SerializedProperty themeItem = themes.GetArrayElementAtIndex(theme);
            SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
            themeObj.Update();

            SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
            themeObjSettings.DeleteArrayElementAtIndex(index);

            themeObj.ApplyModifiedProperties();
        }

        protected virtual SerializedObject ChangeThemeProperty(int index, SerializedObject themeObj, SerializedProperty target, bool isNew = false)
        {
            SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
            themeObjSettings = ThemeInspector.ChangeThemeProperty(index, themeObjSettings, target, GetStates(), isNew);
            return themeObj;
        }

        protected void CreateTheme(SerializedProperty profileItem, SerializedProperty themeItem)
        {
            SerializedProperty gameObject = profileItem.FindPropertyRelative("Target");

            GameObject host = gameObject.objectReferenceValue as GameObject;
            string path = "Assets/Themes";

            if (host != null)
            {
                string themeName = host.name + "Theme.asset";

                path = EditorUtility.SaveFilePanelInProject(
                   "Save New Theme",
                   themeName,
                   "asset",
                   "Create a name and select a location for this theme");

                if (path.Length != 0)
                {
                    Theme newTheme = ScriptableObject.CreateInstance<Theme>();
                    AssetDatabase.CreateAsset(newTheme, path);
                    themeItem.objectReferenceValue = newTheme;
                }
            }
        }

        protected virtual State[] GetStates()
        {
            return instance.GetStates();
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

        protected void ChangeEvent(int[] indexArray, SerializedProperty prop = null)
        {
            SerializedProperty className = prop.FindPropertyRelative("ClassName");
            SerializedProperty name = prop.FindPropertyRelative("Name");
            SerializedProperty settings = prop.FindPropertyRelative("Settings");
            SerializedProperty hideEvents = prop.FindPropertyRelative("HideUnityEvents");
            SerializedProperty assemblyQualifiedName = prop.FindPropertyRelative("AssemblyQualifiedName");

            if (!String.IsNullOrEmpty(className.stringValue))
            {
                InteractableEvent.ReceiverData data = eventList[indexArray[0]].AddReceiver(eventOptions.Types[indexArray[1]]);
                name.stringValue = data.Name;
                hideEvents.boolValue = data.HideUnityEvents;
                assemblyQualifiedName.stringValue = eventOptions.AssemblyQualifiedNames[indexArray[1]];

                InspectorFieldsUtility.PropertySettingsList(settings, data.Fields);
            }
        }

        protected void SetupEventOptions()
        {
            eventOptions = InteractableEvent.GetEventTypes();
        }

        protected string[] GetEventList()
        {
            return new string[] { };
        }

        #endregion Events

        #region PopupUtilities
        /// <summary>
        /// Get the index of the speech keyword array item based on its name, pop-up field helper
        /// Skips the first item in the array (internal added blank value to turn feature off)
        /// and returns a 0 if no match is found for the blank value
        /// </summary>
        /// <param name="option"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <param name="position"></param>
        /// <param name="prop"></param>
        /// <param name="options"></param>
        /// <param name="label"></param>
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
