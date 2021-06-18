// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if !UNITY_2020_1_OR_NEWER
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
#endif // !UNITY_2020_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class MixedRealityOptimizeUtils
    {
        public static bool IsOptimalRenderingPath()
#if UNITY_ANDROID
        => PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass;
#else
        => PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing;
#endif

        public static void SetOptimalRenderingPath()
#if UNITY_ANDROID
        => PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
#else
        => PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
#endif

        /// <summary>
        /// Checks if the project has depth buffer sharing enabled.
        /// </summary>
        /// <returns>True if the project has depth buffer sharing enabled, false otherwise.</returns>
        public static bool IsDepthBufferSharingEnabled()
        {
#if !UNITY_2020_1_OR_NEWER
            if (IsBuildTargetOpenVR())
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VROculus.sharedDepthBuffer)
#pragma warning restore 0618
                {
                    return true;
                }
            }
            else if (IsBuildTargetUWP())
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                if (PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled)
#pragma warning restore 0618
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
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER

            return true;
        }

        public static void SetDepthBufferSharing(bool enableDepthBuffer)
        {
#if !UNITY_2020_1_OR_NEWER
            if (IsBuildTargetOpenVR())
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.VROculus.sharedDepthBuffer = enableDepthBuffer;
#pragma warning restore 0618
            }
            else if (IsBuildTargetUWP())
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = enableDepthBuffer;
#pragma warning restore 0618
#else
                var playerSettings = GetSettingsObject("PlayerSettings");
                ChangeProperty(playerSettings,
                    "vrSettings.hololens.depthBufferSharingEnabled",
                    property => property.boolValue = enableDepthBuffer);
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER
        }

        public static bool IsWMRDepthBufferFormat16bit()
        {
#if !UNITY_2020_1_OR_NEWER
            if (XRSettingsUtilities.XREnabled)
            {
#if UNITY_2019_1_OR_NEWER
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                return PlayerSettings.VRWindowsMixedReality.depthBufferFormat == PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit;
#pragma warning restore 0618
#else
                var playerSettings = GetSettingsObject("PlayerSettings");
                var property = playerSettings?.FindProperty("vrSettings.hololens.depthFormat");
                return property != null && property.intValue == 0;
#endif // UNITY_2019_1_OR_NEWER
            }
#endif // !UNITY_2020_1_OR_NEWER
            return true;
        }

        public static void SetDepthBufferFormat(bool set16BitDepthBuffer)
        {
            int depthFormat = set16BitDepthBuffer ? 0 : 1;

#if !UNITY_2020_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
#pragma warning disable 0618
            PlayerSettings.VRCardboard.depthFormat = depthFormat;
            PlayerSettings.VRDaydream.depthFormat = depthFormat;
#pragma warning restore 0618

            var playerSettings = GetSettingsObject("PlayerSettings");
#if UNITY_2019_1_OR_NEWER
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
#pragma warning disable 0618
            PlayerSettings.VRWindowsMixedReality.depthBufferFormat = set16BitDepthBuffer ?
                PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit :
                PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat24Bit;
#pragma warning restore 0618

            ChangeProperty(playerSettings,
                "vrSettings.lumin.depthFormat",
                property => property.intValue = depthFormat);
#else

            ChangeProperty(playerSettings,
                "vrSettings.hololens.depthFormat",
                property => property.intValue = depthFormat);
#endif // UNITY_2019_1_OR_NEWER
#endif // !UNITY_2020_1_OR_NEWER
        }

        public static bool IsRealtimeGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLightmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableRealtimeLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetRealtimeGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLightmapSettings();
            ChangeProperty(lightmapSettings, "m_GISettings.m_EnableRealtimeLightmaps", property => property.boolValue = enabled);
        }

        public static bool IsBakedGlobalIlluminationEnabled()
        {
            var lightmapSettings = GetLightmapSettings();
            var property = lightmapSettings?.FindProperty("m_GISettings.m_EnableBakedLightmaps");
            return property != null && property.boolValue;
        }

        public static void SetBakedGlobalIlluminationEnabled(bool enabled)
        {
            var lightmapSettings = GetLightmapSettings();
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

        public static SerializedObject GetLightmapSettings()
        {
            var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
            return new SerializedObject(lightmapSettings);
        }
    }
}