// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityRegisteredServiceProvidersProfile))]
    public class MixedRealityRegisteredServiceProviderProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Unregister");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Register a new Service Provider");
        private SerializedProperty configurations;

        private static bool[] configFoldouts;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            configurations = serializedObject.FindProperty("configurations");
            configFoldouts = new bool[configurations.arraySize];
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Service Providers Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This profile defines any additional Services like systems, features, and managers to register with the Mixed Reality Toolkit.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();
            RenderList(configurations);
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
                SerializedProperty managerConfig = list.GetArrayElementAtIndex(list.arraySize - 1);
                var componentName = managerConfig.FindPropertyRelative("componentName");
                componentName.stringValue = $"New Configuration {list.arraySize - 1}";
                var priority = managerConfig.FindPropertyRelative("priority");
                priority.intValue = 10;
                var runtimePlatform = managerConfig.FindPropertyRelative("runtimePlatform");
                runtimePlatform.intValue = -1;
                var configurationProfile = managerConfig.FindPropertyRelative("configurationProfile");
                configurationProfile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                var componentType = ((MixedRealityRegisteredServiceProvidersProfile)serializedObject.targetObject).Configurations[list.arraySize - 1].ComponentType;
                componentType.Type = null;
                configFoldouts = new bool[list.arraySize];
                return;
            }

            GUILayout.Space(12f);

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Register a new Service Provider.", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Configurations", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty managerConfig = list.GetArrayElementAtIndex(i);
                var componentName = managerConfig.FindPropertyRelative("componentName");
                var componentType = managerConfig.FindPropertyRelative("componentType");
                var priority = managerConfig.FindPropertyRelative("priority");
                var runtimePlatform = managerConfig.FindPropertyRelative("runtimePlatform");
                var configurationProfile = managerConfig.FindPropertyRelative("configurationProfile");

                GUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();

                configFoldouts[i] = EditorGUILayout.Foldout(configFoldouts[i], componentName.stringValue, true);

                if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (configFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(componentName);
                    EditorGUILayout.PropertyField(componentType);
                    EditorGUILayout.PropertyField(priority);
                    EditorGUILayout.PropertyField(runtimePlatform);
                    EditorGUILayout.PropertyField(configurationProfile);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                    }

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
                GUILayout.Space(12f);
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}