// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class demonstrates clearing spatial observations.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ClearSpatialObservations")]
    public class ClearSpatialObservations : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether observations are to be cleared (true) or if the observer is to be resumed (false).
        /// </summary>
        private bool clearObservations = true;

        /// <summary>
        /// Toggles the state of the observers.
        /// </summary>
        public void ToggleObservers()
        {
            var spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;
            if (spatialAwarenessSystem != null)
            {
                if (clearObservations)
                {
                    spatialAwarenessSystem.SuspendObservers();
                    spatialAwarenessSystem.ClearObservations();
                    clearObservations = false;
                }
                else
                {
                    spatialAwarenessSystem.ResumeObservers();
                    clearObservations = true;
                }
            }
        }
    }
}