//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.IO;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class BuildDeployPrefs
    {
        // Constants
        private const string EditorPrefs_BuildDir = "BuildDeployWindow_BuildDir";
        private const string EditorPrefs_BuildConfig = "BuildDeployWindow_BuildConfig";
        private const string EditorPrefs_ForceRebuild = "BuildDeployWindow_ForceBuild";
        private const string EditorPrefs_IncrementBuildVersion = "BuildDeployWindow_IncrementBuildVersion";
        private const string EditorPrefs_MSBuildVer = "BuildDeployWindow_MSBuildVer";
        private const string EditorPrefs_TargetIPs = "BuildDeployWindow_DestIPs";
        private const string EditorPrefs_DeviceUser = "BuildDeployWindow_DeviceUser";
        private const string EditorPrefs_DevicePwd = "BuildDeployWindow_DevicePwd";
        private const string EditorPrefs_FullReinstall = "BuildDeployWindow_FullReinstall";

        public static string BuildDirectory
        {
            get { return GetEditorPref(EditorPrefs_BuildDir, "WindowsStoreApp"); }
            set { EditorPrefs.SetString(EditorPrefs_BuildDir, value); }
        }

        public static string AbsoluteBuildDirectory
        {
            get { return Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), BuildDirectory)); }
        }

        public static string MsBuildVersion
        {
            get { return GetEditorPref(EditorPrefs_MSBuildVer, BuildDeployTools.DefaultMSBuildVersion); }
            set { EditorPrefs.SetString(EditorPrefs_MSBuildVer, value); }
        }

        public static string BuildConfig
        {
            get { return GetEditorPref(EditorPrefs_BuildConfig, "Debug"); }
            set { EditorPrefs.SetString(EditorPrefs_BuildConfig, value); }
        }

        public static bool ForceRebuild
        {
            get { return GetEditorPref(EditorPrefs_ForceRebuild, false); }
            set { EditorPrefs.SetBool(EditorPrefs_ForceRebuild, value); }
        }

        public static bool IncrementBuildVersion
        {
            get { return GetEditorPref(EditorPrefs_IncrementBuildVersion, true); }
            set { EditorPrefs.SetBool(EditorPrefs_IncrementBuildVersion, value); }
        }

        public static string TargetIPs
        {
            get { return GetEditorPref(EditorPrefs_TargetIPs, "127.0.0.1"); }
            set { EditorPrefs.SetString(EditorPrefs_TargetIPs, value); }
        }

        public static string DeviceUser
        {
            get { return GetEditorPref(EditorPrefs_DeviceUser, ""); }
            set { EditorPrefs.SetString(EditorPrefs_DeviceUser, value); }
        }

        public static string DevicePassword
        {
            get { return GetEditorPref(EditorPrefs_DevicePwd, ""); }
            set { EditorPrefs.SetString(EditorPrefs_DevicePwd, value); }
        }

        public static bool FullReinstall
        {
            get { return GetEditorPref(EditorPrefs_FullReinstall, true); }
            set { EditorPrefs.SetBool(EditorPrefs_FullReinstall, value); }
        }

        private static string GetEditorPref(string key, string defaultValue)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetString(key);
            }

            EditorPrefs.SetString(key, defaultValue);
            return defaultValue;
        }

        private static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey(key))
            {
                return EditorPrefs.GetBool(key);
            }

            EditorPrefs.SetBool(key, defaultValue);
            return defaultValue;
        }
    }
}
