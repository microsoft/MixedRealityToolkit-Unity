// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class BuildDeployPrefs
    {
        // Constants
        private const string EditorPrefs_BuildDir = "_BuildDeployWindow_BuildDir";
        private const string EditorPrefs_BuildConfig = "_BuildDeployWindow_BuildConfig";
        private const string EditorPrefs_BuildPlatform = "_BuildDeployWindow_BuildPlatform";
        private const string EditorPrefs_ForceRebuild = "_BuildDeployWindow_ForceBuild";
        private const string EditorPrefs_IncrementBuildVersion = "_BuildDeployWindow_IncrementBuildVersion";
        private const string EditorPrefs_MSBuildVer = "_BuildDeployWindow_MSBuildVer";
        private const string EditorPrefs_TargetIPs = "_BuildDeployWindow_DestIPs";
        private const string EditorPrefs_ConnectInfos = "_BuildDeployWindow_ConnectInfos";
        private const string EditorPrefs_DeviceUser = "_BuildDeployWindow_DeviceUser";
        private const string EditorPrefs_DevicePwd = "_BuildDeployWindow_DevicePwd";
        private const string EditorPrefs_FullReinstall = "_BuildDeployWindow_FullReinstall";
        private const string EditorPrefs_UseSSL = "_BuildDeployWindow_UseSSL";
        private const string EditorPrefs_ProcessAll = "_BuildDeployWindow_ProcessAll";

        public static string BuildDirectory
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_BuildDir, "UWP"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_BuildDir, value); }
        }

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

        public static string MsBuildVersion
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_MSBuildVer, BuildDeployTools.DefaultMSBuildVersion); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_MSBuildVer, value); }
        }

        public static string BuildConfig
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_BuildConfig, "Debug"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_BuildConfig, value); }
        }

        public static string BuildPlatform
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_BuildPlatform, "x86"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_BuildPlatform, value); }
        }

        public static bool ForceRebuild
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_ForceRebuild, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_ForceRebuild, value); }
        }

        public static bool IncrementBuildVersion
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_IncrementBuildVersion, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_IncrementBuildVersion, value); }
        }

        public static bool FullReinstall
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_FullReinstall, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_FullReinstall, value); }
        }

        public static string DevicePortalConnections
        {
            get
            {
                return EditorPrefsUtility.GetEditorPref(
                    EditorPrefs_ConnectInfos,
                    JsonUtility.ToJson(
                        new DevicePortalConnections(
                            new ConnectInfo("127.0.0.1", string.Empty, string.Empty, "Local Machine"))));
            }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_ConnectInfos, value); }
        }

        [Obsolete("Use DevicePortalConnections")]
        public static string DeviceUser
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_DeviceUser, string.Empty); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_DeviceUser, value); }
        }

        [Obsolete("Use DevicePortalConnections")]
        public static string DevicePassword
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_DevicePwd, string.Empty); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_DevicePwd, value); }
        }

        [Obsolete("Use DevicePortalConnections")]
        public static string TargetIPs
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_TargetIPs, "127.0.0.1"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_TargetIPs, value); }
        }

        public static bool UseSSL
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_UseSSL, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_UseSSL, value); }
        }

        public static bool TargetAllConnections
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPrefs_ProcessAll, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPrefs_ProcessAll, value); }
        }
    }
}
