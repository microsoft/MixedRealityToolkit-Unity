using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Lighthouse
{
    public class GazeableColorPicker : MonoBehaviour
    {
        public Renderer rendererComponent;

        [System.Serializable]
        public class PickedColorCallback : UnityEvent<Color> { }

        public PickedColorCallback OnGazedColor = new PickedColorCallback();
        public PickedColorCallback OnPickedColor = new PickedColorCallback();

        private bool gazing = false;

        void OnGazeEnter()
        {
            gazing = true;
        }

        void OnGazeLeave()
        {
            gazing = false;
        }

        void OnSelect()
        {
            UpdatePickedColor(OnPickedColor);
        }

        void Update()
        {
            if (gazing == false) return;
            UpdatePickedColor(OnGazedColor);
        }

        void UpdatePickedColor(PickedColorCallback cb)
        {
            RaycastHit hit = HoloToolkit.Unity.GazeManager.Instance.HitInfo;
            if (hit.transform.gameObject != rendererComponent.gameObject) return;
            
            Texture2D texture = rendererComponent.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;

            Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            cb.Invoke(col);
        }
    }
}