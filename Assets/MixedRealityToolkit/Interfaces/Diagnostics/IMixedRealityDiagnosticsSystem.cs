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
        /// Enable / disable diagnostic display.
        /// </summary>
        /// <remarks>
        /// When set to true, visibility settings for individual diagnostics are honored. When set to false,
        /// all visualizations are hidden.
        /// </remarks>
        bool ShowDiagnostics { get; set; }

        /// <summary>
        /// Enable / disable the profiler display
        /// </summary>
        bool ShowProfiler { get; set; }
    }
}
