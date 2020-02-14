// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    [CustomEditor(typeof(UnityARCameraSettingsProfile))]
    public class UnityARCameraSettingsProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private const string ProfileTitle = "Unity AR Foundation Camera Settings";
        private const string ProfileDescription = "";

        // Tracking settings
        private SerializedProperty poseSource;
        private SerializedProperty trackingType;
        private SerializedProperty updateType;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Tracking settings
            poseSource = serializedObject.FindProperty("poseSource");
            trackingType = serializedObject.FindProperty("trackingType");
            updateType = serializedObject.FindProperty("updateType");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Tracking Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(poseSource);
                EditorGUILayout.PropertyField(trackingType);
                EditorGUILayout.PropertyField(updateType);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;

            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile.SettingsConfigurations != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile.SettingsConfigurations.Any(s => s.SettingsProfile == profile);
        }
    }
}