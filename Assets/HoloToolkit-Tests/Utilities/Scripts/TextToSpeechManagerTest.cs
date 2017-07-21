// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;
using System;
using HoloToolkit.Unity.InputModule;

#if UNITY_EDITOR || UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif

public class TextToSpeechManagerTest : MonoBehaviour
{
    public TextToSpeechManager textToSpeechManager;
#if UNITY_EDITOR || UNITY_WSA
    private GestureRecognizer gestureRecognizer;

    // Use this for initialization
    void Start ()
    {
        // Set up a GestureRecognizer to detect Select gestures.
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;
        gestureRecognizer.StartCapturingGestures();
    }

    private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        GazeManager gm = GazeManager.Instance;
        if (gm.IsGazingAtObject)
        {
            // Get the target object
            GameObject obj = gm.HitInfo.collider.gameObject;

            // Try and get a TTS Manager
            TextToSpeechManager tts = null;
            if (obj != null)
            {
                tts = obj.GetComponent<TextToSpeechManager>();
            }

            // If we have a text to speech manager on the target object, say something.
            // This voice will appear to emanate from the object.
            if (tts != null && !tts.IsSpeaking())
            {
                // Get the name
                var voiceName = Enum.GetName(typeof(TextToSpeechVoice), tts.Voice);

                // Create message
                var msg = string.Format("This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.", voiceName);

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
        textToSpeechManager.SpeakText("The time is " + DateTime.Now.ToString("t"));
    }
}
