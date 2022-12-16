// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// Helper methods used by the Windows Text-To-Speech subsystems.
    /// </summary>
    internal static class TextToSpeechHelpers
    {
        /// <summary>
        /// Attempts to convert the provided wave data from an array of bytes to an array of floats,
        /// in the range of -1 to 1.
        /// </summary>
        /// <param name="waveBytes">The audio data, including the wave header, formatted as an array of bytes.</param>
        /// <param name="samples">The number of audio samples per channel in the <see cref="waveBytes"/> array.</param>
        /// <param name="sampleRate">The sample rate (ex: 44.1 kHz) of the audio data.</param>
        /// <param name="floatData">The resulting audio data, formatted as float values from -1 to 1.</param>
        /// <returns>True if the conversion is successful, or false.</returns>
        public static bool TryConvertWaveData(
            byte[] waveBytes,
            out int samples,
            out int sampleRate,
            out int channels,
            out float[] floatData)
        {
            // Validate the array data by ensuring that key chunks contain the expected data.
            if ((waveBytes[(int)WaveHeaderOffset.Riff + 0] != 0x52) || // 'R'
                (waveBytes[(int)WaveHeaderOffset.Riff + 1] != 0x49) || // 'I'
                (waveBytes[(int)WaveHeaderOffset.Riff + 2] != 0x46) || // 'F'
                (waveBytes[(int)WaveHeaderOffset.Riff + 3] != 0x46) || // 'F'
                (waveBytes[(int)WaveHeaderOffset.Wave + 0] != 0x57) || // 'W'
                (waveBytes[(int)WaveHeaderOffset.Wave + 1] != 0x41) || // 'A'
                (waveBytes[(int)WaveHeaderOffset.Wave + 2] != 0x56) || // 'V'
                (waveBytes[(int)WaveHeaderOffset.Wave + 3] != 0x45) || // 'E'
                (waveBytes[(int)WaveHeaderOffset.Format + 0] != 0x66) || // 'f'
                (waveBytes[(int)WaveHeaderOffset.Format + 1] != 0x6D) || // 'm'
                (waveBytes[(int)WaveHeaderOffset.Format + 2] != 0x74) || // 't'
                (waveBytes[(int)WaveHeaderOffset.Format + 3] != 0x20))   // ' '
            {
                samples = 0;
                sampleRate = 0;
                channels = 0;
                floatData = null;
                Debug.LogError("Invalid wave data: malformed header.");
                return false;
            }

            // Read the channel count
            channels = MathUtilities.BytesToShort(waveBytes, (int)WaveHeaderOffset.Channels);

            // Read the sample rate
            sampleRate = MathUtilities.BytesToInt(waveBytes, (int)WaveHeaderOffset.SampleRate);

            // Determine how much, if any, extra info precedes the data chunk
            short extraInfoSize = MathUtilities.BytesToShort(waveBytes, (int)WaveHeaderOffset.ExtraInfoSize);

            // Add the extra info size to the offset in the header to find the data chunk offset
            int dataChunkOffset = (int)WaveHeaderOffset.ExtraInfoSize + 2 + extraInfoSize;

            // Confirm that we found the data chunk
            if ((waveBytes[dataChunkOffset + 0] != 0x64) || // 'd'
                (waveBytes[dataChunkOffset + 1] != 0x61) || // 'a'
                (waveBytes[dataChunkOffset + 2] != 0x74) || // 't'
                (waveBytes[dataChunkOffset + 3] != 0x61))   // 'a'
            {
                samples = 0;
                sampleRate = 0;
                channels = 0;
                floatData = null;
                Debug.LogError("Invalid wave data: could not locate the audio data.");
                return false;
            }

            // The first sample offset immediately follows the data size,
            // which immediately follows the data chunk. Both are four bites in length.
            int firstSampleOffset = dataChunkOffset + 4 + 4;

            // Determine the length of the data, in samples, by dividing the length of the wave data
            // by 2 times the channel count (ex: 16 bit stereo == number of samples / 4).
            samples = (waveBytes.Length - firstSampleOffset) / (2 * channels);

            // Allocate memory for the floating point audio data.
            floatData = new float[samples * channels];

            // Write to the float array.
            int i = 0;
            int index = firstSampleOffset;

            while (index < waveBytes.Length) 
            {
                floatData[i] = MathUtilities.BytesToFloat(waveBytes[index], waveBytes[index + 1]);
                index += 2;
                if (channels == 2)
                {
                    // The data is in stereo
                    i++;
                    floatData[i] = MathUtilities.BytesToFloat(waveBytes[index], waveBytes[index + 1]);
                    index += 2;
                }
                i++;
            }

            return true;
        }

        /// <summary>
        /// Create a Unity AudioClip object from the provided sound data.
        /// </summary>
        /// <param name="clipName">The name to give to the clip.</param>
        /// <param name="waveData">The audio data which will be contained within the clip.</param>
        /// <param name="samples">The number of audio samples per channel in the <see cref="waveData"/> array.</param>
        /// <param name="channels">The number of audio channels, typically 1 (mono) or 2 (stereo).</param>
        /// <param name="sampleRate">The frequency rate (ex: 44100 Hz) of the audio data.</param>
        /// <returns>A Unity AudioClip object containing the provided sound data.</returns>
        public static AudioClip CreateAudioClip(
            string clipName,
            float[] waveData,
            int samples,
            int channels,
            int sampleRate)
        {
            AudioClip audioClip = AudioClip.Create(
                clipName,
                samples,
                channels,
                sampleRate,
                false); // This clip does not stream data.
            audioClip.SetData(waveData, 0); // Load data at offset 0
            return audioClip;
        }
    }
}
