// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Boundary
{
    /// <summary>
    /// Visualizes the inscribed rectangular play area within the configured boundary.
    /// </summary>
    public class PlayAreaVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The currently active Boundary System
        /// </summary>
        protected IMixedRealityBoundarySystem BoundaryManager => boundaryManager ?? (boundaryManager = MixedRealityManager.Instance.GetManager<IMixedRealityBoundarySystem>());
        private IMixedRealityBoundarySystem boundaryManager = null;

        private void Update()
        {
            if (!MixedRealityManager.HasActiveProfile || !MixedRealityManager.Instance.ActiveProfile.IsBoundarySystemEnabled)
            {
                return;
            }

            if (!MixedRealityManager.Instance.ActiveProfile.IsPlatformBoundaryRenderingEnabled)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Creates a rectangular <see cref="GameObject"/> to represent the 
        /// play area within the configured boundary.
        /// </summary>
        private void CreatePlayAreaObject()
        {
            // Get the rectangular bounds.
            Vector2 center;
            float angle;
            float width;
            float height;
            if ((BoundaryManager == null) ||
                !BoundaryManager.TryGetRectangularBoundsParams(out center, out angle, out width, out height) ||
                (MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile == null))
            {
                // No rectangular bounds, therefore do not render the quad.
                return;
            }

            // Render the rectangular bounds.
            if (EdgeUtilities.IsValidPoint(center))
            {
                var playArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
                playArea.transform.SetParent(transform);
                playArea.transform.Translate(new Vector3(center.x, 0.005f, center.y)); // Add fudge factor to avoid z-fighting
                playArea.transform.Rotate(new Vector3(90, -angle, 0));
                playArea.transform.localScale = new Vector3(width, height, 1.0f);
                playArea.GetComponent<Renderer>().sharedMaterial = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile.PlayAreaMaterial;
            }
        }
    }
}
