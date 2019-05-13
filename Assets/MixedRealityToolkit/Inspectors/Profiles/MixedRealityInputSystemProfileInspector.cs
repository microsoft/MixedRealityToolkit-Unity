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
        private SerializedProperty dataProviderConfigurations;

        private SerializedProperty focusProviderType;

        private static bool showPointerProperties = false;
        private SerializedProperty pointerProfile;

        private static bool showActionsProperties = false;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;

        private static bool showControllerProperties = false;
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;
        private SerializedProperty controllerVisualizationProfile;

        private static bool showGestureProperties = false;
        private SerializedProperty gesturesProfile;

        private static bool showSpeechCommandsProperties = false;
        private SerializedProperty speechCommandsProfile;

        private static bool showHandTrackingProperties = false;
        private SerializedProperty handTrackingProfile;

        private static bool[] providerFoldouts;
        private const string ProfileTitle = "Input System Settings";
        private const string ProfileDescription = "The Input System Profile helps developers configure input for cross-platform applications.";

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

            if (providerFoldouts == null || providerFoldouts.Length != dataProviderConfigurations.arraySize)
            {
                providerFoldouts = new bool[dataProviderConfigurations.arraySize];
            }
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, string.Empty))
            {
                return;
            }

            // TODO: Troy
            /*
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(true, !RenderAsSubProfile))
            {
                return;
            }*/

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = wasGUIEnabled && !CheckProfileLock((BaseMixedRealityProfile)target);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            bool changed = false;

            EditorGUILayout.PropertyField(focusProviderType);
            EditorGUILayout.Space();

            bool isSubProfile = RenderAsSubProfile;
            if (!isSubProfile)
            {
                EditorGUI.indentLevel++;
            }

            RenderFoldout(ref showDataProviders, "Data Providers", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(dataProviderConfigurations);
                }
            });

            RenderFoldout(ref showPointerProperties, "Pointer Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(pointerProfile, true, false);
                }
            });

            RenderFoldout(ref showActionsProperties, "Action Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(inputActionsProfile, true, false);
                    EditorGUILayout.Space();
                    changed |= RenderProfile(inputActionRulesProfile, true, false);
                }
            });

            RenderFoldout(ref showControllerProperties, "Controller Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableControllerMapping);
                    changed |= RenderProfile(controllerMappingProfile, true, false);
                    EditorGUILayout.Space();
                    changed |= RenderProfile(controllerVisualizationProfile, true, false, typeof(IMixedRealityControllerVisualizer));
                }
            });

            RenderFoldout(ref showGestureProperties, "Gesture Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(gesturesProfile, true, false);
                }
            });

            RenderFoldout(ref showSpeechCommandsProperties, "Speech Commands Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(speechCommandsProfile, true, false);
                }
            });

            RenderFoldout(ref showHandTrackingProperties, "Hand Tracking Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(handTrackingProfile, true, false);
                }
            });

            if (!isSubProfile)
            {
                EditorGUI.indentLevel--;
            }

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = wasGUIEnabled;

            if (changed && MixedRealityToolkit.IsInitialized)
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

                        // TODO: TROY
                        if (providerFoldouts[i])//|| RenderAsSubProfile)
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

                                changed |= RenderProfile(configurationProfile, true, false, serviceType);
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