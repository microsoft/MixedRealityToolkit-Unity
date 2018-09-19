using System.Linq;
using UnityEngine;
using Voice = ExitGames.Client.Photon.Voice;

/// <summary>
/// Component representing outgoing audio stream in scene. Should be attached to prefab with PhotonView attached.
/// </summary>
[RequireComponent(typeof(PhotonVoiceSpeaker))]
[DisallowMultipleComponent]
public class PhotonVoiceRecorder : Photon.MonoBehaviour
{
    private Voice.LocalVoice voice = Voice.LocalVoice.Dummy;
    
    private string microphoneDevice = null;

    /// <summary>
    /// Set aidio clip in instector for playing back instead of microphone signal streaming.
    /// </summary>
    public AudioClip AudioClip;

    /// <summary>
    /// Loop playback for audio clip sources.
    /// </summary>
    public bool LoopAudioClip = true;

    /// <summary>
    /// Returns voice activity detector for recorder's audio stream.
    /// </summary>
    public Voice.VoiceDetector VoiceDetector
    {
        get { return this.photonView.isMine ? this.voice.VoiceDetector : null; }
    }

    /// <summary>
    /// Set or get microphone device used for streaming.
    /// </summary>
    /// <remarks>
    /// If null, global PhotonVoiceNetwork.MicrophoneDevice is used.
    /// </remarks>    
    public string MicrophoneDevice
    {
        get { return this.microphoneDevice; }
        set
        {
            if (value != null && !Microphone.devices.Contains(value))
            {
                Debug.LogError("PUNVoice: " + value + " is not a valid microphone device");
                return;
            }

            this.microphoneDevice = value;

            // update local voice's mic audio source
            if (this.voice != Voice.LocalVoice.Dummy && AudioClip == null)
            {
                var pvs = PhotonVoiceSettings.Instance;

                Application.RequestUserAuthorization(UserAuthorization.Microphone);

                var micDev = this.MicrophoneDevice != null ? this.MicrophoneDevice : PhotonVoiceNetwork.MicrophoneDevice;
                if (PhotonVoiceSettings.Instance.DebugInfo)
                {
                    Debug.LogFormat("PUNVoice: Setting recorder's microphone device to {0}", micDev);
                }
                // first remove voice and stop mic, then create new mic wrapper
                this.voice.RemoveSelf();
                var mic = new MicWrapper(micDev, (int)pvs.SamplingRate);

                var debugEchoMode = PhotonVoiceNetwork.Client.DebugEchoMode;
                PhotonVoiceNetwork.Client.DebugEchoMode = false;

                Voice.VoiceInfo voiceInfo = new Voice.VoiceInfo((int)pvs.SamplingRate, mic.Channels, (int)pvs.FrameDuration, pvs.Bitrate, photonView.viewID);                
                var prevVoice = this.voice;
                this.voice = PhotonVoiceNetwork.CreateLocalVoice(mic, voiceInfo);
                this.voice.AudioGroup = prevVoice.AudioGroup;
                this.voice.Transmit = prevVoice.Transmit;
                this.voice.VoiceDetector.On = prevVoice.VoiceDetector.On;
                this.voice.VoiceDetector.Threshold = prevVoice.VoiceDetector.Threshold;

                PhotonVoiceNetwork.Client.DebugEchoMode = debugEchoMode;
            }
        }
    }

    /// <summary>If AudioGroup != 0, recorders's audio data is sent only to clients listening to this group.</summary>
    /// <see PhotonVoiceNetwork.Client.ChangeAudioGroups/>
    public byte AudioGroup
    {
        get { return voice.AudioGroup; }
        set { voice.AudioGroup = value; }
    }

    /// <summary>Returns true if audio stream broadcasts.</summary>
    public bool IsTransmitting
    {
        get { return voice.IsTransmitting; }
    }

    /// <summary>
    /// Level meter utility.
    /// </summary>
    public Voice.LevelMeter LevelMeter
    {
        get { return voice.LevelMeter; }
    }

