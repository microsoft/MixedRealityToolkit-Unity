// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation.Editor
{
    /// <summary>
    /// Custom drawer for the CameraSettings object.
    /// </summary>
    [CustomPropertyDrawer(typeof(CameraSimulationSettings))]
    public class CameraSimulationSettingsDrawer : PropertyDrawer
    {
        private readonly GUIContent simEnabledContent = new GUIContent("Simulation enabled");
        private readonly GUIContent originOffsetContent = new GUIContent("Offset");

        private readonly GUIContent moveSpeedContent = new GUIContent("Speed");
        private readonly GUIContent moveSmoothingContent = new GUIContent("Smoothed");
        private readonly GUIContent moveDepthContent = new GUIContent("Depth");
        private readonly GUIContent moveHorizontalContent = new GUIContent("Horizontal");
        private readonly GUIContent moveVerticalContent = new GUIContent("Vertical");

        private readonly GUIContent rotationSensitivityContent = new GUIContent("Sensitivity");
        private readonly GUIContent pitchContent = new GUIContent("Pitch");
        private readonly GUIContent invertPitchContent = new GUIContent("Invert pitch");
        private readonly GUIContent yawContent = new GUIContent("Yaw");
        private readonly GUIContent rollContent = new GUIContent("Roll");

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return PropertyDrawerUtilities.CalculatePropertyHeight(15);
        }

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(
                position,
                GUIUtility.GetControlID(FocusType.Passive),
                label,
                EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            int rowMultiplier = 0;

            SerializedProperty simEnabled = property.FindPropertyRelative("simulationEnabled");
            SerializedProperty originOffset = property.FindPropertyRelative("originOffset");

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                simEnabled, simEnabledContent);

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                originOffset, originOffsetContent);

            #region Move controls

            SerializedProperty moveSpeed = property.FindPropertyRelative("moveSpeed");
            SerializedProperty moveSmoothing = property.FindPropertyRelative("isMovementSmoothed");
            SerializedProperty moveDepth = property.FindPropertyRelative("moveDepth");
            SerializedProperty moveHorizontal = property.FindPropertyRelative("moveHorizontal");
            SerializedProperty moveVertical = property.FindPropertyRelative("moveVertical");

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Movement", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveSpeed, moveSpeedContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveSmoothing, moveSmoothingContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveDepth, moveDepthContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveHorizontal, moveHorizontalContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveVertical, moveVerticalContent);
            EditorGUI.indentLevel--;

            #endregion Move controls

            #region Rotate controls

            SerializedProperty rotationSensitivity = property.FindPropertyRelative("rotationSensitivity");
            SerializedProperty pitch = property.FindPropertyRelative("pitch");
            SerializedProperty invertPitch = property.FindPropertyRelative("invertPitch");
            SerializedProperty yaw = property.FindPropertyRelative("yaw");
            SerializedProperty roll = property.FindPropertyRelative("roll");

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Rotation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
               PropertyDrawerUtilities.GetPosition(
                   position,
                   PropertyDrawerUtilities.VerticalSpacing,
                   ++rowMultiplier,
                   PropertyDrawerUtilities.Height),
               rotationSensitivity, rotationSensitivityContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                pitch, pitchContent);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                invertPitch, invertPitchContent);
            EditorGUI.indentLevel--;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                yaw, yawContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                roll, rollContent);
            EditorGUI.indentLevel--;

            #endregion Rotate controls

            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;

            EditorGUI.EndProperty();
        }
    }
}
