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
        public static bool IsSinglePassInstanced()
        {
            return PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing;
        }

        public static void SetSinglePassInstanced()
        {
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
        }

        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        /// <returns>True if the project has depth buffer sharing enabled, false otherwise.</returns>
        public static bool IsDepthBufferSharingEnabled()
        {
            if (IsBuildTargetOpenVR())
            {
                if (PlayerSettings.VROculus.sharedDepthBuffer)
                {
                    return true;
                }
            }
            else if (IsBuildTargetUWP())
            {
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
            }

            return false;
        }

        public static void SetDepthBufferSharing(bool enableDepthBuffer)
        {
            if (IsBuildTargetOpenVR())
            {
                PlayerSettings.VROculus.sharedDepthBuffer = enableDepthBuffer;
            }
            else if (IsBuildTargetUWP())
            {
#if UNITY_2019
                PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = enableDepthBuffer;
#else
                var playerSettings = GetSettingsObject("PlayerSettings");
                ChangeProperty(playerSettings,
                    "vrSettings.hololens.depthBufferSharingEnabled",
                    property => property.boolValue = enableDepthBuffer);
#endif
            }
        }

        public static bool IsWMRDepthBufferFormat16bit()
        {
#if UNITY_2019_1_OR_NEWER
            return PlayerSettings.VRWindowsMixedReality.depthBufferFormat == PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit;
#else
            var playerSettings = GetSettingsObject("PlayerSettings");
            var property = playerSettings?.FindProperty("vrSettings.hololens.depthFormat");
            return property != null && property.intValue == 0;
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

        public static bool IsRealtimeGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLighmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetRealtimeGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLighmapSettings();
            ChangeProperty(lightmapSettings, "m_GISettings.m_EnableRealtimeLightmaps", property => property.boolValue = enabled);
        }

        public static bool IsBakedGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLighmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableBakedLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetBakedGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLighmapSettings();
            ChangeProperty(lightmapSettings, "m_GISettings.m_EnableBakedLightmaps", property => property.boolValue = enabled);
        }

        public static bool IsBuildTargetOpenVR()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
                EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;
        }

        public static bool IsBuildTargetUWP()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer;
        }

        public static bool IsBuildTargetAndroid()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
        }

        public static bool IsBuildTargetIOS()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
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