    // give user a chance to change MicrophoneDevice in Awake()
    void Start()
    {
        if (Microphone.devices.Length < 1)
        {
            return;
        }
        if (photonView.isMine)
        {
            var pvs = PhotonVoiceSettings.Instance;

            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            // put required sample rate into audio source and encoder - both adjust it if needed
            Voice.IAudioStreamFloat audioStream;
            int channels = 0;
            if (AudioClip == null)
            {
                var micDev = this.MicrophoneDevice != null ? this.MicrophoneDevice : PhotonVoiceNetwork.MicrophoneDevice;
                if (PhotonVoiceSettings.Instance.DebugInfo)
                {
                    Debug.LogFormat("PUNVoice: Setting recorder's microphone device to {0}", micDev);
                }
                var mic = new MicWrapper(micDev, (int)pvs.SamplingRate);
                channels = mic.Channels;
                audioStream = mic;                
            }
            else
            {
                audioStream = new AudioClipWrapper(AudioClip);
                channels = AudioClip.channels;
                if (this.LoopAudioClip)
                {
                    ((AudioClipWrapper)audioStream).Loop = true;
                }
            }

            Voice.VoiceInfo voiceInfo = new Voice.VoiceInfo((int)pvs.SamplingRate, channels, (int)pvs.FrameDuration, pvs.Bitrate, photonView.viewID);            
            this.voice = PhotonVoiceNetwork.CreateLocalVoice(audioStream, voiceInfo);

            this.VoiceDetector.On = PhotonVoiceSettings.Instance.VoiceDetection;
            this.VoiceDetector.Threshold = PhotonVoiceSettings.Instance.VoiceDetectionThreshold;

            if (this.voice != Voice.LocalVoice.Dummy)
            {
                this.voice.Transmit = PhotonVoiceSettings.Instance.AutoTransmit;
            } 
            else if (PhotonVoiceSettings.Instance.AutoTransmit)
            {
                Debug.LogWarning("PUNVoice: Cannot Transmit.");
            }
        }
    }

    void OnDestroy()
    {
        if (this.voice != Voice.LocalVoice.Dummy) // photonView.isMine does not work
        {
            this.voice.RemoveSelf();
        }
    }

    void OnEnable()
    {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    // message sent by Voice client
    void OnJoinedVoiceRoom()
    {
    }

    /// <summary>If true, stream data broadcasted unconditionally.</summary>        
    public bool Transmit { get { return voice.Transmit; } set { voice.Transmit = value; } }

    /// <summary>If true, voice detection enabled.</summary>
    public bool Detect { get { return voice.VoiceDetector.On; } set { voice.VoiceDetector.On = value; } }

    /// <summary>Trigger voice detector calibration process.
    /// While calibrating, keep silence. Voice detector sets threshold basing on measured backgroud noise level.
    /// </summary>
    /// <param name="durationMs">Duration of calibration in milliseconds.</param>
    public void VoiceDetectorCalibrate(int durationMs)
    {
        if (photonView.isMine)
        {
            this.voice.VoiceDetectorCalibrate(durationMs);
        }
    }

    /// <summary>If true, voice detector calibration is in progress.</summary>
    public bool VoiceDetectorCalibrating { get { return voice.VoiceDetectorCalibrating; } }

    private string log0;
    private string log1;

    private string tostr<T>(T[] x, int lim = 10)
    {
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        for (var i = 0; i < (x.Length < lim ? x.Length : lim); i++)
        {
            b.Append("-");
            b.Append(x[i]);
        }
        return b.ToString();
    }

    public string ToStringFull()
    {
        int minFreq = 0;
        int maxFreq = 0;
        Microphone.GetDeviceCaps(MicrophoneDevice, out minFreq, out maxFreq);
        return string.Format("Mic '{0}': {1}..{2} Hz", MicrophoneDevice, minFreq, maxFreq);
    }

}