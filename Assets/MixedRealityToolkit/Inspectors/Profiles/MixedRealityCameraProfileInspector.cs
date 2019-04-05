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
        private static bool showGeneralProperties = true;
        private SerializedProperty isCameraPersistent;

        private static bool showOpaqueProperties = true;
        private SerializedProperty opaqueNearClip;
        private SerializedProperty opaqueClearFlags;
        private SerializedProperty opaqueBackgroundColor;
        private SerializedProperty opaqueQualityLevel;

        private static bool showTransparentProperties = true;
        private SerializedProperty transparentNearClip;
        private SerializedProperty transparentClearFlags;
        private SerializedProperty transparentBackgroundColor;
        private SerializedProperty holoLensQualityLevel;

        private readonly GUIContent nearClipTitle = new GUIContent("Near Clip");
        private readonly GUIContent clearFlagsTitle = new GUIContent("Clear Flags");

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
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
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Camera Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Camera Profile helps configure cross platform camera settings.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            showGeneralProperties = EditorGUILayout.Foldout(showGeneralProperties, "General Settings", true);
            if (showGeneralProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(isCameraPersistent);
                }
            }
            
            EditorGUILayout.Space();
            showOpaqueProperties = EditorGUILayout.Foldout(showOpaqueProperties, "Opaque Display Settings", true);
            if (showOpaqueProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
                    EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);

                    if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
                    {
                        opaqueBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", opaqueBackgroundColor.colorValue);
                    }

                    opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);
                }
            }

            EditorGUILayout.Space();
            showTransparentProperties = EditorGUILayout.Foldout(showTransparentProperties, "Transparent Display Settings", true);
            if (showTransparentProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(transparentNearClip, nearClipTitle);
                    EditorGUILayout.PropertyField(transparentClearFlags, clearFlagsTitle);

                    if ((CameraClearFlags)transparentClearFlags.intValue == CameraClearFlags.Color)
                    {
                        transparentBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", transparentBackgroundColor.colorValue);
                    }

                    holoLensQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", holoLensQualityLevel.intValue, QualitySettings.names);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
