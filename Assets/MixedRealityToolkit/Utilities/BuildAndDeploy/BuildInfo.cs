// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public class BuildInfo : IBuildInfo
    {
        public BuildInfo(bool isCommandLine = false)
        {
            IsCommandLine = isCommandLine;
            BuildSymbols = string.Empty;
            BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            Scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path);
        }

        /// <inheritdoc />
        public virtual BuildTarget BuildTarget { get; }

        /// <inheritdoc />
        public bool IsCommandLine { get; }

        private string outputDirectory;

        /// <inheritdoc />
        public string OutputDirectory
        {
            get => string.IsNullOrEmpty(outputDirectory) ? outputDirectory = BuildDeployPreferences.BuildDirectory : outputDirectory;
            set => outputDirectory = value;
        }

        /// <inheritdoc />
        public IEnumerable<string> Scenes { get; set; }

        /// <inheritdoc />
        public Action<IBuildInfo> PreBuildAction { get; set; }

        /// <inheritdoc />
        public Action<IBuildInfo, BuildReport> PostBuildAction { get; set; }

        /// <inheritdoc />
        public BuildOptions BuildOptions { get; set; }

        /// <inheritdoc />
        public ColorSpace? ColorSpace { get; set; }

        /// <inheritdoc />
        public ScriptingImplementation? ScriptingBackend { get; set; }

        /// <inheritdoc />
        public bool AutoIncrement { get; set; } = false;

        /// <inheritdoc />
        public string BuildSymbols { get; set; }

        /// <inheritdoc />
        public string BuildPlatform { get; set; }

        /// <inheritdoc />
        public string Configuration
        {
            get
            {
                if (!this.HasConfigurationSymbol())
                {
                    return UnityPlayerBuildTools.BuildSymbolMaster;
                }

                return this.HasAnySymbols(UnityPlayerBuildTools.BuildSymbolDebug)
                    ? UnityPlayerBuildTools.BuildSymbolDebug
                    : this.HasAnySymbols(UnityPlayerBuildTools.BuildSymbolRelease)
                        ? UnityPlayerBuildTools.BuildSymbolRelease
                        : UnityPlayerBuildTools.BuildSymbolMaster;
            }
            set
            {
                if (this.HasConfigurationSymbol())
                {
                    this.RemoveSymbols(new[]
                    {
                        UnityPlayerBuildTools.BuildSymbolDebug,
                        UnityPlayerBuildTools.BuildSymbolRelease,
                        UnityPlayerBuildTools.BuildSymbolMaster
                    });
                }

                this.AppendSymbols(value);
            }
        }

        /// <inheritdoc />
        public string LogDirectory { get; set; }
    }
}