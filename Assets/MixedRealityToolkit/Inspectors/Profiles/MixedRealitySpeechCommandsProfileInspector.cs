// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealitySpeechCommandsProfile))]
    public class MixedRealitySpeechCommandsProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Speech Command");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Speech Command", "Add Speech Command");
        private static readonly GUIContent LocalizationContent = new GUIContent("LocalizationKey", "An optional key to lookup a localized value for keyword");
        private static readonly GUIContent KeywordContent = new GUIContent("Keyword", "Spoken word that will trigger the action.  Overriden by a localized version if LocalizationKey is specified and found");
        private static readonly GUIContent KeyCodeContent = new GUIContent("KeyCode", "The keyboard key that will trigger the action.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "The action to trigger when a keyboard key is pressed or keyword is recognized.");

        private static bool showGeneralProperties = true;
        private SerializedProperty recognizerStartBehaviour;
        private SerializedProperty recognitionConfidenceLevel;

        private static bool showSpeechCommands = true;
        private SerializedProperty speechCommands;
        private static GUIContent[] actionLabels;
        private static int[] actionIds;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null) { return; }

            recognizerStartBehaviour = serializedObject.FindProperty("startBehavior");
            recognitionConfidenceLevel = serializedObject.FindProperty("recognitionConfidenceLevel");

            speechCommands = serializedObject.FindProperty("speechCommands");
            actionLabels = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            actionIds = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

                DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile);

                return;
            }

            if (DrawBacktrackProfileButton("Back to Input Profile", MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile))
            {
                return;
            }

            CheckProfileLock(target);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Speech Commands", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Speech Commands are any/all spoken keywords your users will be able say to raise an Input Action in your application.", MessageType.Info);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            serializedObject.Update();

            EditorGUILayout.Space();
            showGeneralProperties = EditorGUILayout.Foldout(showGeneralProperties, "General Settings", true);
            if (showGeneralProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(recognizerStartBehaviour);
                    EditorGUILayout.PropertyField(recognitionConfidenceLevel);
                }
            }

            EditorGUILayout.Space();
            showSpeechCommands = EditorGUILayout.Foldout(showSpeechCommands, "Speech Commands", true);
            if (showSpeechCommands)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(speechCommands);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var speechCommand = list.GetArrayElementAtIndex(list.arraySize - 1);
                var localizationKey = speechCommand.FindPropertyRelative("localizationKey");
                localizationKey.stringValue = string.Empty;
                var keyword = speechCommand.FindPropertyRelative("keyword");
                keyword.stringValue = string.Empty;
                var keyCode = speechCommand.FindPropertyRelative("keyCode");
                keyCode.intValue = (int)KeyCode.None;
                var action = speechCommand.FindPropertyRelative("action");
                var actionId = action.FindPropertyRelative("id");
                actionId.intValue = 0;
            }

            GUILayout.Space(12f);

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Speech Command.", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 36f;
            EditorGUILayout.LabelField(LocalizationContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(KeywordContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(KeyCodeContent, GUILayout.Width(64f));
            EditorGUILayout.LabelField(ActionContent, GUILayout.Width(64f));
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
            EditorGUIUtility.labelWidth = labelWidth;
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SerializedProperty speechCommand = list.GetArrayElementAtIndex(i);
                var localizationKey = speechCommand.FindPropertyRelative("localizationKey");
                EditorGUILayout.PropertyField(localizationKey, GUIContent.none, GUILayout.ExpandWidth(true));
                var keyword = speechCommand.FindPropertyRelative("keyword");
                EditorGUILayout.PropertyField(keyword, GUIContent.none, GUILayout.ExpandWidth(true));
                var keyCode = speechCommand.FindPropertyRelative("keyCode");
                EditorGUILayout.PropertyField(keyCode, GUIContent.none, GUILayout.Width(64f));
                var action = speechCommand.FindPropertyRelative("action");
                var actionId = action.FindPropertyRelative("id");
                var actionDescription = action.FindPropertyRelative("description");
                var actionConstraint = action.FindPropertyRelative("axisConstraint");

                EditorGUI.BeginChangeCheck();
                actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, actionId.intValue, actionLabels, actionIds, GUILayout.Width(64f));

                if (EditorGUI.EndChangeCheck())
                {
                    MixedRealityInputAction inputAction = actionId.intValue == 0 ? MixedRealityInputAction.None : MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions[actionId.intValue - 1];
                    actionDescription.stringValue = inputAction.Description;
                    actionConstraint.enumValueIndex = (int)inputAction.AxisConstraint;
                }

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}
