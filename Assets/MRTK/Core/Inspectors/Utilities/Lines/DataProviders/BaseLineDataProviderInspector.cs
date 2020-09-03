// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(BaseMixedRealityLineDataProvider))]
    public class BaseLineDataProviderInspector : UnityEditor.Editor
    {
        private const string DrawLinePointsKey = "MRTK_Line_Inspector_DrawLinePoints";
        private const string BasicSettingsFoldoutKey = "MRTK_Line_Inspector_BasicSettings";
        private const string DrawLineRotationsKey = "MRTK_Line_Inspector_DrawLineRotations";
        private const string EditorSettingsFoldoutKey = "MRTK_Line_Inspector_EditorSettings";
        private const string RotationArrowLengthKey = "MRTK_Line_Inspector_RotationArrowLength";
        private const string RotationSettingsFoldoutKey = "MRTK_Line_Inspector_RotationSettings";
        private const string ManualUpVectorLengthKey = "MRTK_Line_Inspector_ManualUpVectorLength";
        private const string LinePreviewResolutionKey = "MRTK_Line_Inspector_LinePreviewResolution";
        private const string DistortionSettingsFoldoutKey = "MRTK_Line_Inspector_DistortionSettings";
        private const string DrawLineManualUpVectorsKey = "MRTK_Line_Inspector_DrawLineManualUpVectors";

        private const float ManualUpVectorHandleSizeModifier = 0.1f;

        private static readonly GUIContent BasicSettingsContent = new GUIContent("Basic Settings");
        private static readonly GUIContent EditorSettingsContent = new GUIContent("Editor Settings");
        private static readonly GUIContent ManualUpVectorContent = new GUIContent("Manual Up Vectors");
        private static readonly GUIContent RotationSettingsContent = new GUIContent("Rotation Settings");
        private static readonly GUIContent DistortionSettingsContent = new GUIContent("Distortion Settings");

        private static bool basicSettingsFoldout = true;
        private static bool editorSettingsFoldout = false;
        private static bool rotationSettingsFoldout = true;
        private static bool distortionSettingsFoldout = true;

        protected static int LinePreviewResolution = 16;

        protected static bool DrawLinePoints = false;
        protected static bool DrawLineRotations = false;
        protected static bool DrawLineManualUpVectors = false;

        protected static float ManualUpVectorLength = 1f;
        protected static float RotationArrowLength = 0.5f;

        private SerializedProperty transformMode;
        private SerializedProperty customLineTransform;
        private SerializedProperty lineStartClamp;
        private SerializedProperty lineEndClamp;
        private SerializedProperty loops;
        private SerializedProperty rotationType;
        private SerializedProperty flipUpVector;
        private SerializedProperty originOffset;
        private SerializedProperty manualUpVectorBlend;
        private SerializedProperty manualUpVectors;
        private SerializedProperty velocitySearchRange;
        private SerializedProperty distorters;
        private SerializedProperty distortionMode;
        private SerializedProperty distortionStrength;
        private SerializedProperty distortionEnabled;
        private SerializedProperty uniformDistortionStrength;

        private ReorderableList manualUpVectorList;

        protected BaseMixedRealityLineDataProvider LineData;
        protected bool RenderLinePreview = true;

        protected virtual void OnEnable()
        {
            basicSettingsFoldout = SessionState.GetBool(BasicSettingsFoldoutKey, basicSettingsFoldout);
            editorSettingsFoldout = SessionState.GetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);
            rotationSettingsFoldout = SessionState.GetBool(RotationSettingsFoldoutKey, rotationSettingsFoldout);
            distortionSettingsFoldout = SessionState.GetBool(DistortionSettingsFoldoutKey, distortionSettingsFoldout);

            LinePreviewResolution = SessionState.GetInt(LinePreviewResolutionKey, LinePreviewResolution);
            DrawLinePoints = SessionState.GetBool(DrawLinePointsKey, DrawLinePoints);
            DrawLineRotations = SessionState.GetBool(DrawLineRotationsKey, DrawLineRotations);
            RotationArrowLength = SessionState.GetFloat(RotationArrowLengthKey, RotationArrowLength);
            DrawLineManualUpVectors = SessionState.GetBool(DrawLineManualUpVectorsKey, DrawLineManualUpVectors);
            ManualUpVectorLength = SessionState.GetFloat(ManualUpVectorLengthKey, ManualUpVectorLength);

            LineData = (BaseMixedRealityLineDataProvider)target;
            transformMode = serializedObject.FindProperty("transformMode");
            customLineTransform = serializedObject.FindProperty("customLineTransform");
            lineStartClamp = serializedObject.FindProperty("lineStartClamp");
            lineEndClamp = serializedObject.FindProperty("lineEndClamp");
            loops = serializedObject.FindProperty("loops");
            rotationType = serializedObject.FindProperty("rotationMode");
            flipUpVector = serializedObject.FindProperty("flipUpVector");
            originOffset = serializedObject.FindProperty("originOffset");
            manualUpVectorBlend = serializedObject.FindProperty("manualUpVectorBlend");
            manualUpVectors = serializedObject.FindProperty("manualUpVectors");
            velocitySearchRange = serializedObject.FindProperty("velocitySearchRange");
            distorters = serializedObject.FindProperty("distorters");
            distortionMode = serializedObject.FindProperty("distortionMode");
            distortionStrength = serializedObject.FindProperty("distortionStrength");
            distortionEnabled = serializedObject.FindProperty("distortionEnabled");
            uniformDistortionStrength = serializedObject.FindProperty("uniformDistortionStrength");

            manualUpVectorList = new ReorderableList(serializedObject, manualUpVectors, false, true, true, true);
            manualUpVectorList.drawElementCallback += DrawManualUpVectorListElement;
            manualUpVectorList.drawHeaderCallback += DrawManualUpVectorHeader;

            RenderLinePreview = LineData.gameObject.GetComponent<BaseMixedRealityLineRenderer>() == null;

            var newDistorters = LineData.gameObject.GetComponents<Distorter>();
            distorters.arraySize = newDistorters.Length;

            for (int i = 0; i < newDistorters.Length; i++)
            {
                var distorterProperty = distorters.GetArrayElementAtIndex(i);
                distorterProperty.objectReferenceValue = newDistorters[i];
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            editorSettingsFoldout = EditorGUILayout.Foldout(editorSettingsFoldout, EditorSettingsContent, true);
            SessionState.SetBool(EditorSettingsFoldoutKey, editorSettingsFoldout);

            if (editorSettingsFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.BeginChangeCheck();

                    using (new EditorGUI.DisabledGroupScope(!RenderLinePreview))
                    {
                        EditorGUI.BeginChangeCheck();

                        LinePreviewResolution = EditorGUILayout.IntSlider("Preview Resolution", LinePreviewResolution, 2, 128);

                        if (EditorGUI.EndChangeCheck())
                        {
                            SessionState.SetInt(LinePreviewResolutionKey, LinePreviewResolution);
                        }

                        EditorGUI.BeginChangeCheck();
                        DrawLinePoints = EditorGUILayout.Toggle("Draw Line Points", DrawLinePoints);

                        if (EditorGUI.EndChangeCheck())
                        {
                            SessionState.SetBool(DrawLinePointsKey, DrawLinePoints);
                        }

                    }

                    EditorGUI.BeginChangeCheck();
                    DrawLineRotations = EditorGUILayout.Toggle("Draw Line Rotations", DrawLineRotations);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SessionState.SetBool(DrawLineRotationsKey, DrawLineRotations);
                    }

                    if (DrawLineRotations)
                    {
                        EditorGUI.BeginChangeCheck();
                        RotationArrowLength = EditorGUILayout.Slider("Rotation Arrow Length", RotationArrowLength, 0.01f, 5f);

                        if (EditorGUI.EndChangeCheck())
                        {
                            SessionState.SetFloat(RotationArrowLengthKey, RotationArrowLength);
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    DrawLineManualUpVectors = EditorGUILayout.Toggle("Draw Manual Up Vectors", DrawLineManualUpVectors);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SessionState.SetBool(DrawLineManualUpVectorsKey, DrawLineManualUpVectors);
                    }

                    if (DrawLineManualUpVectors)
                    {
                        EditorGUI.BeginChangeCheck();
                        ManualUpVectorLength = EditorGUILayout.Slider("Manual Up Vector Length", ManualUpVectorLength, 1f, 10f);

                        if (EditorGUI.EndChangeCheck())
                        {
                            SessionState.SetFloat(ManualUpVectorLengthKey, ManualUpVectorLength);
                        }
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        SceneView.RepaintAll();
                    }
                }
            }

            basicSettingsFoldout = EditorGUILayout.Foldout(basicSettingsFoldout, BasicSettingsContent, true);
            SessionState.SetBool(BasicSettingsFoldoutKey, basicSettingsFoldout);

            if (basicSettingsFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(transformMode);
                    EditorGUILayout.PropertyField(customLineTransform);
                    EditorGUILayout.PropertyField(lineStartClamp);
                    EditorGUILayout.PropertyField(lineEndClamp);
                    EditorGUILayout.PropertyField(loops);
                }
            }

            rotationSettingsFoldout = EditorGUILayout.Foldout(rotationSettingsFoldout, RotationSettingsContent, true);
            SessionState.SetBool(RotationSettingsFoldoutKey, rotationSettingsFoldout);

            if (rotationSettingsFoldout)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(rotationType);
                    EditorGUILayout.PropertyField(flipUpVector);
                    EditorGUILayout.PropertyField(originOffset);
                    EditorGUILayout.PropertyField(velocitySearchRange);

                    if (DrawLineManualUpVectors)
                    {
                        manualUpVectorList.DoLayoutList();

                        if (GUILayout.Button("Normalize Up Vectors"))
                        {

                            for (int i = 0; i < manualUpVectors.arraySize; i++)
                            {
                                var manualUpVectorProperty = manualUpVectors.GetArrayElementAtIndex(i);

                                Vector3 upVector = manualUpVectorProperty.vector3Value;

                                if (upVector == Vector3.zero)
                                {
                                    upVector = Vector3.up;
                                }

                                manualUpVectorProperty.vector3Value = upVector.normalized;
                            }
                        }

                        EditorGUILayout.PropertyField(manualUpVectorBlend);
                    }
                }
            }

            distortionSettingsFoldout = EditorGUILayout.Foldout(distortionSettingsFoldout, DistortionSettingsContent, true);
            SessionState.SetBool(DistortionSettingsFoldoutKey, distortionSettingsFoldout);

            if (distortionSettingsFoldout)
            {
                if (distorters.arraySize <= 0)
                {
                    EditorGUILayout.HelpBox("No distorters attached to this line.\nTry adding a distortion component.", MessageType.Info);
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(distortionEnabled);
                    EditorGUILayout.PropertyField(distortionMode);
                    EditorGUILayout.PropertyField(distortionStrength);
                    EditorGUILayout.PropertyField(uniformDistortionStrength);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            if (DrawLineManualUpVectors)
            {
                if (LineData.ManualUpVectors == null || LineData.ManualUpVectors.Length < 2)
                {
                    LineData.ManualUpVectors = new[] { Vector3.up, Vector3.up };
                }

                for (int i = 0; i < LineData.ManualUpVectors.Length; i++)
                {
                    float normalizedLength = (1f / (LineData.ManualUpVectors.Length - 1)) * i;
                    var position = LineData.GetPoint(normalizedLength);
                    float handleSize = HandleUtility.GetHandleSize(position);
                    LineData.ManualUpVectors[i] = MixedRealityInspectorUtility.VectorHandle(LineData, position, LineData.ManualUpVectors[i], false, true, ManualUpVectorLength * handleSize, handleSize * ManualUpVectorHandleSizeModifier);
                }
            }

            if (Application.isPlaying)
            {
                Handles.EndGUI();
                return;
            }

            Vector3 firstPosition = LineData.FirstPoint;
            Vector3 lastPosition = firstPosition;

            for (int i = 1; i < LinePreviewResolution; i++)
            {
                Vector3 currentPosition;
                Quaternion rotation;

                if (i == LinePreviewResolution - 1)
                {
                    currentPosition = LineData.LastPoint;
                    rotation = LineData.GetRotation(LineData.PointCount - 1);
                }
                else
                {
                    float normalizedLength = (1f / (LinePreviewResolution - 1)) * i;
                    currentPosition = LineData.GetPoint(normalizedLength);
                    rotation = LineData.GetRotation(normalizedLength);
                }

                if (RenderLinePreview)
                {
                    Handles.color = Color.magenta;
                    Handles.DrawLine(lastPosition, currentPosition);
                }

                if (DrawLineRotations)
                {
                    float arrowSize = HandleUtility.GetHandleSize(currentPosition) * RotationArrowLength;
                    Handles.color = MixedRealityInspectorUtility.LineVelocityColor;
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.zAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.forward), arrowSize, EventType.Repaint);
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.xAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.right), arrowSize, EventType.Repaint);
                    Handles.color = Color.Lerp(MixedRealityInspectorUtility.LineVelocityColor, Handles.yAxisColor, 0.75f);
                    Handles.ArrowHandleCap(0, currentPosition, Quaternion.LookRotation(rotation * Vector3.up), arrowSize, EventType.Repaint);
                }

                lastPosition = currentPosition;
            }

            if (LineData.Loops && RenderLinePreview)
            {
                Handles.color = Color.magenta;
                Handles.DrawLine(lastPosition, firstPosition);
            }
        }

        private static void DrawManualUpVectorHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, ManualUpVectorContent);
        }

        private void DrawManualUpVectorListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();

            var property = manualUpVectors.GetArrayElementAtIndex(index);
            property.vector3Value = EditorGUI.Vector3Field(rect, GUIContent.none, property.vector3Value);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
