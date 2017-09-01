// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TextToSpeechOnTap : MonoBehaviour, IInputClickHandler
    {
        public TextToSpeechManager TextToSpeech;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // If we have a text to speech manager on the target object, say something.
            // This voice will appear to emanate from the object.
            if (TextToSpeech != null)
            {
                // Get the name
                var voiceName = Enum.GetName(typeof(TextToSpeechVoice), TextToSpeech.Voice);

                // Create message
                var msg = string.Format("This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.", voiceName);

                // Speak message
                TextToSpeech.SpeakText(msg);
            }
        }
    }
}
