// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Diagnostics
{
    /// <summary>
    /// Class that exposes methods to show/hide the visual profiler
    /// </summary>
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
