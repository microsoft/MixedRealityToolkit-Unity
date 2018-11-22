// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerDataProvidersProfile))]
    public class MixedRealityControllerDataProviderProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent AddControllerDataProviderContent = new GUIContent("+ Add a New Controller Data Provider");
        private static readonly GUIContent RemoveControllerDataProviderContent = new GUIContent("-", "Remove Controller Data Provider");

        private SerializedProperty controllerDataProviders;

        private bool[] foldouts = null;

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

            controllerDataProviders = serializedObject.FindProperty("registeredControllerDataProviders");
            foldouts = new bool[controllerDataProviders.arraySize];
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
            EditorGUILayout.LabelField("Controller Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this profile to define all the input sources your application can get input data from.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();

            bool changed = false;

            if (GUILayout.Button(AddControllerDataProviderContent, EditorStyles.miniButton))
            {
                controllerDataProviders.arraySize += 1;
                var newConfiguration = controllerDataProviders.GetArrayElementAtIndex(controllerDataProviders.arraySize - 1);
                var dataProviderType = newConfiguration.FindPropertyRelative("dataProviderType");
                var dataProviderName = newConfiguration.FindPropertyRelative("dataProviderName");
                var priority = newConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = newConfiguration.FindPropertyRelative("runtimePlatform");

                serializedObject.ApplyModifiedProperties();
                dataProviderType.FindPropertyRelative("reference").stringValue = string.Empty;
                dataProviderName.stringValue = "New Controller Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[controllerDataProviders.arraySize];
                changed = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < controllerDataProviders.arraySize; i++)
            {
                var controllerConfiguration = controllerDataProviders.GetArrayElementAtIndex(i);
                var dataProviderName = controllerConfiguration.FindPropertyRelative("dataProviderName");
                var dataProviderType = controllerConfiguration.FindPropertyRelative("dataProviderType");
                var priority = controllerConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = controllerConfiguration.FindPropertyRelative("runtimePlatform");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], dataProviderName.stringValue, true);

                if (GUILayout.Button(RemoveControllerDataProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[controllerDataProviders.arraySize];
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(dataProviderType);
                    EditorGUILayout.PropertyField(dataProviderName);
                    EditorGUILayout.PropertyField(priority);
                    EditorGUILayout.PropertyField(runtimePlatform);

                    if (EditorGUI.EndChangeCheck())
                    {
                        changed = true;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }
    }
}