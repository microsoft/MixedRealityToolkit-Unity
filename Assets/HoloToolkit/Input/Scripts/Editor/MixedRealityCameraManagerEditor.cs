// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using HoloToolkit.Unity.InputModule;
using UnityEditor;
using UnityEngine;

namespace HoloToolKit.Unity
{
    [CustomEditor(typeof(MixedRealityCameraManager))]
    public class MixedRealityCameraManagerEditor : Editor
    {
        private SerializedProperty opaqueNearClip;
        private SerializedProperty opaqueClearFlags;
        private SerializedProperty opaqueBackgroundColor;
        private SerializedProperty opaqueQualityLevel;

        private SerializedProperty transparentNearClip;
        private SerializedProperty transparentClearFlags;
        private SerializedProperty transparentBackgroundColor;
        private SerializedProperty holoLensQualityLevel;

        private GUIContent nearClipTitle;
        private GUIContent clearFlagsTitle;
        private GUIStyle headerStyle;

        private void OnEnable()
        {
            opaqueNearClip = serializedObject.FindProperty("NearClipPlane_OpaqueDisplay");
            opaqueClearFlags = serializedObject.FindProperty("CameraClearFlags_OpaqueDisplay");
            opaqueBackgroundColor = serializedObject.FindProperty("BackgroundColor_OpaqueDisplay");
            opaqueQualityLevel = serializedObject.FindProperty("OpaqueQualityLevel");

            transparentNearClip = serializedObject.FindProperty("NearClipPlane_TransparentDisplay");
            transparentClearFlags = serializedObject.FindProperty("CameraClearFlags_TransparentDisplay");
            transparentBackgroundColor = serializedObject.FindProperty("BackgroundColor_TransparentDisplay");
            holoLensQualityLevel = serializedObject.FindProperty("HoloLensQualityLevel");
        }

        public override void OnInspectorGUI()
        {
            nearClipTitle = new GUIContent("Near Clip");
            clearFlagsTitle = new GUIContent("Clear Flags");
            headerStyle = new GUIStyle("label") { richText = true };

            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Opaque Display Settings:</b>", headerStyle);
            EditorGUILayout.PropertyField(opaqueNearClip, nearClipTitle);
            EditorGUILayout.PropertyField(opaqueClearFlags, clearFlagsTitle);
            if ((CameraClearFlags)opaqueClearFlags.intValue == CameraClearFlags.Color)
            {
                opaqueBackgroundColor.colorValue = EditorGUILayout.ColorField("Background Color", opaqueBackgroundColor.colorValue);
            }

            opaqueQualityLevel.intValue = EditorGUILayout.Popup("Quality Setting", opaqueQualityLevel.intValue, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Transparent Display Settings:</b>", headerStyle);
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
