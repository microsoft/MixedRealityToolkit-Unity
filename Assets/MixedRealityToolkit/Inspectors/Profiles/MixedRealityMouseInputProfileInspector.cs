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
        private const string ProfileTitle = "Mouse Input Settings";
        private const string ProfileDescription = "Settings used to configure the behavior of mouse controllers.";

        private SerializedProperty cursorSpeed;
        private SerializedProperty wheelSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();
            cursorSpeed = serializedObject.FindProperty("cursorSpeed");
            wheelSpeed = serializedObject.FindProperty("wheelSpeed");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target), false))
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(cursorSpeed);
                EditorGUILayout.PropertyField(wheelSpeed);
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            if (!MixedRealityToolkit.IsInitialized || profile == null)
            {
                return false;
            }

            var mouseManager = MixedRealityToolkit.Instance.GetService<IMixedRealityMouseDeviceManager>(null, false);
            return mouseManager != null && profile == mouseManager.ConfigurationProfile;
        }
    }
}
