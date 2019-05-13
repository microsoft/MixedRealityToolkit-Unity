// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DiagnosticsDemoControls : MonoBehaviour
    {
        private IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        private IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                if (diagnosticsSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out diagnosticsSystem);
                }
                return diagnosticsSystem;
            }
        }

        private async void Start()
        {
            await new WaitUntil(() => DiagnosticsSystem != null);

            // Ensure the diagnostic visualizations are turned on.
            DiagnosticsSystem.ShowDiagnostics = true;
        }

        /// <summary>
        /// Shows or hides all enabled diagnostics.
        /// </summary>
        public void OnToggleDiagnostics()
        {
            DiagnosticsSystem.ShowDiagnostics = !DiagnosticsSystem.ShowDiagnostics;
        }

        /// <summary>
        /// Shows or hides the profiler display.
        /// </summary>
        public void OnToggleProfiler()
        {
            DiagnosticsSystem.ShowProfiler = !DiagnosticsSystem.ShowProfiler;
        }
    }
}
