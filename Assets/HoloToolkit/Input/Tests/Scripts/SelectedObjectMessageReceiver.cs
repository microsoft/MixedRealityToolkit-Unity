using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// SelectedObjectMessageReceiver class shows how to handle messages sent by SelectedObjectMessageSender.
    /// This particular implementatoin controls object appearance by changing its color when selected.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class SelectedObjectMessageReceiver : MonoBehaviour
    {
        [Tooltip("Object color changes to this when selected.")]
        public Color SelectedColor = Color.red;

        private Color originalColor;
        private Material cachedMaterial;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
            originalColor = cachedMaterial.GetColor("_Color");
        }

        public void OnSelectObject()
        {
            cachedMaterial.SetColor("_Color", SelectedColor);
        }

        public void OnClearSelection()
        {
            cachedMaterial.SetColor("_Color", originalColor);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }
    }
}