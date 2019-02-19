// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics
{
    /// <summary>
    /// Configuration profile settings for setting up diagnostics.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Diagnostics Profile", fileName = "MixedRealityDiagnosticsProfile", order = (int)CreateProfileMenuItemIndices.Diagnostics)]
    public class MixedRealityDiagnosticsProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [FormerlySerializedAs("visible")]
        [Tooltip("Display all enabled diagnostics")]
        private bool showDiagnostics = false;

        /// <summary>
        /// Show or hide diagnostic visualizations
        /// </summary>
        public bool ShowDiagnostics => showDiagnostics;

        [SerializeField]
        [Tooltip("Display profiler")]
        private bool showProfiler = false;

        /// <summary>
        /// Show or hide the profiler UI
        /// </summary>
        public bool ShowProfiler => showProfiler;
    }
}