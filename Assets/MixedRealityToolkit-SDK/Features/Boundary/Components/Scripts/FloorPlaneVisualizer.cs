// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Boundary
{
    /// <summary>
    /// Visualizes the floor plane.
    /// </summary>
    public class FloorPlaneVisualizer : MonoBehaviour
    {
        [Tooltip("Should the floor plane be visualized?")]
        [SerializeField]
        private bool isFloorPlaneVisualized = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not the floor plane is / should be visualized.
        /// </summary>
        public bool IsFloorPlaneVisualized
        {
            get { return isFloorPlaneVisualized; }
            set
            {
                if (value != isFloorPlaneVisualized)
                {
                    isFloorPlaneVisualized = value;
                }
            }
        }

        private MixedRealityBoundaryVisualizationProfile visualizationProfile;

        /// <summary>
        /// The <see cref="GameObject"/> (Quad) used to visualize the play area.
        /// </summary>
        private GameObject floorPlane = null;

        private void Start()
        {
            if (MixedRealityManager.HasActiveProfile && MixedRealityManager.Instance.ActiveProfile.IsBoundarySystemEnabled)
            {
                visualizationProfile = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile;
            }
        }

        private void Update()
        {
            if ((floorPlane != null) && (isFloorPlaneVisualized != floorPlane.activeSelf))
            {
                // Show/hide the play area visualization
                floorPlane.SetActive(isFloorPlaneVisualized);
            }
            else if ((floorPlane == null) && (isFloorPlaneVisualized))
            {
                CreatePlayAreaObject();
            }
        }

        /// <summary>
        /// Creates a rectangular <see cref="GameObject"/> to represent the 
        /// play area within the configured boundary.
        /// </summary>
        private void CreatePlayAreaObject()
        {
            if (visualizationProfile == null)
            {
                // We do not have a visualization profile configured, therefore do not render the quad.
                return;
            }

            // Render the floor.
            floorPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floorPlane.transform.SetParent(transform);
            floorPlane.transform.Translate(Vector3.zero);
            floorPlane.transform.Rotate(90, 0, 0);
            floorPlane.transform.localScale = visualizationProfile.FloorPlaneScale;
            floorPlane.GetComponent<Renderer>().sharedMaterial = visualizationProfile.FloorPlaneMaterial;
        }
    }
}
