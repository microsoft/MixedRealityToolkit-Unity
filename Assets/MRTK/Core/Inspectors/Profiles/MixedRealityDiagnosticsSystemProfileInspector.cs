// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Diagnostics.Editor
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty showDiagnostics;
        private SerializedProperty showProfiler;
        private SerializedProperty frameSampleRate;
        private SerializedProperty windowAnchor;
        private SerializedProperty windowOffset;
        private SerializedProperty windowScale;
        private SerializedProperty windowFollowSpeed;
        private SerializedProperty showProfilerDuringMRC;

        private const string ProfileTitle = "Diagnostic Settings";
        private const string ProfileDescription = "Diagnostic visualizations can help monitor system resources and performance inside an application.";

        // todo: coming soon
        // private static bool showDebugPanelSettings = true;
        // private SerializedProperty isDebugPanelVisible;

        protected override void OnEnable()
        {
            base.OnEnable();

            showDiagnostics = serializedObject.FindProperty("showDiagnostics");
            showProfiler = serializedObject.FindProperty("showProfiler");
            frameSampleRate = serializedObject.FindProperty("frameSampleRate");
            windowAnchor = serializedObject.FindProperty("windowAnchor");
            windowOffset = serializedObject.FindProperty("windowOffset");
            windowScale = serializedObject.FindProperty("windowScale");
            windowFollowSpeed = serializedObject.FindProperty("windowFollowSpeed");
            showProfilerDuringMRC = serializedObject.FindProperty("showProfilerDuringMRC");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showDiagnostics);
                    if (!showDiagnostics.boolValue)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Diagnostic visualizations have been globally disabled.", MessageType.Info);
                        EditorGUILayout.Space();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Profiler Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showProfiler);
                    EditorGUILayout.PropertyField(frameSampleRate);
                    EditorGUILayout.PropertyField(windowAnchor);
                    EditorGUILayout.PropertyField(windowOffset);
                    EditorGUILayout.PropertyField(windowScale);
                    EditorGUILayout.PropertyField(windowFollowSpeed);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("HoloLens Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showProfilerDuringMRC);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.DiagnosticsSystemProfile;
        }
    }
}
