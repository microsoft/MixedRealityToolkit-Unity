// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// Class representing a value key in the non native keyboard
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
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
        /// The default string value for this key.
        /// </summary>
        public string DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        /// <summary>
        /// The shifted string value for this key.
        /// </summary>
        [SerializeField, Tooltip("The shifted string value for this key.")]
        private string shiftedValue = null;

        /// <summary>
        /// The shifted string value for this key.
        /// </summary>
        public string ShiftedValue
        {
            get => shiftedValue;
            set => shiftedValue = value;
        }

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

            if (string.IsNullOrEmpty(shiftedValue))
            {
                shiftedValue = defaultValue;
            }
        }

        private void Start()
        {
            NonNativeKeyboard.Instance?.OnKeyboardShifted?.AddListener(Shift);
        }

        private void OnValidate()
        {
            if (textMeshProText == null)
            {
                textMeshProText = GetComponentInChildren<TMP_Text>();
            }
            if (textMeshProText != null)
            {
                textMeshProText.text = defaultValue;
            }
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
