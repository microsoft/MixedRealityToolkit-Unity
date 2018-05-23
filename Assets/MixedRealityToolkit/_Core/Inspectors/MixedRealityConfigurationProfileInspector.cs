// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityConfigurationProfile))]
    public class MixedRealityConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty enableSpeechCommands;
        private SerializedProperty speechCommandsProfile;
        private SerializedProperty renderMotionControllers;
        private SerializedProperty enableBoundarySystem;

        private void OnEnable()
        {
            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            enableSpeechCommands = serializedObject.FindProperty("enableSpeechCommands");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

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

            EditorGUILayout.LabelField("Device Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(renderMotionControllers);

            EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(enableBoundarySystem);

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderProfile(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);
            if (property.objectReferenceValue == null)
            {
                if (GUILayout.Button(new GUIContent("+", "Create New Profile"), EditorStyles.miniButton))
                {
                    throw new NotImplementedException();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}