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
        private static bool showGeneralSettings = true;
        private SerializedProperty isProfilerVisible;
        // todo: coming soon
        // private SerializedProperty isDebugPanelVisible;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            isProfilerVisible = serializedObject.FindProperty("isProfilerVisible");
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
            showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings", true);
            if (showGeneralSettings)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(isProfilerVisible);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
