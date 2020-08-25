// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental
{
    /// <summary>
    /// Class to perform checks for configuration checks for the Windows Mixed Reality provider.
    /// </summary>
    [InitializeOnLoad]
    static class WindowsSceneUnderstandingConfigurationChecker
    {
        private const string FileName = "Microsoft.MixedReality.SceneUnderstanding.DotNet.dll";
        private static readonly string[] definitions = { "SCENEUNDERSTADING_PRESENT" };

        static WindowsSceneUnderstandingConfigurationChecker()
        {
            ReconcileSceneUnderstandingDefine();
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the SceneUnderstanding binary.
        /// </summary>
        private static void ReconcileSceneUnderstandingDefine()
        {
            FileInfo[] files = FileUtilities.FindFilesInAssets(FileName);
            if (files.Length > 0)
            {
                ScriptUtilities.AppendScriptingDefinitions(BuildTargetGroup.WSA, definitions);
            }
            else
            {
                ScriptUtilities.RemoveScriptingDefinitions(BuildTargetGroup.WSA, definitions);
            }
        }
    }
}
