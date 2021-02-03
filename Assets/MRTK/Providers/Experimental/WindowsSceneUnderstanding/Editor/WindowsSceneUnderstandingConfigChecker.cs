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
    static class WindowsSceneUnderstandingConfigurationChecker
    {
        private const string FileName = "Microsoft.MixedReality.SceneUnderstanding.DotNet.dll";
        private static readonly string[] definitions = { "SCENE_UNDERSTANDING_PRESENT" };

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the SceneUnderstanding binary.
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Scene Understanding/Check Configuration")]
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
