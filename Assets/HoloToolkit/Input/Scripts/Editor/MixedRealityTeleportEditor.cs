// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(MixedRealityTeleport))]
    public class MixedRealityTeleportEditor : Editor
    {
        private readonly GUIContent verticalRotationLabel = new GUIContent("Vertical Rotation", "Used to check the Horizontal Rotation and the intent of the user to rotate in that direction");

        private static MixedRealityTeleport mixedRealityTeleport;
        private static SerializedProperty teleportMakerPrefab;
        private static SerializedProperty useCustomMappingProperty;
        private static bool useCustomMapping;
        private static bool mappingOverride;

        private void OnEnable()
        {
            mixedRealityTeleport = (MixedRealityTeleport)target;


            teleportMakerPrefab = serializedObject.FindProperty("teleportMarker");
            useCustomMappingProperty = serializedObject.FindProperty("useCustomMapping");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            useCustomMapping = useCustomMappingProperty.boolValue;

            EditorGUILayout.LabelField("Teleport Options", new GUIStyle("Label") { fontStyle = FontStyle.Bold });

            mixedRealityTeleport.EnableTeleport = EditorGUILayout.Toggle("Enable Teleport", mixedRealityTeleport.EnableTeleport);

            mixedRealityTeleport.EnableStrafe = EditorGUILayout.Toggle("Enable Strafe", mixedRealityTeleport.EnableStrafe);
            mixedRealityTeleport.StrafeAmount = EditorGUILayout.FloatField("Strafe Amount", mixedRealityTeleport.StrafeAmount);

            mixedRealityTeleport.EnableRotation = EditorGUILayout.Toggle("Enable Rotation", mixedRealityTeleport.EnableRotation);
            mixedRealityTeleport.RotationSize = EditorGUILayout.FloatField("Rotation Amount", mixedRealityTeleport.RotationSize);

            EditorGUILayout.PropertyField(teleportMakerPrefab);

            EditorGUILayout.LabelField("Teleport Controller Mappings", new GUIStyle("Label") { fontStyle = FontStyle.Bold });

            // Use custom mappings if users have already edited their axis mappings
            if (!mappingOverride &&
                (mixedRealityTeleport.LeftThumbstickX != InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL && mixedRealityTeleport.LeftThumbstickX != string.Empty ||
                 mixedRealityTeleport.LeftThumbstickY != InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL && mixedRealityTeleport.LeftThumbstickY != string.Empty ||
                 mixedRealityTeleport.RightThumbstickX != InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL && mixedRealityTeleport.RightThumbstickX != string.Empty ||
                 mixedRealityTeleport.RightThumbstickY != InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL && mixedRealityTeleport.RightThumbstickY != string.Empty))
            {
                useCustomMapping = true;
            }

            EditorGUI.BeginChangeCheck();

            useCustomMapping = EditorGUILayout.Toggle("Use Custom Mappings", useCustomMapping);

            if (EditorGUI.EndChangeCheck())
            {
                mappingOverride = !useCustomMapping;
            }

            useCustomMappingProperty.boolValue = useCustomMapping;

            if (useCustomMapping)
            {
                mixedRealityTeleport.LeftThumbstickX = EditorGUILayout.TextField("Horizontal Strafe", mixedRealityTeleport.LeftThumbstickX);
                mixedRealityTeleport.LeftThumbstickY = EditorGUILayout.TextField("Forward Movement", mixedRealityTeleport.LeftThumbstickY);
                mixedRealityTeleport.RightThumbstickX = EditorGUILayout.TextField("Horizontal Rotation", mixedRealityTeleport.RightThumbstickX);
                mixedRealityTeleport.RightThumbstickY = EditorGUILayout.TextField(verticalRotationLabel, mixedRealityTeleport.RightThumbstickY);
            }
            else
            {
                mixedRealityTeleport.HorizontalStrafe = (XboxControllerMappingTypes)EditorGUILayout.EnumPopup("Horizontal Strafe", mixedRealityTeleport.HorizontalStrafe);
                mixedRealityTeleport.ForwardMovement = (XboxControllerMappingTypes)EditorGUILayout.EnumPopup("Forward Movement", mixedRealityTeleport.ForwardMovement);
                mixedRealityTeleport.HorizontalRotation = (XboxControllerMappingTypes)EditorGUILayout.EnumPopup("Horizontal Rotation", mixedRealityTeleport.HorizontalRotation);
                mixedRealityTeleport.VerticalRotation = (XboxControllerMappingTypes)EditorGUILayout.EnumPopup(verticalRotationLabel, mixedRealityTeleport.VerticalRotation);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
