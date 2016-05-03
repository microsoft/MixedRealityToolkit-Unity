// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Enables text to speech using the Windows 10 <see cref="SpeechSynthesizer"/> class.
    /// </summary>
    /// <remarks>
    /// <see cref="SpeechSynthesizer"/> generates speech as a <see cref="SpeechSynthesisStream"/>. 
    /// This class converts that stream into a Unity <see cref="AudioClip"/> and plays the clip using 
    /// the <see cref="AudioSource"/> you supply in the inspector. This allows you to position the voice 
    /// as desired in 3D space. One recommended approach is to place the AudioSource on an empty 
    /// GameObject that is a child of Main Camera and position it approximately 0.6 units above the 
    /// camera. This orientation will sound similar to Cortanas speech in the OS.
    /// </remarks>
    public class TextToSpeechManager : Singleton<TextToSpeechManager>
    {
        #region Region Name
        public AudioSource audioSource;
        #endregion // Region Name

        #region Member Variables
        #if WINDOWS_UWP
        private SpeechSynthesizer synthesizer;
        #endif
        #endregion // Member Variables

        #region Internal Methods
        private void StartSpeech()
        {
            try
            {
                if (audioSource == null) { throw new InvalidOperationException("An AudioSource is required and should be assigned to 'Audio Source' in the inspector."); }
                #if WINDOWS_UWP
                synthesizer = new SpeechSynthesizer();
                #endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not start Speech Synthesis");
                Debug.LogException(ex);
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        // Use this for initialization
        void Start()
        {
            // Start speech
            StartSpeech();
        }
        #endregion // Overrides / Event Handlers


        #region Public Methods
        /// <summary>
        /// Speaks the specified text using text-to-speech.
        /// </summary>
        /// <param name="text">
        /// The text to speak.
        /// </param>
        public void Speak(string text)
        {
            // Make sure there's something to speak
            if (string.IsNullOrEmpty(text)) { return; }

            #if UNITY_UWP
            if (synthesizer != null)
            {
                try
                {
                    // Need await, so most of this will be run as a new Task in its own thread.
                    // This is good since it frees up Unity to keep running anyway.
                    Task.Run(async () =>
                    {
                        // Speak text to stream
                        var speechStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                        // Get the size of the original stream
                        var size = speechStream.Size;

                        // Create buffer
                        byte[] buffer = new byte[(int)size];

                        // Get input stream and the size of the original stream
                        using (var inputStream = speechStream.GetInputStreamAt(0))
                        {
                            // Close the original speech stream to free up memory
                            speechStream.Dispose();

                            // Create a new data reader off the input stream
                            using (var dataReader = new DataReader(inputStream))
                            {
                                // Load all bytes into the reader
                                await dataReader.LoadAsync((uint)size);

                                // Copy from reader into buffer
                                dataReader.ReadBytes(buffer);
                            }
                        }

                        // Load buffer as a WAV file
                        var wav = new Wav(buffer);

                        // The remainder must be done back on Unity's main thread
                        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                            {
                            // Convert to an audio clip
                            var clip = wav.ToClip("Speech");

                            // Set the source on the audio clip
                            audioSource.clip = clip;

                            // Play audio
                            audioSource.Play();
                            }, false);
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Speech generation problem: \"{0}\"", ex.Message);
                }
            }
            else
            {
                Debug.LogErrorFormat("Speech not initialized. \"{0}\"", text);
            }

        #else

        Debug.LogFormat("Speech not supported in editor. \"{0}\"", text);
            
        #endif
        }
        #endregion // Public Methods
    }
}