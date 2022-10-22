// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that is ideal for AR/VR applications that do not provide a native keyboard.
    /// </summary>
    public class NonNativeValueKey : NonNativeKey
    {
        private string currentValue;

        public string CurrentValue
        {
            get => currentValue;
            private set
            {
                currentValue = value;
                if (textMeshProUGUI != null)
                {
                    textMeshProUGUI.text = currentValue;
                }
            }
        }

        /// <summary>
        /// The default string value for this key.
        /// </summary>
        [SerializeField, Tooltip("The type of this button.")]
        private string defaultValue;

        /// <summary>
        /// The shifted string value for this key.
        /// </summary>
        private string shiftedValue = null;

        /// <summary>
        /// Reference to child text element.
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI textMeshProUGUI;

        /// <summary>
        /// Get the button component.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (textMeshProUGUI == null)
            {
                textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
            }

            CurrentValue = defaultValue;

            if (char.TryParse(defaultValue, out char parsedChar) && char.IsLower(parsedChar))
            {
                shiftedValue = char.ToUpperInvariant(parsedChar).ToString();
            }
            else
            {
                shiftedValue = defaultValue;
            }
        }

        private void Start()
        {
            NonNativeKeyboard.Instance.OnKeyboardShifted.AddListener(Shift);
        }

        private void OnValidate()
        {
            if (textMeshProUGUI == null)
            {
                textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
            }
            textMeshProUGUI.text = defaultValue;
        }

        protected override void FireKey()
        {
            NonNativeKeyboard.Instance.ProcessValueKeyPress(this);
        }

        /// <summary>
        /// Called by the Keyboard when the shift key is pressed. Updates the text for this key using the Value and ShiftValue fields.
        /// </summary>
        /// <param name="isShifted">Indicates the state of shift, the key needs to be changed to.</param>
        public void Shift(bool isShifted)
        {
            // Shift value should only be applied if a shift value is present.
            if (isShifted)
            {
                CurrentValue = shiftedValue;
            }
            else
            {
                CurrentValue = defaultValue;
            }
        }
    }
}
