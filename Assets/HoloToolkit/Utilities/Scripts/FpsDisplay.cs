using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Simple Behaviour which calculates the frames per second and shows the FPS in a referenced Text control.
    /// </summary>
    public class FpsDisplay : MonoBehaviour
    {
        [Tooltip("Reference to Text UI control where the FPS should be displayed.")]
        public Text Text;

        private float deltaTime;

        private void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            var msec = deltaTime * 1000.0f;
            var fps = 1.0f / deltaTime;
            var text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            Text.text = text;
            // Debug.Log(text);
        }
    }
}