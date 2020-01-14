// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Class to perform checks for configuration checks for the WMR XR SDK provider.
    /// </summary>
    [InitializeOnLoad]
    static class WindowsMixedRealityXRSDKConfigurationChecker
    {
        private const string AsmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef";
        private const string WindowsMixedRealityReference = "Unity.XR.WindowsMixedReality";

#if UNITY_2019_3_OR_NEWER
        private static readonly VersionDefine WindowsMixedRealityDefine = new VersionDefine("com.unity.xr.windowsmr", "", "WMR_ENABLED");
#endif // UNITY_2019_3_OR_NEWER

        static WindowsMixedRealityXRSDKConfigurationChecker()
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
        /// - Load the Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef file
        /// - If Unity 2018: nothing
        /// - If Unity 2019 and newer: Unity.XR.WindowsMixedReality
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.XRSDK.WMR.asmdef file
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

            if (!references.Contains(WindowsMixedRealityReference))
            {
                // Add a reference to the XR SDK WMR assembly
                references.Add(WindowsMixedRealityReference);
                changed = true; 
            }

            if (!versionDefines.Contains(WindowsMixedRealityDefine))
            {
                // Add the WMR #define
                versionDefines.Add(WindowsMixedRealityDefine);
                changed = true;
            }
#else
            if (references.Contains(WindowsMixedRealityReference))
            {
                // Remove the reference to the XR SDK WMR assembly
                references.Remove(WindowsMixedRealityReference);
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
