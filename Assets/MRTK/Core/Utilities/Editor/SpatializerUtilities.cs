// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Collection of utilities to manage the configured audio spatializer.
    /// </summary>
    public static class SpatializerUtilities
    {
        /// <summary>
        /// Returns the name of the currently selected spatializer plugin.
        /// </summary>
        public static string CurrentSpatializer => AudioSettings.GetSpatializerPluginName();

        /// <summary>
        /// Returns the names of installed spatializer plugins.
        /// </summary>
        public static string[] InstalledSpatializers => AudioSettings.GetSpatializerPluginNames();

        /// <summary>
        /// Checks to see if the audio spatializer is configured and/or whether or
        /// not the spatializer collection has changed.
        /// </summary>
        /// <returns>
        /// True if the selected spatializer is installed and no changes have been made to the collection of installed spatializers.
        /// False if the selected spatializer is no longer installed or the collection of installed spatializers has been changed.
        /// </returns>
        public static bool CheckSettings()
        {
            // Check to see if the count of installed spatializers has changed
            if (!CheckSpatializerCount())
            {
                // A spatializer has been added or removed.
                return false;
            }

            string spatializerName = CurrentSpatializer;

            // Check to see if an audio spatializer is configured.
            if (string.IsNullOrWhiteSpace(spatializerName))
            {
                // The user chose to not initialize a spatializer so we are set correctly
                return true;
            }

            string[] installedSpatializers = InstalledSpatializers;

            // Check to see if the configured spatializer is installed.
            if (!installedSpatializers.Contains(spatializerName))
            {
                // The current spatializer has been uninstalled.
                return false;
            }

            // A spatializer is correctly configured.
            return true;
        }

        /// <summary>
        /// Saves the specified spatializer to the audio settings.
        /// </summary>
        public static void SaveSettings(string spatializer)
        {
            if (string.IsNullOrWhiteSpace(spatializer))
            {
                Debug.LogWarning("No spatializer was specified. The application will not support Spatial Sound.");
            }
            else if (!InstalledSpatializers.Contains(spatializer))
            {
                Debug.LogError($"{spatializer} is not an installed spatializer.");
                return;
            }

            SerializedObject audioMgrSettings = MixedRealityOptimizeUtils.GetSettingsObject("AudioManager");
            SerializedProperty spatializerPlugin = audioMgrSettings.FindProperty("m_SpatializerPlugin");
            if (spatializerPlugin == null)
            {
                Debug.LogError("Unable to save the spatializer settings. The field could not be located into the Audio Manager settings object.");
                return;
            }

            AudioSettings.SetSpatializerPluginName(spatializer);
            spatializerPlugin.stringValue = spatializer;
            audioMgrSettings.ApplyModifiedProperties();

            // Cache the count of installed spatializers
            MixedRealityProjectPreferences.AudioSpatializerCount = InstalledSpatializers.Length;
        }

        /// <summary>
        /// Compares the previous and current count of installed spatializer plugins.
        /// </summary>
        /// <returns>True if the count of installed spatializers is unchanged, false otherwise.</returns>
        private static bool CheckSpatializerCount()
        {
            int previousCount = MixedRealityProjectPreferences.AudioSpatializerCount;
            int currentCount = InstalledSpatializers.Length;

            return (previousCount == currentCount);
        }
    }
}
