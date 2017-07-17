// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class TextToSpeechManagerTest : MonoBehaviour
    {
        public TextToSpeechManager TextToSpeechManager;
#if UNITY_WSA
        private GestureRecognizer gestureRecognizer;

        private void Start()
        {
            // Set up a GestureRecognizer to detect Select gestures.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (GazeManager.Instance.IsGazingAtObject)
            {
                // Get the target object
                GameObject obj = GazeManager.Instance.HitInfo.collider.gameObject;

                // Try and get a TTS Manager
                var tts = obj.GetComponent<TextToSpeechManager>();

                if (tts == null) { return; }

                // If we have a text to speech manager on the target object, say something.
                // This voice will appear to emanate from the object.
                if (!tts.IsSpeaking())
                {
                    // Get the name
                    var voiceName = Enum.GetName(typeof(TextToSpeechVoice), tts.Voice);

                    // Create message
                    var msg = string.Format(
                        "This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.",
                        voiceName);

                    // Speak message
                    tts.SpeakText(msg);
                }
                else if (tts.IsSpeaking())
                {
                    tts.StopSpeaking();
                }
            }
        }
#endif

        public void SpeakTime()
        {
            // Say something using the text to speech manager on THIS test class (the "global" one).
            // This voice will appear to follow the user.
            TextToSpeechManager.SpeakText("The time is " + DateTime.Now.ToString("t"));
        }
    }
}
