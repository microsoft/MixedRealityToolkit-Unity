// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Class to perform checks for configuration checks for the UnityAR provider.
    /// </summary>
    [InitializeOnLoad]
    public class UnityARConfigurationChecker
    {
        static UnityARConfigurationChecker()
        {
            // Ensure we have the correct asmdef file for the current version of Unity.
            string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
            FileInfo oldFile = FindFile(asmDefFileName);
            if (oldFile != null)
            {
                FileInfo newFile = GetNewAsmDefFile(oldFile);
                if (newFile != null)
                {
                    File.Copy(newFile.FullName, oldFile.FullName, true);
                }
            }
        }

        /// <summary>
        /// Locates the file that matches the specified name within the application data path.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>FileInfo object representing the file, or null if the file could not be located.</returns>
        private static FileInfo FindFile(string fileName)
        {
            // Find the asmdef file
            DirectoryInfo assets = new DirectoryInfo(Application.dataPath);
            FileInfo[] files = assets.GetFiles(fileName, SearchOption.AllDirectories);
            if (files.Length == 0) { return null; }
            if (files.Length > 1)
            {
                Debug.LogWarning($"More than one copy of {fileName} found in the project. Please ensure there are no duplicate resources and reload the project.");
                return null;
            }

            return files[0];
        }

        /// <summary>
        /// Determines if a file update is needed and returns the replacement.
        /// </summary>
        /// <param name="file">The reference file used to determine the new file.</param>
        /// <returns>FileInfo object representing the file, or null if a new file is not needed.</returns>
        private static FileInfo GetNewAsmDefFile(FileInfo file)
        {
            // Read the file.
            string fileContents = string.Empty;
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    fileContents = reader.ReadToEnd();
                }
            }

            // Key values used to determine if a replacement is required.
            const string arFoundationReference = "Unity.XR.ARFoundation";
            const string spatialTrackingReference = "UnityEngine.SpatialTracking";

            string updateTo = string.Empty;
#if UNITY_2018
            if (!fileContents.Contains(arFoundationReference) ||
                fileContents.Contains(spatialTrackingReference))
            {
                updateTo = "2018";
            }
#elif UNITY_2019_1_OR_NEWER
            if (!fileContents.Contains(arFoundationReference) ||
                !fileContents.Contains(spatialTrackingReference))
            {
                updateTo = "2019";
            }
#endif

            return !string.IsNullOrWhiteSpace(updateTo) ?
                new FileInfo(Path.Combine(file.DirectoryName, $"{file.Name}.{updateTo}")) :
                null;
        }
    }
}