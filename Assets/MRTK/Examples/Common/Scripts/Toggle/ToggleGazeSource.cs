// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Example script to test toggling the gaze provider source from platform-specific overrides to the default camera frame center.
    /// </summary>
    public class ToggleGazeSource : MonoBehaviour
    {
        private IMixedRealityGazeProviderHeadOverride GazeProvider => gazeProvider ?? (gazeProvider = CoreServices.InputSystem?.GazeProvider as IMixedRealityGazeProviderHeadOverride);
        private IMixedRealityGazeProviderHeadOverride gazeProvider = null;

        /// <summary>
        /// Toggles the value of IMixedRealityGazeProviderWithOverride's UseHeadGazeOverride.
        /// </summary>
        public void ToggleGazeOverride()
        {
            if (GazeProvider != null)
            {
                GazeProvider.UseHeadGazeOverride = !GazeProvider.UseHeadGazeOverride;
            }
        }
    }
}
