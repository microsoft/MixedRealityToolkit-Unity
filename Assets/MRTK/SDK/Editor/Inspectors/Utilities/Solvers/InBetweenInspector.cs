// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(InBetween))]
    [CanEditMultipleObjects]
    public class InBetweenEditor : SolverInspector
    {
        private SerializedProperty secondTrackedTargetTypeProperty;
        private SerializedProperty secondTransformOverrideProperty;
        private SerializedProperty partwayOffsetProperty;

        private InBetween solverInBetween;
        private static readonly GUIContent SecondTrackedTypeLabel = new GUIContent("Second Tracked Target Type");
        protected override void OnEnable()
        {
            base.OnEnable();

            secondTrackedTargetTypeProperty = serializedObject.FindProperty("secondTrackedObjectType");
            secondTransformOverrideProperty = serializedObject.FindProperty("secondTransformOverride");
            partwayOffsetProperty = serializedObject.FindProperty("partwayOffset");

            solverInBetween = target as InBetween;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            InspectorUIUtility.DrawEnumSerializedProperty(secondTrackedTargetTypeProperty, SecondTrackedTypeLabel, solverInBetween.SecondTrackedObjectType);

            if (secondTrackedTargetTypeProperty.intValue == (int)TrackedObjectType.CustomOverride)
            {
                EditorGUILayout.PropertyField(secondTransformOverrideProperty);
            }

            EditorGUILayout.PropertyField(partwayOffsetProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
