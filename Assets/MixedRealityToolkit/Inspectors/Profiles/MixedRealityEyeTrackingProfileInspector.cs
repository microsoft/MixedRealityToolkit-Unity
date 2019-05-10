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
            if (!RenderProfileHeader("Eye tracking settings", 
                string.Empty, 
                "Back to Registered Service Providers Profile", 
                MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile))
            {
                return;
            }

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(smoothEyeTracking);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
