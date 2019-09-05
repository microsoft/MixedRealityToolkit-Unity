// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public class UwpBuildInfo : BuildInfo
    {
        public UwpBuildInfo(bool isCommandLine = false) : base(isCommandLine)
        {
        }

        /// <inheritdoc />
        public override BuildTarget BuildTarget => BuildTarget.WSAPlayer;

        /// <summary>
        /// Build the appx bundle after building Unity Player?
        /// </summary>
        public bool BuildAppx { get; set; } = false;

        /// <summary>
        /// Force rebuilding the appx bundle?
        /// </summary>
        public bool RebuildAppx { get; set; } = false;

        /// <summary>
        /// VC Platform Toolset used building the appx bundle
        /// </summary>
        public string PlatformToolset { get; set; }

        /// <summary>
        /// If true, the 'Gaze Input' capability will be added to the AppX
        /// manifest after the Unity build.
        /// </summary>
        public bool GazeInputCapabilityEnabled { get; set; } = false;

        /// <summary>
        /// Use multiple cores for building the appx bundle?
        /// </summary>
        public bool Multicore { get; set; } = false;

        /// <summary>
        /// If true, the 'Research Mode' capability will be added to the AppX
        /// manifest after the Unity build.
        /// </summary>
        public bool ResearchModeCapabilityEnabled { get; set; } = false;

        /// <summary>
        /// If true, unsafe code will be allowed in the generated
        /// Assembly-CSharp project.
        /// </summary>
        public bool AllowUnsafeCode { get; set; } = false;
    }
}