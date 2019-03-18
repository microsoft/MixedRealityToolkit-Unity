// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace MRTKPrefix.Editor.Build
{
    public class UwpBuildInfo : BuildInfo
    {
        public UwpBuildInfo(bool isCommandLine = false) : base(isCommandLine)
        {
            BuildPlatform = "x86";
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
    }
}