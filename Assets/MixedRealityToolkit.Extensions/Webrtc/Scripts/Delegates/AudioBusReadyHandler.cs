using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Delegates
{
    /// <summary>
    /// Audiobus callback
    /// </summary>
    /// <param name="data">audio data</param>
    /// <param name="bitsPerSample">bits per sample</param>
    /// <param name="sampleRate">sample rate</param>
    /// <param name="numberOfChannels">number of channels</param>
    /// <param name="numberOfFrames">number of frames</param>
    public delegate void AudioBusReadyHandler(IntPtr data,
            int bitsPerSample,
            int sampleRate,
            int numberOfChannels,
            int numberOfFrames);
}
