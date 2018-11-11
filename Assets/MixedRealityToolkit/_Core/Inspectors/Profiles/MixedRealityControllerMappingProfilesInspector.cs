// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfiles))]
    public class MixedRealityControllerMappingProfilesInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty mouseControllerMappingProfile;
        private SerializedProperty touchScreenControllerMappingProfile;
        private SerializedProperty windowsMixedRealityControllerMappingProfile;
        private SerializedProperty viveWandControllerMappingProfile;
        private SerializedProperty oculusTouchControllerMappingProfile;
        private SerializedProperty oculusRemoteControllerMappingProfile;
        private SerializedProperty genericUnityControllerMappingProfile;
        private SerializedProperty genericOpenVRControllerMappingProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled ||
                 MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            mouseControllerMappingProfile = serializedObject.FindProperty("mouseControllerMappingProfile");
            touchScreenControllerMappingProfile = serializedObject.FindProperty("touchScreenControllerMappingProfile");
            windowsMixedRealityControllerMappingProfile = serializedObject.FindProperty("windowsMixedRealityControllerMappingProfile");
            viveWandControllerMappingProfile = serializedObject.FindProperty("viveWandControllerMappingProfile");
            oculusTouchControllerMappingProfile = serializedObject.FindProperty("oculusTouchControllerMappingProfile");
            oculusRemoteControllerMappingProfile = serializedObject.FindProperty("oculusRemoteControllerMappingProfile");
            genericUnityControllerMappingProfile = serializedObject.FindProperty("genericUnityControllerMappingProfile");
            genericOpenVRControllerMappingProfile = serializedObject.FindProperty("genericOpenVRControllerMappingProfile");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Input Mappings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the controllers and their inputs your users will be able to use in your application.\n\n" +
                                    "You'll want to define all your Input Actions first, then you can then wire them up to hardware sensors, controllers, gestures, and other input devices.", MessageType.Info);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            CheckProfileLock(target);

            serializedObject.Update();

            bool changed = false;

            changed |= RenderProfile(mouseControllerMappingProfile);
            changed |= RenderProfile(touchScreenControllerMappingProfile);
            changed |= RenderProfile(windowsMixedRealityControllerMappingProfile);
            changed |= RenderProfile(viveWandControllerMappingProfile);
            changed |= RenderProfile(oculusTouchControllerMappingProfile);
            changed |= RenderProfile(oculusRemoteControllerMappingProfile);
            changed |= RenderProfile(genericUnityControllerMappingProfile);
            changed |= RenderProfile(genericOpenVRControllerMappingProfile);

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}