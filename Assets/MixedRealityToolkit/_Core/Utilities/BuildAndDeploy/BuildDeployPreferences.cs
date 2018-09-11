// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Core.Utilities.WindowsDevicePortal.DataStructures;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Build
{
    /// <summary>
    /// Build and Deploy Specific Editor Preferences for the Build and Deploy Window.
    /// </summary>
    public static class BuildDeployPreferences
    {
        // Constants
        private const string EditorPref_BuildDir = "_BuildDeployWindow_BuildDir";
        private const string EditorPref_BuildConfig = "_BuildDeployWindow_BuildConfig";
        private const string EditorPref_BuildPlatform = "_BuildDeployWindow_BuildPlatform";
        private const string EditorPref_ForceRebuild = "_BuildDeployWindow_ForceBuild";
        private const string EditorPref_IncrementBuildVersion = "_BuildDeployWindow_IncrementBuildVersion";
        private const string EditorPref_ConnectInfos = "_BuildDeployWindow_ConnectInfos";
        private const string EditorPref_FullReinstall = "_BuildDeployWindow_FullReinstall";
        private const string EditorPref_UseSSL = "_BuildDeployWindow_UseSSL";
        private const string EditorPref_ProcessAll = "_BuildDeployWindow_ProcessAll";

        /// <summary>
        /// The Build Directory that the MRTK Build window will build to.
        /// </summary>
        public static string BuildDirectory
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildDir, "UWP"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildDir, value); }
        }

        /// <summary>
        /// The absolute path to <see cref="BuildDirectory"/>
        /// </summary>
        public static string AbsoluteBuildDirectory
        {
            get
            {
                string rootBuildDirectory = BuildDirectory;
                int dirCharIndex = rootBuildDirectory.IndexOf("/", StringComparison.Ordinal);
                if (dirCharIndex != -1)
                {
                    rootBuildDirectory = rootBuildDirectory.Substring(0, dirCharIndex);
                }
                return Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), rootBuildDirectory));
            }
        }

        /// <summary>
        /// The current Build Configuration. (Debug, Release, or Master)
        /// </summary>
        public static string BuildConfig
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildConfig, "Debug"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildConfig, value); }
        }

        /// <summary>
        /// The current Build Platform. (x86 or x64)
        /// </summary>
        public static string BuildPlatform
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildPlatform, "x86"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildPlatform, value); }
        }

        /// <summary>
        /// Current setting to force rebuilding the appx.
        /// </summary>
        public static bool ForceRebuild
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_ForceRebuild, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ForceRebuild, value); }
        }

        /// <summary>
        /// Current setting to increment build visioning.
        /// </summary>
        public static bool IncrementBuildVersion
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_IncrementBuildVersion, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_IncrementBuildVersion, value); }
        }

        /// <summary>
        /// Current setting to fully uninstall and reinstall the appx.
        /// </summary>
        public static bool FullReinstall
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_FullReinstall, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_FullReinstall, value); }
        }

        /// <summary>
        /// The current device portal connections.
        /// </summary>
        public static string DevicePortalConnections
        {
            get
            {
                return EditorPrefsUtility.GetEditorPref(
                    EditorPref_ConnectInfos,
                    JsonUtility.ToJson(
                        new DevicePortalConnections(
                            new DeviceInfo("127.0.0.1", string.Empty, string.Empty, "Local Machine"))));
            }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ConnectInfos, value); }
        }

        /// <summary>
        /// Current setting to use Single Socket Layer connections to the device portal.
        /// </summary>
        public static bool UseSSL
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_UseSSL, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_UseSSL, value); }
        }

        /// <summary>
        /// Current setting to target all the devices registered to the build window.
        /// </summary>
        public static bool TargetAllConnections
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_ProcessAll, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ProcessAll, value); }
        }
    }
}
