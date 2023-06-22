// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// Class to bring up the non native keyboard for a MRTK TMP Input Field and routes the keyboard input into the input field.
    /// Requires the MRTKTMPInputField prefab or its variants or the TMP Input Field to be set up with MRTK Button
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
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
            if (field != null)
            {
                field.keyboardType = (TouchScreenKeyboardType)(-1);
            }
        }

        private void OnEnable()
        {
            field.onSelect.AddListener(OnInputFieldSelected);
        }

        private void OnDisable()
        {
            field.onSelect.RemoveListener(OnInputFieldSelected);
        }

        private void OnInputFieldSelected(string _)
        {
            PresentKeyboard();
        }

        /// <summary>
        /// Show the non native keyboard
        /// </summary>
        public void PresentKeyboard()
        {
            NonNativeKeyboard keyboard = NonNativeKeyboard.Instance;
            keyboard.Open();
            keyboard.OnClose?.AddListener(OnKeyboardClose);
            keyboard.OnTextUpdate?.AddListener(UpdateText);
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
            keyboard.OnTextUpdate?.RemoveListener(UpdateText);
            keyboard.OnClose?.RemoveListener(OnKeyboardClose);
        }
    }
}
