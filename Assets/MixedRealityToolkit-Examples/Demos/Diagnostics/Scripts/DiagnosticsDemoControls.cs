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

            // Turn on the diagnostics for this demo.
            MixedRealityToolkit.DiagnosticsSystem.Visible = true;
        }

        /// <summary>
        /// Shows or hides the diagnostics information display.
        /// </summary>
        public void OnToggleDiagnostics()
        {
            MixedRealityToolkit.DiagnosticsSystem.Visible = !MixedRealityToolkit.DiagnosticsSystem.Visible;
        }

        /// <summary>
        /// Shows or hides the frame rate display.
        /// </summary>
        public void OnToggleFrameRate()
        {
            MixedRealityToolkit.DiagnosticsSystem.ShowFps = !MixedRealityToolkit.DiagnosticsSystem.ShowFps;
        }

        /// <summary>
        /// Shows or hides the memory usage display.
        /// </summary>
        public void OnToggleMemory()
        {
            MixedRealityToolkit.DiagnosticsSystem.ShowMemory = !MixedRealityToolkit.DiagnosticsSystem.ShowMemory;
        }

        /// <summary>
        /// Shows or hides the processor usage display.
        /// </summary>
        public void OnToggleProcessor()
        {
            MixedRealityToolkit.DiagnosticsSystem.ShowCpu = !MixedRealityToolkit.DiagnosticsSystem.ShowCpu;
        }
    }
}
