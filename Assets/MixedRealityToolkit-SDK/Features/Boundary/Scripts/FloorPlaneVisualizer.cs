// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Boundary
{
    /// <summary>
    /// Visualizes the floor plane.
    /// </summary>
    public class FloorPlaneVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The currently active Boundary System
        /// </summary>
        protected IMixedRealityBoundarySystem BoundaryManager => boundaryManager ?? (boundaryManager = MixedRealityManager.Instance.GetManager<IMixedRealityBoundarySystem>());
        private IMixedRealityBoundarySystem boundaryManager = null;

        /// <summary>
        /// Creates a rectangular <see cref="GameObject"/> to represent the 
        /// play area within the configured boundary.
        /// </summary>
        private void CreatePlayAreaObject()
        {
            if ((BoundaryManager == null) || MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile == null)
            {
                // We do not have a visualization profile configured, therefore do not render the quad.
                return;
            }

            // Render the floor.
            var floorPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floorPlane.transform.SetParent(transform);
            floorPlane.transform.Translate(Vector3.zero);
            floorPlane.transform.Rotate(90, 0, 0);
            floorPlane.transform.localScale = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.FloorPlaneScale;
            floorPlane.GetComponent<Renderer>().sharedMaterial = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.FloorPlaneMaterial;
        }
    }
}
