// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus
{
    /// <summary>
    /// Class to perform checks for configuration checks for the Oculus XR SDK provider.
    /// </summary>
    [InitializeOnLoad]
    static class OculusXRSDKConfigurationChecker
    {
        private const string AsmDefFileName = "Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef";
        private const string OculusReference = "Unity.XR.Oculus";
        private const string SessionStateKey = "OculusXRSDKConfigurationCheckerSessionStateKey";

#if UNITY_2019_3_OR_NEWER
        private static readonly VersionDefine OculusDefine = new VersionDefine("com.unity.xr.oculus", "", "OCULUS_ENABLED");
#endif // UNITY_2019_3_OR_NEWER

        static OculusXRSDKConfigurationChecker()
        {
            // This InitializeOnLoad handler only runs once at editor launch in order to adjust for Unity version
            // differences. These don't need to (and should not be) run on an ongoing basis. This uses the
            // volatile SessionState which is clear when Unity launches to ensure that this only runs the
            // expensive work (UpdateAsmDef) once.
            if (!SessionState.GetBool(SessionStateKey, false))
            {
                SessionState.SetBool(SessionStateKey, true);
                UpdateAsmDef();
            }
        }

        /// <summary>
        /// Updates the assembly definition to contain the appropriate references based on the Unity version.
        /// </summary>
        /// <remarks>
        /// Versions of Unity may have different factorings of components. To address this, the UpdateAsmDef
        /// method conditionally compiles for the version currently in use.
        /// To ensure proper compilation on each Unity version, the following steps are performed:
        /// - Load the Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef file
        /// - If Unity 2018: nothing
        /// - If Unity 2019 and newer: Unity.XR.Oculus
        /// - Save the Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.asmdef file
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

            if (!references.Contains(OculusReference))
            {
                // Add a reference to the XR SDK Oculus assembly
                references.Add(OculusReference);
                changed = true; 
            }

            if (!versionDefines.Contains(OculusDefine))
            {
                // Add the Oculus #define
                versionDefines.Add(OculusDefine);
                changed = true;
            }
#else
            if (references.Contains(OculusReference))
            {
                // Remove the reference to the XR SDK Oculus assembly
                references.Remove(OculusReference);
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
