// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityDiagnosticsProfile))]
    public class MixedRealityDiagnosticsSystemProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty showCpu;
        private SerializedProperty showFps;
        private SerializedProperty showMemory;
        private SerializedProperty visible;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            showCpu = serializedObject.FindProperty("showCpu");
            showFps = serializedObject.FindProperty("showFps");
            showMemory = serializedObject.FindProperty("showMemory");
            visible = serializedObject.FindProperty("visible");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
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

            EditorGUILayout.PropertyField(visible);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(showCpu);
            EditorGUILayout.PropertyField(showFps);
            EditorGUILayout.PropertyField(showMemory);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
