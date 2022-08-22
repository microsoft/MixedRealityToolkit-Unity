//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(PinchSlider), true)]
    public class PinchSliderInspector : UnityEditor.Editor
    {

        private static GUIStyle labelStyle;

        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }

            PinchSlider slider = target as PinchSlider;
            if (slider != null)
            {
                Handles.color = Color.cyan;
                Vector3 startPos = slider.SliderStartPosition;
                Vector3 endPos = slider.SliderEndPosition;
                Handles.DrawLine(startPos, endPos);


                EditorGUI.BeginChangeCheck();

                float handleSize = HandleUtility.GetHandleSize(startPos) * 0.15f;
                slider.SliderStartPosition = Handles.FreeMoveHandle(startPos,
#if !UNITY_2022_2_OR_NEWER
                    Quaternion.identity,
#endif
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
                slider.SliderEndPosition = Handles.FreeMoveHandle(endPos,
#if !UNITY_2022_2_OR_NEWER
                    Quaternion.identity,
#endif
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);

                DrawLabelWithDottedLine(startPos + (Vector3.up * handleSize * 10f), startPos, handleSize, "slider start");
                DrawLabelWithDottedLine(endPos + (Vector3.up * handleSize * 10f), endPos, handleSize, "slider end");
            }
        }

        private void DrawLabelWithDottedLine(Vector3 labelPos, Vector3 dottedLineStart, float handleSize, string labelText)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = Color.white;

            Handles.color = Color.white;
            Handles.Label(labelPos + Vector3.up * handleSize, labelText, labelStyle);
            Handles.DrawDottedLine(dottedLineStart, labelPos, 5f);
        }
    }
}
