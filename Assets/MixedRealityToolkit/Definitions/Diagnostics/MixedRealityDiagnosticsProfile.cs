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
        [Tooltip("Show on screen profiler?")]
        private bool isProfilerVisible = false;

        /// <summary>
        /// Should the on screen profiler be visible?
        /// </summary>
        public bool IsProfilerVisible => isProfilerVisible;
    }
}