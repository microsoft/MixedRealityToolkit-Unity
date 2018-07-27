// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers
{
    [CustomEditor(typeof(ControllerPoseSynchronizer))]
    public class ControllerPoseSynchronizerInspector : Editor
    {
        private static readonly string[] HandednessLabels = { "Left", "Right" };

        private SerializedProperty handedness;
        private SerializedProperty disableChildren;

        private void OnEnable()
        {
            handedness = serializedObject.FindProperty("handedness");
            disableChildren = serializedObject.FindProperty("disableChildren");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            var currentHandedness = (Handedness)handedness.enumValueIndex;
            var handIndex = currentHandedness == Handedness.Right ? 1 : 0;

            EditorGUI.BeginChangeCheck();
            var newHandednessIndex = EditorGUILayout.Popup(handedness.displayName, handIndex, HandednessLabels);

            if (EditorGUI.EndChangeCheck())
            {
                currentHandedness = newHandednessIndex == 0 ? Handedness.Left : Handedness.Right;
                handedness.enumValueIndex = (int)currentHandedness;
            }

            EditorGUILayout.PropertyField(disableChildren);
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}