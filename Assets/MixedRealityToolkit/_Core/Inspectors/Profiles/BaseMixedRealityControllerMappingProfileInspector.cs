// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(BaseMixedRealityControllerMappingProfile))]
    public class BaseMixedRealityControllerMappingProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private struct ControllerItem
        {
            public SupportedControllerType ControllerType;
            public Handedness Handedness;
            public MixedRealityInteractionMapping[] Interactions;

            public ControllerItem(SupportedControllerType controllerType, Handedness handedness, MixedRealityInteractionMapping[] interactions)
            {
                ControllerType = controllerType;
                Handedness = handedness;
                Interactions = interactions;
            }
        }

        private readonly List<ControllerItem> controllerItems = new List<ControllerItem>();

        private SerializedProperty controllerMappings;

        private BaseMixedRealityControllerMappingProfile profile;

        private GUIStyle controllerButtonStyle;

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

            controllerMappings = serializedObject.FindProperty("controllerMappings");
            profile = target as BaseMixedRealityControllerMappingProfile;
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

            if (GUILayout.Button("Back to controller mapping list"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles;
            }

            EditorGUILayout.Space();
            var deviceName = profile.ControllerType == SupportedControllerType.None ? "Custom Device" : profile.ControllerType.ToString();
            EditorGUILayout.LabelField($"{deviceName} Mappings", EditorStyles.boldLabel);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            CheckProfileLock(target, false);

            if (controllerButtonStyle == null)
            {
                controllerButtonStyle = new GUIStyle("LargeButton")
                {
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold,
                    stretchHeight = true,
                    stretchWidth = true,
                    wordWrap = true,
                    fontSize = 10,
                };
            }

            serializedObject.Update();
            controllerItems.Clear();

            GUILayout.BeginVertical();

            if (controllerMappings.arraySize == 0)
            {
                EditorGUILayout.HelpBox("You must override the controller mappings in your custom implementation to see a list of mappings for your device.", MessageType.Error);
            }

            for (int i = 0; i < controllerMappings?.arraySize; i++)
            {
                var supportedControllerType = profile.ControllerType;
                var controllerMapping = controllerMappings.GetArrayElementAtIndex(i);
                var handednessValue = controllerMapping.FindPropertyRelative("handedness");
                var handedness = (Handedness)handednessValue.intValue;
                var description = controllerMapping.FindPropertyRelative("description");
                var interactions = controllerMapping.FindPropertyRelative("interactions");

                bool skip = false;

                for (int j = 0; j < controllerItems.Count; j++)
                {
                    if (controllerItems[j].ControllerType == supportedControllerType &&
                        controllerItems[j].Handedness == handedness)
                    {
                        profile.ControllerMappings[i].SynchronizeInputActions(controllerItems[j].Interactions);
                        serializedObject.ApplyModifiedProperties();
                        skip = true;
                    }
                }

                if (skip) { continue; }

                controllerItems.Add(new ControllerItem(supportedControllerType, handedness, profile.ControllerMappings[i].Interactions));

                string handednessContent = string.Empty;

                switch (handedness)
                {
                    case Handedness.Left:
                    case Handedness.Right:
                    case Handedness.Other:
                        handednessContent = $" {handedness.ToString()} hand";
                        break;
                    case Handedness.Both:
                        handednessContent = $" {handedness.ToString()} hands";
                        break;
                }

                if (handedness != Handedness.Right)
                {
                    GUILayout.BeginHorizontal();
                }

                var buttonContent = new GUIContent($"Edit {description.stringValue}{handednessContent}", ControllerMappingLibrary.GetControllerTextureScaled(supportedControllerType, handedness));

                if (GUILayout.Button(buttonContent, controllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
                {
                    serializedObject.ApplyModifiedProperties();
                    ControllerPopupWindow.Show(profile.ControllerType, interactions, handedness, MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile);
                }

                if (handedness != Handedness.Left)
                {
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}