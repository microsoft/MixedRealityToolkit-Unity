// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using HoloToolkit.Unity.UX;
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Base class for line editors (splines, Beziers, parabolas, etc)
/// </summary>
public class LineBaseEditor : MRTKEditor
{
    // Const editor properties
    public static readonly Color DefaultDisplayLineColor = Color.white;
    protected static readonly Color lineVelocityColor = new Color(0.9f, 1f, 0f, 0.8f);
    protected const int linePreviewResolutionUnselected = 16;

    // Static editor properties
    protected static int linePreviewResolutionSelected = 16;
    protected static bool drawLinePoints = false;
    protected static bool drawDottedLine = true;
    protected static bool drawLineRotations = false;
    protected static bool drawLineManualUpVectors = false;
    protected static float lineRotationLength = 0.5f;
    protected static float lineManualUpVectorLength = 10f;

    protected virtual StepModeEnum EditorStepMode { get { return StepModeEnum.Interpolated; } }

    protected override void DrawCustomSceneGUI()
    {
        LineBase line = (LineBase)target;
        bool selected = Selection.activeGameObject == line.gameObject;

        int previewResolution = Mathf.Min(linePreviewResolutionSelected, linePreviewResolutionUnselected);
        if (selected)
        {
            previewResolution = linePreviewResolutionSelected;
        }

        // Draw dotted lines regardless of selection
        if (drawDottedLine)
        {
            DrawDottedLine(line, previewResolution);
        }

        // Draw rotations only on selected object
        if (drawLineRotations && selected)
        {
            DrawLineRotations(line, previewResolution);
        }

        // Draw up vectors only on selected object
        if (drawLineManualUpVectors && selected)
        {
            DrawManualUpVectorHandles(line);
        }

        if (drawLinePoints)
        {
            DrawLinePoints(line);
        }

        // Since lines are constantly updating themselves
        // just repaint all by default
    }

    protected override void DrawCustomFooter()
    {
        LineBase line = (LineBase)target;

        if (DrawSectionStart(line.name + " Editor", "Editor Settings"))
        {
            linePreviewResolutionSelected = EditorGUILayout.IntSlider("Preview resolution", linePreviewResolutionSelected, 3, 100);

            drawDottedLine = EditorGUILayout.Toggle("Draw Dotted Line", drawDottedLine);
            drawLinePoints = EditorGUILayout.Toggle("Draw Line Points", drawLinePoints);
            drawLineRotations = EditorGUILayout.Toggle("Draw Line Rotations", drawLineRotations);
            drawLineManualUpVectors = EditorGUILayout.Toggle("Draw Manual Up Vectors", drawLineManualUpVectors);

            if (drawLineRotations)
            {
                lineRotationLength = EditorGUILayout.Slider("Rotation Arrow Length", lineRotationLength, 0.01f, 5f);
            }

            if (drawLineManualUpVectors)
            {
                lineManualUpVectorLength = EditorGUILayout.Slider("Manual Up Vector Length", lineManualUpVectorLength, 1f, 10f);
                if (GUILayout.Button("Normalize Up Vectors"))
                {
                    for (int i = 0; i < line.ManualUpVectors.Length; i++)
                    {
                        Vector3 upVector = line.ManualUpVectors[i];
                        if (upVector == Vector3.zero)
                        {
                            upVector = Vector3.up;
                        }

                        line.ManualUpVectors[i] = upVector.normalized;
                    }
                }
            }
        }
        DrawSectionEnd();

        SceneView.RepaintAll();
    }

    protected void DrawDottedLine(LineBase line, int numSteps)
    {
        Vector3 firstPos = Vector3.zero;
        Vector3 lastPos = Vector3.zero;

        switch (EditorStepMode)
        {
            case StepModeEnum.FromSource:
                firstPos = line.GetPoint(0);
                lastPos = firstPos;

                for (int i = 1; i < line.NumPoints; i++)
                {
                    Vector3 currentPos = line.GetPoint(i);
                    Handles.DrawDottedLine(lastPos, currentPos, MRTKEditor.DottedLineScreenSpace);
                    lastPos = currentPos;
                }

                if (line.Loops)
                {
                    Handles.DrawDottedLine(lastPos, firstPos, MRTKEditor.DottedLineScreenSpace);
                }
                break;

            case StepModeEnum.Interpolated:
            default:
                firstPos = line.GetPoint(0f);
                lastPos = firstPos;
                Handles.color = DefaultDisplayLineColor;

                for (int i = 1; i < numSteps; i++)
                {
                    float normalizedLength = (1f / (numSteps - 1)) * i;
                    Vector3 currentPos = line.GetPoint(normalizedLength);
                    Handles.DrawDottedLine(lastPos, currentPos, MRTKEditor.DottedLineScreenSpace);
                    lastPos = currentPos;
                }

                if (line.Loops)
                {
                    Handles.DrawDottedLine(lastPos, firstPos, MRTKEditor.DottedLineScreenSpace);
                }
                break;
        }
    }

    protected void DrawLinePoints(LineBase line)
    {
        Handles.color = DefaultDisplayLineColor;
        float dotSize = HandleUtility.GetHandleSize(line.transform.position) * 0.025f;
        for (int i = 0; i < line.NumPoints; i++)
        {
            Handles.DotHandleCap(0, line.GetPoint(i), Quaternion.identity, dotSize, EventType.Repaint);
        }
    }

    protected void DrawLineRotations(LineBase line, int numSteps)
    {
        Handles.color = lineVelocityColor;
        float arrowSize = HandleUtility.GetHandleSize(line.transform.position) * lineRotationLength;

        for (int i = 1; i < numSteps; i++)
        {
            float normalizedLength = (1f / (numSteps - 1)) * i;
            Vector3 currentPos = line.GetPoint(normalizedLength);
            Quaternion rotation = line.GetRotation(normalizedLength);

            Handles.color = Color.Lerp(lineVelocityColor, Handles.zAxisColor, 0.75f);
            Handles.ArrowHandleCap(0, currentPos, Quaternion.LookRotation(rotation * Vector3.forward), arrowSize, EventType.Repaint);
            Handles.color = Color.Lerp(lineVelocityColor, Handles.xAxisColor, 0.75f);
            Handles.ArrowHandleCap(0, currentPos, Quaternion.LookRotation(rotation * Vector3.right), arrowSize, EventType.Repaint);
            Handles.color = Color.Lerp(lineVelocityColor, Handles.yAxisColor, 0.75f);
            Handles.ArrowHandleCap(0, currentPos, Quaternion.LookRotation(rotation * Vector3.up), arrowSize, EventType.Repaint);
        }
    }

    protected void DrawManualUpVectorHandles(LineBase line)
    {
        if (line.ManualUpVectors == null || line.ManualUpVectors.Length < 2)
            line.ManualUpVectors = new Vector3[2];

        for (int i = 0; i < line.ManualUpVectors.Length; i++)
        {
            float normalizedLength = (1f / (line.ManualUpVectors.Length - 1)) * i;
            Vector3 currentPoint = line.GetPoint(normalizedLength);
            Vector3 currentUpVector = line.ManualUpVectors[i];
            line.ManualUpVectors[i] = VectorHandle(currentPoint, currentUpVector, false, lineManualUpVectorLength);
        }
    }
}
#endif