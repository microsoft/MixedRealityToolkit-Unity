using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TestOverrideFocusedObject : MonoBehaviour, IInputClickHandler
    {
        InputManager inputManager;
        TextMesh textMesh;

        void Start()
        {
            inputManager = InputManager.Instance;

            if (inputManager != null)
            {
                inputManager.OverrideFocusedObject = this.gameObject;
            }

            textMesh = GameObject.FindObjectOfType<TextMesh>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (textMesh != null && inputManager != null)
            {
                textMesh.text = "Air tap worked and OverrideFocusedObject is null.";
                inputManager.OverrideFocusedObject = null;
            }
        }
    }
}