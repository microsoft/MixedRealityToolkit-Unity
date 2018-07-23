// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
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
        /// </summary>
        private IMixedRealityBoundarySystem boundaryManager = null;

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
            if (!IsBoundarySystemEnabled())
            {
                return;
            }
            boundaryManager = boundaryManager ?? MixedRealityManager.Instance?.GetManager<IMixedRealityBoundarySystem>();

            InscribedRectangle inscribedRectangle = boundaryManager?.InscribedRectangularBounds;

            if (inscribedRectangle != null)
            {
                Vector3 center = new Vector3(inscribedRectangle.Center.x, 0f, inscribedRectangle.Center.y);

                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.SetParent(transform);
                quad.transform.Translate(center + new Vector3(0.0f, 0.005f, 0.0f)); // Add fudge factor to avoid z-fighting
                quad.transform.Rotate(new Vector3(90, -inscribedRectangle.Angle, 0));
                quad.transform.localScale = new Vector3(inscribedRectangle.Width, inscribedRectangle.Height, 1.0f);
                quad.GetComponent<Renderer>().sharedMaterial = inscribedRectangleMaterial;
            }
        }

        /// <summary>
        /// Displays the boundary as an array of spheres where spheres in the
        /// bounds are a different color.
        /// </summary>
        private void AddIndicators()
        {
            if (IsBoundarySystemEnabled())
            {
                boundaryManager = boundaryManager ?? MixedRealityManager.Instance?.GetManager<IMixedRealityBoundarySystem>();
            }

            const int indicatorCount = 20;
            const float indicatorDistance = 0.2f;
            const float dimension = indicatorCount * indicatorDistance;

            Vector3 center = Vector3.zero;
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

                    if (boundaryManager != null)
                    {
                        // Check inscribed rectangle first
                        if (boundaryManager.Contains(position, Boundary.Type.PlayArea))
                        {
                            material = inscribedRectangleMaterial;
                        }
                        // Then check geometry
                        else if (boundaryManager.Contains(position, Boundary.Type.TrackedArea))
                        {
                            material = boundsMaterial;
                        }
                    }

                    marker.GetComponent<MeshRenderer>().sharedMaterial = material;
                }
            }
        }

        private bool IsBoundarySystemEnabled()
        {
            if (!MixedRealityManager.HasActiveProfile)
            {
                return false;
            }

            MixedRealityConfigurationProfile profile = MixedRealityManager.Instance?.ActiveProfile;
            return (bool)(profile?.EnableBoundarySystem);
        }
    }
}
