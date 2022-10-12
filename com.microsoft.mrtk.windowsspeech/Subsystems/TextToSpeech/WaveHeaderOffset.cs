// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// Offsets to key locations within a wave file's RIFF header.
    /// </summary>
    internal enum WaveHeaderOffset
    {
        /// <summary>
        /// The chunk that identifies the data as being in RIFF format.
        /// <para/>
        /// This chunk contains the letters 'R', 'I', 'F', 'F' and is
        /// four bytes in length.
        /// </summary>
        Riff = 0,

        /// <summary>
        /// The size of the wave data, inclusive of the header.
        /// <para/>
        /// This chunk is four bytes in length.
        /// </summary>
        Size = 4,

        /// <summary>
        /// The chunk that identifies the data as being wave audio.
        /// <para/>
        /// This chunk contains the letters 'W', 'A', 'V', 'E' and is
        /// four bytes in length.
        /// </summary>
        Wave = 8,

        /// <summary>
        /// The chunk that identifies the beginning of the format section.
        /// <para/>
        /// This chunk contains the letters 'f', 'm', 't', ' ' and is
        /// four bytes in length.
        /// </summary>
        Format = 12,

        /// <summary>
        /// The size, in bytes, of the sizes of the format chunks that follow.
        /// <para/>
        /// This chunk is four bytes in length and will contain the value '16'.
        /// </summary>
        FormatSize = 16,

        /// <summary>
        /// The format, for example 1 == PCM, of the audio data.
        /// <para/>
        /// This chunk is two bytes in length.
        /// </summary>
        AudioFormat = 20,

        /// <summary>
        /// The number of audio channels (ex: 2 == stereo).
        /// <para/>
        /// This chunk is two bytes in length.
        /// </summary>
        Channels = 22,

        /// <summary>
        /// The wave data sample rate (ex: 44100 Hz).
        /// <para/>
        /// This chunk is four bytes in length.
        /// </summary>
        SampleRate = 24,

        /// <summary>
        /// The wave data byte rate - (sample rate * bits per sample * channels / 8).
        /// <para/>
        /// This chunk is four bytes in length.
        /// </summary>
        ByteRate = 28,

        /// <summary>
        /// The data's block alignment - (bits per sample * channels / 8).
        /// <para/>
        /// This chunk is four bytes in length.
        /// </summary>
        BlockAlign = 32,

        /// <summary>
        /// The number of bits per wave data sample.
        /// <para/>
        /// This chunk is two bytes in length.
        /// </summary>
        BitsPerSample = 34,

        /// <summary>
        /// The size of any extra format information that will precede the data chunk.
        /// <para/>
        /// This chunk is two bytes in length.
        /// </summary>
        ExtraInfoSize = 36
    }
}
