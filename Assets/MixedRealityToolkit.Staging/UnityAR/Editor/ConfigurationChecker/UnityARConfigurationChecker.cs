// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
            UpdateAsmDef(EnsureArFoundationDefine());
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
        /// Updates the assembly definition to contain the appropriate references based on the Unity
        /// version and if the project contains the AR Foundation package.
        /// </summary>
        private static void UpdateAsmDef(bool arFoundationPresent)
        {
            string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
            FileInfo[] asmDefFiles = FileUtilities.FindFilesInAssets(asmDefFileName);

            if (asmDefFiles.Length == 0)
            {
                Debug.LogWarning($"Unable to locate file: {asmDefFileName}");
                return;
            }

            AssemblyDefinition asmDef = AssemblyDefinition.Load(asmDefFiles[0].FullName);
            if (asmDef == null)
            {
                Debug.LogWarning($"Unable to load file: {asmDefFileName}");
                return;
            }

            List<string> references = new List<string>();
            if (asmDef.References != null)
            {
                references.AddRange(asmDef.References);
            }

            // Assemblies required by Unity 2018 and / or 2019 (and newer)
            const string arFoundationReference = "Unity.XR.ARFoundation";
            const string spatialTrackingReference = "UnityEngine.SpatialTracking";
            bool changed = false;

            if (arFoundationPresent)
            {
#if UNITY_2018 || UNITY_2019_1_OR_NEWER
                if (!references.Contains(arFoundationReference))
                {
                    references.Add(arFoundationReference);
                    changed = true; 
                }
#endif
#if UNITY_2019_1_OR_NEWER
                if (!references.Contains(spatialTrackingReference))
                {
                    references.Add(spatialTrackingReference);
                    changed = true;
                }
#elif UNITY_2018
                if (references.Contains(spatialTrackingReference))
                {
                    references.Remove(spatialTrackingReference);
                    changed = true;
                }
#endif
            }
            else
            {
                if (references.Contains(arFoundationReference))
                {
                    references.Remove(arFoundationReference);
                    changed = true;
                }
                if (references.Contains(spatialTrackingReference))
                {
                    references.Remove(spatialTrackingReference);
                    changed = true;
                }
            }

            if (changed)
            {
                asmDef.References = references.ToArray();
                asmDef.Save(asmDefFiles[0].FullName);
            }
        }

//        /// <summary>
//        /// Determines if a file update is needed and returns the replacement.
//        /// </summary>
//        /// <param name="file">The reference file used to determine the new file.</param>
//        /// <returns>FileInfo object representing the file, or null if a new file is not needed.</returns>
//        private static FileInfo GetNewAsmDefFile(FileInfo file)
//        {
//            // Read the file.
//            string fileContents = string.Empty;
//            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
//            {
//                using (StreamReader reader = new StreamReader(fs))
//                {
//                    fileContents = reader.ReadToEnd();
//                }
//            }

//            // Key values used to determine if a replacement is required.
//            const string arFoundationReference = "Unity.XR.ARFoundation";
//            const string spatialTrackingReference = "UnityEngine.SpatialTracking";

//            string updateTo = string.Empty;
//#if UNITY_2018
//            if (!fileContents.Contains(arFoundationReference) ||
//                fileContents.Contains(spatialTrackingReference))
//            {
//                updateTo = "2018";
//            }
//#elif UNITY_2019_2_OR_NEWER
//            if (!fileContents.Contains(arFoundationReference) ||
//                !fileContents.Contains(spatialTrackingReference))
//            {
//                updateTo = "2019";
//            }
//#endif

//            FileInfo fi = new FileInfo(Path.Combine(file.DirectoryName, $"{file.Name}.{updateTo}"));
//            return fi.Exists ? fi : null;
//        }
    }
}