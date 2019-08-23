﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(SpherePointer))]
    public class SpherePointerInspector : BaseControllerPointerInspector
    {
        private SerializedProperty sphereCastRadius;
        private SerializedProperty debugMode;
        private SerializedProperty nearObjectMargin;
        private SerializedProperty grabLayerMasks;
        private SerializedProperty triggerInteraction;


        private bool spherePointerFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
            debugMode = serializedObject.FindProperty("debugMode");
            nearObjectMargin = serializedObject.FindProperty("nearObjectMargin");
            grabLayerMasks = serializedObject.FindProperty("grabLayerMasks");
            triggerInteraction = serializedObject.FindProperty("triggerInteraction");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            spherePointerFoldout = EditorGUILayout.Foldout(spherePointerFoldout, "Sphere Pointer Settings", true);

            if (spherePointerFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(sphereCastRadius);
                    EditorGUILayout.PropertyField(nearObjectMargin);
                    EditorGUILayout.PropertyField(triggerInteraction);
                    EditorGUILayout.PropertyField(grabLayerMasks, true);
                    EditorGUILayout.PropertyField(debugMode);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}