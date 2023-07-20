// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using Speech.Windows;

    /// <summary>
    /// Demonstration script showing how to subscribe to and handle
    /// events fired by a <see cref="DictationSubsystem"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("MRTK/Examples/TextToSpeech Handler")]
    public class TextToSpeechHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The audio source where speech will be played.")] 
        private AudioSource audioSource;

        /// <summary>
        /// Gets or sets the audio source where speech will be played.
        /// </summary>
        public AudioSource AudioSource
        {
            get { return audioSource; }
            set { audioSource = value; }
        }

        /// <summary>
        /// Gets or sets the voice that will be used to generate speech. To use a non en-US voice, set this to Other.
        /// </summary>
        public TextToSpeechVoice Voice 
        { 
            get { return voice; } 
            set { voice = value; } 
        }

        [Tooltip("The voice that will be used to generate speech. To use a non en-US voice, set this to Other.")]
        [SerializeField]
        private TextToSpeechVoice voice;

        /// <summary>
        /// Wrapper of UnityEvent&lt;string&gt; for serialization.
        /// </summary>
        [System.Serializable]
        public class StringUnityEvent : UnityEvent<string>
        {
        }

        /// <summary>
        /// Event raised when an error occurs. Contains the string representation of the error reason.
        /// </summary>
        [field: SerializeField]
        public StringUnityEvent OnSpeakFaulted { get; private set; }

        private TextToSpeechSubsystem textToSpeechSubsystem;

        /// <summary>
        /// Method to demonstrate the text to speech capability
        /// </summary>
        public void Speak()
        {
            // If we have a text to speech manager on the target object, say something.
            // This voice will appear to emanate from the object.
            textToSpeechSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<TextToSpeechSubsystem>();
            if (textToSpeechSubsystem != null)
            {
                // profile associated with the Standalone build target group.
                MRTKProfile profile = MRTKProfile.Instance;

                if (profile == null)
                {
                    Debug.LogError("MRTK Profile could not be retrieved.");
                    return;
                }

                BaseSubsystemConfig config;

                // Attempt to retrieve the config associated with the indicated subsystem.
                if (!profile.TryGetConfigForSubsystem(typeof(WindowsTextToSpeechSubsystem), out config) || config == null)
                {
                    Debug.LogError($"Configuration could not be retrieved for {typeof(WindowsTextToSpeechSubsystem)}.", profile);
                }
                else
                {
                    WindowsTextToSpeechSubsystemConfig winConfig = config as WindowsTextToSpeechSubsystemConfig;
                    if (winConfig != null)
                    {
                        winConfig.Voice = voice;

                        // Create message
                        var msg = string.Format(
                            "This is the {0} voice. It should sound like it's coming from the object you clicked. Feel free to walk around and listen from different angles.",
                            winConfig.Voice.ToString());

                        // Speak message
                        textToSpeechSubsystem.TrySpeak(msg, audioSource);
                    }
                }
            }
            else
            {
                Debug.LogError("Cannot find a running TextToSpeechSubsystem. Please check the MRTK profile settings " +
                               "(Project Settings -> MRTK3) and/or ensure a TextToSpeechSubsystem is running.");
                OnSpeakFaulted?.Invoke("Cannot find a running TextToSpeechSubsystem.");
            }
        }
    }
}
