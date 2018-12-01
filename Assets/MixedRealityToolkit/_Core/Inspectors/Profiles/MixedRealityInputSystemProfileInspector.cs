// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty focusProviderType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty gesturesProfile;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty controllerDataProvidersProfile;
        private SerializedProperty controllerMappingProfiles;
        private SerializedProperty controllerVisualizationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityToolkit.ConfirmInitialized())
            {
                return;
            }

            if (!MixedRealityToolkit.HasActiveProfile)
            {
                return;
            }

            focusProviderType = serializedObject.FindProperty("focusProviderType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerDataProvidersProfile = serializedObject.FindProperty("controllerDataProvidersProfile");
            controllerMappingProfiles = serializedObject.FindProperty("controllerMappingProfiles");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);

            CheckProfileLock(target);

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;

            serializedObject.Update();
            bool changed = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(focusProviderType);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            changed |= RenderProfile(inputActionsProfile);
            changed |= RenderProfile(inputActionRulesProfile);
            changed |= RenderProfile(pointerProfile);
            changed |= RenderProfile(gesturesProfile);
            changed |= RenderProfile(speechCommandsProfile);
            changed |= RenderProfile(controllerDataProvidersProfile);
            changed |= RenderProfile(controllerMappingProfiles);
            changed |= RenderProfile(controllerVisualizationProfile);

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}