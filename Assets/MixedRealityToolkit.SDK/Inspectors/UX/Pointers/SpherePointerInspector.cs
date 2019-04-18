// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.UI.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(SpherePointer))]
    public class SpherePointerInspector : BaseControllerPointerInspector
    {
        private SerializedProperty sphereCastRadius;
        private SerializedProperty debugMode;

        private bool spherePointerFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
            debugMode = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            spherePointerFoldout = EditorGUILayout.Foldout(spherePointerFoldout, "Sphere Pointer Settings", true);

            if (spherePointerFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(sphereCastRadius);
                EditorGUILayout.PropertyField(debugMode);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}