// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities.Lines
{
    [CustomEditor(typeof(SimpleLineDataProvider))]
    public class SimpleLineDataProviderInspector : BaseLineDataProviderInspector
    {
        private SerializedProperty endPoint;
        private SerializedProperty endPointPosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            endPoint = serializedObject.FindProperty("endPoint");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            // We only have two points.
            LinePreviewResolution = 2;

            EditorGUILayout.LabelField("Simple Line Settings");
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

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
                    Undo.RecordObject(LineData, "Change Simple Point Position");
                    LineData.SetPoint(1, newTargetPosition);
                }
            }
            else if (Tools.current == Tool.Rotate)
            {
                EditorGUI.BeginChangeCheck();
                Quaternion newTargetRotation = Handles.RotationHandle(rotation.quaternionValue, LineData.GetPoint(1));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(LineData, "Change Simple Point Rotation");
                    rotation.quaternionValue = newTargetRotation;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}