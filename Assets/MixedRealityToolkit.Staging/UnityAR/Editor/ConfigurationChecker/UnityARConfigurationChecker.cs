// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
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
            string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
            FileInfo[] asmDefFiles = FileUtilities.FindFilesInAssets(asmDefFileName);

            if (EnsureArFoundationDefine())
            {
               if (asmDefFiles.Length > 1)
               {
                   Debug.LogWarning($"More than one copy of {asmDefFileName} found in the project. The first instance will be updated.");
               }

               // Ensure we have the correct asmdef files for the current version of Unity.
               FileInfo newAsmDefFile = GetNewAsmDefFile(asmDefFiles[0]);
               if (newAsmDefFile != null)
               {
                   File.Copy(newAsmDefFile.FullName, asmDefFiles[0].FullName, true);
               }

            }
            else
            {
               // Remove the assembly definition files, if present.
               if ( (asmDefFiles.Length != 0) && asmDefFiles[0].Exists)
               {
                   File.Copy(asmDefFiles[0].FullName, $"{asmDefFiles[0]}.norefs");
               }
            }
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the DotNetWinRT binary.
        /// </summary>
        /// <returns>True if the define was added, false otherwise.</returns>
        private static bool EnsureArFoundationDefine()
        {
            const string fileName = "Unity.XR.ARFoundation.asmdef";
            string[] defintions = { "ARFOUNDATION_PRESENT" };

            FileInfo[] files = FileUtilities.FindFilesInPackageCache(fileName);
            if (files.Length > 0)
            {
                ScriptingUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, defintions);
                ScriptingUtilities.AppendScriptingDefinitions(BuildTargetGroup.iOS, defintions);
                return true;
            }
            else
            {
                ScriptingUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Android, defintions);
                ScriptingUtilities.RemoveScriptingDefinitions(BuildTargetGroup.iOS, defintions);
                return false;
            }
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
#elif UNITY_2019_2_OR_NEWER
            if (!fileContents.Contains(arFoundationReference) ||
                !fileContents.Contains(spatialTrackingReference))
            {
                updateTo = "2019";
            }
#endif

            FileInfo fi = new FileInfo(Path.Combine(file.DirectoryName, $"{file.Name}.{updateTo}"));
            return fi.Exists ? fi : null;
        }
    }
}