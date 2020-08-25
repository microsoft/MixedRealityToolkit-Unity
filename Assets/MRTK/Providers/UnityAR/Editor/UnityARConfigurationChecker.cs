// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Class to perform checks for configuration checks for the UnityAR provider.
    /// </summary>
    /// <remarks>
    /// Note that the checks that this class runs are fairly expensive and are only done manually by the user
    /// as part of their setup steps described here:
    /// https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/UsingARFoundation.html
    /// </remarks>
    static class UnityARConfigurationChecker
    {
        private const string FileName = "Unity.XR.ARFoundation.asmdef";
        private static readonly string[] definitions = { "ARFOUNDATION_PRESENT" };

        [MenuItem("Mixed Reality Toolkit/Utilities/UnityAR/Update Assembly Definitions")]
        private static void UpdateAssemblyDefinitions()
        {
            UpdateAsmDef(ReconcileArFoundationDefine());
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the AR Foundation package.
        /// </summary>
        /// <returns>True if the define was added, false otherwise.</returns>
        private static bool ReconcileArFoundationDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInPackageCache(FileName);
            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, definitions);
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.iOS, definitions);
                return true;
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Android, definitions);
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.iOS, definitions);
                return false;
            }
        }

        /// <summary>
        /// Updates the assembly definition to contain the appropriate references based on the Unity
        /// version and if the project contains the AR Foundation package.
        /// </summary>
        /// <remarks>
        /// Versions of Unity may have different factorings of components. To address this, the UpdateAsmDef
        /// method conditionally compiles for the version currently in use.
        /// To ensure proper compilation on each Unity version, the following steps are performed:
        /// - Load the Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef file
        /// - If AR Foundation has been installed via the Unity Package Manager, add the appropriate assembly references
        ///   - Unity 2018: Unity.XR.ARFoundation
        ///   - Unity 2019 and newer: Unity.XR.ARFoundation, UnityEngine.SpatialTracking
        /// - If AR Foundation has been uninstalled, remove the assembly references.
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef file
        /// This will result in Unity reloading the assembly with the appropriate dependencies.
        /// </remarks>
        private static void UpdateAsmDef(bool arFoundationPresent)
        {
            const string asmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef";
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
            const string arFoundationReference = "Unity.XR.ARFoundation";
            const string spatialTrackingReference = "UnityEngine.SpatialTracking";
            bool changed = false;

            if (arFoundationPresent)
            {
#if UNITY_2018_1_OR_NEWER
                if (!references.Contains(arFoundationReference))
                {
                    // Add a reference to the ARFoundation assembly
                    references.Add(arFoundationReference);
                    changed = true;
                }
#endif
#if UNITY_2019_1_OR_NEWER
                if (!references.Contains(spatialTrackingReference))
                {
                    // Add a reference to the spatial tracking assembly
                    references.Add(spatialTrackingReference);
                    changed = true;
                }
#elif UNITY_2018
                if (references.Contains(spatialTrackingReference))
                {
                    // Remove the reference to the spatial tracking assembly
                    references.Remove(spatialTrackingReference);
                    changed = true;
                }
#endif
            }
            else
            {
                // Remove all ARFoundation related assembly references
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
    }
}
