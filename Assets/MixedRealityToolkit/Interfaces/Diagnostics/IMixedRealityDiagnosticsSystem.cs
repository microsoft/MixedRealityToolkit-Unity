// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// The interface contract that defines the Diagnostics system in the Mixed Reality Toolkit
    /// </summary>
    public interface IMixedRealityDiagnosticsSystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityDiagnosticsProfile DiagnosticsSystemProfile { get; }

        /// <summary>
        /// Enable / disable diagnostic display.
        /// </summary>
        /// <remarks>
        /// When set to true, visibility settings for individual diagnostics are honored. When set to false,
        /// all visualizations are hidden.
        /// </remarks>
        bool ShowDiagnostics { get; set; }

        /// <summary>
        /// Enable / disable the profiler display.
        /// </summary>
        bool ShowProfiler { get; set; }

        /// <summary>
        /// Show or hide the frame info (per frame stats).
        /// </summary>
        bool ShowFrameInfo { get; set; }

        /// <summary>
        /// Show or hide the memory stats (used, peak, and limit).
        /// </summary>
        bool ShowMemoryStats { get; set; }

        /// <summary>
        /// The amount of time, in seconds, to collect frames for frame rate calculation.
        /// </summary>
        float FrameSampleRate { get; }
    }
}
