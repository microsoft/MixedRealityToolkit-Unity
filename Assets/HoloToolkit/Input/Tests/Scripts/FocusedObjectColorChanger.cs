using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// FocusedObjectMessageReceiver class shows how to handle focus events.
    /// This particular implementatoin controls object appearance by changing its color when focused.
    /// </summary>
    public class FocusedObjectColorChanger : MonoBehaviour, IFocusable
    {
        [Tooltip("Object color changes to this when focused.")] public Color FocusedColor = Color.red;

        private Material material;
        private Color originalColor;

        private void Start()
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.color;
        }

        public void OnFocusEnter()
        {
            material.color = FocusedColor;
        }

        public void OnFocusExit()
        {
            material.color = originalColor;
        }
    }
}
