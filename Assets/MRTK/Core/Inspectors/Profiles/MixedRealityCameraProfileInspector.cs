// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Class handles rendering inspector view of MixedRealityCameraProfile object
    /// </summary>
    [CustomEditor(typeof(MixedRealityCameraProfile))]
    public class MixedRealityCameraProfileInspector : BaseDataProviderServiceInspector
    {
        private bool showProviders = false;
        private const string showProvidersPreferenceKey = "ShowCameraSystem_DataProviders_PreferenceKey";

        private bool showDisplaySettings = false;
        private const string showDisplaySettingsPreferenceKey = "ShowCameraSystem_DisplaySettings_PreferenceKey";

        private SerializedProperty opaqueNearClip;
        private SerializedProperty opaqueFarClip;
        private SerializedProperty opaqueClearFlags;
        private SerializedProperty opaqueBackgroundColor;
        private SerializedProperty opaqueQualityLevel;

        private SerializedProperty transparentNearClip;
        private SerializedProperty transparentFarClip;
        private SerializedProperty transparentClearFlags;
        private SerializedProperty transparentBackgroundColor;
        private SerializedProperty transparentQualityLevel;

        private const string DataProviderErrorMsg = "The Mixed Reality Camera System will use default settings.\nAdd a settings provider to customize the camera.";
        private static readonly GUIContent AddProviderTitle = new GUIContent("+ Add Camera Settings Provider", "Add Camera Settings Provider");
        private static readonly GUIContent RemoveProviderTitle = new GUIContent("-", "Remove Camera Settings Provider");

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent farClipTitle = new GUIContent("Far Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");
        private readonly GUIContent backgroundColorTitle = new GUIContent("Background Color");

        private const string profileTitle = "Camera Settings";
        private const string profileDescription = "The Camera Profile helps configure cross platform camera settings.";

        /// <inheritdoc/>
        protected override void OnEnable()
        {
            base.OnEnable();

            opaqueNearClip = serializedObject.FindProperty("nearClipPlaneOpaqueDisplay");
            opaqueFarClip = serializedObject.FindProperty("farClipPlaneOpaqueDisplay");
            opaqueClearFlags = serializedObject.FindProperty("cameraClearFlagsOpaqueDisplay");
            opaqueBackgroundColor = serializedObject.FindProperty("backgroundColorOpaqueDisplay");
            opaqueQualityLevel = serializedObject.FindProperty("opaqueQualityLevel");

            transparentNearClip = serializedObject.FindProperty("nearClipPlaneTransparentDisplay");
            transparentFarClip = serializedObject.FindProperty("farClipPlaneTransparentDisplay");
            transparentClearFlags = serializedObject.FindProperty("cameraClearFlagsTransparentDisplay");
            transparentBackgroundColor = serializedObject.FindProperty("backgroundColorTransparentDisplay");
            transparentQualityLevel = serializedObject.FindProperty("transparentQualityLevel");
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(profileTitle, profileDescription, target))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                RenderFoldout(ref showProviders, "Camera Settings Providers", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        bool changed = RenderDataProviderList(AddProviderTitle, RemoveProviderTitle, DataProviderErrorMsg, typeof(BaseCameraSettingsProfile));

                        if (changed && MixedRealityToolkit.IsInitialized)
                        {
                            EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                        }
                    }
                }, showProvidersPreferenceKey);

                RenderFoldout(ref showDisplaySettings, "Display Settings", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("Opaque", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
                        EditorGUILayout.PropertyField(opaqueFarClip, farClipTitle);
                        EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);

                        if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
                        {
                            EditorGUILayout.PropertyField(opaqueBackgroundColor, backgroundColorTitle);
                        }

                        opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Transparent", EditorStyles.boldLabel);

                        EditorGUILayout.PropertyField(transparentNearClip, nearClipTitle);
                        EditorGUILayout.PropertyField(transparentFarClip, farClipTitle);
                        EditorGUILayout.PropertyField(transparentClearFlags, clearFlagsTitle);

                        if ((CameraClearFlags)transparentClearFlags.intValue == CameraClearFlags.Color)
                        {
                            EditorGUILayout.PropertyField(transparentBackgroundColor, backgroundColorTitle);
                        }

                        transparentQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", transparentQualityLevel.intValue, QualitySettings.names);
                    }
                }, showDisplaySettingsPreferenceKey);

                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <inheritdoc/>
        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.CameraProfile;
        }

        #region DataProvider Inspector Utilities

        /// <inheritdoc/>
        protected override SerializedProperty GetDataProviderConfigurationList()
        {
            return serializedObject.FindProperty("settingsConfigurations");
        }

        /// <inheritdoc/>
        protected override ServiceConfigurationProperties GetDataProviderConfigurationProperties(SerializedProperty providerEntry)
        {
            return new ServiceConfigurationProperties()
            {
                componentName = providerEntry.FindPropertyRelative("componentName"),
                componentType = providerEntry.FindPropertyRelative("componentType"),
                providerProfile = providerEntry.FindPropertyRelative("settingsProfile"),
                runtimePlatform = providerEntry.FindPropertyRelative("runtimePlatform"),
            };
        }

        /// <inheritdoc/>
        protected override IMixedRealityServiceConfiguration GetDataProviderConfiguration(int index)
        {
            var profile = target as MixedRealityCameraProfile;
            var configurations = (profile != null) ? profile.SettingsConfigurations : null;
            if (configurations != null && index >= 0 && index < configurations.Length)
            {
                return configurations[index];
            }

            return null;
        }

        #endregion
    }
}
