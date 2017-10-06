// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TextToSpeechTest : MonoBehaviour
    {
        private TextToSpeech textToSpeech;

        private void Awake()
        {
            textToSpeech = GetComponent<TextToSpeech>();
        }

        public void SpeakTime()
        {
            // Say something using the text to speech manager on THIS test class (the "global" one).
            // This voice will appear to follow the user.
            textToSpeech.StartSpeaking("The time is " + DateTime.Now.ToString("t"));
        }
    }
}
