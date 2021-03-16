// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using System.Linq;
using System.Threading.Tasks;
#endif

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// The well-known voices that can be used by <see cref="TextToSpeech"/>.
    /// </summary>
    public enum TextToSpeechVoice
    {
        /// <summary>
        /// The default system voice.
        /// </summary>
        Default,

        /// <summary>
        /// Microsoft David Mobile
        /// </summary>
        David,

        /// <summary>
        /// Microsoft Mark Mobile
        /// </summary>
        Mark,

        /// <summary>
        /// Microsoft Zira Mobile
        /// </summary>
        Zira,
    }

    /// <summary>
    /// Enables text to speech using the Windows 10 SpeechSynthesizer class.
    /// </summary>
    /// <remarks>
    /// <para>SpeechSynthesizer generates speech as a SpeechSynthesisStream.</para>
    /// <para>This class converts that stream into a Unity AudioClip and plays the clip using 
    /// the <see cref="AudioSource"/> you supply in the inspector. This allows you to position the voice 
    /// as desired in 3D space. One recommended approach is to place the AudioSource on an empty 
    /// GameObject that is a child of Main Camera and position it approximately 0.6 units above the 
    /// camera. This orientation will sound similar to Cortana's speech in the OS.</para>
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Scripts/MRTK/SDK/TextToSpeech")]
    public class TextToSpeech : MonoBehaviour
    {
        [Tooltip("The audio source where speech will be played.")]
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// Gets or sets the audio source where speech will be played.
        /// </summary>
        public AudioSource AudioSource { get { return audioSource; } set { audioSource = value; } }

        /// <summary>
        /// Gets or sets the voice that will be used to generate speech.
        /// </summary>
        public TextToSpeechVoice Voice { get { return voice; } set { voice = value; } }

        [Tooltip("The voice that will be used to generate speech.")]
        [SerializeField]
        private TextToSpeechVoice voice;

#if WINDOWS_UWP
        private SpeechSynthesizer synthesizer;
        private VoiceInformation voiceInfo;
        private bool speechTextInQueue = false;
#endif

        /// <summary>
        /// Converts two bytes to one float in the range -1 to 1.
        /// </summary>
        /// <param name="firstByte">The first byte.</param>
        /// <param name="secondByte"> The second byte.</param>
        /// <returns>The converted float.</returns>
        private static float BytesToFloat(byte firstByte, byte secondByte)
        {
            // Convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);

            // Convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        /// <summary>
        /// Converts an array of bytes to an integer.
        /// </summary>
        /// <param name="bytes"> The byte array.</param>
        /// <param name="offset"> An offset to read from.</param>
        /// <returns>The converted int.</returns>
        private static int BytesToInt(byte[] bytes, int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ((int)bytes[offset + i]) << (i * 8);
            }
            return value;
        }

        /// <summary>
        /// Dynamically creates an <see cref="AudioClip"/> that represents raw Unity audio data.
        /// </summary>
        /// <param name="name"> The name of the dynamically generated clip.</param>
        /// <param name="audioData">Raw Unity audio data.</param>
        /// <param name="sampleCount">The number of samples in the audio data.</param>
        /// <param name="frequency">The frequency of the audio data.</param>
        /// <returns>The <see cref="AudioClip"/>.</returns>
        private static AudioClip ToClip(string name, float[] audioData, int sampleCount, int frequency)
        {
            var clip = AudioClip.Create(name, sampleCount, 1, frequency, false);
            clip.SetData(audioData, 0);
            return clip;
        }

        /// <summary>
        /// Converts raw WAV data into Unity formatted audio data.
        /// </summary>
        /// <param name="wavAudio">The raw WAV data.</param>
        /// <param name="sampleCount">The number of samples in the audio data.</param>
        /// <param name="frequency">The frequency of the audio data.</param>
        /// <returns>The Unity formatted audio data. </returns>
        private static float[] ToUnityAudio(byte[] wavAudio, out int sampleCount, out int frequency)
        {
            // Determine if mono or stereo
            int channelCount = wavAudio[22];  // Speech audio data is always mono but read actual header value for processing

            // Get the frequency
            frequency = BytesToInt(wavAudio, 24);

            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12; // First subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wavAudio[pos] == 100 && wavAudio[pos + 1] == 97 && wavAudio[pos + 2] == 116 && wavAudio[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wavAudio[pos] + wavAudio[pos + 1] * 256 + wavAudio[pos + 2] * 65536 + wavAudio[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            sampleCount = (wavAudio.Length - pos) / 2;  // 2 bytes per sample (16 bit sound mono)
            if (channelCount == 2) { sampleCount /= 2; }  // 4 bytes per sample (16 bit stereo)

            // Allocate memory (supporting left channel only)
            var unityData = new float[sampleCount];

            // Write to double array/s:
            int i = 0;
            while (pos < wavAudio.Length)
            {
                unityData[i] = BytesToFloat(wavAudio[pos], wavAudio[pos + 1]);
                pos += 2;
                if (channelCount == 2)
                {
                    pos += 2;
                }
                i++;
            }

            return unityData;
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
                    speechTextInQueue = true;
                    // Need await, so most of this will be run as a new Task in its own thread.
                    // This is good since it frees up Unity to keep running anyway.
                    Task.Run(async () =>
                    {
                        // Change voice?
                        if (voice != TextToSpeechVoice.Default)
                        {
                            // Get name
                            var voiceName = Enum.GetName(typeof(TextToSpeechVoice), voice);

                            // See if it's never been found or is changing
                            if ((voiceInfo == null) || (!voiceInfo.DisplayName.Contains(voiceName)))
                            {
                                // Search for voice info
                                voiceInfo = SpeechSynthesizer.AllVoices.Where(v => v.DisplayName.Contains(voiceName)).FirstOrDefault();

                                // If found, select
                                if (voiceInfo != null)
                                {
                                    synthesizer.Voice = voiceInfo;
                                }
                                else
                                {
                                    Debug.LogErrorFormat("TTS voice {0} could not be found.", voiceName);
                                }
                            }
                        }

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

                        // Convert raw WAV data into Unity audio data
                        int sampleCount = 0;
                        int frequency = 0;
                        var unityData = ToUnityAudio(buffer, out sampleCount, out frequency);

                        // The remainder must be done back on Unity's main thread
                        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                        {
                            // Convert to an audio clip
                            var clip = ToClip("Speech", unityData, sampleCount, frequency);

                            // Set the source on the audio clip
                            audioSource.clip = clip;

                            // Play audio
                            audioSource.Play();
                            speechTextInQueue = false;
                        }, false);
                    });
                }
                catch (Exception ex)
                {
                    speechTextInQueue = false;
                    Debug.LogErrorFormat("Speech generation problem: \"{0}\"", ex.Message);
                }
            }
            else
            {
                Debug.LogErrorFormat("Speech not initialized. \"{0}\"", text);
            }
        }
