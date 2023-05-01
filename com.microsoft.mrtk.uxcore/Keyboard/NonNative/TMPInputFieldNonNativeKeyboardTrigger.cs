// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Class to bring up the non native keyboard for a MRTK TMP Input Field and routes the keyboard input into the input field.
    /// Requires the MRTKTMPInputField prefab or its variants or the TMP Input Field to be set up with MRTK Button
    /// </summary>
    public class TMPInputFieldNonNativeKeyboardTrigger : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField field;

        private void Awake()
        {
            if (field == null)
            {
                field = GetComponent<TMP_InputField>();
            }
        }

        /// <summary>
        /// Show the non native keyboard
        /// </summary>
        public void PresentKeyboard()
        {
            NonNativeKeyboard keyboard = NonNativeKeyboard.Instance;
            if (keyboard.Active)
            {
                keyboard.Close();
            }
            keyboard.Open(field.text);
            keyboard.OnClose.AddListener(OnKeyboardClose);
            keyboard.OnTextUpdate.AddListener(UpdateText);
        }

        private void UpdateText(string text)
        {
            field.text = text;
        }

        private void OnKeyboardClose(string _)
        {
            RemoveListeners();
        }

        private void RemoveListeners()
        {
            NonNativeKeyboard keyboard = NonNativeKeyboard.Instance;
            keyboard.OnTextUpdate.RemoveListener(UpdateText);
            keyboard.OnClose.RemoveListener(OnKeyboardClose);
        }
    }
}
