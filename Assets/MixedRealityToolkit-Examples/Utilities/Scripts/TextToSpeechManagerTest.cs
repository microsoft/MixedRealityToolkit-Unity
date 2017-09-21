// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class TextToSpeechManagerTest : MonoBehaviour, IInputHandler
{
    public TextToSpeechManager textToSpeechManager;

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
        if (eventData.PressType == InteractionSourcePressType.Select)
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

    public void SpeakTime()
    {
        // Say something using the text to speech manager on THIS test class (the "global" one).
        // This voice will appear to follow the user.
        textToSpeechManager.SpeakText("The time is " + DateTime.Now.ToString("t"));
    }
}
