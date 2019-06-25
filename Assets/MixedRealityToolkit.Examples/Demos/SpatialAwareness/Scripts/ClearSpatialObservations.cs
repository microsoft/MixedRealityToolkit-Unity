// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class demonstrates clearing spatial observations.
    /// </summary>
    public class ClearSpatialObservations : MonoBehaviour
    {
        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The registered instance of the spatial awareness system.
        /// </summary>
        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (spatialAwarenessSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealitySpatialAwarenessSystem>(out spatialAwarenessSystem);
                }
                return spatialAwarenessSystem;
            }
        }

        /// <summary>
        /// Indicates whether observations are to be cleared (true) or if the observer is to be resumed (false).
        /// </summary>
        private bool clearObservations = true;

        private void Start()
        { }

        /// <summary>
        /// Toggles the state of the observers.
        /// </summary>
        public void ToggleObservers()
        {
            if (SpatialAwarenessSystem != null)
            {
                if (clearObservations)
                {
                    SpatialAwarenessSystem.SuspendObservers();
                    SpatialAwarenessSystem.ClearObservations();
                    clearObservations = false;
                }
                else
                {
                    SpatialAwarenessSystem.ResumeObservers();
                    clearObservations = true;
                }
            }
        }
    }
}