// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityInputActionRulesProfile))]
    public class MixedRealityInputActionRulesInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty inputActionRulesDigital;
        private SerializedProperty inputActionRulesSingleAxis;
        private SerializedProperty inputActionRulesDualAxis;
        private SerializedProperty inputActionRulesVectorAxis;
        private SerializedProperty inputActionRulesQuaternionAxis;
        private SerializedProperty inputActionRulesPoseAxis;

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

            inputActionRulesDigital = serializedObject.FindProperty("inputActionRulesDigital");
            inputActionRulesSingleAxis = serializedObject.FindProperty("inputActionRulesSingleAxis");
            inputActionRulesDualAxis = serializedObject.FindProperty("inputActionRulesDualAxis");
            inputActionRulesVectorAxis = serializedObject.FindProperty("inputActionRulesVectorAxis");
            inputActionRulesQuaternionAxis = serializedObject.FindProperty("inputActionRulesQuaternionAxis");
            inputActionRulesPoseAxis = serializedObject.FindProperty("inputActionRulesPoseAxis");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityManager())
            {
                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input Action Rules Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input Action Rules...", MessageType.Info);

            serializedObject.Update();
            EditorGUILayout.PropertyField(inputActionRulesDigital, true);
            EditorGUILayout.PropertyField(inputActionRulesSingleAxis, true);
            EditorGUILayout.PropertyField(inputActionRulesDualAxis, true);
            EditorGUILayout.PropertyField(inputActionRulesVectorAxis, true);
            EditorGUILayout.PropertyField(inputActionRulesQuaternionAxis, true);
            EditorGUILayout.PropertyField(inputActionRulesPoseAxis, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}