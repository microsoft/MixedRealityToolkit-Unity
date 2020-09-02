// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// This class controls the visualization of the spatial mesh.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ToggleSpatialMeshVisualization")]
    public class ToggleSpatialMeshVisualization : MonoBehaviour
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
                // Set to visible
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
            }
            else
            {
                // Set to not visible
                observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
            }
        }
    }
}