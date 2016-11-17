using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace Lighthouse
{
    public class GazeableColorPicker : MonoBehaviour, IFocusable, IInputClickHandler
    {
        public Renderer rendererComponent;

        [System.Serializable]
        public class PickedColorCallback : UnityEvent<Color> { }

        public PickedColorCallback OnGazedColor = new PickedColorCallback();
        public PickedColorCallback OnPickedColor = new PickedColorCallback();

        private bool gazing = false;

        void Update()
        {
            if (gazing == false) return;
            UpdatePickedColor(OnGazedColor);
        }

        void UpdatePickedColor(PickedColorCallback cb)
        {
            RaycastHit hit = GazeManager.Instance.HitInfo;
            if (hit.transform.gameObject != rendererComponent.gameObject) return;
            
            Texture2D texture = rendererComponent.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;

            Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            cb.Invoke(col);
        }

        public void OnFocusEnter()
        {
            gazing = true;
        }

        public void OnFocusExit()
        {
            gazing = false;
        }

        public void OnInputClicked(InputEventData eventData)
        {
            UpdatePickedColor(OnPickedColor);
        }
    }
}