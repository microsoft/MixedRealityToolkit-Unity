using System;
using UnityEngine;
using UnityEngine.EventSystems;
using HoloToolkit.UI.Keyboard;

/// <summary>
/// Class that when placed on an input field will enable keyboard on click
/// </summary>
public class KeyboardInputField : UnityEngine.UI.InputField
{
    /// <summary>
    /// Internal field for overriding keyboard spawn point
    /// </summary>
    [SerializeField]
    private Transform m_KeyboardSpawnPoint = null;

    /// <summary>
    /// Override OnPointerClick to spawn keybaord
    /// </summary>
    public override void OnPointerClick(PointerEventData eventData)
    {
        Keyboard.Instance.Close();
        Keyboard.Instance.PresentKeyboard(this.text, Keyboard.LayoutType.Alpha);

        if (m_KeyboardSpawnPoint != null)
        {
            Keyboard.Instance.RepositionKeyboard(m_KeyboardSpawnPoint, null, 0.045f);
        }

        Keyboard.Instance.onTextUpdated += this.Keyboard_onTextUpdated;
    }

    /// <summary>
    /// Delegate function for getting keyboard input
    /// </summary>
    /// <param name="newText"></param>
    private void Keyboard_onTextUpdated(string newText)
    {
        if (!string.IsNullOrEmpty(newText))
        {
            this.text = newText;
        }
    }
}
