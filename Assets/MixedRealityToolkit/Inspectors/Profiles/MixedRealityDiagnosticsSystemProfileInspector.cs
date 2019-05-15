// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Diagnostics.Editor
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static bool showGeneralSettings = true;
        private SerializedProperty showDiagnostics;

        private static bool showProfilerSettings = true;
        private SerializedProperty showProfiler;
        private SerializedProperty showFrameInfo;
        private SerializedProperty showMemoryStats;
        private SerializedProperty frameSampleRate;
        private SerializedProperty windowAnchor;
        private SerializedProperty windowOffset;
        private SerializedProperty windowScale;
        private SerializedProperty windowFollowSpeed;
        private SerializedProperty defaultInstancedMaterial;

        // todo: coming soon
        // private static bool showDebugPanelSettings = true;
        // private SerializedProperty isDebugPanelVisible;

        protected override void OnEnable()
        {
            base.OnEnable();

            showDiagnostics = serializedObject.FindProperty("showDiagnostics");
            showProfiler = serializedObject.FindProperty("showProfiler");
            showFrameInfo = serializedObject.FindProperty("showFrameInfo");
            showMemoryStats = serializedObject.FindProperty("showMemoryStats");
            frameSampleRate = serializedObject.FindProperty("frameSampleRate");
            windowAnchor = serializedObject.FindProperty("windowAnchor");
            windowOffset = serializedObject.FindProperty("windowOffset");
            windowScale = serializedObject.FindProperty("windowScale");
            windowFollowSpeed = serializedObject.FindProperty("windowFollowSpeed");
            defaultInstancedMaterial = serializedObject.FindProperty("defaultInstancedMaterial");
        }

        public override void OnInspectorGUI()
        {
            RenderTitleDescriptionAndLogo(
                "Diagnostic Visualization Options",
                "Diagnostic visualizations can help monitor system resources and performance inside an application.");

            if (MixedRealityInspectorUtility.CheckMixedRealityConfigured(true, !RenderAsSubProfile))
            {
                if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
                {
                    return;
                }
            }

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings", true);
            if (showGeneralSettings)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showDiagnostics);
                    if(!showDiagnostics.boolValue)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Diagnostic visualizations have been globally disabled.", MessageType.Info);
                        EditorGUILayout.Space();
                    }
                }
            }

            EditorGUILayout.Space();
            showProfilerSettings = EditorGUILayout.Foldout(showProfilerSettings, "Profiler Settings", true);
            if (showProfilerSettings)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showProfiler);
                    EditorGUILayout.PropertyField(showFrameInfo);
                    EditorGUILayout.PropertyField(showMemoryStats);
                    EditorGUILayout.PropertyField(frameSampleRate);
                    EditorGUILayout.PropertyField(windowAnchor);
                    EditorGUILayout.PropertyField(windowOffset);
                    EditorGUILayout.PropertyField(windowScale);
                    EditorGUILayout.PropertyField(windowFollowSpeed);
                    EditorGUILayout.PropertyField(defaultInstancedMaterial);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
