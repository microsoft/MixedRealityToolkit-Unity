// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.Tests
{
    public class TextToSpeechOnTap : MonoBehaviour, IInputHandler
    {
        public TextToSpeechManager TextToSpeech;

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            // Nothing.
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.PressType == InteractionSourcePressType.Select)
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

                    eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
                }
            }
        }
    }
}
