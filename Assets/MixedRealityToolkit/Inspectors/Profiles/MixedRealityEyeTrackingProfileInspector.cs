// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

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
            smoothEyeTracking = serializedObject.FindProperty("smoothEyeTracking");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, string.Empty, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(smoothEyeTracking);
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                MixedRealityToolkit.Instance.HasActiveProfile &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations.Any(s => profile == s.DeviceManagerProfile);
        }
    }
}
