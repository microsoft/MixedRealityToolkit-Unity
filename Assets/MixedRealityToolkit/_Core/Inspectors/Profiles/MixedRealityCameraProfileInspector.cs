// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityCameraProfile))]
    public class MixedRealityCameraProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty isCameraPersistent;
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

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            isCameraPersistent = serializedObject.FindProperty("isCameraPersistent");
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
            EditorGUILayout.LabelField("Camera Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Camera Profile helps tweak camera settings no matter what platform you're building for.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(isCameraPersistent);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Opaque Display Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
            EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);

            if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
            {
                opaqueBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", opaqueBackgroundColor.colorValue);
            }

            opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transparent Display Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(transparentNearClip, nearClipTitle);
            EditorGUILayout.PropertyField(transparentClearFlags, clearFlagsTitle);

            if ((CameraClearFlags)transparentClearFlags.intValue == CameraClearFlags.Color)
            {
                transparentBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", transparentBackgroundColor.colorValue);
            }

            holoLensQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", holoLensQualityLevel.intValue, QualitySettings.names);

            serializedObject.ApplyModifiedProperties();
        }
    }
}