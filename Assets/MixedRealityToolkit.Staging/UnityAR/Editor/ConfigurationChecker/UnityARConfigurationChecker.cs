// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    [InitializeOnLoad]
    public class UnityARConfigurationChecker
    {
        static UnityARConfigurationChecker()
        {
            // Ensure we have the correct packages installed for the current version of Unity.
            // todo
            
            // Ensure we have the correct asmdef file for the current version of Unity.
            string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
            FileInfo oldFile = FindAsmDefFile(asmDefFileName);
            if (oldFile != null)
            {
                FileInfo newFile = GetNewAsmDefFileName(oldFile);
                if (newFile != null)
                {
                    // todo: only do this if the user gives permission
                    OverwriteAsmDefFile(newFile, oldFile);
                }
            }
        }

        private static FileInfo FindAsmDefFile(string fileName)
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

        private static FileInfo GetNewAsmDefFileName(FileInfo file)
        {

            string fileContents = string.Empty;

            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    fileContents = reader.ReadToEnd();
                }
            }

            string arFoundationReference = "Unity.XR.ARFoundation";
            string spatialTrackingReference = "UnityEngine.SpatialTracking";

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

            return !string.IsNullOrWhiteSpace(updateTo) ?
                new FileInfo(Path.Combine(file.DirectoryName, $"{file.Name}.{updateTo}")) :
                null;
        }

        private static void OverwriteAsmDefFile(FileInfo newFile, FileInfo oldFile)
        {
            File.Copy(newFile.FullName, oldFile.FullName, true);
        }
    }
}