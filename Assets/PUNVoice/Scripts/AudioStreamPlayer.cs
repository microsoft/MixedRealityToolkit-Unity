using UnityEngine;
using System.Collections;

public class AudioStreamPlayer
{
    // fast forward if we are more than this value before desired position (stream pos - playDelaySamples)
    const int maxPlayLagMs = 100;
    private int maxPlayLagSamples;

    // buffering by playing few samples back
    private int playDelaySamples;

    private int frameSize = 0;
    private int frameSamples = 0;
    private int streamSamplePos = 0;

    /// <summary>Smoothed difference between (jittering) stream and (clock-driven) player.</summary>
    public int CurrentBufferLag { get; private set; }

    /// <summary>Returns AudioSource object passed by user in constructor.</summary>
    public AudioSource AudioSource { get { return this.source; } }

    // jitter-free stream position
    private int streamSamplePosAvg;

    private AudioSource source;
    private string logPrefix;
    private bool debugInfo;

    public AudioStreamPlayer(AudioSource audioSource, string logPrefix, bool debugInfo)
    {
        this.source = audioSource;
        this.logPrefix = logPrefix;
        this.debugInfo = debugInfo;
    }

    // non-wrapped play position
    private int playSamplePos
    {
        get { return this.source.clip != null ? this.playLoopCount * this.source.clip.samples + this.source.timeSamples : 0; }
        set
        {
            if (this.source.clip != null)
            {
                // if negative value is passed (possible when playback starts?), loop count is < 0 and sample position is positive
                var pos = value % this.source.clip.samples;
                if (pos < 0)
                {
                    pos += this.source.clip.samples;
                }
                this.source.timeSamples = pos;
                this.playLoopCount = value / this.source.clip.samples;
                this.sourceTimeSamplesPrev = this.source.timeSamples;
            }

        }
    }
    private int sourceTimeSamplesPrev = 0;
    private int playLoopCount = 0;

    public bool IsPlaying
    {
        get { return this.source.isPlaying; }
    }

    public bool IsStarted
    {
        get { return this.source.clip != null; }
    }

    internal void Start(int frequency, int channels, int frameSamples, int playDelayMs)
    {

        int bufferSamples = (maxPlayLagMs + playDelayMs) * frequency / 1000 + frameSamples + frequency; // frame + max delay + 1 sec. just in case
        
        this.frameSamples = frameSamples;
        this.frameSize = frameSamples * channels;

        // add 1 frame samples to make sure that we have something to play when delay set to 0
        this.maxPlayLagSamples = maxPlayLagMs * frequency / 1000 + this.frameSamples;
        this.playDelaySamples = playDelayMs * frequency / 1000 + this.frameSamples;

        // init with target value
        this.CurrentBufferLag = this.playDelaySamples;
        this.streamSamplePosAvg = this.playDelaySamples;

        this.source.loop = true;
        // using streaming clip leads to too long delays
        this.source.clip = AudioClip.Create("AudioStreamPlayer", bufferSamples, channels, frequency, false);

        this.streamSamplePos = 0;
        this.playSamplePos = 0;

        this.source.Play();
        this.source.Pause();
    }

    public void Update()
    {
        if (this.source.clip != null)
        {
            // loop detection (pcmsetpositioncallback not called when clip loops)
            if (this.source.isPlaying)
            {
                if (this.source.timeSamples < sourceTimeSamplesPrev)
                {
                    playLoopCount++;
                }
                sourceTimeSamplesPrev = this.source.timeSamples;
            }

            var playPos = this.playSamplePos; // cache calculated value

            // average jittering value
            this.CurrentBufferLag = (this.CurrentBufferLag * 39 + (this.streamSamplePos - playPos)) / 40;

            // calc jitter-free stream position based on clock-driven palyer position and average lag
            this.streamSamplePosAvg = playPos + this.CurrentBufferLag;
            if (this.streamSamplePosAvg > this.streamSamplePos)
            {
                this.streamSamplePosAvg = this.streamSamplePos;
            }

            // start with given delay or when stream position is ok after overrun pause
            if (playPos < this.streamSamplePos - this.playDelaySamples)
            {
                if (!this.source.isPlaying)
                {
                    this.source.UnPause();
                }
            }

            if (playPos > this.streamSamplePos - frameSamples)
            {
                if (this.source.isPlaying)
                {
                    if (this.debugInfo)
                    {
                        Debug.LogWarningFormat("{0} player overrun: {1}/{2}({3}) = {4}", this.logPrefix, playPos, streamSamplePos, streamSamplePosAvg, streamSamplePos - playPos);
                    }

                    // when nothing to play:
                    // pause player  (useful in case if stream is stopped for good) ...
                    this.source.Pause();

                    // ... and rewind to proper position 
                    playPos = this.streamSamplePos;
                    this.playSamplePos = playPos;
                    this.CurrentBufferLag = this.playDelaySamples;
                }
            }
            if (this.source.isPlaying)
            {
                var lowerBound = this.streamSamplePos - this.playDelaySamples - maxPlayLagSamples;
                if (playPos < lowerBound)
                {
                    if (this.debugInfo)
                    {
                        Debug.LogWarningFormat("{0} player underrun: {1}/{2}({3}) = {4}", this.logPrefix, playPos, streamSamplePos, streamSamplePosAvg, streamSamplePos - playPos);
                    }

                    // if lag exceeds max allowable, fast forward to proper position                    
                    playPos = this.streamSamplePos - this.playDelaySamples;
                    this.playSamplePos = playPos;
                    this.CurrentBufferLag = this.playDelaySamples;
                }
            }

        }

    }

    internal void OnAudioFrame(float[] frame)
    {
        if (frame.Length == 0)
        {
            return;
        }
        if (frame.Length != frameSize)
        {
            Debug.LogErrorFormat("{0} Audio frames are not of  size: {1} != {2}", this.logPrefix, frame.Length, frameSize);
            //Debug.LogErrorFormat("{0} {1} {2} {3} {4} {5} {6}", frame[0], frame[1], frame[2], frame[3], frame[4], frame[5], frame[6]);
            return;
        }

        // Store last packet

        this.source.clip.SetData(frame, this.streamSamplePos % this.source.clip.samples);
        this.streamSamplePos += frame.Length / this.source.clip.channels;
    }

    public void Stop()
    {
        this.source.Stop();
        this.source.clip = null;
    }

}