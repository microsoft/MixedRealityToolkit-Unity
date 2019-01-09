// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
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
        [Tooltip("Show diagnostics?")]
        private bool visible = false;

        /// <summary>
        /// Should the diagnostics be visible?
        /// </summary>
        public bool Visible => visible;

        [SerializeField]
        [Tooltip("Should show cpu?")]
        private bool showCpu = true;

        [SerializeField]
        [Tooltip("The type of IMixedRealityDiagnosticsHandler to use for visualization.")]
        [Implements(typeof(IMixedRealityDiagnosticsHandler), TypeGrouping.ByNamespaceFlat)]
        private SystemType handlerType = null;

        /// <summary>
        /// The type of <see cref="IMixedRealityDiagnosticsHandler"/> to use for visualization.
        /// </summary>
        public SystemType HandlerType => handlerType;

        /// <summary>
        /// Should the Cpu diagnostic be visible? 
        /// </summary>
        public bool ShowCpu => showCpu;

        [SerializeField]
        [Tooltip("How many samples should the cpu use tracker use?")]
        private int cpuBuffer = 20;

        /// <summary>
        /// The number of samples the cpu use tracker should use.
        /// </summary>
        public int CpuBuffer => cpuBuffer;

        [SerializeField]
        [Tooltip("Should show fps?")]
        private bool showFps = true;

        /// <summary>
        /// Should the fps diagnostic be visible?
        /// </summary>
        public bool ShowFps => showFps;

        [SerializeField]
        [Tooltip("How many samples should the fps use tracker use?")]
        private int fpsBuffer = 10;

        /// <summary>
        /// The number of samples the Fps use tracker should use.
        /// </summary>
        public int FpsBuffer => fpsBuffer;

        [SerializeField]
        [Tooltip("Should show memory?")]
        private bool showMemory = true;

        /// <summary>
        /// How to show the memory diagnostic
        /// </summary>
        public bool ShowMemory => showMemory;

        [SerializeField]
        [Tooltip("How many samples should the memory use tracker use?")]
        private int memoryBuffer = 10;

        /// <summary>
        /// The number of samples the memory use tracker should use.
        /// </summary>
        public int MemoryBuffer => memoryBuffer;
    }
}