// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utility class that provides methods to both check and configure Unity project for desired settings
    /// </summary>
    public class MixedRealityProjectConfigurator
    {
        private const int SpatialAwarenessDefaultLayer = 31;
        private const AndroidSdkVersions MinAndroidSdk = AndroidSdkVersions.AndroidApiLevel24; // Android 7.0
        private const int RequirediOSArchitecture = 1; // Per https://docs.unity3d.com/ScriptReference/PlayerSettings.SetArchitecture.html, 1 == ARM64
        private const float iOSMinOsVersion = 11.0f;
        private const string iOSCameraUsageDescription = "Required for augmented reality support.";

        /// <summary>
        /// List of available configurations to check and configure with this utility
        /// </summary>
        public enum Configurations
        {
            LatestScriptingRuntime = 1,
            ForceTextSerialization,
            VisibleMetaFiles,
            VirtualRealitySupported,
            SinglePassInstancing,
            SpatialAwarenessLayer,
            EnableMSBuildForUnity,

            // WSA Capabilities
            SpatialPerceptionCapability = 1000,
            MicrophoneCapability,
            InternetClientCapability,
#if UNITY_2019_3_OR_NEWER
            EyeTrackingCapability,
#endif // UNITY_2019_3_OR_NEWER

            // Android Settings
            AndroidMultiThreadedRendering = 2000,
            AndroidMinSdkVersion,

            // iOS Settings
            IOSMinOSVersion = 3000,
            IOSArchitecture,
            IOSCameraUsageDescription,
        };

        private class ConfigGetter
        {
            /// <summary>
            /// Array of build targets where the get action is valid
            /// </summary>
            public BuildTarget[] ValidTargets;

            /// <summary>
            /// Action to perform to determine if the current configuration is correctly enabled
            /// </summary>
            public Func<bool> GetAction;

            public ConfigGetter(Func<bool> get, BuildTarget target = BuildTarget.NoTarget)
            {
                GetAction = get;
                ValidTargets = new BuildTarget[] { target };
            }

            public ConfigGetter(Func<bool> get, BuildTarget[] targets)
            {
                GetAction = get;
                ValidTargets = targets;
            }

            /// <summary>
            /// Returns true if the active build target is contained in the ValidTargets list or the list contains a BuildTarget.NoTarget entry, false otherwise
            /// </summary>
            public bool IsActiveBuildTargetValid()
            {
                foreach (var buildTarget in ValidTargets)
                {
                    if (buildTarget == BuildTarget.NoTarget || buildTarget == EditorUserBuildSettings.activeBuildTarget)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // The check functions for each type of setting
        private static readonly Dictionary<Configurations, ConfigGetter> ConfigurationGetters = new Dictionary<Configurations, ConfigGetter>()
        {
            { Configurations.LatestScriptingRuntime, new ConfigGetter(() => { return IsLatestScriptingRuntime(); }) },
            { Configurations.ForceTextSerialization, new ConfigGetter(() => { return IsForceTextSerialization(); }) },
            { Configurations.VisibleMetaFiles, new ConfigGetter(() => { return IsVisibleMetaFiles(); }) },
            { Configurations.VirtualRealitySupported, new ConfigGetter(() => { return XRSettingsUtilities.LegacyXREnabled; }) },
            { Configurations.SinglePassInstancing, new ConfigGetter(() => { return MixedRealityOptimizeUtils.IsSinglePassInstanced(); }) },
            { Configurations.SpatialAwarenessLayer, new ConfigGetter(() => { return HasSpatialAwarenessLayer(); }) },
            { Configurations.EnableMSBuildForUnity, new ConfigGetter(() => { return PackageManifestUpdater.IsMSBuildForUnityEnabled(); }, BuildTarget.WSAPlayer) },

            // UWP Capabilities
            { Configurations.SpatialPerceptionCapability, new ConfigGetter(() => { return GetCapability(PlayerSettings.WSACapability.SpatialPerception); }, BuildTarget.WSAPlayer) },
            { Configurations.MicrophoneCapability, new ConfigGetter(() => { return GetCapability(PlayerSettings.WSACapability.Microphone); }, BuildTarget.WSAPlayer) },
            { Configurations.InternetClientCapability, new ConfigGetter(() => { return GetCapability(PlayerSettings.WSACapability.InternetClient); }, BuildTarget.WSAPlayer) },
#if UNITY_2019_3_OR_NEWER
            { Configurations.EyeTrackingCapability, new ConfigGetter(() => { return GetCapability(PlayerSettings.WSACapability.GazeInput); }, BuildTarget.WSAPlayer) },
#endif // UNITY_2019_3_OR_NEWER

            // Android Settings
            { Configurations.AndroidMultiThreadedRendering, 
                new ConfigGetter(() => { return PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android) == false; }, BuildTarget.Android) },
            { Configurations.AndroidMinSdkVersion,
                new ConfigGetter(() => { return PlayerSettings.Android.minSdkVersion >= MinAndroidSdk; }, BuildTarget.Android) },

            // iOS Settings
            { Configurations.IOSMinOSVersion, new ConfigGetter(() => {
                    float version;
                    if (!float.TryParse(PlayerSettings.iOS.targetOSVersionString, out version)) { return false; }
                    return version >= iOSMinOsVersion; }, BuildTarget.iOS) },
            { Configurations.IOSArchitecture, 
                new ConfigGetter(() => { return PlayerSettings.GetArchitecture(BuildTargetGroup.iOS) == RequirediOSArchitecture; }, BuildTarget.iOS) },
            { Configurations.IOSCameraUsageDescription, 
                new ConfigGetter(() => { return !string.IsNullOrWhiteSpace(PlayerSettings.iOS.cameraUsageDescription); }, BuildTarget.iOS) },
        };

        // The configure functions for each type of setting
        private static readonly Dictionary<Configurations, Action> ConfigurationSetters = new Dictionary<Configurations, Action>()
        {
            { Configurations.LatestScriptingRuntime, () => { SetLatestScriptingRuntime(); } },
            { Configurations.ForceTextSerialization,  () => { SetForceTextSerialization(); } },
            { Configurations.VisibleMetaFiles,  () => { SetVisibleMetaFiles(); } },
            { Configurations.VirtualRealitySupported,  () => { XRSettingsUtilities.LegacyXREnabled = true; } },
            { Configurations.SinglePassInstancing,  () => { MixedRealityOptimizeUtils.SetSinglePassInstanced(); } },
            { Configurations.SpatialAwarenessLayer,  () => { SetSpatialAwarenessLayer(); } },
            { Configurations.EnableMSBuildForUnity, () => { PackageManifestUpdater.EnsureMSBuildForUnity(); } },

            // UWP Capabilities
            { Configurations.SpatialPerceptionCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true); } },
            { Configurations.MicrophoneCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true); } },
            { Configurations.InternetClientCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true); } },
