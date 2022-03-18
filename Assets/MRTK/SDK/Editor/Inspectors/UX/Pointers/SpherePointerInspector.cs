// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Input.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(SpherePointer))]
    public class SpherePointerInspector : BaseControllerPointerInspector
    {
        private SerializedProperty nearObjectSectorAngle;
        private SerializedProperty sphereCastRadius;
        private SerializedProperty pullbackDistance;
        private SerializedProperty nearObjectMargin;
        private SerializedProperty nearObjectSmoothingFactor;
        private SerializedProperty grabLayerMasks;
        private SerializedProperty triggerInteraction;
        private SerializedProperty sceneQueryBufferSize;
        private SerializedProperty ignoreCollidersNotInFOV;
        private SerializedProperty graspPointPlacement;

        private bool spherePointerFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            sphereCastRadius = serializedObject.FindProperty("sphereCastRadius");
            pullbackDistance = serializedObject.FindProperty("pullbackDistance");
            sceneQueryBufferSize = serializedObject.FindProperty("sceneQueryBufferSize");
            nearObjectSectorAngle = serializedObject.FindProperty("nearObjectSectorAngle");
            nearObjectMargin = serializedObject.FindProperty("nearObjectMargin");
            nearObjectSmoothingFactor = serializedObject.FindProperty("nearObjectSmoothingFactor");
            grabLayerMasks = serializedObject.FindProperty("grabLayerMasks");
            triggerInteraction = serializedObject.FindProperty("triggerInteraction");
            ignoreCollidersNotInFOV = serializedObject.FindProperty("ignoreCollidersNotInFOV");
            graspPointPlacement = serializedObject.FindProperty("graspPointPlacement");
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
                    EditorGUILayout.PropertyField(nearObjectSectorAngle);
                    EditorGUILayout.PropertyField(sphereCastRadius);
                    EditorGUILayout.PropertyField(pullbackDistance);
                    EditorGUILayout.PropertyField(sceneQueryBufferSize);
                    EditorGUILayout.PropertyField(nearObjectMargin);
                    EditorGUILayout.PropertyField(nearObjectSmoothingFactor);
                    EditorGUILayout.PropertyField(triggerInteraction);
                    EditorGUILayout.PropertyField(grabLayerMasks, true);
                    EditorGUILayout.PropertyField(ignoreCollidersNotInFOV);
                    EditorGUILayout.PropertyField(graspPointPlacement);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}