// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities.Lines
{
    [CustomEditor(typeof(ParabolaConstrainedLineDataProvider))]
    public class ParabolicConstrainedLineDataProviderInspector : BaseLineDataProviderInspector
    {
        private SerializedProperty height;
        private SerializedProperty endPoint;
        private SerializedProperty upDirection;
        private SerializedProperty endPointPosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            height = serializedObject.FindProperty("height");
            endPoint = serializedObject.FindProperty("endPoint");
            upDirection = serializedObject.FindProperty("upDirection");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("Constrained Parabola Line Settings");
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(upDirection);
            EditorGUILayout.PropertyField(endPoint);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        
            serializedObject.Update();

            var rotation = endPoint.FindPropertyRelative("rotation");

            if (Tools.current == Tool.Move)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(LineData.GetPoint(1), Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(LineData, "Change Parabola Point Position");
                    LineData.SetPoint(1, newTargetPosition);
                }
            }
            else if (Tools.current == Tool.Rotate)
            {
                EditorGUI.BeginChangeCheck();
                Quaternion newTargetRotation = Handles.RotationHandle(rotation.quaternionValue, LineData.GetPoint(1));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(LineData, "Change Parabola Point Rotation");
                    rotation.quaternionValue = newTargetRotation;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}