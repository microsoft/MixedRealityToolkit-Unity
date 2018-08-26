// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private struct ControllerRenderProfile
        {
            public SupportedControllerType ControllerType;
            public Handedness Handedness;
            public MixedRealityInteractionMapping[] Interactions;
            public bool UseDefaultModel;
            public Object OverrideModel;

            public ControllerRenderProfile(SupportedControllerType controllerType, Handedness handedness, MixedRealityInteractionMapping[] interactions, bool useDefaultModel, Object overrideModel)
            {
                ControllerType = controllerType;
                Handedness = handedness;
                Interactions = interactions;
                UseDefaultModel = useDefaultModel;
                OverrideModel = overrideModel;
            }
        }

        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Template");
        private static readonly GUIContent GenericTypeContent = new GUIContent("Generic Type");

        private static readonly GUIContent[] GenericTypeListContent =
        {
            new GUIContent("Unity Controller"),
            new GUIContent("Open VR Controller")
        };

        private static readonly int[] GenericTypeIds = { 0, 1 };

        private static MixedRealityControllerMappingProfile thisProfile;

        private SerializedProperty mixedRealityControllerMappingProfiles;
        private SerializedProperty renderMotionControllers;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;
        private float defaultLabelWidth;
        private float defaultFieldWidth;
        private GUIStyle controllerButtonStyle;

        private List<ControllerRenderProfile> controllerRenderList = new List<ControllerRenderProfile>();

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            mixedRealityControllerMappingProfiles = serializedObject.FindProperty("mixedRealityControllerMappingProfiles");

            if (!MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled ||
                 MixedRealityManager.Instance.ActiveProfile.InputActionsProfile == null)
            {
                return;
            }

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandModel");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandModel");

            thisProfile = target as MixedRealityControllerMappingProfile;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Controller Templates", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Controller templates define all the controllers your users will be able to use in your application.\n\n" +
                                    "After defining all your Input Actions, you can then wire them up to hardware sensors, controllers, and other input devices.", MessageType.Info);

            if (!MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);
                return;
            }

            if (MixedRealityManager.Instance.ActiveProfile.InputActionsProfile == null)
            {
                EditorGUILayout.HelpBox("No input actions found, please specify a input action profile in the main configuration.", MessageType.Error);
                return;
            }

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

            EditorGUIUtility.labelWidth = 152f;
            EditorGUILayout.PropertyField(renderMotionControllers);

            if (renderMotionControllers.boolValue)
            {
                EditorGUILayout.PropertyField(useDefaultModels);

                if (!useDefaultModels.boolValue)
                {
                    EditorGUILayout.PropertyField(globalLeftHandModel);
                    EditorGUILayout.PropertyField(globalRightHandModel);
                }
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            RenderControllerProfilesList(mixedRealityControllerMappingProfiles, renderMotionControllers.boolValue);

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerProfilesList(SerializedProperty controllerList, bool renderControllerModels)
        {
            if (thisProfile.MixedRealityControllerMappingProfiles.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
                var mixedRealityControllerMapping = controllerList.GetArrayElementAtIndex(controllerList.arraySize - 1);
                var mixedRealityControllerMappingId = mixedRealityControllerMapping.FindPropertyRelative("id");
                var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = $"Generic Unity Controller {mixedRealityControllerMappingId.intValue = controllerList.arraySize}";
                var mixedRealityControllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                mixedRealityControllerHandedness.intValue = 0;
                var mixedRealityControllerInteractions = mixedRealityControllerMapping.FindPropertyRelative("interactions");
                var useCustomInteractionMappings = mixedRealityControllerMapping.FindPropertyRelative("useCustomInteractionMappings");
                useCustomInteractionMappings.boolValue = true;
                mixedRealityControllerInteractions.ClearArray();
                serializedObject.ApplyModifiedProperties();
                thisProfile.MixedRealityControllerMappingProfiles[controllerList.arraySize - 1].ControllerType.Type = typeof(GenericUnityController);
                return;
            }

            GUILayout.Space(12f);

            controllerRenderList.Clear();

            GUILayout.BeginVertical();

            for (int i = 0; i < controllerList.arraySize; i++)
            {
                var controllerType = thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type;
                var supportedControllerType = SupportedControllerType.None;
                var mixedRealityControllerMapping = controllerList.GetArrayElementAtIndex(i);
                var controllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                var handedness = (Handedness)controllerHandedness.intValue;
                var interactionsList = mixedRealityControllerMapping.FindPropertyRelative("interactions");
                var useDefaultModel = mixedRealityControllerMapping.FindPropertyRelative("useDefaultModel");
                var controllerModel = mixedRealityControllerMapping.FindPropertyRelative("overrideModel");
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
                else if (controllerType == typeof(GenericUnityController))
                {
                    supportedControllerType = SupportedControllerType.GenericUnity;
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
                        useDefaultModel.boolValue = controllerRenderList[j].UseDefaultModel;
                        controllerModel.objectReferenceValue = controllerRenderList[j].OverrideModel;
                        serializedObject.ApplyModifiedProperties();
                        skip = true;
                    }
                }

                if (skip) { continue; }

                controllerRenderList.Add(new ControllerRenderProfile(supportedControllerType, handedness, thisProfile.MixedRealityControllerMappingProfiles[i].Interactions, useDefaultModel.boolValue, controllerModel.objectReferenceValue));

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

                    if (controllerType == typeof(GenericUnityController))
                    {
                        currentGenericType = 0;
                    }

                    if (controllerType == typeof(GenericOpenVRController))
                    {
                        currentGenericType = 1;
                    }

                    Debug.Assert(currentGenericType != -1);

                    currentGenericType = EditorGUILayout.IntPopup(GenericTypeContent, currentGenericType, GenericTypeListContent, GenericTypeIds);

                    if (controllerType != typeof(GenericUnityController))
                    {
                        EditorGUILayout.PropertyField(controllerHandedness);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (currentGenericType)
                        {
                            case 0:
                                controllerType = typeof(GenericUnityController);
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

                    if (renderControllerModels && controllerHandedness.intValue != 0)
                    {
                        EditorGUILayout.PropertyField(useDefaultModel);

                        if (!useDefaultModel.boolValue)
                        {
                            EditorGUILayout.PropertyField(controllerModel);
                        }
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
    }
}