#if UNITY_2019_3_OR_NEWER
            { Configurations.EyeTrackingCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.GazeInput, true); } },
#endif // UNITY_2019_3_OR_NEWER

            // Android Settings
            { Configurations.AndroidMultiThreadedRendering, () => { PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false); } },
            { Configurations.AndroidMinSdkVersion, () => { PlayerSettings.Android.minSdkVersion = MinAndroidSdk; } },

            // iOS Settings
            { Configurations.IOSMinOSVersion, () => { PlayerSettings.iOS.targetOSVersionString = iOSMinOsVersion.ToString("n1"); } },
            { Configurations.IOSArchitecture, () => { PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, RequirediOSArchitecture); } },
            { Configurations.IOSCameraUsageDescription, () => { PlayerSettings.iOS.cameraUsageDescription = iOSCameraUsageDescription; } },
        };

        /// <summary>
        /// Checks whether the supplied setting type has been properly configured
        /// </summary>
        /// <param name="config">The setting configuration that needs to be checked</param>
        /// <returns>true if properly configured, false otherwise</returns>
        public static bool IsConfigured(Configurations config)
        {
            if (ConfigurationGetters.ContainsKey(config))
            {
                var configGetter = ConfigurationGetters[config];
                if (configGetter.IsActiveBuildTargetValid())
                {
                    return configGetter.GetAction.Invoke();
                }
            }

            return false;
        }

        /// <summary>
        /// Configures the supplied setting type to the desired values for MRTK
        /// </summary>
        /// <param name="config">The setting configuration that needs to be checked</param>
        public static void Configure(Configurations config)
        {
            // We use the config getter to check to see if a configuration is valid for the current build target.
            if (ConfigurationGetters.ContainsKey(config))
            {
                var configGetter = ConfigurationGetters[config];
                if (configGetter.IsActiveBuildTargetValid() && ConfigurationSetters.ContainsKey(config))
                {
                    ConfigurationSetters[config].Invoke();
                }
            }
        }

        /// <summary>
        /// Is this Unity project configured for all setting types properly
        /// </summary>
        /// <returns>true if entire project is configured as recommended, false otherwise</returns>
        public static bool IsProjectConfigured()
        {
            foreach (var configGetter in ConfigurationGetters)
            {
                if (configGetter.Value.IsActiveBuildTargetValid() && !configGetter.Value.GetAction.Invoke())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Configures the Unity project properly for the list of setting types provided. If null, configures all possibles setting types
        /// </summary>
        /// <param name="filter">List of setting types to target with configure action</param>
        public static void ConfigureProject(HashSet<Configurations> filter = null)
        {
            if (filter == null)
            {
                foreach (var setter in ConfigurationSetters)
                {
                    Configure(setter.Key);
                }
            }
            else
            {
                foreach (var key in filter)
                {
                    Configure(key);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        /// <summary>
        /// Checks if current Unity project has latest scripting runtime
        /// </summary>
        public static bool IsLatestScriptingRuntime()
        {
#if !UNITY_2019_3_OR_NEWER
            return PlayerSettings.scriptingRuntimeVersion == ScriptingRuntimeVersion.Latest;
#else
        return true;
#endif // UNITY_2019_3_OR_NEWER
        }

        /// <summary>
        /// Configures current Unity project to use latest scripting runtime and reloads project
        /// </summary>
        public static void SetLatestScriptingRuntime()
        {
#if !UNITY_2019_3_OR_NEWER
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
            EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).ToString());
#endif // UNITY_2019_3_OR_NEWER
        }

        /// <summary>
        /// Checks if current Unity projects uses force text serialization
        /// </summary>
        public static bool IsForceTextSerialization()
        {
            return EditorSettings.serializationMode == SerializationMode.ForceText;
        }

        /// <summary>
        /// Configures current Unity project to force text serialization
        /// </summary>
        public static void SetForceTextSerialization()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
        }

        /// <summary>
        /// Checks if current Unity project uses visible meta files
        /// </summary>
        public static bool IsVisibleMetaFiles()
        {
            return EditorSettings.externalVersionControl.Equals("Visible Meta Files");
        }

        /// <summary>
        /// Configures current Unity project to enabled visible meta files
        /// </summary>
        public static void SetVisibleMetaFiles()
        {
            EditorSettings.externalVersionControl = "Visible Meta Files";
        }

        /// <summary>
        /// Checks if current Unity project has the default Spatial Awareness layer set in the Layers settings
        /// </summary>
        public static bool HasSpatialAwarenessLayer()
        {
            return !string.IsNullOrEmpty(LayerMask.LayerToName(SpatialAwarenessDefaultLayer));
        }

        /// <summary>
        /// Configures current Unity project to contain the default Spatial Awareness layer set in the Layers settings
        /// </summary>
        public static void SetSpatialAwarenessLayer()
        {
            if (!HasSpatialAwarenessLayer())
            {
                if (!EditorLayerExtensions.SetupLayer(SpatialAwarenessDefaultLayer, "Spatial Awareness"))
                {
                    Debug.LogWarning(string.Format($"Can't modify project layers. It's possible the format of the layers and tags data has changed in this version of Unity. Set layer {SpatialAwarenessDefaultLayer} to \"Spatial Awareness\" manually via Project Settings > Tags and Layers window."));
                }
            }
        }

        /// <summary>
        /// Discover and set the appropriate XR Settings for virtual reality supported for the current build target.
        /// </summary>
        public static void ApplyXRSettings()
        {
            // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
            // with legacy requirements.
#pragma warning disable 0618
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            List<string> targetSDKs = new List<string>();
            foreach (string sdk in PlayerSettings.GetAvailableVirtualRealitySDKs(targetGroup))
            {
                if (sdk.Contains("OpenVR") || sdk.Contains("Windows"))
                {
                    targetSDKs.Add(sdk);
                }
            }

            if (targetSDKs.Count != 0)
            {
                PlayerSettings.SetVirtualRealitySDKs(targetGroup, targetSDKs.ToArray());
                PlayerSettings.SetVirtualRealitySupported(targetGroup, true);
            }
#pragma warning restore 0618
        }

        private static bool GetCapability(PlayerSettings.WSACapability capability)
        {
            return MixedRealityOptimizeUtils.IsBuildTargetUWP() ? PlayerSettings.WSA.GetCapability(capability) : true;
        }
    }
}
