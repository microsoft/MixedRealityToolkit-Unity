// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Class that exposes methods to show/hide the visual profiler
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Services/VisualProfilerControl")]
    public class VisualProfilerControl : MonoBehaviour
    {
        public void ToggleProfiler()
        {
            if (CoreServices.DiagnosticsSystem != null)
            {
                CoreServices.DiagnosticsSystem.ShowProfiler = !CoreServices.DiagnosticsSystem.ShowProfiler;
            }
        }

        public void SetProfilerVisibility(bool isVisible)
        {
            if (CoreServices.DiagnosticsSystem != null)
            {
                CoreServices.DiagnosticsSystem.ShowProfiler = isVisible;
            }
        }
    }
}
