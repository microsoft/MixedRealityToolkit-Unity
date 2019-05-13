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
        private const string ProfileTitle = "Mouse Input Settings";
        private const string ProfileDescription = "Settings for mouse input in the editor.";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false)) { return; }

            mouseSpeed = serializedObject.FindProperty("mouseSpeed");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, BackProfileType.Input))
            if (MixedRealityInspectorUtility.CheckMixedRealityConfigured(true, !RenderAsSubProfile))
            {
                if (GUILayout.Button("Back to Input Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
                }
            }
            else
            {
                return;
            }

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(mouseSpeed);

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = true;
        }
    }
}
