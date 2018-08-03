// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
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
        [Tooltip("Should the play area be visualized?")]
        [SerializeField]
        private bool isPlayAreaVisualized = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not the play area is / should be visualized.
        /// </summary>
        public bool IsPlayAreaVisualized
        {
            get { return isPlayAreaVisualized; }
            set
            {
                if (value != isPlayAreaVisualized)
                {
                    isPlayAreaVisualized = value;
                }
            }
        }

        /// <summary>
        /// Boundary system implementation.
        /// </summary
        private IMixedRealityBoundarySystem boundaryManager = null;
        private IMixedRealityBoundarySystem BoundaryManager => boundaryManager ?? (boundaryManager = MixedRealityManager.Instance.GetManager<IMixedRealityBoundarySystem>());

        private MixedRealityBoundaryVisualizationProfile visualizationProfile;

        /// <summary>
        /// The <see cref="GameObject"/> (Quad) used to visualize the play area.
        /// </summary>
        private GameObject playArea = null;

        private void Start()
        {
            if (MixedRealityManager.HasActiveProfile && MixedRealityManager.Instance.ActiveProfile.IsBoundarySystemEnabled)
            {
                visualizationProfile = MixedRealityManager.Instance.ActiveProfile.BoundaryVisualizationProfile;
            }
        }

        private void Update()
        {
            if ((playArea != null) &&
                (isPlayAreaVisualized != playArea.activeSelf))
            {
                // Show/hide the play area visualization
                playArea.SetActive(isPlayAreaVisualized);
            }
            else if ((playArea == null) && (isPlayAreaVisualized))
            {
                if (MixedRealityManager.HasActiveProfile && MixedRealityManager.Instance.ActiveProfile.IsBoundarySystemEnabled)
                {
                    CreatePlayAreaObject();
                }
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
                (visualizationProfile == null))
            {
                // No rectangular bounds, therefore do not render the quad.
                return;
            }
            
            // Render the rectangular bounds.
            if (EdgeUtilities.IsValidPoint(center))
            {
                playArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
                playArea.transform.SetParent(transform);
                playArea.transform.Translate(new Vector3(center.x, 0.005f, center.y)); // Add fudge factor to avoid z-fighting
                playArea.transform.Rotate(new Vector3(90, -angle, 0));
                playArea.transform.localScale = new Vector3(width, height, 1.0f);
                playArea.GetComponent<Renderer>().sharedMaterial = visualizationProfile.PlayAreaMaterial;
            }
        }
    }
}
