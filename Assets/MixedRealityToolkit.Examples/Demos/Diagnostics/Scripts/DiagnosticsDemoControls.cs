// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
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

            // Turn on the profiler for this demos.
            MixedRealityToolkit.DiagnosticsSystem.IsProfilerVisible = true;
        }

        /// <summary>
        /// Shows or hides the visual profiler display.
        /// </summary>
        public void OnToggleProfiler()
        {
            MixedRealityToolkit.DiagnosticsSystem.IsProfilerVisible = !MixedRealityToolkit.DiagnosticsSystem.IsProfilerVisible;
        }
    }
}
