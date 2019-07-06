// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class MixedRealityOptimizeUtils
    {
        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        /// <returns>True if the project has depth buffer sharing enabled, false otherwise.</returns>
        public static bool IsDepthBufferSharingEnabled()
        {
            if (PlayerSettings.VROculus.sharedDepthBuffer)
            {
                return true;
            }

#if UNITY_2019_1_OR_NEWER
            if (PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled)
            {
                return true;
            }
#else
            var playerSettings = GetSettingsObject("PlayerSettings");
            var property = playerSettings?.FindProperty("vrSettings.hololens.depthBufferSharingEnabled");
            if (property != null && property.boolValue)
            {
                return true;
            }
#endif

            return false;
        }

        public static void SetDepthBufferSharing(bool enableDepthBuffer)
        {
            PlayerSettings.VROculus.sharedDepthBuffer = enableDepthBuffer;

#if UNITY_2019
        PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = enableDepthBuffer;
#else
            var playerSettings = GetSettingsObject("PlayerSettings");
            ChangeProperty(playerSettings,
                "vrSettings.hololens.depthBufferSharingEnabled",
                property => property.boolValue = enableDepthBuffer);
#endif
        }

        public static void SetDepthBufferFormat(bool set16BitDepthBuffer)
        {
            int depthFormat = set16BitDepthBuffer ? 0 : 1;

            PlayerSettings.VRCardboard.depthFormat = depthFormat;
            PlayerSettings.VRDaydream.depthFormat = depthFormat;

            var playerSettings = GetSettingsObject("PlayerSettings");
#if UNITY_2019
        PlayerSettings.VRWindowsMixedReality.depthBufferFormat = set16BitDepthBuffer ? 
            PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit :
            PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat24Bit;

        ChangeProperty(playerSettings, 
                "vrSettings.lumin.depthFormat", 
                property => property.intValue = depthFormat);
#else
            ChangeProperty(playerSettings,
                "vrSettings.hololens.depthFormat",
                property => property.intValue = depthFormat);
#endif
        }

        public static void ChangeProperty(SerializedObject target, string name, Action<SerializedProperty> changer)
        {
            var prop = target.FindProperty(name);
            if (prop != null)
            {
                changer(prop);
                target.ApplyModifiedProperties();
            }
            else Debug.LogError("property not found: " + name);
        }

        public static SerializedObject GetSettingsObject(string className)
        {
            var settings = Unsupported.GetSerializedAssetInterfaceSingleton(className);
            return new SerializedObject(settings);
        }

        public static SerializedObject GetLighmapSettings()
        {
            var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
            return new SerializedObject(lightmapSettings);
        }
    }
}