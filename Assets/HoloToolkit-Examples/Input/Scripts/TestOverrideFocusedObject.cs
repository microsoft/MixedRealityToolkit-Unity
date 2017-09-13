using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TestOverrideFocusedObject : MonoBehaviour, IInputClickHandler
    {
        InputManager inputManager;
        TextMesh textMesh;

        int clickCount;
        bool isOverridingFocus;

        void Start()
        {
            inputManager = InputManager.Instance;

            if (inputManager != null)
            {
                OverrideFocus();
            }

            textMesh = GameObject.FindObjectOfType<TextMesh>();

            if (textMesh != null)
            {
                UpdateText();
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (textMesh != null && inputManager != null)
            {
                clickCount++;

                if (isOverridingFocus)
                {
                    UndoOverrideFocus();
                }
                else
                {
                    OverrideFocus();
                }

                UpdateText();

                eventData.Use();
            }
        }

        private void OverrideFocus()
        {
            inputManager.AddGlobalListener(gameObject);
            isOverridingFocus = true;
        }

        private void UndoOverrideFocus()
        {
            inputManager.RemoveGlobalListener(gameObject);
            isOverridingFocus = false;
        }

        private void UpdateText()
        {
            textMesh.text = string.Format("The click handler was called {0} times. Currently, focus is {1}.",
                clickCount,
                isOverridingFocus ? "overridden" : "not overridden"
                );
        }
    }
}