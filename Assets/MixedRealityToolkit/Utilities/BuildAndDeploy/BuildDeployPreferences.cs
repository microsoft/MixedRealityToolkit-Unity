// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    /// <summary>
    /// Build and Deploy Specific Editor Preferences for the Build and Deploy Window.
    /// </summary>
    public static class BuildDeployPreferences
    {
        // Constants
        private const string EDITOR_PREF_BUILD_DIR = "BuildDeployWindow_BuildDir";
        private const string EDITOR_PREF_INCREMENT_BUILD_VERSION = "BuildDeployWindow_IncrementBuildVersion";

        /// <summary>
        /// The Build Directory that the Mixed Reality Toolkit will build to.
        /// </summary>
        /// <remarks>
        /// This is a root build folder path. Each platform build will be put into a child directory with the name of the current active build target.
        /// </remarks>
        public static string BuildDirectory
        {
            get => $"{EditorPreferences.Get(EDITOR_PREF_BUILD_DIR, "Builds")}/{EditorUserBuildSettings.activeBuildTarget}";
            set => EditorPreferences.Set(EDITOR_PREF_BUILD_DIR, value.Replace($"/{EditorUserBuildSettings.activeBuildTarget}", string.Empty));
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
        /// Current setting to increment build visioning.
        /// </summary>
        public static bool IncrementBuildVersion
        {
            get => EditorPreferences.Get(EDITOR_PREF_INCREMENT_BUILD_VERSION, true);
            set => EditorPreferences.Set(EDITOR_PREF_INCREMENT_BUILD_VERSION, value);
        }
    }
}
