using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class ArUcoMarkerVisualCanvasScaleFactorScraper : MonoBehaviour
    {
        [SerializeField] Canvas _parentCanvas;
        [SerializeField] ArUcoMarkerVisual _markerVisual;

        void Awake()
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