// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class controls the observer of the spatial mesh.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleSpatialMeshObserver")]
    public class ToggleSpatialMeshObserver : MonoBehaviour
    {
        /// <summary>
        /// Toggles the state of the mesh display option.
        /// </summary>
        public void ToggleSpatialMeshVisual(bool mode)
        {
            // Get the first Mesh Observer available, generally we have only one registered
            var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

            if (mode == true)
            {
                observer.Resume();
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
            }
            else
            {
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
                observer.Suspend();
                observer.ClearObservations();
            }
        }
    }
}