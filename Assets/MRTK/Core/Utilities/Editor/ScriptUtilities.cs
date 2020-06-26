// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A set of utilities to configure script compilation. 
    /// </summary>
    public static class ScriptUtilities
    {
        /// <summary>
        /// Appends a set of symbolic constant definitions to Unity's Scripting Define Symbols for the
        /// specified build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group for which the symbols are to be defined.</param>
        /// <param name="symbols">Array of symbols to define.</param>
        public static void AppendScriptingDefinitions(
            BuildTargetGroup targetGroup,
            string[] symbols)
        {
            if (symbols == null || symbols.Length == 0) { return; }

            List<string> toAdd = new List<string>(symbols);
            List<string> defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines.Union(toAdd).ToArray()));
        }

        /// <summary>
        /// Removes a set of symbolic constant definitions to Unity's Scripting Define Symbols from the
        /// specified build target group.
        /// </summary>
        /// <param name="targetGroup">The build target group for which the symbols are to be removed.</param>
        /// <param name="symbols">Array of symbols to remove.</param>
        public static void RemoveScriptingDefinitions(
            BuildTargetGroup targetGroup,
            string[] symbols)
        {
            if (symbols == null || symbols.Length == 0) { return; }

            List<string> toRemove = new List<string>(symbols);
            List<string> defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defines.Except(toRemove).ToArray()));
        }
    }
}
