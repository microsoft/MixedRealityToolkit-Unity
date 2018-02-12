// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.Utilities;
using UnityEngine;

namespace MixedRealityToolkit.Examples.Utilities
{
    /// <summary>
    /// Tap Test for text to speech. This voice will appear to emanate from the object
    /// </summary>
    public class TextToSpeechOnTapTest : MonoBehaviour, IInputClickHandler
    {
        private TextToSpeech textToSpeech;

        private void Awake()
        {
            textToSpeech = GetComponent<TextToSpeech>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // If we have a text to speech manager on the target object, say something.
            // This voice will appear to emanate from the object.
            if (textToSpeech != null && eventData.PressType == InteractionSourcePressInfo.Select)
            {
                // Create message
                var msg = string.Format(
                "This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.",
                textToSpeech.Voice.ToString());

                // Speak message
                textToSpeech.StartSpeaking(msg);

                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
            }
        }
    }
}
