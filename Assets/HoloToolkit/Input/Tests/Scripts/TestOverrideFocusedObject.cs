using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TestOverrideFocusedObject : MonoBehaviour, IInputClickHandler
    {
        TextMesh textMesh;

        void Start()
        {
            InputManager inputManager = InputManager.Instance;

            if (inputManager != null)
            {
                inputManager.OverrideFocusedObject = this.gameObject;
            }

            textMesh = GameObject.FindObjectOfType<TextMesh>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (textMesh != null)
            {
                textMesh.text = "Air tap worked!";
            }
        }
    }
}