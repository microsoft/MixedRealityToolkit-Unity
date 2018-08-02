using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace Microsoft.MixedReality.Toolkit.SDK.Boundary
{
    public class VisualizePlayArea : MonoBehaviour
    {
        [Tooltip("The material to use when visualizing the play area rectangle.")]
        [SerializeField]
        private Material playAreaMaterial = null;

        /// <summary>
        /// The material to use when visualizing the play area rectangle.
        /// </summary>
        public Material PlayAreaMaterial
        {
            get { return playAreaMaterial; }
            set { playAreaMaterial = value; }
        }

        [Tooltip("Should the play area be visualized?")]
        [SerializeField]
        private bool isPlayAreaVisualized = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not the play area is visualized.
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

        /// <summary>
        /// The <see cref="GameObject"/> (Quad) used to visualize the play area.
        /// </summary>
        private GameObject playArea = null;

        private void Update()
        {
            if (!isPlayAreaVisualized)
            {
                // Hide the play area object
                playArea?.SetActive(false);
            }
            else
            {
                if (playArea != null)
                {
                    playArea.SetActive(true);
                }
                else
                {
                    CreatePlayAreaObject();
                }
            }
        }

        private void CreatePlayAreaObject()
        {
            // Get the rectangular bounds.
            Vector2 center;
            float angle;
            float width;
            float height;
            if ((BoundaryManager == null) || !BoundaryManager.TryGetRectangularBoundsParams(out center, out angle, out width, out height))
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
                playArea.GetComponent<Renderer>().sharedMaterial = playAreaMaterial;
            }
        }
    }
}
