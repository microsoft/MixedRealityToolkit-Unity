// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Build
{
    public class BuildInfo
    {
        public string OutputDirectory { get; set; }

        public IEnumerable<string> Scenes { get; set; }

        public IEnumerable<CopyDirectoryInfo> CopyDirectories { get; set; }

        public Action<BuildInfo> PreBuildAction { get; set; }

        public Action<BuildInfo, BuildReport> PostBuildAction { get; set; }

        public BuildOptions BuildOptions { get; set; }

        public BuildTarget BuildTarget { get; set; }

        public string Configuration
        {
            get
            {
                if (!HasConfigurationSymbol() || HasAnySymbols(UwpPlayerBuildTools.BuildSymbolDebug))
                {
                    return UwpPlayerBuildTools.BuildSymbolDebug;
                }

                return HasAnySymbols(UwpPlayerBuildTools.BuildSymbolRelease) ?
                        UwpPlayerBuildTools.BuildSymbolRelease :
                        UwpPlayerBuildTools.BuildSymbolMaster;
            }
        }

        public string BuildPlatform { get; set; }

        public WSASDK? WSASdk { get; set; }

        public string WSAUwpSdk { get; set; }

        public WSAUWPBuildType? WSAUWPBuildType { get; set; }

        public bool? WSAGenerateReferenceProjects { get; set; }

        public ColorSpace? ColorSpace { get; set; }

        public bool IsCommandLine { get; set; }

        public bool BuildAppx => HasAnySymbols("-buildAppx");

        public string BuildSymbols { get; private set; }

        public BuildInfo()
        {
            BuildSymbols = string.Empty;
            BuildPlatform = "x86";
        }

        public void AppendSymbols(params string[] symbol)
        {
            AppendSymbols((IEnumerable<string>)symbol);
        }

        public void AppendSymbols(IEnumerable<string> symbols)
        {
            string[] toAdd = symbols.Except(BuildSymbols.Split(';'))
                                    .Where(sym => !string.IsNullOrEmpty(sym)).ToArray();

            if (!toAdd.Any())
            {
                return;
            }

            if (!string.IsNullOrEmpty(BuildSymbols))
            {
                BuildSymbols += ";";
            }

            BuildSymbols += string.Join(";", toAdd);
        }

        public bool HasAnySymbols(params string[] symbols)
        {
            return BuildSymbols.Split(';').Intersect(symbols).Any();
        }

        public bool HasConfigurationSymbol()
        {
            return HasAnySymbols(
                UwpPlayerBuildTools.BuildSymbolDebug,
                UwpPlayerBuildTools.BuildSymbolRelease,
                UwpPlayerBuildTools.BuildSymbolMaster);
        }

        public static IEnumerable<string> RemoveConfigurationSymbols(string symbols)
        {
            return symbols.Split(';').Except(new[]
            {
                UwpPlayerBuildTools.BuildSymbolDebug,
                UwpPlayerBuildTools.BuildSymbolRelease,
                UwpPlayerBuildTools.BuildSymbolMaster
            });
        }

        public bool HasAnySymbols(IEnumerable<string> symbols)
        {
            return BuildSymbols.Split(';').Intersect(symbols).Any();
        }
    }
}