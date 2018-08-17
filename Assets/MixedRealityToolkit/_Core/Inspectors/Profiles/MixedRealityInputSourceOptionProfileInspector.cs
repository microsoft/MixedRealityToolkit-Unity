// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityInputSourceOptionsProfile))]
    public class MixedRealityInputSourceOptionProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty hypothesisAction;
        private SerializedProperty resultAction;
        private SerializedProperty completeAction;
        private SerializedProperty errorAction;
        private SerializedProperty pointerAction;
        private SerializedProperty holdAction;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            hypothesisAction = serializedObject.FindProperty("hypothesisAction");
            resultAction = serializedObject.FindProperty("resultAction");
            completeAction = serializedObject.FindProperty("completeAction");
            errorAction = serializedObject.FindProperty("errorAction");
            pointerAction = serializedObject.FindProperty("pointerAction");
            holdAction = serializedObject.FindProperty("holdAction");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Input Source Options", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("These input sources do not have controller definitions, but each input source raises events when specific criteria are met. These optional input actions could help determine additional logic to take when the event is raised.", MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dictation Input Source Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(hypothesisAction);
            EditorGUILayout.PropertyField(resultAction);
            EditorGUILayout.PropertyField(completeAction);
            EditorGUILayout.PropertyField(errorAction);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Touch Screen Input Source Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(pointerAction);
            EditorGUILayout.PropertyField(holdAction);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}