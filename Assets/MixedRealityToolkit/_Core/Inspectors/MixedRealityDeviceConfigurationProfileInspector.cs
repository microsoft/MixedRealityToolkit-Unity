// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityDevicesConfigurationProfile))]
    public class MixedRealityDeviceConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove Device Template");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New Device Template", "Add New Device Template");

        private SerializedProperty deviceTemplateList;

        private void OnEnable()
        {
            deviceTemplateList = serializedObject.FindProperty("deviceTemplates");
            var deviceConfigProfile = (MixedRealityDevicesConfigurationProfile)target;
            Debug.Assert(deviceConfigProfile.DeviceTemplates != null);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Device Templates", EditorStyles.boldLabel);

            CheckMixedRealityManager();

            EditorGUILayout.HelpBox("Device templates define all the devices your users will be able to use in your application.\n\n" +
                                    "After defining all your Input Actions, you can then wire them up to hardware sensors, controllers, and other input devices.", MessageType.Info);

            RenderList(deviceTemplateList);

            serializedObject.ApplyModifiedProperties();
        }

        private static void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var deviceTemplate = list.GetArrayElementAtIndex(list.arraySize - 1);
                var deviceTemplateId = deviceTemplate.FindPropertyRelative("id");
                var deviceTemplateDescription = deviceTemplate.FindPropertyRelative("description");
                deviceTemplateDescription.stringValue = $"New Device Template {deviceTemplateId.intValue = list.arraySize}";
            }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            for (int i = 0; i < list?.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 64f;
                SerializedProperty deviceTemplate = list.GetArrayElementAtIndex(i);
                SerializedProperty deviceTemplateDescription = deviceTemplate.FindPropertyRelative("description");
                SerializedProperty deviceTemplateId = deviceTemplate.FindPropertyRelative("id");
                EditorGUILayout.PropertyField(deviceTemplateDescription, new GUIContent($"Device {deviceTemplateId.intValue = i + 1}"));
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}
