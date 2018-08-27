// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityConfigurationProfile))]
    public class MixedRealityConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");
        private static readonly GUIContent SpeechConfidenceContent = new GUIContent("Recognition Confidence Level", "The speech recognizer's minimum confidence level setting that will raise the action.");
        private static readonly GUIContent[] SpeechConfidenceOptionContent =
        {
            new GUIContent("High"),
            new GUIContent("Medium"),
            new GUIContent("Low"),
            new GUIContent("Unrecognized")
        };

        private static readonly int[] SpeechConfidenceOptions = { 0, 1, 2, 3 };

        // Experience properties
        private SerializedProperty targetExperienceScale;
        // Camera properties
        private SerializedProperty enableCameraProfile;
        private SerializedProperty cameraProfile;
        // Input system properties
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty pointerProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty recognitionConfidenceLevel;
        private SerializedProperty enableDictation;
        private SerializedProperty enableTouchScreenInput;
        private SerializedProperty touchScreenInputProfile;
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;
        // Boundary system properties
        private SerializedProperty enableBoundarySystem;
        private SerializedProperty boundarySystemType;
        private SerializedProperty boundaryHeight;
        // Teleport system properties
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;
        private SerializedProperty teleportDuration;
        private SerializedProperty boundaryVisualizationProfile;

        private MixedRealityConfigurationProfile configurationProfile;

        private void OnEnable()
        {
            configurationProfile = target as MixedRealityConfigurationProfile;

            // Create The MR Manager if none exists.
            if (!MixedRealityManager.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityManager>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Manager in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later"))
                    {
                        MixedRealityManager.Instance.ActiveProfile = configurationProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Manager in your scene.");
                        return;
                    }
                }
            }

            if (!MixedRealityManager.ConfirmInitialized())
            {
                return;
            }

            if (!MixedRealityManager.HasActiveProfile)
            {
                return;
            }

            // Experience configuration
            targetExperienceScale = serializedObject.FindProperty("targetExperienceScale");
            // Camera configuration
            enableCameraProfile = serializedObject.FindProperty("enableCameraProfile");
            cameraProfile = serializedObject.FindProperty("cameraProfile");
            // Input system configuration
            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            enableSpeechCommands = serializedObject.FindProperty("enableSpeechCommands");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            recognitionConfidenceLevel = serializedObject.FindProperty("recognitionConfidenceLevel");
            enableDictation = serializedObject.FindProperty("enableDictation");
            enableTouchScreenInput = serializedObject.FindProperty("enableTouchScreenInput");
            touchScreenInputProfile = serializedObject.FindProperty("touchScreenInputProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
            // Boundary system configuration
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
            boundarySystemType = serializedObject.FindProperty("boundarySystemType");
            boundaryHeight = serializedObject.FindProperty("boundaryHeight");
            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty("enableTeleportSystem");
            teleportSystemType = serializedObject.FindProperty("teleportSystemType");
            teleportDuration = serializedObject.FindProperty("teleportDuration");
            boundaryVisualizationProfile = serializedObject.FindProperty("boundaryVisualizationProfile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            if (!MixedRealityManager.IsInitialized)
            {
                EditorGUILayout.HelpBox("Unable to find Mixed Reality Manager!", MessageType.Error);
                return;
            }

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();

            // Experience configuration
            EditorGUILayout.LabelField("Experience Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(targetExperienceScale, TargetScaleContent);
            ExperienceScale scale = (ExperienceScale)targetExperienceScale.intValue;
            string scaleDescription = string.Empty;

            switch (scale)
            {
                case ExperienceScale.OrientationOnly:
                    scaleDescription = "The user is stationary. Position data does not change.";
                    break;

                case ExperienceScale.Seated:
                    scaleDescription = "The user is stationary and seated. The origin of the world is at a neutral head-level position.";
                    break;

                case ExperienceScale.Standing:
                    scaleDescription = "The user is stationary and standing. The origin of the world is on the floor, facing forward.";
                    break;

                case ExperienceScale.Room:
                    scaleDescription = "The user is free to move about the room. The origin of the world is on the floor, facing forward. Boundaries are available.";
                    break;

                case ExperienceScale.World:
                    scaleDescription = "The user is free to move about the world. Relies upon knowledge of the environment (Spatial Anchors and Spatial Mapping).";
                    break;
            }

            if (scaleDescription != string.Empty)
            {
                EditorGUILayout.HelpBox(scaleDescription, MessageType.Info);
            }

            // Camera Profile configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProfile);

            if (enableCameraProfile.boolValue)
            {
                RenderProfile(cameraProfile);
            }

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);

            if (enableInputSystem.boolValue)
            {
                EditorGUILayout.PropertyField(inputSystemType);
                RenderProfile(inputActionsProfile);
                RenderProfile(pointerProfile);

                EditorGUILayout.PropertyField(enableSpeechCommands);

                if (enableSpeechCommands.boolValue)
                {
                    RenderProfile(speechCommandsProfile);
                    recognitionConfidenceLevel.intValue = EditorGUILayout.IntPopup(SpeechConfidenceContent, recognitionConfidenceLevel.intValue, SpeechConfidenceOptionContent, SpeechConfidenceOptions);
                }

                EditorGUILayout.PropertyField(enableDictation);

                EditorGUILayout.PropertyField(enableTouchScreenInput);

                if (enableTouchScreenInput.boolValue)
                {
                    RenderProfile(touchScreenInputProfile);
                }

                EditorGUILayout.PropertyField(enableControllerMapping);

                if (enableControllerMapping.boolValue)
                {
                    RenderProfile(controllerMappingProfile);
                }
            }

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);

            if (enableBoundarySystem.boolValue)
            {
                EditorGUILayout.PropertyField(boundarySystemType);

                // Boundary settings depend on the experience scale
                if (scale == ExperienceScale.Room)
                {
                    EditorGUILayout.PropertyField(boundaryHeight);
                    RenderProfile(boundaryVisualizationProfile);
                }
                else
                {
                    GUILayout.Space(6f);
                    EditorGUILayout.HelpBox("Boundary visualization is only supported in Room scale experiences.", MessageType.Info);
                }
            }

            // Teleport System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Teleport System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableTeleportSystem);

            if (enableTeleportSystem.boolValue)
            {
                EditorGUILayout.PropertyField(teleportSystemType);
                EditorGUILayout.PropertyField(teleportDuration);
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.delayCall += () => MixedRealityManager.Instance.ResetConfiguration(configurationProfile);
            }
        }

        private static void RenderProfile(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);

            if (property.objectReferenceValue == null)
            {
                if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton))
                {
                    var profileTypeName = property.type.Replace("PPtr<$", string.Empty).Replace(">", string.Empty);
                    Debug.Assert(profileTypeName != null, "No Type Found");
                    ScriptableObject profile = CreateInstance(profileTypeName);
                    profile.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject));
                    property.objectReferenceValue = profile;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}