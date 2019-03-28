// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(BaseControllerPointer))]
    public class BaseControllerPointerInspector : ControllerPoseSynchronizerInspector
    {
        private SerializedProperty cursorPrefab;
        private SerializedProperty disableCursorOnStart;
        private SerializedProperty setCursorVisibilityOnSourceDetected;
        private SerializedProperty raycastOrigin;
        private SerializedProperty pointerExtent;
        private SerializedProperty defaultPointerExtent;
        private SerializedProperty activeHoldAction;
        private SerializedProperty pointerAction;
        private SerializedProperty pointerOrientation;
        private SerializedProperty requiresHoldAction;
        private SerializedProperty requiresActionBeforeEnabling;

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
            defaultPointerExtent = serializedObject.FindProperty("defaultPointerExtent");
            activeHoldAction = serializedObject.FindProperty("activeHoldAction");
            pointerAction = serializedObject.FindProperty("pointerAction");
            pointerOrientation = serializedObject.FindProperty("pointerOrientation");
            requiresHoldAction = serializedObject.FindProperty("requiresHoldAction");
            requiresActionBeforeEnabling = serializedObject.FindProperty("requiresActionBeforeEnabling");

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
                EditorGUILayout.PropertyField(defaultPointerExtent);

                // Pointer orientation is a field that is present on some pointers (for example, TeleportPointer)
                // but not on others (for example, BaseControllerPointer).
                if (pointerOrientation != null)
                {
                    EditorGUILayout.PropertyField(pointerOrientation);
                }

                EditorGUILayout.PropertyField(pointerAction);

                if (DrawBasePointerActions)
                {
                    EditorGUILayout.PropertyField(requiresHoldAction);

                    if (requiresHoldAction.boolValue)
                    {
                        EditorGUILayout.PropertyField(activeHoldAction);
                    }

                    EditorGUILayout.PropertyField(requiresActionBeforeEnabling);
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}