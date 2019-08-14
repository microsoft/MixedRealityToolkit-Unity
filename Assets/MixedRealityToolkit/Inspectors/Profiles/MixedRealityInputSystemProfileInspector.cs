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

        private static bool showDataProviders = false;
        private const string ShowInputSystem_DataProviders_PreferenceKey = "ShowInputSystem_DataProviders_PreferenceKey";
        private SerializedProperty dataProviderConfigurations;

        private SerializedProperty focusProviderType;
        private SerializedProperty focusQueryBufferSize;
        private SerializedProperty raycastProviderType;
        private SerializedProperty focusIndividualCompoundCollider;

        private static bool showPointerProperties = false;
        private const string ShowInputSystem_Pointers_PreferenceKey = "ShowInputSystem_Pointers_PreferenceKey";
        private SerializedProperty pointerProfile;

        private static bool showActionsProperties = false;
        private const string ShowInputSystem_Actions_PreferenceKey = "ShowInputSystem_Actions_PreferenceKey";
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;

        private static bool showControllerProperties = false;
        private const string ShowInputSystem_Controller_PreferenceKey = "ShowInputSystem_Controller_PreferenceKey";
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;
        private SerializedProperty controllerVisualizationProfile;

        private static bool showGestureProperties = false;
        private const string ShowInputSystem_Gesture_PreferenceKey = "ShowInputSystem_Gesture_PreferenceKey";
        private SerializedProperty gesturesProfile;

        private static bool showSpeechCommandsProperties = false;
        private const string ShowInputSystem_Speech_PreferenceKey = "ShowInputSystem_Speech_PreferenceKey";
        private SerializedProperty speechCommandsProfile;

        private static bool showHandTrackingProperties = false;
        private const string ShowInputSystem_HandTracking_PreferenceKey = "ShowInputSystem_HandTracking_PreferenceKey";
        private SerializedProperty handTrackingProfile;

        private static bool[] providerFoldouts;
        private const string ProfileTitle = "Input System Settings";
        private const string ProfileDescription = "The Input System Profile helps developers configure input for cross-platform applications.";

        protected override void OnEnable()
        {
            base.OnEnable();

            dataProviderConfigurations = serializedObject.FindProperty("dataProviderConfigurations");
            focusProviderType = serializedObject.FindProperty("focusProviderType");
            focusQueryBufferSize = serializedObject.FindProperty("focusQueryBufferSize");
            raycastProviderType = serializedObject.FindProperty("raycastProviderType");
            focusIndividualCompoundCollider = serializedObject.FindProperty("focusIndividualCompoundCollider");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
            handTrackingProfile = serializedObject.FindProperty("handTrackingProfile");

            if (providerFoldouts == null || providerFoldouts.Length != dataProviderConfigurations.arraySize)
            {
                providerFoldouts = new bool[dataProviderConfigurations.arraySize];
            }
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, string.Empty, target);

            bool changed = false;
            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(focusProviderType);
                EditorGUILayout.PropertyField(focusQueryBufferSize);
                EditorGUILayout.PropertyField(raycastProviderType);
                EditorGUILayout.PropertyField(focusIndividualCompoundCollider);
                changed |= EditorGUI.EndChangeCheck();

                EditorGUILayout.Space();

                bool isSubProfile = RenderAsSubProfile;
                if (!isSubProfile)
                {
                    EditorGUI.indentLevel++;
                }

                RenderFoldout(ref showDataProviders, "Input Data Providers", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        RenderList(dataProviderConfigurations);
                    }
                }, ShowInputSystem_DataProviders_PreferenceKey);

                RenderFoldout(ref showPointerProperties, "Pointers", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfile(pointerProfile, typeof(MixedRealityPointerProfile), true, false);
                    }
                }, ShowInputSystem_Pointers_PreferenceKey);

                RenderFoldout(ref showActionsProperties, "Input Actions", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfile(inputActionsProfile, typeof(MixedRealityInputActionsProfile), true, false);
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        changed |= RenderProfile(inputActionRulesProfile, typeof(MixedRealityInputActionRulesProfile), true, false);
                    }
                }, ShowInputSystem_Actions_PreferenceKey);

                RenderFoldout(ref showControllerProperties, "Controllers", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(enableControllerMapping);
                        changed |= RenderProfile(controllerMappingProfile, typeof(MixedRealityControllerMappingProfile), true, false);
                        EditorGUILayout.Space();
                        changed |= RenderProfile(controllerVisualizationProfile, null, true, false, typeof(IMixedRealityControllerVisualizer));
                    }
                }, ShowInputSystem_Controller_PreferenceKey);

                RenderFoldout(ref showGestureProperties, "Gestures", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfile(gesturesProfile, typeof(MixedRealityGesturesProfile), true, false);
                    }
                }, ShowInputSystem_Gesture_PreferenceKey);

                RenderFoldout(ref showSpeechCommandsProperties, "Speech", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfile(speechCommandsProfile, typeof(MixedRealitySpeechCommandsProfile), true, false);
                    }
                }, ShowInputSystem_Speech_PreferenceKey);

                RenderFoldout(ref showHandTrackingProperties, "Hand Tracking", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfile(handTrackingProfile, typeof(MixedRealityHandTrackingProfile), true, false);
                    }
                }, ShowInputSystem_HandTracking_PreferenceKey);

                if (!isSubProfile)
                {
                    EditorGUI.indentLevel--;
                }

                serializedObject.ApplyModifiedProperties();
            }

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
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

                EditorGUILayout.Space();

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

                        if (providerFoldouts[i])
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
                                    changed = true;
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

                                changed |= RenderProfile(configurationProfile, null, true, false, serviceType);
                            }

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }

            if (changed && MixedRealityToolkit.IsInitialized)
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