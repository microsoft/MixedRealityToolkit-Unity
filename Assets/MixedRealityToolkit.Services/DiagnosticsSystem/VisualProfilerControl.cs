// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Class that exposes methods to show/hide the visual profiler
    /// </summary>
    public class VisualProfilerControl : MonoBehaviour
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

        public void ToggleProfiler()
        {
            if (DiagnosticsSystem != null)
            {
                DiagnosticsSystem.ShowProfiler = !DiagnosticsSystem.ShowProfiler;
            }
        }

        public void SetProfilerVisibility(bool isVisible)
        {
            if (DiagnosticsSystem != null)
            {
                DiagnosticsSystem.ShowProfiler = isVisible;
            }
        }
    }
}
