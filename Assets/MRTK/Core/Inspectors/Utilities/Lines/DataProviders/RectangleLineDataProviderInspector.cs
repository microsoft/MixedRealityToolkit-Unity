// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(RectangleLineDataProvider))]
    public class RectangleLineDataProviderInspector : BaseLineDataProviderInspector
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

            // Rectangle doesn't support line rotations
            DrawLineRotations = false;

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
                if (height.floatValue <= 0)
                {
                    height.floatValue = prevHeight;
                }

                if (width.floatValue <= 0)
                {
                    width.floatValue = prevWidth;
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