// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public static class UwpBuildDeployPreferences
    {
        /// <summary>
        /// The minimum Windows SDK that must be present on the build machine in order
        /// for a build to be successful.
        /// </summary>
        /// <remarks>
        /// This controls the version of the Windows SDK that is build against on the local
        /// machine, NOT the version of the OS that must be present on the device that
        /// the built application is deployed to (this other aspect is controlled by
        /// MIN_PLATFORM_VERSION)
        /// </remarks>
        public static Version MIN_SDK_VERSION = new Version("10.0.18362.0");

        /// <summary>
        /// The minimum version of the OS that must exist on the device that the application
        /// is deployed to.
        /// </summary>
        /// <remarks>
        /// This is intentionally set to a very low version, so that the application can be
        /// deployed to variety of different devices which may be on older OS versions.
        /// </remarks>
        public static Version MIN_PLATFORM_VERSION = new Version("10.0.10240.0");

        private const string EDITOR_PREF_BUILD_CONFIG = "BuildDeployWindow_BuildConfig";
        private const string EDITOR_PREF_PLATFORM_TOOLSET = "BuildDeployWindow_PlatformToolset";
        private const string EDITOR_PREF_FORCE_REBUILD = "BuildDeployWindow_ForceRebuild";
        private const string EDITOR_PREF_CONNECT_INFOS = "BuildDeployWindow_DeviceConnections";
        private const string EDITOR_PREF_LOCAL_CONNECT_INFO = "BuildDeployWindow_LocalConnection";
        private const string EDITOR_PREF_FULL_REINSTALL = "BuildDeployWindow_FullReinstall";
        private const string EDITOR_PREF_USE_SSL = "BuildDeployWindow_UseSSL";
        private const string EDITOR_PREF_PROCESS_ALL = "BuildDeployWindow_ProcessAll";
        private const string EDITOR_PREF_GAZE_INPUT_CAPABILITY_ENABLED = "BuildDeployWindow_GazeInputCapabilityEnabled";
        private const string EDITOR_PREF_MULTICORE_APPX_BUILD_ENABLED = "BuildDeployWindow_MulticoreAppxBuildEnabled";
        private const string EDITOR_PREF_RESEARCH_MODE_CAPABILITY_ENABLED = "BuildDeployWindow_ResearchModeCapabilityEnabled";
        private const string EDITOR_PREF_ALLOW_UNSAFE_CODE = "BuildDeployWindow_AllowUnsafeCode";

        /// <summary>
        /// The current Build Configuration. (Debug, Release, or Master)
        /// </summary>
        public static string BuildConfig
        {
            get => EditorPreferences.Get(EDITOR_PREF_BUILD_CONFIG, "master");
            set => EditorPreferences.Set(EDITOR_PREF_BUILD_CONFIG, value.ToLower());
        }

        /// <summary>
        /// Gets the build configuraition type as a WSABuildType enum
        /// </summary>
        public static WSABuildType BuildConfigType
        {
            get
            {
                string curBuildConfigString = BuildConfig;
                if (curBuildConfigString.Equals("master", StringComparison.OrdinalIgnoreCase))
                {
                    return WSABuildType.Master;
                }
                else if (curBuildConfigString.Equals("release", StringComparison.OrdinalIgnoreCase))
                {
                    return WSABuildType.Release;
                }
                else
                {
                    return WSABuildType.Debug;
                }
            }
        }

        /// <summary>
        /// The current Platform Toolset. (Solution, v141, or v142)
        /// </summary>
        public static string PlatformToolset
        {
            get => EditorPreferences.Get(EDITOR_PREF_PLATFORM_TOOLSET, string.Empty);
            set => EditorPreferences.Set(EDITOR_PREF_PLATFORM_TOOLSET, value.ToLower());
        }

        /// <summary>
        /// Current setting to force rebuilding the appx.
        /// </summary>
        public static bool ForceRebuild
        {
            get => EditorPreferences.Get(EDITOR_PREF_FORCE_REBUILD, false);
            set => EditorPreferences.Set(EDITOR_PREF_FORCE_REBUILD, value);
        }

        /// <summary>
        /// Current setting to fully uninstall and reinstall the appx.
        /// </summary>
        public static bool FullReinstall
        {
            get => EditorPreferences.Get(EDITOR_PREF_FULL_REINSTALL, true);
            set => EditorPreferences.Set(EDITOR_PREF_FULL_REINSTALL, value);
        }

        /// <summary>
        /// The current device portal connections.
        /// </summary>
        public static string DevicePortalConnections
        {
            get => EditorPreferences.Get(
                    EDITOR_PREF_CONNECT_INFOS,
                    JsonUtility.ToJson(
                            new DevicePortalConnections(
                                    new DeviceInfo(DeviceInfo.LocalIPAddress, string.Empty, string.Empty, DeviceInfo.LocalMachine))));
            set => EditorPreferences.Set(EDITOR_PREF_CONNECT_INFOS, value);
        }

        /// <summary>
        /// The current device portal connections.
        /// </summary>
        public static string LocalConnectionInfo
        {
            get => EditorPreferences.Get(
                    EDITOR_PREF_LOCAL_CONNECT_INFO,
                    JsonUtility.ToJson(new DeviceInfo(DeviceInfo.LocalIPAddress, string.Empty, string.Empty, DeviceInfo.LocalMachine)));
            set => EditorPreferences.Set(EDITOR_PREF_LOCAL_CONNECT_INFO, value);
        }

        /// <summary>
        /// Current setting to use Single Socket Layer connections to the device portal.
        /// </summary>
        public static bool UseSSL
        {
            get => EditorPreferences.Get(EDITOR_PREF_USE_SSL, true);
            set => EditorPreferences.Set(EDITOR_PREF_USE_SSL, value);
        }

        /// <summary>
        /// Current setting to target all the devices registered to the build window.
        /// </summary>
        public static bool TargetAllConnections
        {
            get => EditorPreferences.Get(EDITOR_PREF_PROCESS_ALL, false);
            set => EditorPreferences.Set(EDITOR_PREF_PROCESS_ALL, value);
        }

        /// <summary>
        /// If true, the 'Gaze Input' capability will be added to the AppX manifest
        /// after the Unity build.
        /// </summary>
        public static bool GazeInputCapabilityEnabled
        {
            get => EditorPreferences.Get(EDITOR_PREF_GAZE_INPUT_CAPABILITY_ENABLED, false);
            set => EditorPreferences.Set(EDITOR_PREF_GAZE_INPUT_CAPABILITY_ENABLED, value);
        }

        /// <summary>
        /// If true, the appx will be built with multicore support enabled in the
        /// MSBuild process.
        /// </summary>
        public static bool MulticoreAppxBuildEnabled
        {
            get => EditorPreferences.Get(EDITOR_PREF_MULTICORE_APPX_BUILD_ENABLED, false);
            set => EditorPreferences.Set(EDITOR_PREF_MULTICORE_APPX_BUILD_ENABLED, value);
        }

        /// <summary>
        /// Current setting to modify 'Package.appxmanifest' file for sensor access.
        /// </summary>
        public static bool ResearchModeCapabilityEnabled
        {
            get => EditorPreferences.Get(EDITOR_PREF_RESEARCH_MODE_CAPABILITY_ENABLED, false);
            set => EditorPreferences.Set(EDITOR_PREF_RESEARCH_MODE_CAPABILITY_ENABLED, value);
        }

        /// <summary>
        /// Current setting to modify 'Assembly-CSharp.csproj' file to allow unsafe code.
        /// </summary>
        public static bool AllowUnsafeCode
        {
            get => EditorPreferences.Get(EDITOR_PREF_ALLOW_UNSAFE_CODE, false);
            set => EditorPreferences.Set(EDITOR_PREF_ALLOW_UNSAFE_CODE, value);
        }
    }
}