// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// Class to perform checks for configuration checks for the UnityAR provider.
    /// </summary>
    [InitializeOnLoad]
    static class XRSDKConfigurationChecker
    {
        static XRSDKConfigurationChecker()
        {
            UpdateAsmDef();
        }

        /// <summary>
        /// Updates the assembly definition to contain the appropriate references based on the Unity version.
        /// </summary>
        /// <remarks>
        /// Versions of Unity may have different factorings of components. To address this, the UpdateAsmDef
        /// method conditionally compiles for the version currently in use.
        /// To ensure proper compilation on each Unity version, the following steps are performed:
        /// - Load the Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef file
        /// - If Unity 2018: nothing
        /// - If Unity 2019 and newer: Unity.XR.Management, 
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef file
        /// This will result in Unity reloading the assembly with the appropriate dependencies.
        /// </remarks>
        private static void UpdateAsmDef()
        {
            string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef";
            FileInfo[] asmDefFiles = FileUtilities.FindFilesInAssets(asmDefFileName);

            if (asmDefFiles.Length == 0)
            {
                Debug.LogWarning($"Unable to locate file: {asmDefFileName}");
                return;
            }
            if (asmDefFiles.Length > 1)
            {
                Debug.LogWarning($"Multiple ({asmDefFiles.Length}) {asmDefFileName} instances found. Modifying only the first.");
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

            // ARFoundation assembly names
            const string xrManagementReference = "Unity.XR.Management";
            const string arSubsystemsReference = "Unity.XR.ARSubsystems";
            bool changed = false;

#if UNITY_2019_3_OR_NEWER
            if (!references.Contains(xrManagementReference))
            {
                // Add a reference to the ARFoundation assembly
                references.Add(xrManagementReference);
                changed = true; 
            }
            if (!references.Contains(arSubsystemsReference))
            {
                // Add a reference to the spatial tracking assembly
                references.Add(arSubsystemsReference);
                changed = true;
            }
#else
            if (references.Contains(xrManagementReference))
            {
                // Remove the reference to the spatial tracking assembly
                references.Remove(xrManagementReference);
                changed = true;
            }
            if (references.Contains(arSubsystemsReference))
            {
                // Add a reference to the spatial tracking assembly
                references.Remove(arSubsystemsReference);
                changed = true;
            }
#endif

            if (changed)
            {
                asmDef.References = references.ToArray();
                asmDef.Save(asmDefFiles[0].FullName);
            }
        }
    }
}
