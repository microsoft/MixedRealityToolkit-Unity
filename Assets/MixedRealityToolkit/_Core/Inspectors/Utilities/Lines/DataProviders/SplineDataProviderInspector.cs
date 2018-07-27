// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(SplineDataProvider))]
    public class SplineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private SerializedProperty alignControlPoints;

        protected override void OnEnable()
        {
            base.OnEnable();

            alignControlPoints = serializedObject.FindProperty("alignControlPoints");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("Spline Settings");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(alignControlPoints);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}