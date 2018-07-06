// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityConfigurationProfile))]
    public class MixedRealityConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");

        // Camera properties
        private SerializedProperty enableCameraProfile;
        private SerializedProperty cameraProfile;
        // Input system properties
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty enableControllerProfiles;
        private SerializedProperty controllersProfile;
        // Boundary system properties
        private SerializedProperty enableBoundarySystem;
        private SerializedProperty boundaryExperienceScale;
        private SerializedProperty boundaryHeight;
        private SerializedProperty enablePlatformBoundaryRendering;

        private SerializedProperty testThis;

        private MixedRealityConfigurationProfile configurationProfile;

        private void OnEnable()
        {
            // Create The MR Manager if none exists.
            if (!MixedRealityManager.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityManager>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog("Attention!",
                        "There is no active Mixed Reality Manager in your scene. Would you like to create one now?", "Yes",
                        "Later"))
                    {
                        var profile = target as MixedRealityConfigurationProfile;
                        Debug.Assert(profile != null);
                        profile.ActiveManagers.Clear();
                        MixedRealityManager.Instance.ActiveProfile = profile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Manager in your scene.");
                    }
                }
                else
                {
                    MixedRealityManager.ConfirmInitialized();
                }
            }

            configurationProfile = target as MixedRealityConfigurationProfile;
            enableCameraProfile = serializedObject.FindProperty("enableCameraProfile");
            cameraProfile = serializedObject.FindProperty("cameraProfile");
            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            enableSpeechCommands = serializedObject.FindProperty("enableSpeechCommands");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            enableControllerProfiles = serializedObject.FindProperty("enableControllerProfiles");
            controllersProfile = serializedObject.FindProperty("controllersProfile");
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
            boundaryExperienceScale = serializedObject.FindProperty("boundaryExperienceScale");
            boundaryHeight = serializedObject.FindProperty("boundaryHeight");
            enablePlatformBoundaryRendering = serializedObject.FindProperty("enablePlatformBoundaryRendering");

            testThis = serializedObject.FindProperty("testThis");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();

            // Camera Profile Configuration
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProfile);

            if (enableCameraProfile.boolValue)
            {
                RenderProfile(cameraProfile);
            }

            //Input System configuration
            EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);

            if (enableInputSystem.boolValue)
            {
                EditorGUILayout.PropertyField(inputSystemType);
                RenderProfile(inputActionsProfile);

                EditorGUILayout.PropertyField(enableSpeechCommands);
                if (enableSpeechCommands.boolValue)
                {
                    RenderProfile(speechCommandsProfile);
                }
            }

            //Controller mapping configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Controller Mapping Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableControllerProfiles);

            if (enableControllerProfiles.boolValue)
            {
                RenderProfile(controllersProfile);
            }

            //Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableBoundarySystem);

            if (enableBoundarySystem.boolValue)
            {
                EditorGUILayout.PropertyField(boundaryExperienceScale, new GUIContent("Experience Scale:"));
                if ((ExperienceScale)boundaryExperienceScale.intValue == ExperienceScale.Room)
                {
                    EditorGUILayout.PropertyField(boundaryHeight, new GUIContent("Boundary Height (in m):"));
                    EditorGUILayout.PropertyField(enablePlatformBoundaryRendering, new GUIContent("Platform Rendering:"));
                }
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                MixedRealityManager.Instance.ResetConfiguration(configurationProfile);
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