// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Class representing a value key in the non native keyboard
    /// </summary>
    public class NonNativeValueKey : NonNativeKey
    {
        private string currentValue;

        /// <summary>
        /// The current string value of this value key. Note the value may change based on the shift status of the keyboard.
        /// </summary>
        public string CurrentValue
        {
            get => currentValue;
            private set
            {
                currentValue = value;
                if (textMeshProText != null)
                {
                    textMeshProText.text = currentValue;
                }
            }
        }

        /// <summary>
        /// The default string value for this key.
        /// </summary>
        [SerializeField, Tooltip("The default string value for this key.")]
        private string defaultValue;

        /// <summary>
        /// The shifted string value for this key.
        /// </summary>
        private string shiftedValue = null;

        /// <summary>
        /// Reference to child text element.
        /// </summary>
        [SerializeField, Tooltip("Reference to child text element.")]
        private TMP_Text textMeshProText;

        protected override void Awake()
        {
            base.Awake();
            if (textMeshProText == null)
            {
                textMeshProText = GetComponentInChildren<TMP_Text>();
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
            if (textMeshProText == null)
            {
                textMeshProText = GetComponentInChildren<TMP_Text>();
            }
            textMeshProText.text = defaultValue;
        }

        /// <inheritdoc/>
        protected override void FireKey()
        {
            NonNativeKeyboard.Instance.ProcessValueKeyPress(this);
        }

        private void Shift(bool isShifted)
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
