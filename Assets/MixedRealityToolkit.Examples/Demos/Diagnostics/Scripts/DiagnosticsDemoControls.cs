// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class DiagnosticsDemoControls : MonoBehaviour
    {
        private async void Start()
        {
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                Debug.LogWarning("Diagnostics system is disabled. To run this demo, it needs to be enabled. Check your configuration settings.");
                return;
            }

            await new WaitUntil(() => MixedRealityToolkit.DiagnosticsSystem != null);

            // Turn on the diagnostic visualizations for this demo.
            MixedRealityToolkit.DiagnosticsSystem.ShowDiagnostics = true;
        }

        /// <summary>
        /// Shows or hides all enabled diagnostics.
        /// </summary>
        public void OnToggleDiagnostics()
        {
            MixedRealityToolkit.DiagnosticsSystem.ShowDiagnostics = !MixedRealityToolkit.DiagnosticsSystem.ShowDiagnostics;
        }

        /// <summary>
        /// Shows or hides the profiler display.
        /// </summary>
        public void OnToggleProfiler()
        {
            MixedRealityToolkit.DiagnosticsSystem.ShowProfiler = !MixedRealityToolkit.DiagnosticsSystem.ShowProfiler;
        }
    }
}
