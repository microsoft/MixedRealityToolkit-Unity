// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Boundary.Tests
{
    /// <summary>
    /// Demo class to show different ways of using the boundary API.
    /// </summary>
    public class BoundaryVisualizer : MonoBehaviour
    {
#if UNITY_2017_2_OR_NEWER
        [SerializeField]
        [Tooltip("Material used to draw the inscribed rectangle bounds.")]
        private Material boundsMaterial = null;

        [SerializeField]
        [Tooltip("Material used to draw items in the tracked area bounds.")]
        private Material trackedAreaBoundsMaterial = null;

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
            Vector3 center;
            float angle;
            float width;
            float height;
            BoundaryManager.Instance.TryGetBoundaryRectangleParams(out center, out angle, out width, out height);

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(transform);
            quad.transform.Translate(center + new Vector3(0.0f, 0.005f, 0.0f)); // Add fudge factor to avoid z-fighting
            quad.transform.Rotate(new Vector3(90, -angle, 0));
            quad.transform.localScale = new Vector3(width, height, 1.0f);
        }

        /// <summary>
        /// Displays the boundary as a rectangle using a LineRenderer.
        /// </summary>
        private void AddRectangleBounds()
        {
            var points = BoundaryManager.Instance.TryGetBoundaryRectanglePoints();
            if (points == null)
            {
                return;
            }

            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.sharedMaterial = boundsMaterial;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.positionCount = points.Length;
            lr.SetPositions(points);
        }

        /// <summary>
        /// Displays the boundary as an array of spheres where spheres in the
        /// bounds are a different color.
        /// </summary>
        private void AddIndicators()
        {
            const int indicatorCount = 15;
            const float indicatorDistance = 0.2f;
            const float dimension = indicatorCount * indicatorDistance;

            Vector3 center;
            float angle;
            float width;
            float height;
            if (!BoundaryManager.Instance.TryGetBoundaryRectangleParams(out center, out angle, out width, out height))
            {
                return;
            }

            Vector3 corner = center - (new Vector3(dimension, 0.0f, dimension) / 2.0f);
            corner.y += 0.05f;
            for (int xIndex = 0; xIndex < indicatorCount; ++xIndex)
            {
                for (int yIndex = 0; yIndex < indicatorCount; ++yIndex)
                {
                    var offset = new Vector3(xIndex * indicatorDistance, 0.0f, yIndex * indicatorDistance);
                    var position = corner + offset;
                    var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    marker.transform.SetParent(transform);
                    marker.transform.position = position;
                    marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    var markerRenderer = marker.GetComponent<MeshRenderer>();

                    if (BoundaryManager.Instance.ContainsObject(position, UnityEngine.Experimental.XR.Boundary.Type.TrackedArea))
                    {
                        markerRenderer.sharedMaterial = trackedAreaBoundsMaterial;
                    }

                    if (BoundaryManager.Instance.ContainsObject(position, UnityEngine.Experimental.XR.Boundary.Type.PlayArea))
                    {
                        markerRenderer.sharedMaterial = boundsMaterial;
                    }
                }
            }
        }
#endif
    }
}
