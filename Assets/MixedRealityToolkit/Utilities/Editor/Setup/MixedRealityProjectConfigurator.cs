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
#endif

            // Android Settings
            AndroidMultiThreadedRendering = 2000,
            AndroidMinSdkVersion,

            // iOS Settings
            IOSMinOSVersion = 3000,
            IOSArchitecture,
            IOSCameraUsageDescription,
        };

        // The check functions for each type of setting
        private static readonly Dictionary<Configurations, Func<bool>> ConfigurationGetters = new Dictionary<Configurations, Func<bool>>()
        {
            { Configurations.LatestScriptingRuntime,  () => { return IsLatestScriptingRuntime(); } },
            { Configurations.ForceTextSerialization,  () => { return IsForceTextSerialization(); } },
            { Configurations.VisibleMetaFiles,  () => { return IsVisibleMetaFiles(); } },
            { Configurations.VirtualRealitySupported,  () => { return PlayerSettings.virtualRealitySupported; } },
            { Configurations.SinglePassInstancing,  () => { return MixedRealityOptimizeUtils.IsSinglePassInstanced(); } },
            { Configurations.SpatialAwarenessLayer,  () => { return HasSpatialAwarenessLayer(); } },
            { Configurations.EnableMSBuildForUnity, () => { return IsMSBuildForUnityEnabled(); } },

            // UWP Capabilities
            { Configurations.SpatialPerceptionCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.SpatialPerception); } },
            { Configurations.MicrophoneCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.Microphone); } },
            { Configurations.InternetClientCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InternetClient); } },
#if UNITY_2019_3_OR_NEWER
            { Configurations.EyeTrackingCapability,  () => { return PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.GazeInput); } },
#endif

            // Android Settings
            { Configurations.AndroidMultiThreadedRendering, () => { return PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android) == false; } },
            { Configurations.AndroidMinSdkVersion, () => { return PlayerSettings.Android.minSdkVersion >= MinAndroidSdk; } },

            // iOS Settings
            { Configurations.IOSMinOSVersion, () => {
                    float version;
                    if (!float.TryParse(PlayerSettings.iOS.targetOSVersionString, out version)) { return false; }
                    return version >= iOSMinOsVersion; } },
            { Configurations.IOSArchitecture, () => { return PlayerSettings.GetArchitecture(BuildTargetGroup.iOS) == RequirediOSArchitecture; } },
            { Configurations.IOSCameraUsageDescription, () => { return !string.IsNullOrWhiteSpace(PlayerSettings.iOS.cameraUsageDescription); } },
        };

        // The configure functions for each type of setting
        private static Dictionary<Configurations, Action> ConfigurationSetters = new Dictionary<Configurations, Action>()
        {
            { Configurations.LatestScriptingRuntime,  () => { SetLatestScriptingRuntime(); } },
            { Configurations.ForceTextSerialization,  () => { SetForceTextSerialization(); } },
            { Configurations.VisibleMetaFiles,  () => { SetVisibleMetaFiles(); } },
            { Configurations.VirtualRealitySupported,  () => { PlayerSettings.virtualRealitySupported = true; } },
            { Configurations.SinglePassInstancing,  () => { MixedRealityOptimizeUtils.SetSinglePassInstanced(); } },
            { Configurations.SpatialAwarenessLayer,  () => { SetSpatialAwarenessLayer(); } },
            { Configurations.EnableMSBuildForUnity, () => { PackageManifestUpdater.EnsureMSBuildForUnity(); } },

            // UWP Capabilities
            { Configurations.SpatialPerceptionCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true); } },
            { Configurations.MicrophoneCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true); } },
            { Configurations.InternetClientCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true); } },
#if UNITY_2019_3_OR_NEWER
            { Configurations.EyeTrackingCapability,  () => { PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.GazeInput, true); } },
#endif

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
                return ConfigurationGetters[config].Invoke();
            }

            return false;
        }

        /// <summary>
        /// Configures the supplied setting type to the desired values for MRTK
        /// </summary>
        /// <param name="config">The setting configuration that needs to be checked</param>
        public static void Configure(Configurations config)
        {
            if (ConfigurationSetters.ContainsKey(config))
            {
                ConfigurationSetters[config].Invoke();
            }
        }

        /// <summary>
        /// Is this Unity project configured for all setting types properly
        /// </summary>
        /// <returns>true if entire project is configured as recommended, false otherwise</returns>
        public static bool IsProjectConfigured()
        {
            foreach (var getter in ConfigurationGetters)
            {
                if (!getter.Value.Invoke())
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
                    setter.Value.Invoke();
                }
            }
            else
            {
                foreach (var key in filter)
                {
                    if (ConfigurationSetters.ContainsKey(key))
                    {
                        ConfigurationSetters[key].Invoke();
                    }
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
#endif
        }

        /// <summary>
        /// Configures current Unity project to use latest scripting runtime and reloads project
        /// </summary>
        public static void SetLatestScriptingRuntime()
        {
#if !UNITY_2019_3_OR_NEWER
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
            EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).ToString());
#endif
        }

        /// <summary>
        /// Checks if current Unity projects uses force text serialization
        /// </summary>
        public static bool IsForceTextSerialization()
        {
            return EditorSettings.serializationMode == SerializationMode.ForceText;
        }

        /// <summary>
        /// Checks package manifest to see if MSBuild for Unity is included in the dependencies.
        /// </summary>
        public static bool IsMSBuildForUnityEnabled()
        {
            // Locate the full path to the package manifest.
            DirectoryInfo projectRoot = new DirectoryInfo(Application.dataPath).Parent;
            string[] paths = { projectRoot.FullName, "Packages", "manifest.json" };
            string manifestPath = Path.Combine(paths);

            // Verify that the package manifest file exists.
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"Package manifest file ({manifestPath}) could not be found.");
                return false;
            }

            // Load the manfiest file.
            string manifestFileContents = File.ReadAllText(manifestPath);
            if (string.IsNullOrWhiteSpace(manifestFileContents))
            {
                Debug.LogError($"Failed to read the package manifest file ({manifestPath})");
                return false;
            }

            // Attempt to find the MSBuild for Unity package name.
            const string msBuildPackageName = "com.microsoft.msbuildforunity";
            return manifestFileContents.Contains(msBuildPackageName);
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
        }
    }
}
