//
// Copyright (C) Microsoft. All rights reserved.
//

public class AudioConstants 
{
#if !(UNITY_STANDALONE_WIN || (UNITY_EDITOR && UNITY_STANDALONE_WIN) || (UNITY_EDITOR && UNITY_METRO))
    /// <summary>
    /// the base frequency for both recording and playback
    /// </summary>
    public const int BaseFrequency = 48000;
#else
    /// <summary>
    /// the base frequency for both recording and playback
    /// </summary>
    public const int BaseFrequency = 48000;
#endif

    /// <summary>
    /// AudioSink.halfSecond data points is 1/2 of a second in samples
    /// </summary>
    public const int HalfSecond = BaseFrequency / 2;

    /// <summary>
    /// AudioSink.halfSecond data points is 1/4 of a second in samples and 
    /// must be a multiple of 4!
    /// </summary>
    public const int QuarterSecond = (BaseFrequency / 16) * 4;

    /// <summary>
    /// number of audio channels before HRTF
    /// </summary>
    public const int AudioChannelsPreHRTF = 1;

    /// <summary>
    /// number of audio channels after HRTF
    /// </summary>
    public const int AudioChannelsAfterHRTF = 2;

    /// <summary>
    /// don't start to deliver any audio until this time has passed so we are sure to 
    /// have some data in the buffer.  When we do begin to send data, we ignore old audio data 
    /// </summary>
    public const int DeliveryLatency = BaseFrequency / 30;

    /// <summary>
    /// no audio buffer size (1/20th of a second)
    /// </summary>
    public const int NullAudioBufferSize = BaseFrequency / 20;

    /// <summary>
    /// target audio buffer latency in milliseconds - not guaranteed
    /// </summary>
    public const int AudioBufferLatency = 10;

    /// <summary>
    /// This is a standard packet size, we try to sync the incoming data and current playback positions to 
    /// within this amount of time
    /// </summary>
    public const int StandardPacketSize = 960; // 20ms @ 48KHz

    /// <summary>
    /// Maximum number of prominent speakers we should ever encounter
    /// </summary>
    public const int MaximumProminentSpeakers = 4;
}
