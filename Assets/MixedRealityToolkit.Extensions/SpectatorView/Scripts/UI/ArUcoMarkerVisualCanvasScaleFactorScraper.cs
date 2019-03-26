using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    /// <summary>
    /// Helper class for obtaining additional scaling information to apply to ArUco Marker Images
    /// </summary>
    public class ArUcoMarkerVisualCanvasScaleFactorScraper : MonoBehaviour
    {
        /// <summary>
        /// Parent canvas containing ArUco marker visual.
        /// </summary>
        [Tooltip("Parent canvas containing ArUco marker visual.")]
        [SerializeField]
        protected Canvas _parentCanvas;

        /// <summary>
        /// ArUcoMarkerVisual requiring additional scaling based on parent canvas.
        /// </summary>
        [Tooltip("ArUcoMarkerVisual requiring additional scaling based on parent canvas.")]
        [SerializeField]
        protected ArUcoMarkerVisual _markerVisual;

        protected void Awake()
        {
            if (_parentCanvas == null)
            {
                Debug.LogError("Parent canvas not defined for MarkerVisualCanvasSizeScraper");
                return;
            }

            if (_markerVisual == null)
            {
                Debug.LogError("ArUcoMarkerVisual not dfined for MarkerVisualCanvasSizeScraper");
            }

            Debug.Log("ArUcoMarkerVisual found to have parent canvas with scale factor of: " + _parentCanvas.scaleFactor);
            _markerVisual._additionalScaleFactor = 1.0f / _parentCanvas.scaleFactor;
        }
    }

}