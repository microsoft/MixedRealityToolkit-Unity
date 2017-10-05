// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.UI.Keyboard
{
    /// <summary>
    /// Represents a key on the keyboard that has a string value for input.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class KeyboardValueKey : MonoBehaviour
    {
        /// <summary>
        /// The default string value for this key.
        /// </summary>
        public string Value;

        /// <summary>
        /// The shifted string value for this key.
        /// </summary>
        public string ShiftValue;

        /// <summary>
        /// Reference to child text element.
        /// </summary>
        private Text m_Text;

        /// <summary>
        /// Reference to the GameObject's Button component.
        /// </summary>
        private Button m_Button;

        /// <summary>
        /// Get the button component.
        /// </summary>
        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        /// <summary>
        /// Initialize key text, subscribe to the onClick event, and subscribe to keyboard shift event.
        /// </summary>
        private void Start()
        {
            m_Text = gameObject.GetComponentInChildren<Text>();
            m_Text.text = Value;

            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(FireAppendValue);

            Keyboard.Instance.OnKeyboardShifted += Shift;
        }

        /// <summary>
        /// Method injected into the button's onClick listener.
        /// </summary>
        private void FireAppendValue()
        {
            Keyboard.Instance.AppendValue(this);
        }

        /// <summary>
        /// Called by the Keyboard when the shift key is pressed. Updates the text for this key using the Value and ShiftValue fields.
        /// </summary>
        /// <param name="isShifted"></param>
        public void Shift(bool isShifted)
        {
            // Shift value should only be applied if a shift value is present.
            if (isShifted && !string.IsNullOrEmpty(ShiftValue))
            {
                m_Text.text = ShiftValue;
            }
            else
            {
                m_Text.text = Value;
            }
        }
    }
}
