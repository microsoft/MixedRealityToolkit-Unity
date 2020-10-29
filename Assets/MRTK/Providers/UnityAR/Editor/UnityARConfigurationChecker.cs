// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;
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

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the AR Foundation package.
        /// </summary>
        /// <returns>True if the define was added, false otherwise.</returns>
        [MenuItem("Mixed Reality Toolkit/Utilities/UnityAR/Update Scripting Defines")]
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
    }
}
