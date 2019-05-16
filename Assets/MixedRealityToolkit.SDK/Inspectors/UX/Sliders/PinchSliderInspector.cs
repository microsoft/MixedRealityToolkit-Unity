//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(PinchSlider))]
    public class PinchSliderInspector : UnityEditor.Editor
    {
        private static GUIStyle labelStyle;

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
                    Quaternion.identity,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);
                slider.SliderEndPosition = Handles.FreeMoveHandle(endPos,
                    Quaternion.identity,
                    handleSize,
                    Vector3.zero,
                    Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    var sliderStartSerialized = serializedObject.FindProperty("sliderStartDistance");
                    var sliderEndSerialized = serializedObject.FindProperty("sliderEndDistance");
                    sliderStartSerialized.floatValue = slider.SliderStartDistance;
                    sliderEndSerialized.floatValue = slider.SliderEndDistance;
                    serializedObject.ApplyModifiedProperties();
                }

                DrawLabelWithDottedLine(startPos + (Vector3.up * handleSize * 10f), startPos, handleSize, "slider start");
                DrawLabelWithDottedLine(endPos + (Vector3.up * handleSize * 10f), endPos, handleSize, "slider end");
            }
        }

        private void DrawLabelWithDottedLine(Vector3 labelPos, Vector3 dottedLineStart, float handleSize, string labelText)
        {
            Handles.color = Color.white;
            Handles.Label(labelPos + Vector3.up * handleSize, labelText, labelStyle);
            Handles.DrawDottedLine(dottedLineStart, labelPos, 5f);
        }
    }

}