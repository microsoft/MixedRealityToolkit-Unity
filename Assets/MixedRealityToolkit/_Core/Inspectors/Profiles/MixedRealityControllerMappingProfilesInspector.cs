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
        private static readonly GUIContent CustomControllerButtonContent = new GUIContent("+ Add a Custom Device Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Device Definition");

        private static readonly GUIContent MouseProfileContent = new GUIContent("Mouse Devices");
        private static readonly GUIContent TouchProfileContent = new GUIContent("Touch Devices");
        private static readonly GUIContent XboxControllerProfileContent = new GUIContent("Xbox Device");
        private static readonly GUIContent WmrProfileContent = new GUIContent("WMR Devices");
        private static readonly GUIContent ViveWandProfileContent = new GUIContent("Vive Devices");
        private static readonly GUIContent OculusTouchProfileContent = new GUIContent("Oculus Touch Devices");
        private static readonly GUIContent OculusRemoteProfileContent = new GUIContent("Oculus Remote Devices");
        private static readonly GUIContent GenericUnityProfileContent = new GUIContent("Generic Unity Devices");
        private static readonly GUIContent GenericOpenVrProfileContent = new GUIContent("Generic Open VR Devices");

        private SerializedProperty mouseControllerMappingProfile;
        private SerializedProperty touchScreenControllerMappingProfile;
        private SerializedProperty xboxControllerMappingProfile;
        private SerializedProperty windowsMixedRealityControllerMappingProfile;
        private SerializedProperty viveWandControllerMappingProfile;
        private SerializedProperty oculusTouchControllerMappingProfile;
        private SerializedProperty oculusRemoteControllerMappingProfile;
        private SerializedProperty genericUnityControllerMappingProfile;
        private SerializedProperty genericOpenVRControllerMappingProfile;
        private SerializedProperty customControllerProfiles;

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
            xboxControllerMappingProfile = serializedObject.FindProperty("xboxControllerMappingProfile");
            windowsMixedRealityControllerMappingProfile = serializedObject.FindProperty("windowsMixedRealityControllerMappingProfile");
            viveWandControllerMappingProfile = serializedObject.FindProperty("viveWandControllerMappingProfile");
            oculusTouchControllerMappingProfile = serializedObject.FindProperty("oculusTouchControllerMappingProfile");
            oculusRemoteControllerMappingProfile = serializedObject.FindProperty("oculusRemoteControllerMappingProfile");
            genericUnityControllerMappingProfile = serializedObject.FindProperty("genericUnityControllerMappingProfile");
            genericOpenVRControllerMappingProfile = serializedObject.FindProperty("genericOpenVRControllerMappingProfile");
            customControllerProfiles = serializedObject.FindProperty("customControllerProfiles");
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
                                    "You'll want to define all your Input Actions first, then you can then wire them up to hardware sensors, controllers, gestures, and other input devices.\n\n" +
                                    "Note: These profiles can be empty if controller support is not reqiured.", MessageType.Info);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Supported Devices", EditorStyles.boldLabel);

            bool changed = false;

            changed |= RenderProfile(mouseControllerMappingProfile, MouseProfileContent);
            changed |= RenderProfile(touchScreenControllerMappingProfile, TouchProfileContent);
            changed |= RenderProfile(xboxControllerMappingProfile, XboxControllerProfileContent);
            changed |= RenderProfile(windowsMixedRealityControllerMappingProfile, WmrProfileContent);
            changed |= RenderProfile(viveWandControllerMappingProfile, ViveWandProfileContent);
            changed |= RenderProfile(oculusTouchControllerMappingProfile, OculusTouchProfileContent);
            changed |= RenderProfile(oculusRemoteControllerMappingProfile, OculusRemoteProfileContent);
            changed |= RenderProfile(genericUnityControllerMappingProfile, GenericUnityProfileContent);
            changed |= RenderProfile(genericOpenVRControllerMappingProfile, GenericOpenVrProfileContent);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Custom Devices", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Custom Devices can be added by creating your own profile and extending the CustomMixedRealityControllerMappingProfile.\n\n" +
                                    "Note: You'll also need to create your own device manager and register it in the extension services profile.", MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button(CustomControllerButtonContent, EditorStyles.miniButton))
            {
                customControllerProfiles.arraySize += 1;
                var newItem = customControllerProfiles.GetArrayElementAtIndex(customControllerProfiles.arraySize - 1);
                newItem.objectReferenceValue = null;
                changed = true;
            }

            for (int i = 0; i < customControllerProfiles.arraySize; i++)
            {
                var customControllerProfile = customControllerProfiles.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                changed |= RenderProfile(customControllerProfile, new GUIContent($"Custom Device {i + 1}"));

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    customControllerProfiles.DeleteArrayElementAtIndex(i);
                    changed = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}