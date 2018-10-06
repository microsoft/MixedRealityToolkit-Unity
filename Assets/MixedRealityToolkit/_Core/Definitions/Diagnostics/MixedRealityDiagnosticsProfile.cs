// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Diagnostics Profile", fileName = "MixedRealityDiagnosticsProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Should show fps?")]
        private bool showFps = true;

        /// <summary>
        /// Should the fps diagnostic be visible?
        /// </summary>
        public bool ShowFps => showFps;

        [SerializeField]
        [Tooltip("Should show cpu?")]
        private bool showCpu = true;

        /// <summary>
        /// Should the Cpu diagnostic be visible? 
        /// </summary>
        public bool ShowCpu => showCpu;

        [SerializeField]
        [Tooltip("Should show memory?")]
        private bool showMemory = true;

        /// <summary>
        /// How to show the memory diagnostic
        /// </summary>
        public bool ShowMemory => showMemory;

        [SerializeField]
        [Tooltip("Show diagnostics?")]
        private bool visible = false;

        /// <summary>
        /// Should the diagnostics be visible?
        /// </summary>
        public bool Visible => visible;
    }
}