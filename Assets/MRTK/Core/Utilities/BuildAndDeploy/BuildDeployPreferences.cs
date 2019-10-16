// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private const string EDITOR_PREF_LIVE_CUBE_MODEL_LOCATION = "BuildDeployLiveCubeModelLocation";

        private static BuildDeployPreferencesSO _savedPreferences;

        private const string _savedPreferencesLocation = "Assets/SavedBuildDeployPreferences.asset";

        static BuildDeployPreferences()
        {
            string[] result = AssetDatabase.FindAssets("SavedBuildDeployPreferences");

            if (result.Length != 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(result[0]);
                _savedPreferences = (BuildDeployPreferencesSO)AssetDatabase.LoadAssetAtPath(path, typeof(BuildDeployPreferencesSO));
                Debug.Log("Found Saved Deploy Preferences");
            }
            else
            {
                Debug.Log("NOT Found SavedBuildDeployPreferences.asset");
                _savedPreferences = ScriptableObject.CreateInstance<BuildDeployPreferencesSO>();
                AssetDatabase.CreateAsset(_savedPreferences, _savedPreferencesLocation);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// The Build Directory that the Mixed Reality Toolkit will build to.
        /// </summary>
        /// <remarks>
        /// This is a root build folder path. Each platform build will be put into a child directory with the name of the current active build target.
        /// </remarks>
        public static string BuildDirectory
        {
            get => $"{_savedPreferences.BuildDirectory}/{EditorUserBuildSettings.activeBuildTarget}";
            set
            {
                _savedPreferences.BuildDirectory = value.Replace($"/{EditorUserBuildSettings.activeBuildTarget}", string.Empty);
                EditorUtility.SetDirty(_savedPreferences);
                AssetDatabase.SaveAssets();
            }
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
            get => _savedPreferences.IncrementBuildVersion;
            set
            {
                _savedPreferences.IncrementBuildVersion = value;
                EditorUtility.SetDirty(_savedPreferences);
                AssetDatabase.SaveAssets();
            }
        }

        public static string LiveCubeModelLocation
        {
            get => _savedPreferences.LiveCubeModelLocation;
            set
            {
                _savedPreferences.LiveCubeModelLocation = value;
                Debug.Log($"Changed Value of LiveCubeModelLocation to: {value}");
                EditorUtility.SetDirty(_savedPreferences);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
