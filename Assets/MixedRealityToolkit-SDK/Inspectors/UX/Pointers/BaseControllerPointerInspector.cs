// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(BaseControllerPointer))]
    public class BaseControllerPointerInspector : ControllerPoseSynchronizerInspector
    {
        private SerializedProperty cursorPrefab;
        private SerializedProperty disableCursorOnStart;
        private SerializedProperty setCursorVisibilityOnSourceDetected;
        private SerializedProperty raycastOrigin;
        private SerializedProperty pointerExtent;
        private SerializedProperty activeHoldAction;
        private SerializedProperty pointerAction;
        private SerializedProperty pointerOrientation;
        private SerializedProperty requiresHoldAction;

        private bool basePointerFoldout = true;

        protected bool DrawBasePointerActions = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            cursorPrefab = serializedObject.FindProperty("cursorPrefab");
            disableCursorOnStart = serializedObject.FindProperty("disableCursorOnStart");
            setCursorVisibilityOnSourceDetected = serializedObject.FindProperty("setCursorVisibilityOnSourceDetected");
            raycastOrigin = serializedObject.FindProperty("raycastOrigin");
            pointerExtent = serializedObject.FindProperty("pointerExtent");
            activeHoldAction = serializedObject.FindProperty("activeHoldAction");
            pointerAction = serializedObject.FindProperty("pointerAction");
            pointerOrientation = serializedObject.FindProperty("pointerOrientation");
            requiresHoldAction = serializedObject.FindProperty("requiresHoldAction");

            DrawHandednessProperty = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            basePointerFoldout = EditorGUILayout.Foldout(basePointerFoldout, "Base Pointer Settings", true);

            if (basePointerFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(cursorPrefab);
                EditorGUILayout.PropertyField(disableCursorOnStart);
                EditorGUILayout.PropertyField(setCursorVisibilityOnSourceDetected);
                EditorGUILayout.PropertyField(raycastOrigin);
                EditorGUILayout.PropertyField(pointerExtent);
                EditorGUILayout.PropertyField(pointerOrientation);
                EditorGUILayout.PropertyField(pointerAction);

                if (DrawBasePointerActions)
                {
                    EditorGUILayout.PropertyField(requiresHoldAction);

                    if (requiresHoldAction.boolValue)
                    {
                        EditorGUILayout.PropertyField(activeHoldAction);
                    }
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}