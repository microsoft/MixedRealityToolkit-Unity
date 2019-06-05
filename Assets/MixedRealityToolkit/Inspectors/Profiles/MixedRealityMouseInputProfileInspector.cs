// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

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
            mouseSpeed = serializedObject.FindProperty("mouseSpeed");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target), false))
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(mouseSpeed);
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            var mouseManager = MixedRealityToolkit.Instance.GetService<MouseDeviceManager>();
            return MixedRealityToolkit.IsInitialized && profile != null &&
                mouseManager != null && profile == mouseManager.MouseInputProfile;
        }
    }
}
