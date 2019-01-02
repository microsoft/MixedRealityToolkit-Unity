// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Core.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private struct ControllerRenderProfile
        {
            public SupportedControllerType ControllerType;
            public Handedness Handedness;
            public MixedRealityInteractionMapping[] Interactions;

            public ControllerRenderProfile(SupportedControllerType controllerType, Handedness handedness, MixedRealityInteractionMapping[] interactions)
            {
                ControllerType = controllerType;
                Handedness = handedness;
                Interactions = interactions;
            }
        }

        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");
        private static readonly GUIContent GenericTypeContent = new GUIContent("Generic Type");

        private static readonly GUIContent[] GenericTypeListContent =
        {
            new GUIContent("Unity Controller"),
            new GUIContent("Open VR Controller")
        };

        private static readonly int[] GenericTypeIds = { 0, 1 };

        private static MixedRealityControllerMappingProfile thisProfile;

        private SerializedProperty mixedRealityControllerMappingProfiles;
        private float defaultLabelWidth;
        private float defaultFieldWidth;
        private GUIStyle controllerButtonStyle;

        private readonly List<ControllerRenderProfile> controllerRenderList = new List<ControllerRenderProfile>();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            mixedRealityControllerMappingProfiles = serializedObject.FindProperty("mixedRealityControllerMappingProfiles");

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled ||
                 MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            thisProfile = target as MixedRealityControllerMappingProfile;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
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
            EditorGUILayout.LabelField("Controller Input Mapping", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the controllers and their inputs your users will be able to use in your application.\n\n" +
                                    "You'll want to define all your Input Actions first, then you can then wire them up to hardware sensors, controllers, gestures, and other input devices.", MessageType.Info);

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

            CheckProfileLock(target);

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

            RenderControllerList(mixedRealityControllerMappingProfiles);

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.MixedRealityControllerMappingProfiles.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                AddController(controllerList, typeof(GenericJoystickController));
                return;
            }

            bool reset = false;
            if (controllerRenderList.Count > 0)
            {
                for (var type = 1; type <= (int)SupportedControllerType.Mouse; type++)
                {
                    if (controllerRenderList.All(profile => profile.ControllerType != (SupportedControllerType)type))
                    {
                        if ((SupportedControllerType)type == SupportedControllerType.TouchScreen)
                        {
                            AddController(controllerList, typeof(UnityTouchController));
                            reset = true;
                        }

                        if ((SupportedControllerType)type == SupportedControllerType.Mouse)
                        {
                            AddController(controllerList, typeof(MouseController));
                            reset = true;
                        }
                    }
                }
            }

            controllerRenderList.Clear();
            if (reset) { return; }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            for (int i = 0; i < controllerList.arraySize; i++)
            {
                var controllerType = thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type;
                var supportedControllerType = SupportedControllerType.None;
                var mixedRealityControllerMapping = controllerList.GetArrayElementAtIndex(i);
                var controllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                var handedness = (Handedness)controllerHandedness.intValue;
                var interactionsList = mixedRealityControllerMapping.FindPropertyRelative("interactions");
                var useCustomInteractionMappings = mixedRealityControllerMapping.FindPropertyRelative("useCustomInteractionMappings");

                if (controllerType == typeof(XboxController))
                {
                    supportedControllerType = SupportedControllerType.Xbox;
                }
                else if (controllerType == typeof(WindowsMixedRealityController) ||
                         controllerType == typeof(WindowsMixedRealityOpenVRMotionController))
                {
                    supportedControllerType = SupportedControllerType.WindowsMixedReality;
                }
                else if (controllerType == typeof(OculusTouchController))
                {
                    supportedControllerType = SupportedControllerType.OculusTouch;
                }
                else if (controllerType == typeof(OculusRemoteController))
                {
                    supportedControllerType = SupportedControllerType.OculusRemote;
                }
                else if (controllerType == typeof(ViveWandController))
                {
                    supportedControllerType = SupportedControllerType.ViveWand;
                }
                else if (controllerType == typeof(GenericOpenVRController))
                {
                    supportedControllerType = SupportedControllerType.GenericOpenVR;
                }
                else if (controllerType == typeof(GenericJoystickController))
                {
                    supportedControllerType = SupportedControllerType.GenericUnity;
                }
                else if (controllerType == typeof(UnityTouchController))
                {
                    supportedControllerType = SupportedControllerType.TouchScreen;
                }
                else if (controllerType == typeof(MouseController))
                {
                    supportedControllerType = SupportedControllerType.Mouse;
                }

                bool skip = false;

                for (int j = 0; j < controllerRenderList.Count; j++)
                {
                    if (supportedControllerType == SupportedControllerType.GenericOpenVR ||
                        supportedControllerType == SupportedControllerType.GenericUnity)
                    {
                        continue;
                    }

                    if (controllerRenderList[j].ControllerType == supportedControllerType &&
                        controllerRenderList[j].Handedness == handedness)
                    {
                        thisProfile.MixedRealityControllerMappingProfiles[i].SynchronizeInputActions(controllerRenderList[j].Interactions);
                        serializedObject.ApplyModifiedProperties();
                        skip = true;
                    }
                }

                if (skip) { continue; }

                controllerRenderList.Add(new ControllerRenderProfile(supportedControllerType, handedness, thisProfile.MixedRealityControllerMappingProfiles[i].Interactions));

                var handednessTitleText = handedness != Handedness.None ? $"{handedness} Hand " : string.Empty;
                var controllerTitle = $"{supportedControllerType.ToString().ToProperCase()} {handednessTitleText}Controller";

                if (useCustomInteractionMappings.boolValue)
                {
                    GUILayout.Space(24f);

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();

                    EditorGUIUtility.labelWidth = 64f;
                    EditorGUIUtility.fieldWidth = 64f;
                    EditorGUILayout.LabelField(controllerTitle);
                    EditorGUIUtility.fieldWidth = defaultFieldWidth;
                    EditorGUIUtility.labelWidth = defaultLabelWidth;

                    if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                    {
                        controllerList.DeleteArrayElementAtIndex(i);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;

                    EditorGUIUtility.labelWidth = 128f;
                    EditorGUIUtility.fieldWidth = 64f;

                    EditorGUI.BeginChangeCheck();

                    int currentGenericType = -1;

                    if (controllerType == typeof(GenericJoystickController))
                    {
                        currentGenericType = 0;
                    }

                    if (controllerType == typeof(GenericOpenVRController))
                    {
                        currentGenericType = 1;
                    }

                    Debug.Assert(currentGenericType != -1);

                    currentGenericType = EditorGUILayout.IntPopup(GenericTypeContent, currentGenericType, GenericTypeListContent, GenericTypeIds);

                    if (controllerType != typeof(GenericJoystickController))
                    {
                        EditorGUILayout.PropertyField(controllerHandedness);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (currentGenericType)
                        {
                            case 0:
                                controllerType = typeof(GenericJoystickController);
                                controllerHandedness.intValue = 0;
                                break;
                            case 1:
                                controllerType = typeof(GenericOpenVRController);
                                break;
                        }

                        interactionsList.ClearArray();
                        serializedObject.ApplyModifiedProperties();
                        thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type = controllerType;
                        GUILayout.EndVertical();
                        return;
                    }

                    if (interactionsList.arraySize == 0 && controllerType == typeof(GenericOpenVRController))
                    {
                        thisProfile.MixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping(true);
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUIUtility.labelWidth = defaultLabelWidth;
                    EditorGUIUtility.fieldWidth = defaultFieldWidth;

                    EditorGUI.indentLevel--;

                    if (GUILayout.Button("Edit Input Action Map"))
                    {
                        ControllerPopupWindow.Show(supportedControllerType, interactionsList, (Handedness)controllerHandedness.intValue);
                    }

                    if (GUILayout.Button("Reset Input Actions"))
                    {
                        interactionsList.ClearArray();
                        serializedObject.ApplyModifiedProperties();
                        thisProfile.MixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping(true);
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    if (supportedControllerType == SupportedControllerType.WindowsMixedReality &&
                        handedness == Handedness.None)
                    {
                        controllerTitle = "HoloLens Gestures";
                    }

                    if (handedness != Handedness.Right)
                    {
                        GUILayout.BeginHorizontal();
                    }

                    var buttonContent = new GUIContent(controllerTitle, ControllerMappingLibrary.GetControllerTextureScaled(supportedControllerType, handedness));

                    if (GUILayout.Button(buttonContent, controllerButtonStyle, GUILayout.Height(128f), GUILayout.MinWidth(32f), GUILayout.ExpandWidth(true)))
                    {
                        ControllerPopupWindow.Show(supportedControllerType, interactionsList, (Handedness)controllerHandedness.intValue);
                    }

                    if (handedness != Handedness.Left)
                    {
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(8f);
            }

            GUILayout.EndVertical();
        }

        private void AddController(SerializedProperty controllerList, Type controllerType)
        {
            controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
            var index = controllerList.arraySize - 1;
            var mixedRealityControllerMapping = controllerList.GetArrayElementAtIndex(index);
            var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");
            mixedRealityControllerMappingDescription.stringValue = controllerType.Name;
            var mixedRealityControllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
            mixedRealityControllerHandedness.intValue = 0;
            var mixedRealityControllerInteractions = mixedRealityControllerMapping.FindPropertyRelative("interactions");
            var useCustomInteractionMappings = mixedRealityControllerMapping.FindPropertyRelative("useCustomInteractionMappings");
            useCustomInteractionMappings.boolValue = controllerType == typeof(GenericOpenVRController) || controllerType == typeof(GenericJoystickController);
            mixedRealityControllerInteractions.ClearArray();
            serializedObject.ApplyModifiedProperties();
            thisProfile.MixedRealityControllerMappingProfiles[index].ControllerType.Type = controllerType;
            thisProfile.MixedRealityControllerMappingProfiles[index].SetDefaultInteractionMapping(true);
        }
    }
}