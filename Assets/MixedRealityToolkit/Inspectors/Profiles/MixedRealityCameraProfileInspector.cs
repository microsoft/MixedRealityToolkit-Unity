// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.CameraSystem;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityCameraProfile))]
    public class MixedRealityCameraProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private bool showProviders = false;
        private const string showProvidersPreferenceKey = "ShowCameraSystem_DataProviders_PreferenceKey";
        private SerializedProperty providerConfigurations;

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

        private static readonly GUIContent addSettingsProviderTitle  = new GUIContent("+ Add Camera Settings Provider", "Add Camera Settings Provider");
        private static readonly GUIContent removeSettingsProviderTitle = new GUIContent("-", "Remove Camera Settings Provider");

        private static readonly GUIContent componentType = new GUIContent("Type");
        private static readonly GUIContent supportedPlatformsTitle = new GUIContent("Supported Platform(s)");

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent farClipTitle = new GUIContent("Far Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");
        private readonly GUIContent backgroundColorTitle = new GUIContent("Background Color");

        private const string profileTitle = "Camera Settings";
        private const string profileDescription = "The Camera Profile helps configure cross platform camera settings.";

        private static bool[] providerFoldouts;

        protected override void OnEnable()
        {
            base.OnEnable();

            providerConfigurations = serializedObject.FindProperty("settingsConfigurations");
            if (providerFoldouts == null || providerFoldouts.Length != providerConfigurations.arraySize)
            {
                providerFoldouts = new bool[providerConfigurations.arraySize];
            }

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

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(profileTitle, profileDescription, target))
            {
                return;
            }

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                RenderFoldout(ref showProviders, "Camera Settings Providers", () =>
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        RenderList(providerConfigurations);
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

        private void RenderList(SerializedProperty list)
        {
            bool changed = false;

            using (new EditorGUILayout.VerticalScope())
            {
                if (list == null || list.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("The Mixed Reality Camera System will use default settings.\nAdd a settings provider to customize the camera.", MessageType.Info);
                }

                if (InspectorUIUtility.RenderIndentedButton(addSettingsProviderTitle, EditorStyles.miniButton))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                    SerializedProperty provider = list.GetArrayElementAtIndex(list.arraySize - 1);

                    SerializedProperty providerName = provider.FindPropertyRelative("componentName");
                    providerName.stringValue = $"New camera settings {list.arraySize - 1}";

                    SerializedProperty runtimePlatform = provider.FindPropertyRelative("runtimePlatform");
                    runtimePlatform.intValue = -1;

                    SerializedProperty configurationProfile = provider.FindPropertyRelative("settingsProfile");
                    configurationProfile.objectReferenceValue = null;

                    serializedObject.ApplyModifiedProperties();

                    SystemType providerType = ((MixedRealityCameraProfile)serializedObject.targetObject).SettingsConfigurations[list.arraySize - 1].ComponentType;
                    providerType.Type = null;

                    providerFoldouts = new bool[list.arraySize];
                    return;
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty provider = list.GetArrayElementAtIndex(i);
                    SerializedProperty providerName = provider.FindPropertyRelative("componentName");
                    SerializedProperty providerType = provider.FindPropertyRelative("componentType");
                    SerializedProperty providerProfile = provider.FindPropertyRelative("settingsProfile");
                    SerializedProperty runtimePlatform = provider.FindPropertyRelative("runtimePlatform");

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            providerFoldouts[i] = EditorGUILayout.Foldout(providerFoldouts[i], providerName.stringValue, true);

                            if (GUILayout.Button(removeSettingsProviderTitle, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                            {
                                list.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                changed = true;
                                break;
                            }
                        }

                        if (providerFoldouts[i])
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(providerType, componentType);
                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                System.Type type = ((MixedRealityCameraProfile)serializedObject.targetObject).SettingsConfigurations[i].ComponentType.Type;
                                ApplyProviderConfiguration(type, providerName, providerProfile, runtimePlatform);
                                changed = true;
                                break;
                            }

                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(runtimePlatform, supportedPlatformsTitle);
                            changed |= EditorGUI.EndChangeCheck();

                            var serviceType = (target as MixedRealityCameraProfile).SettingsConfigurations[i].ComponentType;

                            changed |= RenderProfile(providerProfile, typeof(BaseCameraSettingsProfile), true, false, serviceType);

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }

                if (changed && MixedRealityToolkit.IsInitialized)
                {
                    EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }
        }
        private void ApplyProviderConfiguration(
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

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.CameraProfile;
        }
    }
}
