// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation.Editor
{
    /// <summary>
    /// Custom inspector drawing for the ControllerSimulationSettings object.
    /// </summary>
    [CustomPropertyDrawer(typeof(ControllerSimulationSettings))]
    public class ControllerSimulationSettingsDrawer : PropertyDrawer
    {
        private readonly GUIContent simModeContent = new GUIContent("Simulation mode");

        private readonly GUIContent anchorPointContent = new GUIContent("Anchor point");
        private readonly GUIContent neutralPositionContent = new GUIContent("Neutral position");
        private readonly GUIContent neutralHandshapeContent = new GUIContent("Neutral handshape");
        private readonly GUIContent triggerHandshapeContent = new GUIContent("Trigger handshape");
        private readonly GUIContent secondaryNeutralHandshapeContent = new GUIContent("Secondary neutral handshape");
        private readonly GUIContent secondaryTriggerHandshapeContent = new GUIContent("Secondary trigger handshape");
        private readonly GUIContent toggleSecondaryHandshapesContent = new GUIContent("Toggle secondary handshapes");

        private readonly GUIContent trackContent = new GUIContent("Momentary tracking");
        private readonly GUIContent toggleContent = new GUIContent("Toggle tracking");

        // private readonly GUIContent moveSpeedContent = new GUIContent("Speed");
        private readonly GUIContent jitterStrengthContent = new GUIContent("Jitter strength");
        private readonly GUIContent moveSmoothingContent = new GUIContent("Smoothed");
        private readonly GUIContent moveDepthContent = new GUIContent("Depth");
        private readonly GUIContent depthSensitivityContent = new GUIContent("Sensitivity");
        private readonly GUIContent moveHorizontalContent = new GUIContent("Horizontal");
        private readonly GUIContent moveVerticalContent = new GUIContent("Vertical");

        // private readonly GUIContent rotationSensitivityContent = new GUIContent("Sensitivity");
        private readonly GUIContent pitchContent = new GUIContent("Pitch");
        private readonly GUIContent invertPitchContent = new GUIContent("Invert pitch");
        private readonly GUIContent yawContent = new GUIContent("Yaw");
        private readonly GUIContent rollContent = new GUIContent("Roll");

        private readonly GUIContent faceTheCameraContent = new GUIContent("Face the camera");

        private readonly GUIContent triggerContent = new GUIContent("Trigger");
        private readonly GUIContent gripContent = new GUIContent("Grip");

        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return PropertyDrawerUtilities.CalculatePropertyHeight(29);
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

            #region Core controls

            SerializedProperty simMode = property.FindPropertyRelative("simulationMode");
            SerializedProperty anchorPoint = property.FindPropertyRelative("anchorPoint");
            SerializedProperty defaultPosition = property.FindPropertyRelative("defaultPosition");
            SerializedProperty neutralHandshape = property.FindPropertyRelative("neutralHandshape");
            SerializedProperty triggerHandshape = property.FindPropertyRelative("triggerHandshape");
            SerializedProperty secondaryNeutralHandshape = property.FindPropertyRelative("secondaryNeutralHandshape");
            SerializedProperty secondaryTriggerHandshape = property.FindPropertyRelative("secondaryTriggerHandshape");
            SerializedProperty toggleSecondaryHandshapes = property.FindPropertyRelative("toggleSecondaryHandshapes");

            SerializedProperty track = property.FindPropertyRelative("track");
            SerializedProperty toggle = property.FindPropertyRelative("toggle");

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                simMode, simModeContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                anchorPoint, anchorPointContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                defaultPosition, neutralPositionContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                neutralHandshape, neutralHandshapeContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                triggerHandshape, triggerHandshapeContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                secondaryNeutralHandshape, secondaryNeutralHandshapeContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                secondaryTriggerHandshape, secondaryTriggerHandshapeContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                toggleSecondaryHandshapes, toggleSecondaryHandshapesContent);

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                track, trackContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                toggle, toggleContent);

            #endregion Core controls

            #region Move controls

            // SerializedProperty moveSpeed = property.FindPropertyRelative("moveSpeed");
            SerializedProperty jitterStrength = property.FindPropertyRelative("jitterStrength");
            SerializedProperty moveSmoothing = property.FindPropertyRelative("isMovementSmoothed");
            SerializedProperty moveDepth = property.FindPropertyRelative("moveDepth");
            SerializedProperty depthSensitivity = property.FindPropertyRelative("depthSensitivity");
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
            /* todo
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                moveSpeed, moveSpeedContent);
            */
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                jitterStrength, jitterStrengthContent);
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
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                depthSensitivity, depthSensitivityContent);
            EditorGUI.indentLevel--;
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

            // SerializedProperty rotationSensitivity = property.FindPropertyRelative("rotationSensitivity");
            SerializedProperty pitch = property.FindPropertyRelative("pitch");
            SerializedProperty yaw = property.FindPropertyRelative("yaw");
            SerializedProperty roll = property.FindPropertyRelative("roll");
            SerializedProperty invertPitch = property.FindPropertyRelative("invertPitch");

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Rotation", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // EditorGUI.PropertyField(
            //    PropertyDrawerUtilities.GetPosition(
            //        position,
            //        PropertyDrawerUtilities.VerticalSpacing,
            //        ++rowMultiplier,
            //        PropertyDrawerUtilities.Height),
            //    rotationSensitivity, rotationSensitivityContent);
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

            #region Hand pose controls

            SerializedProperty faceTheCamera = property.FindPropertyRelative("faceTheCamera");

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Hand pose", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                faceTheCamera, faceTheCameraContent);
            EditorGUI.indentLevel--;

            #endregion Hand pose controls

            #region Action controls

            SerializedProperty triggerAxis = property.FindPropertyRelative("triggerAxis");
            SerializedProperty triggerButton = property.FindPropertyRelative("triggerButton");
            SerializedProperty gripAxis = property.FindPropertyRelative("gripAxis");
            SerializedProperty gripButton = property.FindPropertyRelative("gripButton");

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
#if LATER
            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Axis", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                gripAxis, gripContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                triggerAxis, triggerContent);
            EditorGUI.indentLevel--;
#endif // LATER
            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Button", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                gripButton, gripContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                triggerButton, triggerContent);
            EditorGUI.indentLevel--;
            // EditorGUI.LabelField(
            //     PropertyDrawerUtilities.GetPosition(
            //         position,
            //         PropertyDrawerUtilities.VerticalSpacing,
            //         ++rowMultiplier,
            //         PropertyDrawerUtilities.Height),
            //     "Touch", EditorStyles.boldLabel);
            // EditorGUI.indentLevel++;
            // // todo
            // EditorGUI.indentLevel--;
            // EditorGUI.indentLevel--;

            #endregion Action controls

            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;

            EditorGUI.EndProperty();
        }
    }
}
