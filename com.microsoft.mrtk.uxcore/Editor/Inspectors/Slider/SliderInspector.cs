// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(Slider), true)]
    public class SliderInspector : StatefulInteractableEditor
    {
        private static GUIStyle labelStyle;

        protected override void DrawProperties()
        {
            base.DrawProperties(showToggleMode: false);
        }

        private const float SliderEndpointHandleSize = 0.15f;
        private const float SliderTickVisualSize = 0.10f;

        private void OnSceneGUI()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.white;
            }

            Slider slider = target as Slider;

            if (slider != null)
            {
                Handles.color = Color.cyan;
                Vector3 startPos = slider.SliderStart.position;
                Vector3 endPos = slider.SliderEnd.position;
                Handles.DrawLine(startPos, endPos);

                float handleSize = HandleUtility.GetHandleSize(startPos) * SliderEndpointHandleSize;

                // Draw the tick mark locations where applicable
                Handles.color = Color.blue - Color.black * 0.3f;
                handleSize = HandleUtility.GetHandleSize(startPos) * SliderTickVisualSize;
                if (slider.UseSliderStepDivisions)
                {
                    Vector3 SliderStepVector = (endPos - startPos) / slider.SliderStepDivisions;

                    for (int i = 1; i < slider.SliderStepDivisions; i++)
                    {
                        Handles.SphereHandleCap(0, startPos + SliderStepVector * i, Quaternion.identity, handleSize, EventType.Repaint);
                    }
                }

                // Signal the start and end positions of the slider
                DrawLabelWithDottedLine(startPos + (10f * handleSize * Vector3.up), startPos, handleSize, "slider start");
                DrawLabelWithDottedLine(endPos + (10f * handleSize * Vector3.up), endPos, handleSize, "slider end");

                // Do not draw editable handles if the slider is using a rect transform
                if (slider.GetComponent<RectTransform>() != null)
                {
                    return;
                }

                EditorGUI.BeginChangeCheck();
 
#if UNITY_2022_1_OR_NEWER
                Vector3 newStartPosition = Handles.FreeMoveHandle(startPos,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
#else
                Vector3 newStartPosition = Handles.FreeMoveHandle(startPos,
                    Quaternion.identity,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
#endif

#if UNITY_2022_1_OR_NEWER
                Vector3 newEndPosition = Handles.FreeMoveHandle(endPos,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
#else
                Vector3 newEndPosition = Handles.FreeMoveHandle(endPos,
                    Quaternion.identity,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
#endif

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(slider.SliderStart, "Changed Slider Start Position");
                    slider.SliderStart.position = newStartPosition;//Vector3.ProjectOnPlane(newStartPosition, slider.transform.forward);
                    slider.SliderStart.localPosition = slider.SliderStart.localPosition.Mul(new Vector3(1, 1, 0));

                    Undo.RecordObject(slider.SliderEnd, "Changed Slider End Position");
                    slider.SliderEnd.position = newEndPosition;//Vector3.ProjectOnPlane(newEndPosition, slider.transform.forward);
                    slider.SliderEnd.localPosition = slider.SliderEnd.localPosition.Mul(new Vector3(1, 1, 0));
                }

                Handles.BeginGUI();
                if (GUI.Button(new Rect(10, 10, 100, 30), "Recenter Slider"))
                {
                    slider.transform.position = (slider.SliderStart.position + slider.SliderEnd.position) * 0.5f;

                    Vector3 cachedTrackDirection = slider.SliderTrackDirection;

                    slider.SliderStart.localPosition = -cachedTrackDirection * 0.5f;
                    slider.SliderEnd.localPosition = cachedTrackDirection * 0.5f;
                }
                Handles.EndGUI();
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