#endif

        private void Awake()
        {
            try
            {
                if (audioSource == null)
                {
                    audioSource = GetComponent<AudioSource>();
                }
#if WINDOWS_UWP
                synthesizer = new SpeechSynthesizer();
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not start Speech Synthesis: " + ex.Message);
            }
        }

        // Public Methods

        /// <summary>
        /// Speaks the specified SSML markup using text-to-speech.
        /// </summary>
        /// <param name="ssml">The SSML markup to speak.</param>
        public void SpeakSsml(string ssml)
        {
            // Make sure there's something to speak
            if (string.IsNullOrEmpty(ssml)) { return; }

            // Pass to helper method
#if WINDOWS_UWP
            PlaySpeech(ssml, () => synthesizer.SynthesizeSsmlToStreamAsync(ssml));
#else
            Debug.LogWarningFormat("Text to Speech not supported in editor.\n\"{0}\"", ssml);
#endif
        }

        /// <summary>
        /// Speaks the specified text using text-to-speech.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        public void StartSpeaking(string text)
        {
            // Make sure there's something to speak
            if (string.IsNullOrEmpty(text)) { return; }

            // Pass to helper method
#if WINDOWS_UWP
            PlaySpeech(text, ()=> synthesizer.SynthesizeTextToStreamAsync(text));
#else
            Debug.LogWarningFormat("Text to Speech not supported in editor.\n\"{0}\"", text);
#endif
        }

        /// <summary>
        /// Returns info whether a text is submitted and being processed by PlaySpeech method
        /// Handy for avoiding situations when a text is submitted, but audio clip is not yet ready because the audio source isn't playing yet.
        /// Example: yield return new WaitWhile(() => textToSpeechManager.SpeechTextInQueue() || textToSpeechManager.IsSpeaking())
        /// </summary>
        public bool SpeechTextInQueue()
        {
#if WINDOWS_UWP
            return speechTextInQueue;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns whether or not the AudioSource is actively playing.
        /// </summary>
        /// <returns>
        /// True, if the AudioSource is playing. False, if the AudioSource is not playing or is null.
        /// </returns>
        public bool IsSpeaking()
        {
            if (audioSource != null)
            {
                return audioSource.isPlaying;
            }

            return false;
        }

        /// <summary>
        /// Stops text-to-speech playback.
        /// </summary>
        public void StopSpeaking()
        {
            if (IsSpeaking())
            {
                audioSource.Stop();
            }
        }
    }
}
