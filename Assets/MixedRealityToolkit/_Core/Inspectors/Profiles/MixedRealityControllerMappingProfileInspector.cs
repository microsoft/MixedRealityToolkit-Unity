// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Custom Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Template");

        private static MixedRealityControllerMappingProfile thisProfile;

        private SerializedProperty mixedRealityControllerMappingProfiles;
        private SerializedProperty renderMotionControllers;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;


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

            serializedObject.Update();

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

            RenderControllerProfilesList(mixedRealityControllerMappingProfiles);

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerProfilesList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
                var mixedRealityControllerMapping = list.GetArrayElementAtIndex(list.arraySize - 1);
                var mixedRealityControllerMappingId = mixedRealityControllerMapping.FindPropertyRelative("id");
                var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = $"New Controller Template {mixedRealityControllerMappingId.intValue = list.arraySize}";
                var mixedRealityControllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                mixedRealityControllerHandedness.intValue = 0;
                var mixedRealityControllerInteractions = mixedRealityControllerMapping.FindPropertyRelative("interactions");
                mixedRealityControllerInteractions.ClearArray();
                serializedObject.ApplyModifiedProperties();

                thisProfile.MixedRealityControllerMappingProfiles[list.arraySize - 1].ControllerType.Type = null;

                serializedObject.ApplyModifiedProperties();
                return;
            }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            if (list == null || list.arraySize == 0)
            {
                // TODO Auto populate with known controller Definitions?
            }

            for (int i = 0; i < list?.arraySize; i++)
            {
                GUILayout.BeginVertical();

                var previousLabelWidth = EditorGUIUtility.labelWidth;
                var mixedRealityControllerMapping = list.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 96f;
                EditorGUILayout.PropertyField(mixedRealityControllerMappingDescription);
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();

                    var controllerTypeProperty = mixedRealityControllerMapping.FindPropertyRelative("controllerType");
                    var controllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                    var useDefaultModel = mixedRealityControllerMapping.FindPropertyRelative("useDefaultModel");
                    var controllerModel = mixedRealityControllerMapping.FindPropertyRelative("overrideModel");
                    var interactionsList = mixedRealityControllerMapping.FindPropertyRelative("interactions");
                    var useCustomInteractionMappings = mixedRealityControllerMapping.FindPropertyRelative("useCustomInteractionMappings");

                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 128f;

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(controllerTypeProperty);
                    EditorGUILayout.PropertyField(controllerHandedness);
                    EditorGUIUtility.labelWidth = 224f;

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();

                        if (thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type == null)
                        {
                            controllerHandedness.intValue = 0;
                        }

                        // Only allow custom interaction mappings on generic controller types
                        var controllerType = thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type;
                        useCustomInteractionMappings.boolValue = controllerType == typeof(GenericUnityController) ||
                                                                 controllerType == typeof(GenericOpenVRController);
                        interactionsList.ClearArray();
                        serializedObject.ApplyModifiedProperties();
                        thisProfile.MixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping();
                        serializedObject.ApplyModifiedProperties();

                        // Bail so we can execute our controller type set command.
                        EditorGUI.indentLevel--;
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        return;
                    }

                    EditorGUIUtility.labelWidth = 128f;
                    EditorGUILayout.PropertyField(useDefaultModel);

                    if (!useDefaultModel.boolValue)
                    {
                        EditorGUILayout.PropertyField(controllerModel);
                    }

                    EditorGUIUtility.labelWidth = previousLabelWidth;

                    if (GUILayout.Button("Edit Controller Input Actions"))
                    {
                        var controllerType = thisProfile.MixedRealityControllerMappingProfiles[i].ControllerType.Type;

                        if (controllerType == typeof(XboxController))
                        {
                            ControllerPopupWindow.Show(SupportedControllerType.XboxController, interactionsList);
                        }
                        else if (controllerType == typeof(WindowsMixedRealityController) ||
                                 controllerType == typeof(WindowsMixedRealityOpenVRMotionController))
                        {
                            ControllerPopupWindow.Show(SupportedControllerType.WindowsMixedReality, interactionsList, (Handedness)controllerHandedness.intValue);
                        }
                        else if (controllerType == typeof(OculusTouchController))
                        {
                            ControllerPopupWindow.Show(SupportedControllerType.OculusTouch, interactionsList, (Handedness)controllerHandedness.intValue);
                        }
                        else if (controllerType == typeof(ViveWandController))
                        {
                            ControllerPopupWindow.Show(SupportedControllerType.ViveWand, interactionsList, (Handedness)controllerHandedness.intValue);
                        }
                        else if (controllerType == typeof(GenericOpenVRController))
                        {
                            ControllerPopupWindow.Show(SupportedControllerType.GenericOpenVR, interactionsList, (Handedness)controllerHandedness.intValue);
                        }
                    }

                    if (useCustomInteractionMappings.boolValue)
                    {
                        if (GUILayout.Button("Reset Input Actions"))
                        {
                            interactionsList.ClearArray();
                            serializedObject.ApplyModifiedProperties();
                            thisProfile.MixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping();
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    GUILayout.Space(24f);
                    GUILayout.BeginHorizontal();
                    GUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}