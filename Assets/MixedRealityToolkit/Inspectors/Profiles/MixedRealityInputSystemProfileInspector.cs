// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent AddProviderContent = new GUIContent("+ Add Data Provider", "Add Data Provider");
        private static readonly GUIContent RemoveProviderContent = new GUIContent("-", "Remove Data Provider");

        private static readonly GUIContent ComponentTypeContent = new GUIContent("Type");
        private static readonly GUIContent RuntimePlatformContent = new GUIContent("Platform(s)");
        private static readonly GUIContent ProviderProfileContent = new GUIContent("Profile");

        private static bool showDataProviders = true;
        private SerializedProperty dataProviderConfigurations;

        private static bool showFocusProperties = true;
        private SerializedProperty focusProviderType;

        private static bool showPointerProperties = true;
        private SerializedProperty pointerProfile;

        private static bool showActionsProperties = true;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;

        private static bool showControllerProperties = true;
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;
        private SerializedProperty controllerVisualizationProfile;

        private static bool showGestureProperties = true;
        private SerializedProperty gesturesProfile;

        private static bool showSpeechCommandsProperties = true;
        private SerializedProperty speechCommandsProfile;

        private static bool showHandTrackingProperties = true;
        private SerializedProperty handTrackingProfile;

        private static bool[] providerFoldouts;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            dataProviderConfigurations = serializedObject.FindProperty("dataProviderConfigurations");
            focusProviderType = serializedObject.FindProperty("focusProviderType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
            handTrackingProfile = serializedObject.FindProperty("handTrackingProfile");

            providerFoldouts = new bool[dataProviderConfigurations.arraySize];
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input for cross-platform applications.", MessageType.Info);

            CheckProfileLock(target);

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            EditorGUILayout.Space();
            showDataProviders = EditorGUILayout.Foldout(showDataProviders, "Data Providers", true);
            if (showDataProviders)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(dataProviderConfigurations);
                }
            }

            EditorGUILayout.Space();
            showFocusProperties = EditorGUILayout.Foldout(showFocusProperties, "Focus Settings", true);
            if (showFocusProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(focusProviderType);
                }
            }

            EditorGUILayout.Space();
            showPointerProperties = EditorGUILayout.Foldout(showPointerProperties, "Pointer Settings", true);
            if (showPointerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(pointerProfile);
                }
            }

            EditorGUILayout.Space();
            showActionsProperties = EditorGUILayout.Foldout(showActionsProperties, "Action Settings", true);
            if (showActionsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(inputActionsProfile);
                    changed |= RenderProfile(inputActionRulesProfile);
                }
            }

            EditorGUILayout.Space();
            showControllerProperties = EditorGUILayout.Foldout(showControllerProperties, "Controller Settings", true);
            if (showControllerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableControllerMapping);
                    changed |= RenderProfile(controllerMappingProfile);
                    changed |= RenderProfile(controllerVisualizationProfile, true, typeof(IMixedRealityControllerVisualizer));
                }
            }

            EditorGUILayout.Space();
            showGestureProperties = EditorGUILayout.Foldout(showGestureProperties, "Gesture Settings", true);
            if (showGestureProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(gesturesProfile);
                }
            }

            EditorGUILayout.Space();
            showSpeechCommandsProperties = EditorGUILayout.Foldout(showSpeechCommandsProperties, "Speech Command Settings", true);
            if (showSpeechCommandsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(speechCommandsProfile);
                }
            }

            EditorGUILayout.Space();
            showHandTrackingProperties = EditorGUILayout.Foldout(showHandTrackingProperties, "Hand Tracking Settings", true);
            if (showHandTrackingProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(handTrackingProfile);
                }
            }

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }

        private void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();

            bool changed = false;

            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button(AddProviderContent, EditorStyles.miniButton))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                    SerializedProperty dataProvider = list.GetArrayElementAtIndex(list.arraySize - 1);

                    SerializedProperty providerName = dataProvider.FindPropertyRelative("componentName");
                    providerName.stringValue = $"New data provider {list.arraySize - 1}";

                    SerializedProperty configurationProfile = dataProvider.FindPropertyRelative("deviceManagerProfile");
                    configurationProfile.objectReferenceValue = null;

                    SerializedProperty runtimePlatform = dataProvider.FindPropertyRelative("runtimePlatform");
                    runtimePlatform.intValue = -1;

                    serializedObject.ApplyModifiedProperties();

                    SystemType providerType = ((MixedRealityInputSystemProfile)serializedObject.targetObject).DataProviderConfigurations[list.arraySize - 1].ComponentType;
                    providerType.Type = null;

                    providerFoldouts = new bool[list.arraySize];

                    return;
                }

                GUILayout.Space(12f);

                if (list == null || list.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("The Mixed Reality Input System requires one or more data providers.", MessageType.Warning);
                    return;
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty dataProvider = list.GetArrayElementAtIndex(i);
                    SerializedProperty providerName = dataProvider.FindPropertyRelative("componentName");
                    SerializedProperty providerType = dataProvider.FindPropertyRelative("componentType");
                    SerializedProperty configurationProfile = dataProvider.FindPropertyRelative("deviceManagerProfile");
                    SerializedProperty runtimePlatform = dataProvider.FindPropertyRelative("runtimePlatform");

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            providerFoldouts[i] = EditorGUILayout.Foldout(providerFoldouts[i], providerName.stringValue, true);

                            if (GUILayout.Button(RemoveProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                            {
                                list.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                changed = true;
                                break;
                            }
                        }

                        if (providerFoldouts[i] || RenderAsSubProfile)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(providerType, ComponentTypeContent);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    serializedObject.ApplyModifiedProperties();
                                    System.Type type = ((MixedRealityInputSystemProfile)serializedObject.targetObject).DataProviderConfigurations[i].ComponentType.Type;
                                    ApplyDataProviderConfiguration(type, providerName, configurationProfile, runtimePlatform);
                                    break;
                                }

                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(runtimePlatform, RuntimePlatformContent);
                                changed |= EditorGUI.EndChangeCheck();

                                System.Type serviceType = null;
                                if (configurationProfile.objectReferenceValue != null)
                                {
                                    serviceType = (target as MixedRealityInputSystemProfile).DataProviderConfigurations[i].ComponentType;
                                }

                                changed |= RenderProfile(configurationProfile, ProviderProfileContent, true, serviceType);
                            }

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }

        private void ApplyDataProviderConfiguration(
            System.Type type, 
            SerializedProperty providerName,
            SerializedProperty configurationProfile,
            SerializedProperty runtimePlatform)
        {
            if (type != null)
            {
                MixedRealityDataProviderAttribute providerAttribute = MixedRealityDataProviderAttribute.Find(type) as MixedRealityDataProviderAttribute;
                if (providerAttribute != null)
                {
                    providerName.stringValue = !string.IsNullOrWhiteSpace(providerAttribute.Name) ? providerAttribute.Name : type.Name;
                    configurationProfile.objectReferenceValue = providerAttribute.DefaultProfile;
                    runtimePlatform.intValue = (int)providerAttribute.RuntimePlatforms;
                }
                else
                {
                    providerName.stringValue = type.Name;
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}