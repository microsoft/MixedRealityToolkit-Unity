// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(EllipseLineDataProvider))]
    public class EllipseLineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private SerializedProperty resolution;
        private SerializedProperty radius;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Bump up the resolution, in case it's too low
            if (LinePreviewResolution < 32)
            {
                LinePreviewResolution = 32;
            }

            resolution = serializedObject.FindProperty("resolution");
            radius = serializedObject.FindProperty("radius");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.LabelField("Ellipse Settings");

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(resolution);
            EditorGUILayout.PropertyField(radius);

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}