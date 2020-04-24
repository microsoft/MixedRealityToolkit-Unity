// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class SpatializerUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        public static string CurrentSpatializer => AudioSettings.GetSpatializerPluginName();

        /// <summary>
        /// 
        /// </summary>
        public static string[] InstalledSpatializers => AudioSettings.GetSpatializerPluginNames();

        /// <summary>
        /// Checks to see if the audio spatializer is configured and/or whether or
        /// not the spatializer collection has changed.
        /// </summary>
        /// <returns>
        /// True if; the selected spatializer is installed and no changes have been made to the collection of installed spatializers.
        /// False if; There is no selected spatializer, the selected spatializer is no longer installed or the collection of installed spatializers has been changed.
        /// </returns>
        public static bool CheckSettings()
        {
            string spatializerName = CurrentSpatializer;

            // Check to see if an audio spatializer is configured.
            if (string.IsNullOrWhiteSpace(spatializerName)) 
            {
                // A spatializer has not been configured.
                return false; 
            }

            string[] installedSpatializers = AudioSettings.GetSpatializerPluginNames();

            // Check to see if the configured spatializer is installed.
            if (!installedSpatializers.Contains<string>(spatializerName))
            {
                // The current spatializer has been uninstalled.
                return false;
            }

            // Next, check to see if the cached collection matches the current install
            bool collectionIsSmaller = false;
            if (SpatializerCollectionChanged(out collectionIsSmaller) && 
                !collectionIsSmaller)
            {
                // A new spatializer has been installed.
                return false;
            }

            // A spatializer is correctly confiugured.
            return true;
        }

        /// <summary>
        /// Saves the specified spatializer to the audio settings.
        /// </summary>
        public static void SaveSettings(string spatializer)
        {
            if (!InstalledSpatializers.Contains<string>(spatializer))
            {
                Debug.LogError($"{spatializer} is not an installed spatializer.");
                return;
            }

            AudioSettings.SetSpatializerPluginName(spatializer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool SpatializerCollectionChanged(out bool isSmaller)
        {
            isSmaller = false;

            // todo

            return false;
        }
    }
}
