// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(SimpleLineDataProvider))]
    public class SimpleLineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private SerializedProperty endPoint;

        protected override void OnEnable()
        {
            base.OnEnable();

            endPoint = serializedObject.FindProperty("endPoint");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.LabelField("Simple Line Settings");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(endPoint);
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            serializedObject.Update();

            if (Tools.current == Tool.Move)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(LineData.transform.TransformPoint(endPoint.vector3Value), Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(LineData, "Change Spline Point Position");
                    endPoint.vector3Value = LineData.transform.InverseTransformPoint(newTargetPosition);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}