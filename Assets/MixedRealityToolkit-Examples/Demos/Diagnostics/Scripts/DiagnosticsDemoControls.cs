// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DiagnosticsDemoControls : MonoBehaviour
    {
        private IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        /// <summary>
        /// Provides the instance, if enabled, of the diagnostics system.
        /// </summary>
        private IMixedRealityDiagnosticsSystem DiagnosticsSystem => diagnosticsSystem ?? (diagnosticsSystem = MixedRealityToolkit.DiagnosticsSystem);

        /// <summary>
        /// Shows or hides the diagnostics information display.
        /// </summary>
        public void OnToggleDiagnostics()
        {
            if (DiagnosticsSystem == null) { return; }
            DiagnosticsSystem.Visible = !(DiagnosticsSystem.Visible);
        }

        /// <summary>
        /// Shows or hides the frame rate display.
        /// </summary>
        public void OnToggleFrameRate()
        {
            if (DiagnosticsSystem == null) { return; }
            DiagnosticsSystem.ShowFps = !(DiagnosticsSystem.ShowFps);
        }

        /// <summary>
        /// Shows or hides the memory usage display.
        /// </summary>
        public void OnToggleMemory()
        {
            if (DiagnosticsSystem == null) { return; }
            DiagnosticsSystem.ShowMemory = !(DiagnosticsSystem.ShowMemory);
        }

        /// <summary>
        /// Shows or hides the processor usage display.
        /// </summary>
        public void OnToggleProcessor()
        {
            if (DiagnosticsSystem == null) { return; }
            DiagnosticsSystem.ShowCpu = !(DiagnosticsSystem.ShowCpu);
        }
    }
}
