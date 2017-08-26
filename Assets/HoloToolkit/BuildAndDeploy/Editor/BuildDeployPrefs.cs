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
        private const string EditorPrefs_BuildDir = "_BuildDeployWindow_BuildDir";
        private const string EditorPrefs_BuildConfig = "_BuildDeployWindow_BuildConfig";
        private const string EditorPrefs_ForceRebuild = "_BuildDeployWindow_ForceBuild";
        private const string EditorPrefs_IncrementBuildVersion = "_BuildDeployWindow_IncrementBuildVersion";
        private const string EditorPrefs_MSBuildVer = "_BuildDeployWindow_MSBuildVer";
        private const string EditorPrefs_TargetIPs = "_BuildDeployWindow_DestIPs";
        private const string EditorPrefs_DeviceUser = "_BuildDeployWindow_DeviceUser";
        private const string EditorPrefs_DevicePwd = "_BuildDeployWindow_DevicePwd";
        private const string EditorPrefs_FullReinstall = "_BuildDeployWindow_FullReinstall";

        public static string BuildDirectory
        {
            get { return GetEditorPref(EditorPrefs_BuildDir, "UWP"); }
            set { SetEditorPref(EditorPrefs_BuildDir, value); }
        }

        public static string AbsoluteBuildDirectory
        {
            get { return Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), BuildDirectory)); }
        }

        public static string MsBuildVersion
        {
            get { return GetEditorPref(EditorPrefs_MSBuildVer, BuildDeployTools.DefaultMSBuildVersion); }
            set { SetEditorPref(EditorPrefs_MSBuildVer, value); }
        }

        public static string BuildConfig
        {
            get { return GetEditorPref(EditorPrefs_BuildConfig, "Debug"); }
            set { SetEditorPref(EditorPrefs_BuildConfig, value); }
        }

        public static bool ForceRebuild
        {
            get { return GetEditorPref(EditorPrefs_ForceRebuild, false); }
            set { SetEditorPref(EditorPrefs_ForceRebuild, value); }
        }

        public static bool IncrementBuildVersion
        {
            get { return GetEditorPref(EditorPrefs_IncrementBuildVersion, true); }
            set { SetEditorPref(EditorPrefs_IncrementBuildVersion, value); }
        }

        public static string TargetIPs
        {
            get { return GetEditorPref(EditorPrefs_TargetIPs, "127.0.0.1"); }
            set { SetEditorPref(EditorPrefs_TargetIPs, value); }
        }

        public static string DeviceUser
        {
            get { return GetEditorPref(EditorPrefs_DeviceUser, ""); }
            set { SetEditorPref(EditorPrefs_DeviceUser, value); }
        }

        public static string DevicePassword
        {
            get { return GetEditorPref(EditorPrefs_DevicePwd, ""); }
            set { SetEditorPref(EditorPrefs_DevicePwd, value); }
        }

        public static bool FullReinstall
        {
            get { return GetEditorPref(EditorPrefs_FullReinstall, true); }
            set { SetEditorPref(EditorPrefs_FullReinstall, value); }
        }

        private static void SetEditorPref(string key, string value)
        {
            EditorPrefs.SetString(Application.productName + key, value);
        }

        private static void SetEditorPref(string key, bool value)
        {
            EditorPrefs.SetBool(Application.productName + key, value);
        }

        private static string GetEditorPref(string key, string defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetString(Application.productName + key);
            }

            EditorPrefs.SetString(Application.productName + key, defaultValue);
            return defaultValue;
        }

        private static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetBool(Application.productName + key);
            }

            EditorPrefs.SetBool(Application.productName + key, defaultValue);
            return defaultValue;
        }
    }
}
