using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardTest : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Keyboard keyboard = null;

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
        keyboard.Close();

        keyboard.OnTextUpdated -= UpdateText;
        keyboard.OnClosed -= DisableKeyboard;
        keyboard.OnTextSubmitted -= DisableKeyboard;
    }
}
