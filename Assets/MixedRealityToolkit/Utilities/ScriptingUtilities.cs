// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A set of utilities to configure script compilation.
    /// </summary>
    [Obsolete("The ScriptingUtilities class is obsolete and will be removed from a future version of MRTK. Please use the ScriptUtilities class.")]
    public static class ScriptingUtilities
    {
        /// <summary>
        /// Appends a set of symbolic constant definitions to Unity's Scripting Define Symbols for the
        /// specified build target group.
        /// </summary>
        /// </summary>
        /// <param name="fileName">The name of an optional file locate before appending.</param>
        /// <returns>
        /// <param name="targetGroup">The build target group for which the symbols are to be defined.</param>
        /// <param name="symbols">Array of symbols to define.</param>
        /// <remarks>
        /// To always append the symbols, pass null (or the empty string) for the fileName parameter.
        /// </remarks>
        [Obsolete("ScriptingUtilties.AppendScriptingDefinitions is obsolete and will be removed from a future version of MRTK. Please use FileUtilities.FindFilesInAssets and ScriptUtilties.AppendScriptingDefinitions.")]
        public static void AppendScriptingDefinitions(
            string fileName,
            BuildTargetGroup targetGroup,
            string[] symbols)
        {
            // Note: Typically, obsolete methods are re-implemented using the replacement versions.
            // In this instance, that is not possible as the replacement exists in a separate assembly that is
            // not appropriate for referencing (the new assembly is editor only).

            if (symbols == null || symbols.Length == 0) { return; }

            bool appendSymbols = true;

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                DirectoryInfo assets = new DirectoryInfo(Application.dataPath);
                FileInfo[] files = assets.GetFiles(fileName, SearchOption.AllDirectories);
                appendSymbols = (files.Length > 0);
            }

            if (!appendSymbols) { return; }

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            foreach (string symbol in symbols)
            {
                if (string.IsNullOrWhiteSpace(defines))
                {
                    defines = symbol;
                    continue;
                }

                if (!defines.Contains(symbol))
                {
                    defines = string.Concat(defines, $";{symbol}");
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
    }
}
#endif // UNITY_EDITOR
