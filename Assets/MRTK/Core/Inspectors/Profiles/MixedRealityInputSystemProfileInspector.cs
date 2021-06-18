// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// Class handles rendering inspector view of MixedRealityInputSystemProfile object
    /// </summary>
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseDataProviderServiceInspector
    {
        private const string DataProviderErrorMsg = "The Mixed Reality Input System requires one or more data providers.";
        private static readonly GUIContent AddProviderContent = new GUIContent("+ Add Data Provider", "Add Data Provider");
        private static readonly GUIContent RemoveProviderContent = new GUIContent("-", "Remove Data Provider");

        private static bool showDataProviders = false;
        private const string ShowInputSystem_DataProviders_PreferenceKey = "ShowInputSystem_DataProviders_PreferenceKey";

        private SerializedProperty focusProviderType;
        private SerializedProperty focusQueryBufferSize;
        private SerializedProperty raycastProviderType;
        private SerializedProperty focusIndividualCompoundCollider;
        private SerializedProperty shouldUseGraphicsRaycast;

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

        private const string ProfileTitle = "Input System Settings";
        private const string ProfileDescription = "The Input System Profile helps developers configure input for cross-platform applications.";

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            focusProviderType = serializedObject.FindProperty("focusProviderType");
            focusQueryBufferSize = serializedObject.FindProperty("focusQueryBufferSize");
            raycastProviderType = serializedObject.FindProperty("raycastProviderType");
            focusIndividualCompoundCollider = serializedObject.FindProperty("focusIndividualCompoundCollider");
            shouldUseGraphicsRaycast = serializedObject.FindProperty("shouldUseGraphicsRaycast");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
            handTrackingProfile = serializedObject.FindProperty("handTrackingProfile");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target))
            {
                return;
            }

            bool changed = false;
            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(focusProviderType);
                EditorGUILayout.PropertyField(focusQueryBufferSize);
                EditorGUILayout.PropertyField(raycastProviderType);
                EditorGUILayout.PropertyField(focusIndividualCompoundCollider);
                EditorGUILayout.PropertyField(shouldUseGraphicsRaycast);
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
                        changed |= RenderDataProviderList(AddProviderContent, RemoveProviderContent, DataProviderErrorMsg);
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

                RenderFoldout(ref showHandTrackingProperties, "Articulated Hand Tracking", () =>
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

        /// <inheritdoc/>
        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
        }

        #region DataProvider Inspector Utilities

        /// <inheritdoc/>
        protected override SerializedProperty GetDataProviderConfigurationList()
        {
            return serializedObject.FindProperty("dataProviderConfigurations");
        }

        /// <inheritdoc/>
        protected override ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry)
        {
            return new ServiceConfigurationProperties()
            {
                componentName = providerEntry.FindPropertyRelative("componentName"),
                componentType = providerEntry.FindPropertyRelative("componentType"),
                providerProfile = providerEntry.FindPropertyRelative("deviceManagerProfile"),
                runtimePlatform = providerEntry.FindPropertyRelative("runtimePlatform"),
            };
        }

        /// <inheritdoc/>
        protected override IMixedRealityServiceConfiguration GetDataProviderConfiguration(int index)
        {
            MixedRealityInputSystemProfile profile = target as MixedRealityInputSystemProfile;
            var configurations = (profile != null) ? profile.DataProviderConfigurations : null;
            if (configurations != null && index >= 0 && index < configurations.Length)
            {
                return configurations[index];
            }

            return null;
        }

        #endregion
    }
}