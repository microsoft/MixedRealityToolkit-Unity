// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Build.DataStructures;
using MixedRealityToolkit.Common.EditorScript;
using System;
using System.IO;
using UnityEngine;

namespace MixedRealityToolkit.Build
{
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

        public static string BuildDirectory
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildDir, "UWP"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildDir, value); }
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

        public static string BuildConfig
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildConfig, "Debug"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildConfig, value); }
        }

        public static string BuildPlatform
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_BuildPlatform, "x86"); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_BuildPlatform, value); }
        }

        public static bool ForceRebuild
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_ForceRebuild, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ForceRebuild, value); }
        }

        public static bool IncrementBuildVersion
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_IncrementBuildVersion, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_IncrementBuildVersion, value); }
        }

        public static bool FullReinstall
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_FullReinstall, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_FullReinstall, value); }
        }

        public static string DevicePortalConnections
        {
            get
            {
                return EditorPrefsUtility.GetEditorPref(
                    EditorPref_ConnectInfos,
                    JsonUtility.ToJson(
                        new DevicePortalConnections(
                            new ConnectInfo("127.0.0.1", string.Empty, string.Empty, "Local Machine"))));
            }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ConnectInfos, value); }
        }

        public static bool UseSSL
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_UseSSL, true); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_UseSSL, value); }
        }

        public static bool TargetAllConnections
        {
            get { return EditorPrefsUtility.GetEditorPref(EditorPref_ProcessAll, false); }
            set { EditorPrefsUtility.SetEditorPref(EditorPref_ProcessAll, value); }
        }
    }
}
