// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static bool showGeneralProperties = true;
        private SerializedProperty visible;
        private SerializedProperty handlerType;

        private static bool showFpsProperties = true;
        private SerializedProperty showFps;
        private SerializedProperty fpsBuffer;
        private readonly GUIContent showFpsContent = new GUIContent("Show Frame Rate");


        private static bool showCpuProperties = true;
        private SerializedProperty showCpu;
        private SerializedProperty cpuBuffer;
        private readonly GUIContent showCpuContent = new GUIContent("Show CPU Usage");

        private static bool showMemoryProperties = true;
        private SerializedProperty showMemory;
        private SerializedProperty memoryBuffer;
        private readonly GUIContent showMemoryContent = new GUIContent("Show Memory Usage");

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            visible = serializedObject.FindProperty("visible");
            handlerType = serializedObject.FindProperty("handlerType");
            showCpu = serializedObject.FindProperty("showCpu");
            cpuBuffer = serializedObject.FindProperty("cpuBuffer");
            showFps = serializedObject.FindProperty("showFps");
            fpsBuffer = serializedObject.FindProperty("fpsBuffer");
            showMemory = serializedObject.FindProperty("showMemory");
            memoryBuffer = serializedObject.FindProperty("memoryBuffer");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diagnostic Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Diagnostic visualizations can help monitor system resources and performance inside an application.", MessageType.Info);

            EditorGUILayout.Space();
            showGeneralProperties = EditorGUILayout.Foldout(showGeneralProperties, "General Settings", true);
            if (showGeneralProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(visible);
                    EditorGUILayout.PropertyField(handlerType);
                }
            }

            EditorGUILayout.Space();
            showCpuProperties = EditorGUILayout.Foldout(showCpuProperties, "Processor Settings", true);
            if (showCpuProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showCpu, showCpuContent);
                    EditorGUILayout.PropertyField(cpuBuffer);
                }
            }

            EditorGUILayout.Space();
            showFpsProperties = EditorGUILayout.Foldout(showFpsProperties, "Frame Rate Settings", true);
            if (showFpsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showFps, showFpsContent);
                    EditorGUILayout.PropertyField(fpsBuffer);
                }
            }

            EditorGUILayout.Space();
            showMemoryProperties = EditorGUILayout.Foldout(showMemoryProperties, "Memory Settings", true);
            if (showMemoryProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showMemory, showMemoryContent);
                    EditorGUILayout.PropertyField(memoryBuffer);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
