// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(ParabolaPhysicalLineDataProvider))]
    public class ParabolaPhysicalLineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private SerializedProperty gravity;
        private SerializedProperty velocity;
        private SerializedProperty direction;
        private SerializedProperty timeMultiplier;
        private SerializedProperty useCustomGravity;

        protected override void OnEnable()
        {
            base.OnEnable();

            gravity = serializedObject.FindProperty("gravity");
            velocity = serializedObject.FindProperty("velocity");
            direction = serializedObject.FindProperty("direction");
            timeMultiplier = serializedObject.FindProperty("timeMultiplier");
            useCustomGravity = serializedObject.FindProperty("useCustomGravity");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("Physical Parabola Line Settings");
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(velocity);
            EditorGUILayout.PropertyField(direction);
            EditorGUILayout.PropertyField(timeMultiplier);
            EditorGUILayout.PropertyField(useCustomGravity);

            if (useCustomGravity.boolValue)
            {
                EditorGUILayout.PropertyField(gravity);
            }

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}