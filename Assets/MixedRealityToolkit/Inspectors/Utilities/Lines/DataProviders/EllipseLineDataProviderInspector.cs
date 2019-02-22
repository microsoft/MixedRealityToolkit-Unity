// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities.Lines
{
    [CustomEditor(typeof(EllipseLineDataProvider))]
    public class EllipseLineDataProviderInspector : BaseLineDataProviderInspector
    {
        private SerializedProperty resolution;
        private SerializedProperty radius;
        private Vector2 tempRadius;

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

            var prevRadius = radius.vector2Value;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(radius);

            if (EditorGUI.EndChangeCheck())
            {
                bool update = false;
                tempRadius = radius.vector2Value;

                if (radius.vector2Value.x <= 0)
                {
                    tempRadius.x = prevRadius.x;
                    update = true;
                }

                if (radius.vector2Value.y <= 0)
                {
                    tempRadius.y = prevRadius.x;
                    update = true;
                }

                if (update)
                {
                    radius.vector2Value = tempRadius;
                }
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}