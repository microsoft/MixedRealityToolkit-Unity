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
    public class BoundaryVisualizer : MonoBehaviour
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
        /// The material used to display the inscribed rectangle and the indicators that are within it.
        /// </summary>
        [SerializeField]
        [Tooltip("Material used to display the inscribed rectangle and the indicators that are within it.")]
        private Material inscribedRectangleMaterial = null;

        /// <summary>
        /// Boundary system implementation.
        /// </summary
        private IMixedRealityBoundarySystem boundaryManager = null;
        private IMixedRealityBoundarySystem BoundaryManager => boundaryManager ?? (boundaryManager = MixedRealityManager.Instance.GetManager<IMixedRealityBoundarySystem>());

        private void Start()
        {
            AddQuad();
            AddIndicators();
        }

        /// <summary>
        /// Displays the boundary as a quad primitive.
        /// </summary>
        private void AddQuad()
        {
            // Get the rectangular bounds.
            Vector2 center = EdgeUtilities.InvalidPoint;
            float angle = 0f;
            float width = 0f;
            float height = 0f;
            if (!(bool)(BoundaryManager?.TryGetRectangularBoundsParams(out center, out angle, out width, out height)))
            {
                // No rectangular bounds, therefore do not render the quad.
                return;
            }
            
            // Render the rectangular bounds.
            if (EdgeUtilities.IsValidPoint(center))
            {
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.SetParent(transform);
                quad.transform.Translate(new Vector3(center.x, 0.005f, center.y)); // Add fudge factor to avoid z-fighting
                quad.transform.Rotate(new Vector3(90, -angle, 0));
                quad.transform.localScale = new Vector3(width, height, 1.0f);
                quad.GetComponent<Renderer>().sharedMaterial = inscribedRectangleMaterial;
            }
        }

        /// <summary>
        /// Displays the boundary as an array of spheres where spheres in the
        /// bounds are a different color.
        /// </summary>
        private void AddIndicators()
        {
            // Get the rectangular bounds.
            Vector2 centerRect = EdgeUtilities.InvalidPoint;
            float angleRect = 0f;
            float widthRect = 0f;
            float heightRect = 0f;
            if (!(bool)(BoundaryManager?.TryGetRectangularBoundsParams(out centerRect, out angleRect, out widthRect, out heightRect)))
            {
                // If we have no boundary manager or rectangular bounds we will show no indicators
                return;
            }

                const int indicatorCount = 20;
            const float indicatorDistance = 0.2f;
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
                    marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

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
