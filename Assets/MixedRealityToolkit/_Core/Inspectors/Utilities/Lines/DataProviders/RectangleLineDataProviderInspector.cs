// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(RectangleLineDataProvider))]
    public class RectangleLineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private SerializedProperty height;
        private SerializedProperty width;
        private SerializedProperty zOffset;

        protected override void OnEnable()
        {
            base.OnEnable();

            height = serializedObject.FindProperty("height");
            width = serializedObject.FindProperty("width");
            zOffset = serializedObject.FindProperty("zOffset");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Rectangles only support 4 points, so our preview will reflect that.
            LinePreviewResolution = 4;

            serializedObject.Update();

            EditorGUILayout.LabelField("Rectangle Settings");

            EditorGUI.indentLevel++;

            var prevHeight = height.floatValue;
            var prevWidth = width.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(zOffset);

            if (EditorGUI.EndChangeCheck())
            {
                if (height.floatValue.Equals(0f))
                {
                    height.floatValue = prevHeight;
                }

                if (height.floatValue < 0)
                {
                    height.floatValue = height.floatValue * -1f;
                }

                if (width.floatValue.Equals(0f))
                {
                    width.floatValue = prevWidth;
                }

                if (width.floatValue < 0)
                {
                    width.floatValue = width.floatValue * -1f;
                }
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            if (Application.isPlaying || !RenderLinePreview)
            {
                return;
            }

            Vector3 firstPos = LineData.GetPoint(0);
            Vector3 lastPos = firstPos;
            Handles.color = Color.magenta;

            for (int i = 1; i < LineData.PointCount; i++)
            {
                Vector3 currentPos = LineData.GetPoint(i);
                Handles.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            Handles.DrawLine(lastPos, firstPos);
        }
    }
}