// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.Utilities.Attributes;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Utilities
{
    /// <summary>
    /// This class will automatically link buttons to speech keywords
    /// (Currently disabled)
    /// </summary>
    [RequireComponent (typeof(CompoundButton))]
    public class CompoundButtonSpeech : MonoBehaviour, ISpeechHandler
    {
        public enum KeywordSourceEnum
        {
            None,
            LocalOverride,
            ButtonText,
        }

        /// <summary>
        /// Source of the keyword to be used
        /// By default the text in a CompoundButtonText component will be used
        /// </summary>
        [HideInMRTKInspector]
        public KeywordSourceEnum KeywordSource = KeywordSourceEnum.ButtonText;

        /// <summary>
        /// Keyword used when KeywordSource is set to LocalOverride
        /// </summary>
        [HideInMRTKInspector]
        public string Keyword = string.Empty;

        /// <summary>
        /// Variable to keep track of previous button text in case the button text changes after registration.
        /// </summary>
        private string prevButtonText;

        /// <summary>
        /// The final keyword that is registered with the Keyword Manager
        /// </summary>
        private string keyWord;

        /// <summary>
        /// Have internal member reference to button
        /// </summary>
        private CompoundButton m_button;

        /// <summary>
        /// Have internal member reference to button
        /// </summary>
        private CompoundButtonText m_button_text;

        public void Start ()
        {
            // Disable if no microphone devices are found
            if (Microphone.devices.Length == 0) {
                enabled = false;
                return;
            }

            if (KeywordSource == KeywordSourceEnum.None)
                return;

            keyWord = string.Empty;

            // Assign internal cached components
            m_button = GetComponent<CompoundButton>();
            m_button_text = GetComponent<CompoundButtonText>();

            switch (KeywordSource)
            {
                case KeywordSourceEnum.ButtonText:
                default:
                    keyWord = prevButtonText = m_button_text.Text;
                    break;

                case KeywordSourceEnum.LocalOverride:
                    keyWord = Keyword;
                    break;
            }


        }

        public void Update()
        {
            // Check if Button text has changed. If so, remove previous keyword and add new button text
            if (KeywordSource == KeywordSourceEnum.ButtonText &&
                prevButtonText != null &&
                m_button_text.Text != prevButtonText)
            {
                prevButtonText = m_button_text.Text;
            }
        }

        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(this.keyWord))
                return;

        }

        /// <summary>
        /// On Speech keyword recognizer handle speech event
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (!gameObject.activeSelf || !enabled)
                return;

            if(eventData.RecognizedText.Equals(keyWord))
            {
                // Send a pressed message to the button through the InputManager
                m_button.TriggerClicked();
            }
        }
    }
}
