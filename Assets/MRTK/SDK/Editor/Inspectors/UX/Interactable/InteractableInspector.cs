// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Editor
{
    [CustomEditor(typeof(Interactable))]
    [CanEditMultipleObjects]
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
        protected SerializedProperty resetOnDestroy;

        protected const string ShowProfilesPrefKey = "InteractableInspectorProfiles";
        protected const string ShowEventsPrefKey = "InteractableInspectorProfiles_ShowEvents";
        protected const string ShowEventReceiversPrefKey = "InteractableInspectorProfiles_ShowEvents_Receivers";
        protected bool enabled = false;

        protected string[] inputActionOptions = null;
        protected string[] speechKeywordOptions = null;

        private static readonly GUIContent InputActionsLabel = new GUIContent("Input Actions");
        private static readonly GUIContent selectionModeLabel = new GUIContent("Selection Mode", "The selection mode of the Interactable is based on the number of dimensions available.");
        private static readonly GUIContent dimensionsLabel = new GUIContent("Dimensions", "The amount of theme layers for sequence button functionality (3-9)");
        private static readonly GUIContent startDimensionLabel = new GUIContent("Start Dimension Index", "The dimensionIndex value to set on start.");
        private static readonly GUIContent isToggledLabel = new GUIContent("Is Toggled", "Should this Interactable be toggled on or off by default on start.");
        private static readonly GUIContent CreateThemeLabel = new GUIContent("Create and Assign New Theme", "Create a new theme");
        private static readonly GUIContent SpeechComamndsLabel = new GUIContent("Speech Command", "Speech Commands to use with Interactable, pulled from MRTK/Input/Speech Commands Profile");
        private static readonly GUIContent OnClickEventLabel = new GUIContent("OnClick", "Fired when this Interactable is triggered by a click.");
        private static readonly GUIContent AddEventReceiverLabel = new GUIContent("Add Event", "Add event receiver to this Interactable for special event handling.");
        private static readonly GUIContent VoiceRequiresFocusLabel = new GUIContent("Requires Focus");

        protected virtual void OnEnable()
        {
            instance = (Interactable)target;

            profileList = serializedObject.FindProperty("profiles");
            statesProperty = serializedObject.FindProperty("states");
            enabledProperty = serializedObject.FindProperty("enabledOnStart");
            voiceCommands = serializedObject.FindProperty("VoiceCommand");
            actionId = serializedObject.FindProperty("InputActionId");
            isGlobal = serializedObject.FindProperty("isGlobal");
            canSelect = serializedObject.FindProperty("CanSelect");
            canDeselect = serializedObject.FindProperty("CanDeselect");
            startDimensionIndex = serializedObject.FindProperty("startDimensionIndex");
            dimensionIndex = serializedObject.FindProperty("dimensionIndex");
            dimensions = serializedObject.FindProperty("Dimensions");
            resetOnDestroy = serializedObject.FindProperty("resetOnDestroy");

            enabled = true;
        }

        protected virtual void RenderBaseInspector()
        {
            base.OnInspectorGUI();
        }

        /// <remarks>
        /// <para>There is a check in here that verifies whether or not we can get InputActions, if we can't we show an error help box; otherwise we get them.
        /// This method is sealed, if you wish to override <see cref="OnInspectorGUI"/>, then override <see cref="RenderCustomInspector"/> method instead.</para>
        /// </remarks>
        public sealed override void OnInspectorGUI()
        {
            if ((inputActionOptions == null && !TryGetInputActions(out inputActionOptions))
                || (speechKeywordOptions == null && !TryGetSpeechKeywords(out speechKeywordOptions)))
            {
                EditorGUILayout.HelpBox("Mixed Reality Toolkit is missing, configure it by invoking the 'Mixed Reality Toolkit > Add to Scene and Configure...' menu", MessageType.Error);
            }

            RenderCustomInspector();
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            // Disable inspector UI if in play mode
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                RenderGeneralSettings();

                EditorGUILayout.Space();

                RenderProfileSettings();

                EditorGUILayout.Space();

                RenderEventSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderProfileSettings()
        {
            if (profileList.arraySize < 1)
            {
                AddProfile(0);
            }

            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Profiles", ShowProfilesPrefKey, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
            {
                EditorGUILayout.PropertyField(resetOnDestroy);

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
                                    showThemeSettings = InspectorUIUtility.DrawSectionFoldoutWithKey(themeLabel, prefKey, null, false);
                                    EditorGUILayout.PropertyField(themeItem, new GUIContent(string.Empty, "Theme properties for interaction feedback"));
                                }

                                if (themeItem.objectReferenceValue != null)
                                {
                                    // TODO: Odd bug where themeStates below is null when it shouldn't be. Use instance object as workaround atm
                                    // SerializedProperty themeStates = themeItem.FindPropertyRelative("States");
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
            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Events", ShowEventsPrefKey, MixedRealityStylesUtility.BoldTitleFoldoutStyle))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnClick"), OnClickEventLabel);

                    if (InspectorUIUtility.DrawSectionFoldoutWithKey("Receivers", ShowEventReceiversPrefKey, MixedRealityStylesUtility.TitleFoldoutStyle, false))
                    {
                        SerializedProperty events = serializedObject.FindProperty("Events");
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

                        if (GUILayout.Button(AddEventReceiverLabel))
                        {
                            AddEvent(events.arraySize);
                        }
                    }
                }
            }
        }

        protected void RenderGeneralSettings()
        {
            Rect position;
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

                EditorGUILayout.PropertyField(statesProperty, new GUIContent("States"));

                if (statesProperty.objectReferenceValue == null)
                {
                    InspectorUIUtility.DrawError("Please assign a States object!");
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                EditorGUILayout.PropertyField(enabledProperty, new GUIContent("Enabled"));

                // Input Actions
                bool validActionOptions = inputActionOptions != null;
                using (new EditorGUI.DisabledScope(!validActionOptions))
                {
                    var actionOptions = validActionOptions ? inputActionOptions : new string[] { "Missing Mixed Reality Toolkit" };
                    DrawDropDownProperty(EditorGUILayout.GetControlRect(), actionId, actionOptions, InputActionsLabel);
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(isGlobal, new GUIContent("Is Global"));
                }

                // Speech keywords
                bool validSpeechKeywords = speechKeywordOptions != null;
                using (new EditorGUI.DisabledScope(!validSpeechKeywords))
                {
                    string[] keywordOptions = validSpeechKeywords ? speechKeywordOptions : new string[] { "Missing Speech Commands" };
                    int currentIndex = validSpeechKeywords ? SpeechKeywordLookup(voiceCommands.stringValue, speechKeywordOptions) : 0;
                    position = EditorGUILayout.GetControlRect();

                    // BeginProperty allows tracking of serialized properties for bolding prefab changes etc
                    using (new EditorGUI.PropertyScope(position, SpeechComamndsLabel, voiceCommands))
                    {
                        currentIndex = EditorGUI.Popup(position, SpeechComamndsLabel.text, currentIndex, keywordOptions);
                        if (validSpeechKeywords)
                        {
                            voiceCommands.stringValue = currentIndex > 0 ? speechKeywordOptions[currentIndex] : string.Empty;
                        }
                    }
                }

                // show requires gaze because voice command has a value
                if (!string.IsNullOrEmpty(voiceCommands.stringValue))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        SerializedProperty requireGaze = serializedObject.FindProperty("voiceRequiresFocus");
                        EditorGUILayout.PropertyField(requireGaze, VoiceRequiresFocusLabel);
                    }
                }

                // should be 1 or more
                dimensions.intValue = Mathf.Clamp(dimensions.intValue, 1, 9);

                // user-friendly dimension settings
                SelectionModes selectionMode = SelectionModes.Button;
                position = EditorGUILayout.GetControlRect();
                using (new EditorGUI.PropertyScope(position, selectionModeLabel, dimensions))
                {
                    // Show enum popup for selection mode, hide option to select SelectionModes.Invalid
                    selectionMode = (SelectionModes)EditorGUI.EnumPopup(position, selectionModeLabel,
                        Interactable.ConvertToSelectionMode(dimensions.intValue),
                        (value) => { return (SelectionModes)value != SelectionModes.Invalid; });

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
                            var mode = Interactable.ConvertToSelectionMode(dimensions.intValue);
                            if (mode == SelectionModes.Toggle)
                            {
                                bool isToggled = EditorGUI.Toggle(position, isToggledLabel, startDimensionIndex.intValue > 0);
                                startDimensionIndex.intValue = isToggled ? 1 : 0;
                            }
                            else if (mode == SelectionModes.MultiDimension)
                            {
                                startDimensionIndex.intValue = EditorGUI.IntField(position, startDimensionLabel, startDimensionIndex.intValue);
                            }

                            startDimensionIndex.intValue = Mathf.Clamp(startDimensionIndex.intValue, 0, dimensions.intValue - 1);
                        }
                    }
                }
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
            string themeFileName = (string.IsNullOrEmpty(themeName) ? "New " : themeName) + "Theme.asset";

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
        #endregion KeywordUtilities

        #region Inspector Helpers

        /// <summary>
        /// Get a list of Mixed Reality Input Actions from the input actions profile.
        /// </summary>
        public static bool TryGetInputActions(out string[] descriptionsArray)
        {
            if (!MixedRealityToolkit.ConfirmInitialized() || !MixedRealityToolkit.Instance.HasActiveProfile)
            {
                descriptionsArray = null;
                return false;
            }

            MixedRealityInputAction[] actions = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions;

            descriptionsArray = new string[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                descriptionsArray[i] = actions[i].Description;
            }

            return true;
        }

        /// <summary>
        /// Try to get a list of speech commands from the MRTK/Input/SpeechCommands profile
        /// </summary>
        public static bool TryGetMixedRealitySpeechCommands(out SpeechCommands[] commands)
        {
            if (!MixedRealityToolkit.ConfirmInitialized() || !MixedRealityToolkit.Instance.HasActiveProfile)
            {
                commands = null;
                return false;
            }

            MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
            if (inputSystemProfile != null && inputSystemProfile.SpeechCommandsProfile != null)
            {
                commands = inputSystemProfile.SpeechCommandsProfile.SpeechCommands;
            }
            else
            {
                commands = null;
            }

            if (commands == null || commands.Length < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Look for speech commands in the MRTK Speech Command profile
        /// Adds a blank value at index zero so the developer can turn the feature off.
        /// </summary>
        public static bool TryGetSpeechKeywords(out string[] keywords)
        {
            SpeechCommands[] commands;
            if (!TryGetMixedRealitySpeechCommands(out commands))
            {
                keywords = null;
                return false;
            }

            List<string> keys = new List<string>
            {
                "(No Selection)"
            };

            for (var i = 0; i < commands.Length; i++)
            {
                keys.Add(commands[i].Keyword);
            }

            keywords = keys.ToArray();
            return true;
        }

        #endregion
    }
}
