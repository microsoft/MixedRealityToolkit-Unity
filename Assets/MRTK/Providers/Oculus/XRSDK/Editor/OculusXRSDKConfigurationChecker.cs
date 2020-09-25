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
        private const string SessionStateKey = "OculusXRSDKConfigurationCheckerSessionStateKey";
        private static readonly string[] Definitions = { "OCULUS_ENABLED" };

        static OculusXRSDKConfigurationChecker()
        {
            // This InitializeOnLoad handler only runs once at editor launch in order to adjust for Unity version
            // differences. These don't need to (and should not be) run on an ongoing basis. This uses the
            // volatile SessionState which is clear when Unity launches to ensure that this only runs the
            // expensive work (UpdateAsmDef) once.
            if (!SessionState.GetBool(SessionStateKey, false))
            {
                SessionState.SetBool(SessionStateKey, true);
                UpdateScriptingDefinitions();
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
        private static void UpdateScriptingDefinitions()
        {
#if UNITY_2019_3_OR_NEWER

            ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Android, Definitions);
            ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
#else
            ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Android, Definitions);
            ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.Standalone, Definitions);
#endif
        }
    }
}
