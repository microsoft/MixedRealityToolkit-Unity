// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(MixedRealityMouseInputProfile))]
    public class MixedRealityMouseInputProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty mouseSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false)) { return; }

            mouseSpeed = serializedObject.FindProperty("mouseSpeed");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mouse Input settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Settings for mouse input in the editor.", MessageType.Info);
            CheckProfileLock(target);

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured()) { return; }

            serializedObject.Update();

            bool isGUIEnabled = GUI.enabled;

            GUILayout.Space(12f);
            EditorGUILayout.PropertyField(mouseSpeed);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
