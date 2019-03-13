// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities.Lines
{
    [CustomEditor(typeof(BezierDataProvider))]
    public class BezierDataProviderInspector : BaseLineDataProviderInspector
    {
        private const float OverlappingPointThreshold = 0.015f;
        private const float HandleSizeModifier = 0.04f;
        private const float PickSizeModifier = 0.06f;

        private static readonly HashSet<int> OverlappingPointIndexes = new HashSet<int>();

        private static readonly Vector2 ControlPointButtonSize = new Vector2(16, 16);
        private static readonly Vector2 LeftControlPointPositionOffset = Vector2.left * 12;
        private static readonly Vector2 RightControlPointPositionOffset = Vector2.right * 24;

        private static readonly Vector2 ControlPointButtonHandleOffset = Vector3.up * 24;

        private static readonly GUIContent PositionContent = new GUIContent("Position");

        private SerializedProperty controlPoints;
        private SerializedProperty useLocalTangentPoints;

        private BezierDataProvider bezierData;

        private int selectedHandleIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            controlPoints = serializedObject.FindProperty("controlPoints");
            useLocalTangentPoints = serializedObject.FindProperty("useLocalTangentPoints");

            bezierData = (BezierDataProvider)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            // We always draw line points for bezier.
            DrawLinePoints = true;

            EditorGUILayout.PropertyField(controlPoints, true);
            EditorGUILayout.PropertyField(useLocalTangentPoints);

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            // We skip the first point as it should always remain at the GameObject's local origin (Pose.ZeroIdentity)
            for (int i = 0; i < 4; i++)
            {
                bool isTangentHandle = i % 3 != 0;

                serializedObject.Update();

                bool isLastPoint = i == 3;

                var controlPointPosition = LineData.GetPoint(i);
                var controlPointProperty = controlPoints.FindPropertyRelative("point" + (i + 1));

                // Draw our tangent lines
                Handles.color = Color.gray;
                if (i == 0)
                {
                    Handles.DrawLine(LineData.GetPoint(0), LineData.GetPoint(1));
                }
                else if (!isTangentHandle)
                {
                    Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i - 1));

                    if (!isLastPoint)
                    {
                        Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i + 1));
                    }
                }

                Handles.color = isTangentHandle ? Color.white : Color.green;
                float handleSize = HandleUtility.GetHandleSize(controlPointPosition);

                if (Handles.Button(controlPointPosition, Quaternion.identity, handleSize * HandleSizeModifier, handleSize * PickSizeModifier, Handles.DotHandleCap))
                {
                    selectedHandleIndex = i;
                }

                // Draw our handles
                if (Tools.current == Tool.Move && selectedHandleIndex == i)
                {
                    EditorGUI.BeginChangeCheck();

                    var newTargetPosition = Handles.PositionHandle(controlPointPosition, Quaternion.identity);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Bezier Point Position");
                        LineData.SetPoint(i, newTargetPosition);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawControlPointElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 88f;

            var property = controlPoints.GetArrayElementAtIndex(index);
            var fieldHeight = EditorGUIUtility.singleLineHeight * 0.5f;
            var labelRect = new Rect(rect.x - 8f, rect.y + fieldHeight * 2, rect.width, EditorGUIUtility.singleLineHeight);
            var positionRect = new Rect(rect.x, rect.y + fieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var rotationRect = new Rect(rect.x, rect.y + fieldHeight * 3, rect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, $"{index + 1}");

            EditorGUI.indentLevel++;

            GUI.enabled = index != 0;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"), PositionContent);
            bool hasPositionChanged = EditorGUI.EndChangeCheck();
            
            if (hasPositionChanged)
            {
                EditorUtility.SetDirty(target);
            }

            GUI.enabled = true;
            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;
            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}