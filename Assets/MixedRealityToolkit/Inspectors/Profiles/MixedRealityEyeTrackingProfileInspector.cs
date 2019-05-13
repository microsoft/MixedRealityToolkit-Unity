// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityEyeTrackingProfile))]
    public class MixedRealityEyeTrackingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty smoothEyeTracking;

        private const string ProfileTitle = "Eye tracking Settings";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false)) { return; }

            smoothEyeTracking = serializedObject.FindProperty("smoothEyeTracking");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
                "Eye tracking settings",
                "Configuration for eye tracking settings");
            if (!RenderProfileHeader(ProfileTitle, string.Empty, BackProfileType.RegisteredServices))
            {
                return;
            }

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured()) { return; }
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(smoothEyeTracking);

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = true;
        }
    }
}
