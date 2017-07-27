// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;
using System;
using HoloToolkit.Unity.InputModule;

#if UNITY_EDITOR || UNITY_WSA
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;
#endif

public class TextToSpeechManagerTest : MonoBehaviour, IInputHandler
{
    public TextToSpeechManager textToSpeechManager;

#if UNITY_EDITOR || UNITY_WSA
    private bool started = false;
    private bool addedInputManagerHandler = false;

    private void Start()
    {
        started = true;
        TryAddInputManagerHandler();
    }

    private void OnEnable()
    {
        if (started)
        {
            TryAddInputManagerHandler();
        }
    }

    private void TryAddInputManagerHandler()
    {
        if (!addedInputManagerHandler)
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
            addedInputManagerHandler = true;
        }
    }

    private void OnDisable()
    {
        if (addedInputManagerHandler)
        {
            InputManager.Instance.PopFallbackInputHandler();
            addedInputManagerHandler = false;
        }
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        // Nothing.
    }

    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        if (eventData.PressKind == InteractionPressKind.Select)
        {
            GameObject obj = FocusManager.Instance.TryGetFocusedObject(eventData);

            // Try and get a TTS Manager
            TextToSpeechManager tts = (obj == null)
                ? null
                : obj.GetComponent<TextToSpeechManager>();

            if (tts != null)
            {
                // If we have a text to speech manager on the target object, say something.
                // This voice will appear to emanate from the object.
                if (!tts.IsSpeaking())
                {
                    // Get the name
                    var voiceName = Enum.GetName(typeof(TextToSpeechVoice), tts.Voice);

                    // Create message
                    var msg = string.Format("This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.", voiceName);

                    // Speak message
                    tts.SpeakText(msg);
                }
                else
                {
                    tts.StopSpeaking();
                }

                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
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
