// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics
{
    /// <summary>
    /// The interface contract that defines the Diagnostics system in the Mixed Reality Toolkit
    /// </summary>
    public interface IMixedRealityDiagnosticsSystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Enable / disable the diagnostic display
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Enable / disable cpu profiling when the diagnostic panel is visible. 
        /// </summary>
        bool ShowCpu { get; set; }

        /// <summary>
        /// The cpu use tracker.
        /// </summary>
        CpuUseTracker CpuUseTracker { get; }

        /// <summary>
        /// Enable / disable fps profiling when the diagnostic panel is visible. 
        /// </summary>
        bool ShowFps { get; set; }

        /// <summary>
        /// The fps use tracker.
        /// </summary>
        FpsUseTracker FpsUseTracker { get; }

        /// <summary>
        /// Enable / disable memory profiling when the diagnostic panel is visible. 
        /// </summary>
        bool ShowMemory { get; set; }

        /// <summary>
        /// The memory use tracker.
        /// </summary>
        MemoryUseTracker MemoryUseTracker { get; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> that represents the diagnostic visualization
        /// </summary>
        GameObject DiagnosticVisualization { get; }
    }
}