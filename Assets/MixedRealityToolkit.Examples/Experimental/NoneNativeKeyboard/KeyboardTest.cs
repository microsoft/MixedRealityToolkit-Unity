using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// This component links the NoneNativeKeyboard to a TMP_InputField
    /// Put it on the TMP_InputField and assign the NoneNativeKeyboard.prefab
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class KeyboardTest : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private NoneNativeKeyboard keyboard = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            keyboard.PresentKeyboard();

            keyboard.OnClosed += DisableKeyboard;
            keyboard.OnTextSubmitted += DisableKeyboard;
            keyboard.OnTextUpdated += UpdateText;
        }

        private void UpdateText(string text)
        {
            GetComponent<TMP_InputField>().text = text;
        }

        private void DisableKeyboard(object sender, EventArgs e)
        {
            keyboard.OnTextUpdated -= UpdateText;
            keyboard.OnClosed -= DisableKeyboard;
            keyboard.OnTextSubmitted -= DisableKeyboard;

            keyboard.Close();
        }
    }
}