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
        private SerializedProperty inputActionsProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
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
            enableDictation = serializedObject.FindProperty("enableDictation");
            enableTouchScreenInput = serializedObject.FindProperty("enableTouchScreenInput");
            touchScreenInputProfile = serializedObject.FindProperty("touchScreenInputProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityManager())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile;
            }

            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
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