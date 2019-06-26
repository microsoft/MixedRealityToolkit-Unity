// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Interactable))]
    public class InteractableInspector : UnityEditor.Editor
    {
        protected Interactable instance;
        protected List<InteractableEvent> eventList;
        protected SerializedProperty profileList;
        protected static bool showProfiles;
        protected string prefKey = "InteractableInspectorProfiles";
        protected bool enabled = false;

        protected InteractableTypesContainer eventOptions;
        protected InteractableTypesContainer themeOptions;
        protected string[] shaderOptions;

        protected string[] actionOptions = null;
        protected GUIContent[] speechKeywords = null;
        protected static bool ProfilesSetup = false;

        // indent tracker
        protected static int indentOnSectionStart = 0;

        protected List<InspectorUIUtility.ListSettings> listSettings;
        protected GUIStyle boxStyle;
        private SerializedProperty tempSettings;
        
        protected virtual void OnEnable()
        {
            instance = (Interactable)target;
            eventList = instance.Events;

            profileList = serializedObject.FindProperty("Profiles");
            listSettings = InspectorUIUtility.AdjustListSettings(null, profileList.arraySize);
            showProfiles = EditorPrefs.GetBool(prefKey, showProfiles);

            SetupEventOptions();
            SetupThemeOptions();

            enabled = true;
        }

        #region OnInspector

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
            if ((actionOptions == null && !Interactable.TryGetInputActions(out actionOptions)) || (speechKeywords == null && !Interactable.TryGetSpeechKeywords(out speechKeywords)))
            {
                EditorGUILayout.HelpBox("Mixed Reality Toolkit is missing, configure it by invoking the 'Mixed Reality Toolkit > Add to Scene and Configure...' menu", MessageType.Error);
            }
            
            //RenderBaseInspector()
            RenderCustomInspector();
        }

        public virtual void RenderCustomInspector()
        {
            serializedObject.Update();

            Rect position;

            EditorGUILayout.Space();
            InspectorUIUtility.DrawTitle("Interactable");

            EditorGUILayout.BeginVertical("Box");
            bool isPlayMode = EditorApplication.isPlaying || EditorApplication.isPaused;

            // States
            bool showStates = false;
            SerializedProperty states = serializedObject.FindProperty("States");
            bool drawerStarted = false;
            if (states.objectReferenceValue != null)
            {
                string statesPrefKey = "Settings_States";
                bool prefsShowStates = EditorPrefs.GetBool(statesPrefKey);
                EditorGUI.indentLevel = indentOnSectionStart + 1;
                showStates = InspectorUIUtility.DrawSectionStart(states.objectReferenceValue.name + " (Click to edit)", indentOnSectionStart + 2, prefsShowStates, FontStyle.Normal, false);
                drawerStarted = true;
                if (showStates != prefsShowStates)
                {
                    EditorPrefs.SetBool(statesPrefKey, showStates);
                }
            }
            else
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
                            states.objectReferenceValue = defaultStates;
                            break;
                        }
                    }
                }
                else
                {
                    showStates = true;
                }
            }

            if (showStates)
            {
                GUI.enabled = !isPlayMode;
                EditorGUILayout.PropertyField(states, new GUIContent("States", "The States this Interactable is based on"));
                GUI.enabled = true;
            }

            if (drawerStarted)
            {
                InspectorUIUtility.DrawSectionEnd(indentOnSectionStart);
            }

            if (states.objectReferenceValue == null)
            {
                InspectorUIUtility.DrawError("Please assign a States object!");
                EditorGUILayout.EndVertical();
                serializedObject.ApplyModifiedProperties();
                return;
            }

            //standard Interactable Object UI
            SerializedProperty enabled = serializedObject.FindProperty("Enabled");
            EditorGUILayout.PropertyField(enabled, new GUIContent("Enabled", "Is this Interactable Enabled?"));

            SerializedProperty actionId = serializedObject.FindProperty("InputActionId");

            if (actionOptions == null)
            {
                GUI.enabled = false;
                EditorGUILayout.Popup("Input Actions", 0, new string[] { "Missing Mixed Reality Toolkit" });
                GUI.enabled = true;
            }
            else
            {
                position = EditorGUILayout.GetControlRect();
                DrawDropDownProperty(position, actionId, actionOptions, new GUIContent("Input Actions", "The input action filter"));
            }

            EditorGUI.indentLevel = indentOnSectionStart + 1;												 
            SerializedProperty isGlobal = serializedObject.FindProperty("IsGlobal");
            EditorGUILayout.PropertyField(isGlobal, new GUIContent("Is Global", "Like a modal, does not require focus"));
            EditorGUI.indentLevel = indentOnSectionStart;
            SerializedProperty voiceCommands = serializedObject.FindProperty("VoiceCommand");

            // check speech commands profile for a list of commands
            if (speechKeywords == null)
            {
                GUI.enabled = false;
                EditorGUILayout.Popup("Speech Command", 0, new string[] { "Missing Speech Commands" });
                InspectorUIUtility.DrawNotice("Create speech commands in the MRTK/Input/Speech Commands Profile");
                GUI.enabled = true;
            }
            else
            {
                //look for items in the sppech commands list that match the voiceCommands string
                // this string should be empty if we are not listening to speech commands
                // will return zero if empty, to match the inserted off value.
                int currentIndex = SpeechKeywordLookup(voiceCommands.stringValue, speechKeywords);
                GUI.enabled = !isPlayMode;
                position = EditorGUILayout.GetControlRect();
                GUIContent label = new GUIContent("Speech Command", "Speech Commands to use with Interactable, pulled from MRTK/Input/Speech Commands Profile");
                EditorGUI.BeginProperty(position, label, voiceCommands);
                {
                    currentIndex = EditorGUI.Popup(position, label, currentIndex, speechKeywords);

                    if (currentIndex > 0)
                    {
                        voiceCommands.stringValue = speechKeywords[currentIndex].text;
                    }
                    else
                    {
                        voiceCommands.stringValue = "";
                    }
                }
                EditorGUI.EndProperty();
                GUI.enabled = true;
            }
            
            // show requires gaze because voice command has a value
            if (!string.IsNullOrEmpty(voiceCommands.stringValue))
            {
                EditorGUI.indentLevel = indentOnSectionStart + 1;

                SerializedProperty requireGaze = serializedObject.FindProperty("RequiresFocus");
                EditorGUILayout.PropertyField(requireGaze, new GUIContent("Requires Focus", "Does the voice command require gazing at this interactable?"));

                EditorGUI.indentLevel = indentOnSectionStart;
            }
            
            SerializedProperty dimensions = serializedObject.FindProperty("Dimensions");
            // should be 1 or more
            dimensions.intValue = Mathf.Clamp(dimensions.intValue, 1, 9);
            int minMultiDimensionValue = 3;

            // user-friendly dimension settings
            string[] EnumNames = Enum.GetNames(typeof(Interactable.SelectionTypes));
            int selectionTypeLength = EnumNames.Length;
            int selectionTypeIndex = Mathf.Clamp(dimensions.intValue, 1, selectionTypeLength) - 1;

            Interactable.SelectionTypes selectionType = Interactable.SelectionTypes.Button;
            position = EditorGUILayout.GetControlRect();
            GUIContent selectionTypeLabel = new GUIContent("Selection Mode", "How the Interactable should react to input");
            GUI.enabled = !isPlayMode;
            EditorGUI.BeginProperty(position, selectionTypeLabel, dimensions);
            {
                selectionType = (Interactable.SelectionTypes)EditorGUI.EnumPopup(position, selectionTypeLabel, (Interactable.SelectionTypes)(selectionTypeIndex));
                selectionTypeIndex = (int)selectionType;

                if (selectionTypeIndex >= minMultiDimensionValue - 1)
                {
                    // multi dimension mode
                    if (dimensions.intValue < minMultiDimensionValue)
                    {
                        dimensions.intValue = minMultiDimensionValue;
                    }

                    EditorGUI.indentLevel = indentOnSectionStart + 1;
                    position = EditorGUILayout.GetControlRect();
                    selectionTypeLabel = new GUIContent("Dimensions", "The amount of theme layers for sequence button functionality (3-9)");
                    dimensions.intValue = EditorGUI.IntField(position, selectionTypeLabel, dimensions.intValue);
                }
                else
                {
                    // button or toggle
                    dimensions.intValue = selectionTypeIndex + 1;
                }
            }
            EditorGUI.EndProperty();
            GUI.enabled = true;

            if (dimensions.intValue > 1)
            {
                // toggle or multi dimensional button
                EditorGUI.indentLevel = indentOnSectionStart + 1;

                SerializedProperty canSelect = serializedObject.FindProperty("CanSelect");
                SerializedProperty canDeselect = serializedObject.FindProperty("CanDeselect");
                SerializedProperty startDimensionIndex = serializedObject.FindProperty("StartDimensionIndex");

                EditorGUILayout.PropertyField(canSelect, new GUIContent("Can Select", "The user can toggle this button"));
                EditorGUILayout.PropertyField(canDeselect, new GUIContent("Can Deselect", "The user can untoggle this button, set false for a radial interaction."));

                position = EditorGUILayout.GetControlRect();
                bool appPlaying = EditorApplication.isPlaying || EditorApplication.isPaused;
                GUIContent startDimensionLabel = new GUIContent("Start Dimension Index", "The dimension value to set on start.");
                EditorGUI.BeginProperty(position, startDimensionLabel, startDimensionIndex);
                {
                    if (dimensions.intValue >= minMultiDimensionValue)
                    {
                        // multi dimensions
                        GUI.enabled = !appPlaying;
                        EditorGUI.IntField(position, startDimensionLabel, startDimensionIndex.intValue);
                        GUI.enabled = true;
                    }
                    else if (dimensions.intValue == 2)
                    {
                        // toggle
                        startDimensionLabel = new GUIContent("Is Toggled", "The toggled value to set on start.");
                        if (!appPlaying)
                        {
                            bool isToggled = EditorGUI.Toggle(position, startDimensionLabel, startDimensionIndex.intValue > 0);
                            startDimensionIndex.intValue = isToggled ? 1 : 0;
                        }
                        else
                        {
                            SerializedProperty dimensionIndex = serializedObject.FindProperty("dimensionIndex");
                            GUI.enabled = false;
                            bool isToggled = EditorGUI.Toggle(position, startDimensionLabel, dimensionIndex.intValue > 0);
                            GUI.enabled = true;
                        }
                    }

                    startDimensionIndex.intValue = Mathf.Clamp(startDimensionIndex.intValue, 0, dimensions.intValue - 1);
                }
                EditorGUI.EndProperty();
                EditorGUI.indentLevel = indentOnSectionStart;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            InspectorUIUtility.DrawDivider();

            if (!ProfilesSetup && !showProfiles)
            {
                InspectorUIUtility.DrawWarning("Profiles (Optional) have not been set up or has errors.");
            }

            // profiles section
            string profileTitle = "Profiles";
            bool isOPen = InspectorUIUtility.DrawSectionStart(profileTitle, indentOnSectionStart + 1, showProfiles, InspectorUIUtility.LableStyle(InspectorUIUtility.TitleFontSize, InspectorUIUtility.ColorTint50).fontStyle, false, InspectorUIUtility.TitleFontSize);

            if (showProfiles != isOPen)
            {
                showProfiles = isOPen;
                EditorPrefs.SetBool(prefKey, showProfiles);
            }

            if (profileList.arraySize < 1)
            {
                AddProfile(0);
            }

            int validProfileCnt = 0;
            int themeCnt = 0;

            if (showProfiles)
            {
                for (int i = 0; i < profileList.arraySize; i++)
                {
                    EditorGUILayout.BeginVertical("Box");
                    // get profiles
                    SerializedProperty sItem = profileList.GetArrayElementAtIndex(i);
                    EditorGUI.indentLevel = indentOnSectionStart;

                    SerializedProperty gameObject = sItem.FindPropertyRelative("Target");
                    string targetName = "Profile " + (i + 1);
                    if (gameObject.objectReferenceValue != null)
                    {
                        targetName = gameObject.objectReferenceValue.name;
                        validProfileCnt++;
                    }

                    EditorGUILayout.BeginHorizontal();
                    InspectorUIUtility.DrawLabel(targetName, 12, InspectorUIUtility.ColorTint100);

                    bool triggered = InspectorUIUtility.SmallButton(new GUIContent(InspectorUIUtility.Minus, "Remove Profile"), i, RemoveProfile);

                    if (triggered)
                    {
                        continue;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel = indentOnSectionStart + 1;
                    EditorGUILayout.PropertyField(gameObject, new GUIContent("Target", "Target gameObject for this theme properties to manipulate"));

                    // get themes
                    SerializedProperty themes = sItem.FindPropertyRelative("Themes");

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

                    for (int t = 0; t < themes.arraySize; t++)
                    {
                        SerializedProperty themeItem = themes.GetArrayElementAtIndex(t);
                        EditorGUI.indentLevel = indentOnSectionStart + 2;
                        string themeLabel = "Theme";
                        if (dimensions.intValue > 1)
                        {
                            themeLabel = "Theme " + (t + 1);

                            if (dimensions.intValue == 2 && t > 0)
                            {
                                themeLabel += " (Toggled)";
                            }
                        }
                        EditorGUILayout.PropertyField(themeItem, new GUIContent(themeLabel, "Theme properties for interaction feedback"));

                        if (themeItem.objectReferenceValue != null && gameObject.objectReferenceValue)
                        {
                            if (themeItem.objectReferenceValue.name == "DefaultTheme")
                            {
                                EditorGUILayout.BeginHorizontal();
                                InspectorUIUtility.DrawWarning("DefaultTheme should not be edited.  ");
                                bool newTheme = InspectorUIUtility.FlexButton(new GUIContent("Create Theme", "Create a new theme"), new int[] { i, t, 0 }, CreateTheme);
                                if (newTheme)
                                {
                                    continue;
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            SerializedProperty hadDefault = sItem.FindPropertyRelative("HadDefaultTheme");
                            hadDefault.boolValue = true;
                            EditorGUI.indentLevel = indentOnSectionStart + 3;

                            string prefKey = themeItem.objectReferenceValue.name + "Profiles" + i + "_Theme" + t + "_Edit";
                            bool hasPref = EditorPrefs.HasKey(prefKey);
                            bool showSettings = EditorPrefs.GetBool(prefKey);
                            if (!hasPref)
                            {
                                showSettings = true;
                            }

                            InspectorUIUtility.ListSettings settings = listSettings[i];
                            bool show = InspectorUIUtility.DrawSectionStart(themeItem.objectReferenceValue.name + " (Click to edit)", indentOnSectionStart + 3, showSettings, FontStyle.Normal, false);

                            if (show != showSettings)
                            {
                                EditorPrefs.SetBool(prefKey, show);
                                settings.Show = show;
                            }

                            if (show)
                            {
                                SerializedObject themeObj = new SerializedObject(themeItem.objectReferenceValue);
                                SerializedProperty themeObjSettings = themeObj.FindProperty("Settings");
                                themeObj.Update();

                                GUILayout.Space(5);

                                if (themeObjSettings.arraySize < 1)
                                {
                                    AddThemeProperty(new int[] { i, t, 0 });
                                }

                                int[] location = new int[] { i, t, 0 };
                                State[] iStates = GetStates();

                                ThemeInspector.RenderThemeSettings(themeObjSettings, themeObj, themeOptions, gameObject, location, iStates);
                                InspectorUIUtility.FlexButton(new GUIContent("+", "Add Theme Property"), location, AddThemeProperty);
                                ThemeInspector.RenderThemeStates(themeObjSettings, iStates, 30);

                                themeObj.ApplyModifiedProperties();
                            }

                            InspectorUIUtility.DrawSectionEnd(indentOnSectionStart + 2);
                            listSettings[i] = settings;

                            validProfileCnt++;
                        }
                        else
                        {
                            // show message about profile setup
                            string themeMsg = "Assign a ";
                            if (gameObject.objectReferenceValue == null)
                            {
                                themeMsg += "Target ";
                            }

                            if (themeItem.objectReferenceValue == null)
                            {
                                if (gameObject.objectReferenceValue == null)
                                {
                                    themeMsg += "and ";
                                }
                                themeMsg += "Theme ";
                            }

                            themeMsg += "above to add visual effects";
                            SerializedProperty hadDefault = sItem.FindPropertyRelative("HadDefaultTheme");

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
                    }

                    EditorGUI.indentLevel = indentOnSectionStart;
                    EditorGUILayout.EndVertical();
                    themeCnt += themes.arraySize;
                }

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

            InspectorUIUtility.DrawSectionEnd(indentOnSectionStart);
            EditorGUILayout.Space();
            InspectorUIUtility.DrawDivider();

            // Events section
            InspectorUIUtility.DrawTitle("Events");

            SerializedProperty onClick = serializedObject.FindProperty("OnClick");
            EditorGUILayout.PropertyField(onClick, new GUIContent("OnClick"));

            SerializedProperty events = serializedObject.FindProperty("Events");
            GUI.enabled = !isPlayMode;
            for (int i = 0; i < events.arraySize; i++)
            {
                SerializedProperty eventItem = events.GetArrayElementAtIndex(i);
                InteractableReceiverListInspector.RenderEventSettings(eventItem, i, eventOptions, ChangeEvent, RemoveEvent);
            }
            GUI.enabled = true;

            if (eventOptions.ClassNames.Length > 1)
            {
                if (GUILayout.Button(new GUIContent("Add Event")))
                {
                    AddEvent(events.arraySize);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        #endregion OnInspector
        
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

            listSettings.Add(new InspectorUIUtility.ListSettings() { Show = false, Scroll = new Vector2() });
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

        protected virtual void AddThemeProperty(int[] arr, SerializedProperty prop = null)
        {
            int profile = arr[0];
            int theme = arr[1];

            SerializedProperty sItem = profileList.GetArrayElementAtIndex(profile);
            SerializedProperty themes = sItem.FindPropertyRelative("Themes");
            SerializedProperty serializedTarget = sItem.FindPropertyRelative("Target");

            SerializedProperty themeItem = themes.GetArrayElementAtIndex(theme);
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

        protected void CreateTheme(int[] arr, SerializedProperty prop = null)
        {
            SerializedProperty sItem = profileList.GetArrayElementAtIndex(arr[0]);
            SerializedProperty themes = sItem.FindPropertyRelative("Themes");
            SerializedProperty themeItem = themes.GetArrayElementAtIndex(arr[1]);

            SerializedProperty gameObject = sItem.FindPropertyRelative("Target");

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
        protected int SpeechKeywordLookup(string option, GUIContent[] options)
        {
            // starting on 1 to skip the blank value
            for (int i = 1; i < options.Length; i++)
            {
                if (options[i].text == option)
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
#endif
}
