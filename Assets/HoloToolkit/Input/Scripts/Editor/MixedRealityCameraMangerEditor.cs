// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using HoloToolkit.Unity.InputModule;
using UnityEditor;
using UnityEngine;

namespace HoloToolKit.Unity
{
    [CustomEditor(typeof(MixedRealityCameraManager))]
    public class MixedRealityCameraMangerEditor : Editor
    {
        private MixedRealityCameraManager cameraManager;

        private void OnEnable()
        {
            cameraManager = (MixedRealityCameraManager)target;
        }

        public override void OnInspectorGUI()
        {
            var headerStyle = new GUIStyle("label") { richText = true };
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Opaque Display Settings:</b>", headerStyle);
            cameraManager.NearClipPlane_OpaqueDisplay = EditorGUILayout.FloatField("Near Clip", cameraManager.NearClipPlane_OpaqueDisplay);
            cameraManager.CameraClearFlags_OpaqueDisplay = (CameraClearFlags)EditorGUILayout.EnumPopup("Clear Flags", cameraManager.CameraClearFlags_OpaqueDisplay);
            if (cameraManager.CameraClearFlags_OpaqueDisplay == CameraClearFlags.Color)
            {
                cameraManager.BackgroundColor_OpaqueDisplay = EditorGUILayout.ColorField("Background Color", cameraManager.BackgroundColor_OpaqueDisplay);
            }

            cameraManager.OpaqueQualityLevel = EditorGUILayout.Popup("Quality Setting", cameraManager.OpaqueQualityLevel, QualitySettings.names);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Transparent Display Settings:</b>", headerStyle);
            cameraManager.NearClipPlane_TransparentDisplay = EditorGUILayout.FloatField("Near Clip", cameraManager.NearClipPlane_TransparentDisplay);
            cameraManager.CameraClearFlags_TransparentDisplay = (CameraClearFlags)EditorGUILayout.EnumPopup("Clear Flags", cameraManager.CameraClearFlags_TransparentDisplay);
            if (cameraManager.CameraClearFlags_TransparentDisplay == CameraClearFlags.Color)
            {
                cameraManager.BackgroundColor_TransparentDisplay = EditorGUILayout.ColorField("Background Color", cameraManager.BackgroundColor_TransparentDisplay);
            }

            cameraManager.HoloLensQualityLevel = EditorGUILayout.Popup("Quality Setting", cameraManager.HoloLensQualityLevel, QualitySettings.names);
        }
    }
}
