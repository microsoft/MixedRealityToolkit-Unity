using UnityEngine;

/// <summary>
/// Component representing remote audio stream in local scene. Automatically attached to the PUN object which owner's instance has streaming Recorder attached.
/// </summary>
[RequireComponent(typeof (AudioSource))]
[DisallowMultipleComponent]
public class PhotonVoiceSpeaker : Photon.MonoBehaviour
{
    private AudioStreamPlayer player;

    /// <summary>Time when last audio packet was received for the speaker.</summary>
    public float LastRecvTime { get; private set; }

    /// <summary>Is the speaker playing right now.</summary>
    public bool IsPlaying { get { return this.player.IsPlaying; } }

    /// <summary>Smoothed difference between (jittering) stream and (clock-driven) player.</summary>
    public int CurrentBufferLag { get { return this.player.CurrentBufferLag; } }

    /// <summary>Is the speaker linked to the remote voice (info available and streaming is possible).</summary>
    public bool IsVoiceLinked { get { return this.player != null && this.player.IsStarted; } }

    void Awake()
    {
        this.player = new AudioStreamPlayer(GetComponent<AudioSource>(), "PUNVoice: PhotonVoiceSpeaker:", PhotonVoiceSettings.Instance.DebugInfo);
        PhotonVoiceNetwork.LinkSpeakerToRemoteVoice(this);
    }

    // initializes the speaker with remote voice info
    internal void OnVoiceLinked(int frequency, int channels, int frameSamplesPerChannel, int playDelayMs)
    {
        this.player.Start(frequency, channels, frameSamplesPerChannel, playDelayMs);
    }

    internal void OnVoiceUnlinked()
    {
        this.player.Stop();
    }

    void Update()
    {
        this.player.Update();
    }

    void OnDestroy()
    {
        PhotonVoiceNetwork.UnlinkSpeakerFromRemoteVoice(this);
        this.player.Stop();
    }

    void OnApplicationQuit()
    {
        this.player.Stop();
    }

    internal void OnAudioFrame(float[] frame)
    {
        // Set last time we got something
        this.LastRecvTime = Time.time;

        this.player.OnAudioFrame(frame);
    }
}
