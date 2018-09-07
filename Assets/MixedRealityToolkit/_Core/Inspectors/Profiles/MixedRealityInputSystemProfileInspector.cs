// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent SpeechConfidenceContent = new GUIContent("Recognition Confidence Level", "The speech recognizer's minimum confidence level setting that will raise the action.");
        private static readonly GUIContent[] SpeechConfidenceOptionContent =
        {
            new GUIContent("High"),
            new GUIContent("Medium"),
            new GUIContent("Low"),
            new GUIContent("Unrecognized")
        };

        private static readonly int[] SpeechConfidenceOptions = { 0, 1, 2, 3 };

        private SerializedProperty inputActionsProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty recognitionConfidenceLevel;
        private SerializedProperty enableDictation;
        private SerializedProperty enableTouchScreenInput;
        private SerializedProperty touchScreenInputProfile;
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;

        private void OnEnable()
        {
            if (!MixedRealityManager.ConfirmInitialized())
            {
                return;
            }

            if (!MixedRealityManager.HasActiveProfile)
            {
                return;
            }
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            enableSpeechCommands = serializedObject.FindProperty("enableSpeechCommands");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            recognitionConfidenceLevel = serializedObject.FindProperty("recognitionConfidenceLevel");
            enableDictation = serializedObject.FindProperty("enableDictation");
            enableTouchScreenInput = serializedObject.FindProperty("enableTouchScreenInput");
            touchScreenInputProfile = serializedObject.FindProperty("touchScreenInputProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            changed |= RenderProfile(inputActionsProfile);
            changed |= RenderProfile(pointerProfile);

            EditorGUILayout.PropertyField(enableSpeechCommands);
            changed |= RenderProfile(speechCommandsProfile);
            recognitionConfidenceLevel.intValue = EditorGUILayout.IntPopup(SpeechConfidenceContent, recognitionConfidenceLevel.intValue, SpeechConfidenceOptionContent, SpeechConfidenceOptions);

            EditorGUILayout.PropertyField(enableDictation);

            EditorGUILayout.PropertyField(enableTouchScreenInput);
            changed |= RenderProfile(touchScreenInputProfile);

            EditorGUILayout.PropertyField(enableControllerMapping);
            changed |= RenderProfile(controllerMappingProfile);

            if (!changed)
            {
                changed = EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityManager.Instance.ResetConfiguration(MixedRealityManager.Instance.ActiveProfile);
            }
        }
    }
}