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

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false)) { return; }

            smoothEyeTracking = serializedObject.FindProperty("smoothEyeTracking");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            if (GUILayout.Button("Back to Registered Service Providers Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Eye tracking settings", EditorStyles.boldLabel);
            CheckProfileLock(target);

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured()) { return; }

            serializedObject.Update();

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(smoothEyeTracking);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
