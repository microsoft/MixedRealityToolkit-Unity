// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityCameraProfile))]
    public class MixedRealityCameraProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty opaqueNearClip;
        private SerializedProperty opaqueClearFlags;
        private SerializedProperty opaqueBackgroundColor;
        private SerializedProperty opaqueQualityLevel;

        private SerializedProperty transparentNearClip;
        private SerializedProperty transparentClearFlags;
        private SerializedProperty transparentBackgroundColor;
        private SerializedProperty holoLensQualityLevel;

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");

        private const string ProfileTitle = "Camera Settings";
        private const string ProfileDescription = "The Camera Profile helps configure cross platform camera settings.";

        protected override void OnEnable()
        {
            base.OnEnable();

            opaqueNearClip = serializedObject.FindProperty("nearClipPlaneOpaqueDisplay");
            opaqueClearFlags = serializedObject.FindProperty("cameraClearFlagsOpaqueDisplay");
            opaqueBackgroundColor = serializedObject.FindProperty("backgroundColorOpaqueDisplay");
            opaqueQualityLevel = serializedObject.FindProperty("opaqueQualityLevel");

            transparentNearClip = serializedObject.FindProperty("nearClipPlaneTransparentDisplay");
            transparentClearFlags = serializedObject.FindProperty("cameraClearFlagsTransparentDisplay");
            transparentBackgroundColor = serializedObject.FindProperty("backgroundColorTransparentDisplay");
            holoLensQualityLevel = serializedObject.FindProperty("holoLensQualityLevel");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Opaque Display Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
                    EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);

                    if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
                    {
                        opaqueBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", opaqueBackgroundColor.colorValue);
                    }

                    opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Transparent Display Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(transparentNearClip, nearClipTitle);
                    EditorGUILayout.PropertyField(transparentClearFlags, clearFlagsTitle);

                    if ((CameraClearFlags)transparentClearFlags.intValue == CameraClearFlags.Color)
                    {
                        transparentBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", transparentBackgroundColor.colorValue);
                    }

                    holoLensQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", holoLensQualityLevel.intValue, QualitySettings.names);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.CameraProfile;
        }
    }
}
