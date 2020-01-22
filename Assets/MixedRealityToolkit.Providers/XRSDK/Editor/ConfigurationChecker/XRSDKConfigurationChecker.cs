// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// Class to perform checks for configuration checks for the XR SDK provider.
    /// </summary>
    [InitializeOnLoad]
    static class XRSDKConfigurationChecker
    {
        private const string AsmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef";
        private const string XRManagementReference = "Unity.XR.Management";
        private const string ARSubsystemsReference = "Unity.XR.ARSubsystems";
        private const string SpatialTrackingReference = "UnityEngine.SpatialTracking";

#if UNITY_2019_3_OR_NEWER
        private static readonly VersionDefine XRManagementDefine = new VersionDefine("com.unity.xr.management", "", "XR_MANAGEMENT_ENABLED");
        private static readonly VersionDefine ARSubsystemsDefine = new VersionDefine("com.unity.xr.arsubsystems", "", "ARSUBSYSTEMS_ENABLED");
        private static readonly VersionDefine SpatialTrackingDefine = new VersionDefine("com.unity.xr.legacyinputhelpers", "", "SPATIALTRACKING_ENABLED");
#endif // UNITY_2019_3_OR_NEWER

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
        /// - If Unity 2019 and newer: Unity.XR.Management and Unity.XR.ARSubsystems
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.XRSDK.asmdef file
        /// This will result in Unity reloading the assembly with the appropriate dependencies.
        /// </remarks>
        private static void UpdateAsmDef()
        {
            FileInfo[] asmDefFiles = FileUtilities.FindFilesInAssets(AsmDefFileName);

            if (asmDefFiles.Length == 0)
            {
                Debug.LogWarning($"Unable to locate file: {AsmDefFileName}");
                return;
            }
            if (asmDefFiles.Length > 1)
            {
                Debug.LogWarning($"Multiple ({asmDefFiles.Length}) {AsmDefFileName} instances found. Modifying only the first.");
            }

            AssemblyDefinition asmDef = AssemblyDefinition.Load(asmDefFiles[0].FullName);
            if (asmDef == null)
            {
                Debug.LogWarning($"Unable to load file: {AsmDefFileName}");
                return;
            }

            List<string> references = new List<string>();
            if (asmDef.References != null)
            {
                references.AddRange(asmDef.References);
            }

            bool changed = false;

#if UNITY_2019_3_OR_NEWER
            List<VersionDefine> versionDefines = new List<VersionDefine>();
            if (asmDef.VersionDefines != null)
            {
                versionDefines.AddRange(asmDef.VersionDefines);
            }

            if (!references.Contains(XRManagementReference))
            {
                // Add a reference to the ARFoundation assembly
                references.Add(XRManagementReference);
                changed = true; 
            }
            if (!references.Contains(ARSubsystemsReference))
            {
                // Add a reference to the ARSubsystems assembly
                references.Add(ARSubsystemsReference);
                changed = true;
            }
            if (!references.Contains(SpatialTrackingReference))
            {
                // Add a reference to the spatial tracking assembly
                references.Add(SpatialTrackingReference);
                changed = true;
            }

            if (!versionDefines.Contains(XRManagementDefine))
            {
                // Add the XRManagement #define
                versionDefines.Add(XRManagementDefine);
                changed = true;
            }
            if (!versionDefines.Contains(ARSubsystemsDefine))
            {
                // Add the ARSubsystems #define
                versionDefines.Add(ARSubsystemsDefine);
                changed = true;
            }
            if (!versionDefines.Contains(SpatialTrackingDefine))
            {
                // Add the spatial tracking #define
                versionDefines.Add(SpatialTrackingDefine);
                changed = true;
            }
#else
            if (references.Contains(XRManagementReference))
            {
                // Remove the reference to the XRManagement assembly
                references.Remove(XRManagementReference);
                changed = true;
            }
            if (references.Contains(ARSubsystemsReference))
            {
                // Remove the reference to the ARSubsystems assembly
                references.Remove(ARSubsystemsReference);
                changed = true;
            }
            if (references.Contains(SpatialTrackingReference))
            {
                // Remove the reference to the spatial tracking assembly
                references.Remove(SpatialTrackingReference);
                changed = true;
            }
#endif

            if (changed)
            {
                asmDef.References = references.ToArray();
#if UNITY_2019_3_OR_NEWER
                asmDef.VersionDefines = versionDefines.ToArray();
#endif // UNITY_2019_3_OR_NEWER
                asmDef.Save(asmDefFiles[0].FullName);
            }
        }
    }
}
