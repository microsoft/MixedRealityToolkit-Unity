using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.SpatialCoordinates
{
    public class MarkerSpatialCoordinateServiceVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceVisual
    {
        [SerializeField] Text _text;

        public void ShowVisual()
        {
            gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            gameObject.SetActive(false);
        }

        public void UpdateText(string text)
        {
            if (_text == null)
            {
                Debug.LogError("Error: Text not defined for MarkerSpatialCoordinateServiceVisual");
            }

            _text.text = text;
        }
    }
}
