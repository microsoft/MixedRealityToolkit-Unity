﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfiles))]
    public class MixedRealityControllerMappingProfilesInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent CustomControllerButtonContent = new GUIContent("+ Add a Device Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Device Definition");

        private SerializedProperty controllerMappingProfiles;

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

            controllerMappingProfiles = serializedObject.FindProperty("controllerMappingProfiles");
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

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Supported Devices", EditorStyles.boldLabel);

            bool changed = false;

            if (GUILayout.Button(CustomControllerButtonContent, EditorStyles.miniButton))
            {
                controllerMappingProfiles.arraySize += 1;
                var newItem = controllerMappingProfiles.GetArrayElementAtIndex(controllerMappingProfiles.arraySize - 1);
                newItem.objectReferenceValue = null;
                changed = true;
            }

            EditorGUILayout.Space();

            for (int i = 0; i < controllerMappingProfiles.arraySize; i++)
            {
                var profileChanged = false;
                var controllerProfile = controllerMappingProfiles.GetArrayElementAtIndex(i);
                var profileObject = controllerProfile.objectReferenceValue;
                var profileName = "Assign or create a profile";

                if (profileObject != null)
                {
                    profileName = controllerProfile.objectReferenceValue.name.ToProperCase().Replace("Default ", string.Empty).Replace(" Controller Mapping Profile", string.Empty);
                }

                EditorGUILayout.BeginHorizontal();
                profileChanged |= RenderProfile(controllerProfile, new GUIContent(profileName), false);

                if (profileChanged && controllerProfile.objectReferenceValue != null)
                {
                    var knownProfiles = new List<Object>();

                    for (int j = 0; j < controllerMappingProfiles.arraySize; j++)
                    {
                        var knownProfile = controllerMappingProfiles.GetArrayElementAtIndex(j);

                        if (knownProfile.objectReferenceValue != null)
                        {
                            knownProfiles.Add(knownProfile.objectReferenceValue);
                        }
                    }

                    var count = 0;

                    for (int j = 0; j < knownProfiles.Count; j++)
                    {
                        if (knownProfiles[j] == controllerProfile.objectReferenceValue)
                        {
                            count++;
                        }
                    }

                    if (count >= 2)
                    {
                        Debug.LogWarning($"{controllerProfile.objectReferenceValue.name} is already registered!");
                        controllerProfile.objectReferenceValue = null;
                        serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }

                changed |= profileChanged;

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerMappingProfiles.DeleteArrayElementAtIndex(i);
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