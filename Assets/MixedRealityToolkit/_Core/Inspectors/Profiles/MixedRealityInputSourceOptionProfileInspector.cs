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
        private SerializedProperty pointerDownAction;
        private SerializedProperty pointerClickedAction;
        private SerializedProperty pointerUpAction;
        private SerializedProperty holdStartedAction;
        private SerializedProperty holdUpdatedAction;
        private SerializedProperty holdCompletedAction;
        private SerializedProperty holdCanceledAction;

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
            pointerDownAction = serializedObject.FindProperty("pointerDownAction");
            pointerClickedAction = serializedObject.FindProperty("pointerClickedAction");
            pointerUpAction = serializedObject.FindProperty("pointerUpAction");
            holdStartedAction = serializedObject.FindProperty("holdStartedAction");
            holdUpdatedAction = serializedObject.FindProperty("holdUpdatedAction");
            holdCompletedAction = serializedObject.FindProperty("holdCompletedAction");
            holdCanceledAction = serializedObject.FindProperty("holdCanceledAction");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Input Source Options", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Each input source raises events when specific criteria are met. These optional input actions could help determine additional logic to take when the event is raised.", MessageType.Info);

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

            EditorGUILayout.PropertyField(pointerDownAction);
            EditorGUILayout.PropertyField(pointerClickedAction);
            EditorGUILayout.PropertyField(pointerUpAction);
            EditorGUILayout.PropertyField(holdStartedAction);
            EditorGUILayout.PropertyField(holdUpdatedAction);
            EditorGUILayout.PropertyField(holdCompletedAction);
            EditorGUILayout.PropertyField(holdCanceledAction);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}