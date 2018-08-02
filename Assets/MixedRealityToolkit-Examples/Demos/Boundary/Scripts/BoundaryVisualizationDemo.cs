// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demo class to show different ways of using the boundary API.
    /// </summary>
    public class BoundaryVisualizationDemo : MonoBehaviour
    {
        /// <summary>
        /// The material used to display indicators that are within the boundary geometry.
        /// </summary>
        [SerializeField]
        [Tooltip("Material used to display indicators that are within the boundary geometry.")]
        private Material boundsMaterial = null;

        /// <summary>
        /// The material used to display indicators that are outside of the boundary geometry.
        /// </summary>
        [SerializeField]
        [Tooltip("Material used to display indicators that are outside of the boundary geometry.")]
        private Material outOfBoundsMaterial = null;

        /// <summary>
        /// The material used to display the indicators that are within the inscribed rectangle..
        /// </summary>
        [SerializeField]
        [Tooltip("Material used to display the indicators that are within the inscribed rectangle.")]
        private Material inscribedRectangleMaterial = null;

        /// <summary>
        /// Boundary system implementation.
        /// </summary
        private IMixedRealityBoundarySystem boundaryManager = null;
        private IMixedRealityBoundarySystem BoundaryManager => boundaryManager ?? (boundaryManager = MixedRealityManager.Instance.GetManager<IMixedRealityBoundarySystem>());

        private void Start()
        {
            if (MixedRealityManager.HasActiveProfile && MixedRealityManager.Instance.ActiveProfile.IsBoundarySystemEnabled)
            {
                AddIndicators();
            }
        }

        /// <summary>
        /// Displays the boundary as an array of spheres where spheres in the
        /// bounds are a different color.
        /// </summary>
        private void AddIndicators()
        {
            // Get the rectangular bounds.
            Vector2 centerRect;
            float angleRect;
            float widthRect;
            float heightRect;
            if ((BoundaryManager == null) || !BoundaryManager.TryGetRectangularBoundsParams(out centerRect, out angleRect, out widthRect, out heightRect))
            {
                // If we have no boundary manager or rectangular bounds we will show no indicators
                return;
            }

            const int indicatorCount = 20;
            const float indicatorDistance = 0.2f;
            const float indicatorScale = 0.1f;
            const float dimension = indicatorCount * indicatorDistance;

            Vector3 center = new Vector3(centerRect.x, 0f, centerRect.y);
            Vector3 corner = center - (new Vector3(dimension, 0.0f, dimension) * 0.5f);

            corner.y += 0.05f;
            for (int xIndex = 0; xIndex < indicatorCount; ++xIndex)
            {
                for (int yIndex = 0; yIndex < indicatorCount; ++yIndex)
                {
                    Vector3 offset = new Vector3(xIndex * indicatorDistance, 0.0f, yIndex * indicatorDistance);
                    Vector3 position = corner + offset;
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    marker.transform.SetParent(transform);
                    marker.transform.position = position;
                    marker.transform.localScale = Vector3.one * indicatorScale;

                    // Get the desired material for the marker.
                    Material material = outOfBoundsMaterial;

                    // Check inscribed rectangle first
                    if (BoundaryManager.Contains(position, Boundary.Type.PlayArea))
                    {
                        material = inscribedRectangleMaterial;
                    }
                    // Then check geometry
                    else if (BoundaryManager.Contains(position, Boundary.Type.TrackedArea))
                    {
                        material = boundsMaterial;
                    }

                    marker.GetComponent<MeshRenderer>().sharedMaterial = material;
                }
            }
        }
    }
}
