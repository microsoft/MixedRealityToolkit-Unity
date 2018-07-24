// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
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
        [SerializeField]
        [Tooltip("Material used to draw the boundary geometry.")]
        private Material boundsMaterial = null;

        [SerializeField]
        [Tooltip("Material used to draw the area outside of the boundary geometry.")]
        private Material outOfBoundsMaterial = null;

        [SerializeField]
        [Tooltip("Material used to draw the area inside of the inscribed rectangle.")]
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
                // No rectangular bounds.
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
            bool haveBoundary = (bool)(BoundaryManager?.TryGetRectangularBoundsParams(out centerRect, out angleRect, out widthRect, out heightRect));

            const int indicatorCount = 20;
            const float indicatorDistance = 0.2f;
            const float dimension = indicatorCount * indicatorDistance;

            Vector3 center = new Vector3(centerRect.x, 0f, centerRect.y);
            Vector3 corner = center - (new Vector3(dimension, 0.0f, dimension) / 2.0f);

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

                    if (haveBoundary)
                    {
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
                    }

                    marker.GetComponent<MeshRenderer>().sharedMaterial = material;
                }
            }
        }
    }
}
