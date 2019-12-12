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
    public static class WindowsMixedRealityConfigurationChecker
    {
        static WindowsMixedRealityConfigurationChecker()
        {
            EnsureDotNetWinRT();
        }

        /// <summary>
        /// Ensures that the appropriate value is defined based on the presence of the DotNetWinRT binary.
        /// </summary>
        private static void EnsureDotNetWinRT()
        {
            const string fileName = "Microsoft.Windows.MixedReality.DotNetWinRT.dll";
            string[] defintions = { "DOTNETWINRT_PRESENT" };

            FileInfo[] files = FileUtilities.FindFilesInAssets(fileName);
            if (files.Length > 0)
            {
                ScriptingUtilities.AppendScriptingDefinitions(BuildTargetGroup.WSA, defintions);
            }
            else
            {
                ScriptingUtilities.RemoveScriptingDefinitions(BuildTargetGroup.WSA, defintions);
            }
        }
    }
}