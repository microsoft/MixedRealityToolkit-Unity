// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public static class BuildInfoExtensions
    {
        /// <summary>
        /// Append symbols to the end of the <see cref="IBuildInfo"/>'s<see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="symbol">The string array to append.</param>
        public static void AppendSymbols(this IBuildInfo buildInfo, params string[] symbol)
        {
            buildInfo.AppendSymbols((IEnumerable<string>)symbol);
        }

        /// <summary>
        /// Append symbols to the end of the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="symbols">The string collection to append.</param>
        public static void AppendSymbols(this IBuildInfo buildInfo, IEnumerable<string> symbols)
        {
            string[] toAdd = symbols.Except(buildInfo.BuildSymbols.Split(';'))
                                    .Where(symbol => !string.IsNullOrEmpty(symbol)).ToArray();

            if (!toAdd.Any())
            {
                return;
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                buildInfo.BuildSymbols += ";";
            }

            buildInfo.BuildSymbols += string.Join(";", toAdd);
        }

        /// <summary>
        /// Remove symbols from the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/>.
        /// </summary>
        /// <param name="symbolsToRemove">The string collection to remove.</param>
        public static void RemoveSymbols(this IBuildInfo buildInfo, IEnumerable<string> symbolsToRemove)
        {
            string[] toKeep = buildInfo.BuildSymbols.Split(';').Except(symbolsToRemove).ToArray();

            if (!toKeep.Any())
            {
                return;
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                buildInfo.BuildSymbols = string.Empty;
            }

            buildInfo.BuildSymbols += string.Join(";", toKeep);
        }

        /// <summary>
        /// Does the <see cref="IBuildInfo"/> contain any of the provided symbols in the <see cref="IBuildInfo.BuildSymbols"/>?
        /// </summary>
        /// <param name="symbols">The string array of symbols to match.</param>
        /// <returns>True, if any of the provided symbols are in the <see cref="IBuildInfo.BuildSymbols"/></returns>
        public static bool HasAnySymbols(this IBuildInfo buildInfo, params string[] symbols)
        {
            if (string.IsNullOrEmpty(buildInfo.BuildSymbols)) { return false; }

            return buildInfo.BuildSymbols.Split(';').Intersect(symbols).Any();
        }

        /// <summary>
        /// Does the <see cref="IBuildInfo"/> contain any of the provided symbols in the <see cref="IBuildInfo.BuildSymbols"/>?
        /// </summary>
        /// <param name="symbols">The string collection of symbols to match.</param>
        /// <returns>True, if any of the provided symbols are in the <see cref="IBuildInfo.BuildSymbols"/></returns>
        public static bool HasAnySymbols(this IBuildInfo buildInfo, IEnumerable<string> symbols)
        {
            if (string.IsNullOrEmpty(buildInfo.BuildSymbols)) { return false; }

            return buildInfo.BuildSymbols.Split(';').Intersect(symbols).Any();
        }

        /// <summary>
        /// Checks if the <see cref="IBuildInfo"/> has any configuration symbols (i.e. debug, release, or master).
        /// </summary>
        /// <returns>True, if the <see cref="IBuildInfo.BuildSymbols"/> contains debug, release, or master.</returns>
        public static bool HasConfigurationSymbol(this IBuildInfo buildInfo)
        {
            return buildInfo.HasAnySymbols(
                UnityPlayerBuildTools.BuildSymbolDebug,
                UnityPlayerBuildTools.BuildSymbolRelease,
                UnityPlayerBuildTools.BuildSymbolMaster);
        }

        /// <summary>
        /// Appends the <see cref="IBuildInfo"/>'s <see cref="IBuildInfo.BuildSymbols"/> without including debug, release or master.
        /// </summary>
        /// <param name="symbols">Symbols to append.</param>
        public static void AppendWithoutConfigurationSymbols(this IBuildInfo buildInfo, string symbols)
        {
            buildInfo.AppendSymbols(symbols.Split(';').Except(new[]
            {
                UnityPlayerBuildTools.BuildSymbolDebug,
                UnityPlayerBuildTools.BuildSymbolRelease,
                UnityPlayerBuildTools.BuildSymbolMaster
            }).ToArray());
        }

        /// <summary>
        /// Gets the <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see> for the <see cref="IBuildInfo"/>'s <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.html">BuildTarget</see>
        /// </summary>
        /// <returns>The <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see> for the <see cref="IBuildInfo"/>'s <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.html">BuildTarget</see></returns>
        public static BuildTargetGroup GetGroup(this BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return BuildTargetGroup.Standalone;
                default:
                    return BuildTargetGroup.Unknown;
            }
        }
    }
}