// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics
{
    /// <summary>
    /// 
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
        /// Enable / disable fps profiling when the diagnostic panel is visible. 
        /// </summary>
        bool ShowFps { get; set; }

        /// <summary>
        /// Enable / disable memory profiling when the diagnostic panel is visible. 
        /// </summary>
        bool ShowMemory { get; set; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> that represents the diagnostic visualization
        /// </summary>
        GameObject DiagnosticVisualization { get; }
    }
}