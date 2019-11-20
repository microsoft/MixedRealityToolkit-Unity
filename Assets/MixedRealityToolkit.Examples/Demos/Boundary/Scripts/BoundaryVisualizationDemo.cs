// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityBoundary = UnityEngine.Experimental.XR.Boundary;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demo class to show different ways of using the boundary system and visualizing the data.
    /// </summary>
    public class BoundaryVisualizationDemo : MonoBehaviour, IMixedRealityBoundaryHandler
    {
        private GameObject markerParent;
        private readonly List<GameObject> markers = new List<GameObject>();

        [SerializeField]
        private bool showFloor = true;

        [SerializeField]
        private bool showPlayArea = true;

        [SerializeField]
        private bool showTrackedArea = true;

        [SerializeField]
        private bool showBoundaryWalls = true;

        [SerializeField]
        private bool showBoundaryCeiling = true;

        private IMixedRealityBoundarySystem boundarySystem = null;

        private IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                if (boundarySystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityBoundarySystem>(out boundarySystem);
                }
                return boundarySystem;
            }
        }

        #region MonoBehaviour Implementation

        private void Awake()
        {
            markerParent = new GameObject();
            markerParent.name = "Boundary Demo Markers";
            MixedRealityPlayspace.AddChild(markerParent.transform);
        }

        private void Start()
        {
            if (BoundarySystem != null)
            {
                if (markers.Count == 0)
                {
                    AddMarkers();
                }
            }
        }

        private void Update()
        {
            if (BoundarySystem != null)
            {
                BoundarySystem.ShowFloor = showFloor;
                BoundarySystem.ShowPlayArea = showPlayArea;
                BoundarySystem.ShowTrackedArea = showTrackedArea;
                BoundarySystem.ShowBoundaryWalls = showBoundaryWalls;
                BoundarySystem.ShowBoundaryCeiling = showBoundaryCeiling;
            }
        }

        private void OnEnable()
        {
            BoundarySystem?.RegisterHandler<IMixedRealityBoundaryHandler>(this);
        }

        private void OnDisable()
        {
            BoundarySystem?.UnregisterHandler<IMixedRealityBoundaryHandler>(this);
        }

        #endregion MonoBehaviour Implementation

        #region IMixedRealityBoundaryHandler Implementation

        /// <inheritdoc />
        public void OnBoundaryVisualizationChanged(BoundaryEventData eventData)
        {
            Debug.Log("[BoundaryVisualizationDemo] Boundary visualization changed.");
        }

        #endregion IMixedRealityBoundaryHandler Implementation

        /// <summary>
        /// Displays the boundary as an array of spheres where spheres in the
        /// bounds are a different color.
        /// </summary>
        private void AddMarkers()
        {
            // Get the rectangular bounds.
            Vector2 centerRect;
            float angleRect;
            float widthRect;
            float heightRect;

            if (BoundarySystem == null) { return; }

            if (!BoundarySystem.TryGetRectangularBoundsParams(out centerRect, out angleRect, out widthRect, out heightRect))
            {
                // If we have no boundary manager or rectangular bounds we will show no indicators
                return;
            }

            // Get the materials needed for marker display
            GameObject playArea = BoundarySystem.GetPlayAreaVisualization();
            if (playArea == null)
            {
                // Failed to get the play area visualization;
                return;
            }
            Material playAreaMaterial = playArea.GetComponent<Renderer>().sharedMaterial;

            GameObject trackedArea = BoundarySystem.GetTrackedAreaVisualization();
            if (trackedArea == null)
            {
                // Failed to get the tracked area visualization;
                return;
            }
            Material trackedAreaMaterial = trackedArea.GetComponent<Renderer>().sharedMaterial;

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

                    Material material = null;
                    // Check inscribed rectangle first
                    if (BoundarySystem.Contains(position, UnityBoundary.Type.PlayArea))
                    {
                        material = playAreaMaterial;
                    }
                    // Then check geometry
                    else if (BoundarySystem.Contains(position, UnityBoundary.Type.TrackedArea))
                    {
                        material = trackedAreaMaterial;
                    }

                    if (material != null)
                    {
                        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        marker.name = "Boundary Demo Marker";
                        marker.transform.parent = markerParent.transform;
                        marker.transform.position = position;
                        marker.transform.localScale = Vector3.one * indicatorScale;
                        marker.GetComponent<MeshRenderer>().sharedMaterial = material;
                        markers.Add(marker);
                    }
                }
            }
        }
    }
}
