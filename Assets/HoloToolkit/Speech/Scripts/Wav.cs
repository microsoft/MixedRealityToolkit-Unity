// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Supports dynamic loading of WAV data in memory.
    /// </summary>
    /// <remarks>
    /// This class is partially based on the excellent 
    /// <see href="http://answers.unity3d.com/questions/737002/wav-byte-to-audioclip.html">sample by Jeff Kesselman</see> 
    /// on the Unity forums. 
    /// </remarks>
    public class Wav
    {
        #region Static Version
        // Convert two bytes to one float in the range -1 to 1
        static private float bytesToFloat(byte firstByte, byte secondByte)
        {
            // Convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);

            // Convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }

        static private int bytesToInt(byte[] bytes, int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ((int)bytes[offset + i]) << (i * 8);
            }
            return value;
        }
        #endregion // Static Version

        #region Instance Version
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="Wav"/> instance.
        /// </summary>
        /// <param name="wav">
        /// The raw WAV byte data.
        /// </param>
        public Wav(byte[] wav)
        {
            // Determine if mono or stereo
            ChannelCount = wav[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels

            // Get the frequency
            Frequency = bytesToInt(wav, 24);

            // Get past all the other sub chunks to get to the data subchunk:
            int pos = 12;   // First subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.
            SampleCount = (wav.Length - pos) / 2;     // 2 bytes per sample (16 bit sound mono)
            if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)

            // Allocate memory (right will be null if only mono sound)
            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2) RightChannel = new float[SampleCount];
            else RightChannel = null;

            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length)
            {
                LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }
        }
        #endregion // Constructors

        #region Overrides / Event Handlers
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
        }
        #endregion // Overrides / Event Handlers

        #region Public Methods
        /// <summary>
        /// Dynamically creates an <see cref="AudioClip"/> that represents the WAV file.
        /// </summary>
        /// <param name="name">
        /// The name of the dynamically generated clip.
        /// </param>
        /// <returns>
        /// The <see cref="AudioClip"/>.
        /// </returns>
        public AudioClip ToClip(string name)
        {
            // Create the audio clip
            var clip = AudioClip.Create(name, SampleCount, 1, Frequency, false); // TODO: Support stereo

            // Set the data
            clip.SetData(LeftChannel, 0);

            // Done
            return clip;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the number of audio channels.
        /// </summary>
        public int ChannelCount { get; internal set; }

        /// <summary>
        /// Gets the frequency of the audio data.
        /// </summary>
        public int Frequency { get; internal set; }

        /// <summary>
        /// Gets the left channel audio data.
        /// </summary>
        public float[] LeftChannel { get; internal set; }

        /// <summary>
        /// Gets the right channel audio data.
        /// </summary>
        public float[] RightChannel { get; internal set; }

        /// <summary>
        /// Gets the number of samples.
        /// </summary>
        public int SampleCount { get; internal set; }
        #endregion // Public Properties
        #endregion // Instance Version
    }
}