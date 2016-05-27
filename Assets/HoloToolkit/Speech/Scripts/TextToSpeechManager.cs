// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Foundation;
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
    /// camera. This orientation will sound similar to Cortana's speech in the OS.
    /// </remarks>
    public class TextToSpeechManager : MonoBehaviour
    {
        // Inspector Variables
        [Tooltip("The audio source where speech will be played.")]
        public AudioSource audioSource;

        // Member Variables
        #if WINDOWS_UWP
        private SpeechSynthesizer synthesizer;
        #endif

        // Internal Methods

        /// <summary>
        /// Logs speech text that normally would have been played.
        /// </summary>
        /// <param name="text">
        /// The speech text.
        /// </param>
        private void LogSpeech(string text)
        {
            Debug.LogFormat("Speech not supported in editor. \"{0}\"", text);
        }

        #if WINDOWS_UWP
        /// <summary>
        /// Executes a function that generates a speech stream and then converts and plays it in Unity.
        /// </summary>
        /// <param name="text">
        /// A raw text version of what's being spoken for use in debug messages when speech isn't supported.
        /// </param>
        /// <param name="speakFunc">
        /// The actual function that will be executed to generate speech.
        /// </param>
        private void PlaySpeech(string text, Func<IAsyncOperation<SpeechSynthesisStream>> speakFunc)
        {
            // Make sure there's something to speak
            if (speakFunc == null) throw new ArgumentNullException(nameof(speakFunc));

            if (synthesizer != null)
            {
                try
                {
                    // Need await, so most of this will be run as a new Task in its own thread.
                    // This is good since it frees up Unity to keep running anyway.
                    Task.Run(async () =>
                    {
                        // Speak and get stream
                        var speechStream = await speakFunc();

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
        }
        #endif

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

        // MonoBehaviour Methods
        void Start()
        {
            // Start speech
            StartSpeech();
        }

        // Public Methods

        /// <summary>
        /// Speaks the specified SSML markup using text-to-speech.
        /// </summary>
        /// <param name="ssml">
        /// The SSML markup to speak.
        /// </param>
        public void SpeakSsml(string ssml)
        {
            // Make sure there's something to speak
            if (string.IsNullOrEmpty(ssml)) { return; }

            // Pass to helper method
            #if WINDOWS_UWP
            PlaySpeech(ssml, () => synthesizer.SynthesizeSsmlToStreamAsync(ssml));
            #else
            LogSpeech(ssml);
            #endif
        }

        /// <summary>
        /// Speaks the specified text using text-to-speech.
        /// </summary>
        /// <param name="text">
        /// The text to speak.
        /// </param>
        public void SpeakText(string text)
        {
            // Make sure there's something to speak
            if (string.IsNullOrEmpty(text)) { return; }

            // Pass to helper method
            #if WINDOWS_UWP
            PlaySpeech(text, ()=> synthesizer.SynthesizeTextToStreamAsync(text));
            #else
            LogSpeech(text);
            #endif
        }
    }
}