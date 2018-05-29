// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityDevicesConfigurationProfile))]
    public class MixedRealityDeviceConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent DeviceAddButtonContent = new GUIContent("+ Add a New Device Template");
        private static readonly GUIContent DeviceMinusButtonContent = new GUIContent("-", "Remove Device Template");
        private static readonly GUIContent InteractionAddButtonContent = new GUIContent("+ Add a New Interaction Mapping");
        private static readonly GUIContent InteractionMinusButtonContent = new GUIContent("-", "Remove Interaction Mapping");
        private static readonly GUIContent AxisTypeContent = new GUIContent("Axis Type", "The axis type of the button, e.g. Analogue, Digital, etc.");
        private static readonly GUIContent DeviceInputTypeContent = new GUIContent("Input Type", "The primary action of the input as defined by the controller SDK.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "Action to be raised to the Input Manager when the input data has changed.");

        private static bool[] deviceFoldouts;

        private SerializedProperty deviceTemplates;
        private static GUIContent[] actionLabels;
        private static int[] actionIds;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager())
            {
                return;
            }

            deviceTemplates = serializedObject.FindProperty("deviceTemplates");
            if (deviceFoldouts == null || deviceFoldouts.Length != deviceTemplates.arraySize)
            {
                deviceFoldouts = new bool[deviceTemplates.arraySize];
            }

            actionLabels = MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions.Select(
                action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            actionIds = MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions.Select(
                action => (int)action.Id).Prepend(0).ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Device Templates", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Device templates define all the devices your users will be able to use in your application.\n\n" +
                                    "After defining all your Input Actions, you can then wire them up to hardware sensors, controllers, and other input devices.", MessageType.Info);

            RenderList(deviceTemplates);

            serializedObject.ApplyModifiedProperties();
        }

        private static void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(DeviceAddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var deviceTemplate = list.GetArrayElementAtIndex(list.arraySize - 1);
                var deviceTemplateId = deviceTemplate.FindPropertyRelative("id");
                var deviceTemplateDescription = deviceTemplate.FindPropertyRelative("description");
                deviceTemplateDescription.stringValue = $"New Device Template {deviceTemplateId.intValue = list.arraySize}";
                deviceFoldouts = new bool[list.arraySize];
                return;
            }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Device Template.", MessageType.Warning);
            }

            for (int i = 0; i < list?.arraySize; i++)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();

                var previousLabelWidth = EditorGUIUtility.labelWidth;
                var deviceTemplate = list.GetArrayElementAtIndex(i);
                var deviceTemplateDescription = deviceTemplate.FindPropertyRelative("description");
                var deviceTemplateId = deviceTemplate.FindPropertyRelative("id");

                EditorGUIUtility.labelWidth = 64f;
                EditorGUILayout.PropertyField(deviceTemplateDescription, new GUIContent($"Device {deviceTemplateId.intValue = i + 1}"));
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(DeviceMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();

                    var deviceSdkType = deviceTemplate.FindPropertyRelative("sdkType");
                    var deviceHandedness = deviceTemplate.FindPropertyRelative("handedness");
                    var deviceInteractionList = deviceTemplate.FindPropertyRelative("interactions");

                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 96f;
                    EditorGUILayout.PropertyField(deviceSdkType);
                    EditorGUILayout.PropertyField(deviceHandedness);
                    EditorGUIUtility.labelWidth = previousLabelWidth;

                    deviceFoldouts[i] = EditorGUILayout.Foldout(deviceFoldouts[i], new GUIContent("Interaction Mappings"), true);
                    if (deviceFoldouts[i])
                    {
                        GUILayout.BeginHorizontal();
                        RenderInteractionList(deviceInteractionList);
                        GUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
                GUILayout.Space(12f);
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private static void RenderInteractionList(SerializedProperty list)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(24f);
            if (GUILayout.Button(InteractionAddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var interaction = list.GetArrayElementAtIndex(list.arraySize - 1);
                var axisType = interaction.FindPropertyRelative("axisType");
                axisType.enumValueIndex = 0;
                var inputType = interaction.FindPropertyRelative("inputType");
                inputType.enumValueIndex = 0;
                var action = interaction.FindPropertyRelative("inputAction");
                var actionId = action.FindPropertyRelative("id");
                actionId.intValue = 0;
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a Interaction Mapping.", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            var prevFieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUILayout.LabelField("Id", GUILayout.Width(32f));
            EditorGUIUtility.labelWidth = 24f;
            EditorGUIUtility.fieldWidth = 24f;
            EditorGUILayout.LabelField(DeviceInputTypeContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(AxisTypeContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(ActionContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
            EditorGUIUtility.labelWidth = prevLabelWidth;
            EditorGUIUtility.fieldWidth = prevFieldWidth;
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty interaction = list.GetArrayElementAtIndex(i);

                EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(32f));
                EditorGUIUtility.labelWidth = 24f;
                EditorGUIUtility.fieldWidth = 24f;
                var inputType = interaction.FindPropertyRelative("inputType");
                EditorGUILayout.PropertyField(inputType, GUIContent.none, GUILayout.ExpandWidth(true));
                var axisType = interaction.FindPropertyRelative("axisType");
                EditorGUILayout.PropertyField(axisType, GUIContent.none, GUILayout.ExpandWidth(true));
                var action = interaction.FindPropertyRelative("inputAction");
                var actionId = action.FindPropertyRelative("id");
                actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, CheckValue(actionId.intValue, actionIds.Length), actionLabels, actionIds, GUILayout.ExpandWidth(true));

                if (GUILayout.Button(InteractionMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUIUtility.labelWidth = prevLabelWidth;
                EditorGUIUtility.fieldWidth = prevFieldWidth;
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private static int CheckValue(int value, int against)
        {
            if (value > against)
            {
                value = 0;
            }

            return value;
        }
    }
}
