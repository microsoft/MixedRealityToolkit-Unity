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
        bool IsProfilerVisible { get; set; }

        /// <summary>
        /// Gets the <see cref="GameObject"/> that represents the diagnostic visualization
        /// </summary>
        GameObject DiagnosticVisualization { get; }
    }
}