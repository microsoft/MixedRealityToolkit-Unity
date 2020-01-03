// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.IO;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Class to perform checks for configuration checks for the Windows Mixed Reality provider.
    /// </summary>
    [InitializeOnLoad]
    static class WindowsMixedRealityConfigurationChecker
    {
        private const string FileName = "Microsoft.Windows.MixedReality.DotNetWinRT.dll";
        private static readonly string[] definitions = { "DOTNETWINRT_PRESENT" };

        static WindowsMixedRealityConfigurationChecker()
        {
            ReconcileDotNetWinRTDefine();
        }

        /// <summary>
        /// Ensures that the appropriate symbolic constant is defined based on the presence of the DotNetWinRT binary.
        /// </summary>
        private static void ReconcileDotNetWinRTDefine()
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
