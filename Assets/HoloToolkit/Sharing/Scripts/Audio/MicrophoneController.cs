//
// Copyright (C) Microsoft. All rights reserved.
//

using System;
using UnityEngine;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Utilities;
using System.Collections;

#if UNITY_METRO && !UNITY_EDITOR
using Windows.Media.Devices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using AudioIO_DLL;
#endif

/// <summary>
/// There is only 1 microphone input.  This interfaces with the AudioIO assembly.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MicrophoneController : MonoBehaviour
{
    public static MicrophoneController Instance = null;

    public float DefaultMicBoostAmount = 16.0f;

    /// <summary>
    /// provide debug spew
    /// </summary>
    public bool DebugSpewOn = false;

    /// <summary>
    /// provide debug spew
    /// </summary>
    public bool DebugSpewVolumeOn = false;

    /// <summary>
    /// Increase the microphone volume to account for old hardware.
    /// </summary>
    public bool MicBoost = false;
    public float MicBoostAmount { get; set; }

    /// <summary>
    /// Show the device name
    /// </summary>
    public bool ShowDeviceName = false;

    public bool ShowInterPacketTime = false;

    public Transform GlobalAnchorTransform;		   // In XTools, this is typically set to TerrainSite.Instance.transform

    /// <summary>
    /// Easy to turn off audio by setting this to false...
    /// </summary>
    public bool TransmitData = false;

    private System.DateTime timeOfLastPacketSend;
    private float worstTimeBetweenPackets = 0.0f;

    private object lockObject;
    private AudioIO_DLL.Utils.CircularBuffer circularBuffer = new AudioIO_DLL.Utils.CircularBuffer(AudioConstants.QuarterSecond);

    private AudioSource audioSource;
    private int minFreq;
    private int maxFreq;
    private int deviceFrequency;

    // the floats are internal, Unity gives us floats
    private float[] dataStreamFloats;
#if !UNITY_METRO || UNITY_EDITOR
    private int sampleRateType = 3;	// 48000Hz
    private float[] dataChunked = new float[AudioConstants.StandardPacketSize];	// use this to break down large audio samples
#else
    private int sampleRateType = 1; // 16000Hz
    private float[] dataChunked = new float[AudioConstants.StandardPacketSize / 3];	// 3 = 48000 / 16000
#endif

    // the bytes are for sending over the network only!
    private byte[] dataStreamSendBytes;
    private float lastVolume = 0;

    // network connection information
    private NetworkConnection connection;
    private NetworkConnectionAdapter listener;
    private bool connectionVerified = false;

    private const byte microphonePlaybackMessageID = (byte)MessageID.StatusOnly;

    // bit packers
    private BitManipulator versionPacker = new BitManipulator(0x7, 0);			 // 3 bits, 0 shift
    private BitManipulator audioStreamCountPacker = new BitManipulator(0x38, 3); // 3 bits, 3 shift
    private BitManipulator channelCountPacker = new BitManipulator(0x1c0, 6);	 // 3 bits, 6 shift
    private BitManipulator sampleRatePacker = new BitManipulator(0x600, 9);		 // 2 bits, 9 shift
    private BitManipulator sampleTypePacker = new BitManipulator(0x1800, 11);	 // 2 bits, 11 shift
    private BitManipulator sampleCountPacker = new BitManipulator(0x7fe000, 13); // 10 bits, 13 shift
    private BitManipulator codecTypePacker = new BitManipulator(0x1800000, 23);	 // 2 bits, 23 shift
    private BitManipulator mutePacker = new BitManipulator((int)0x2000000, 25);	 // 1 bits, 25 shift
    private BitManipulator sequenceNumberPacker = new BitManipulator((int)0x7C000000, 26);	// 6 bits, 26 shift

    private int fixedUpdateCounter;
    private bool muteMicrophone = false;

    private int clientId;


    public void MuteMicrophone()
    {
        this.muteMicrophone = true;
    }

    public void UnmuteMicrophone()
    {
        this.muteMicrophone = false;
    }

    public float GetVolume()
    {
        return this.lastVolume;
    }

    private void Awake()
    {
        MicrophoneController.Instance = this;
        this.lockObject = new object();
        this.MicBoostAmount = this.DefaultMicBoostAmount;
    }

    private IEnumerator Start()
    {
        // get permission to use the microphone 
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            if (this.DebugSpewOn)
            {
                UnityEngine.Debug.Log("microphone auth obtained");
            }
            this.InitMic();
        }
        else
        {
            if (this.DebugSpewOn)
            {
                UnityEngine.Debug.Log("microphone not auth");
            }
        }

        // get the network connection information
        this.connection = SharingStage.Instance.Manager.GetServerConnection();

        this.listener = new NetworkConnectionAdapter();
        this.listener.ConnectedCallback += this.OnConnected;
        this.listener.DisconnectedCallback += this.OnDisconnected;
        this.listener.ConnectionFailedCallback += this.OnConnectedFailed;

        this.connection.AddListener(MicrophoneController.microphonePlaybackMessageID, this.listener);

        internalStart();
    }

    protected void OnDestroy()
    {
        internalDestroy();

        this.connection.RemoveListener(MicrophoneController.microphonePlaybackMessageID, this.listener);

        this.listener.Dispose();
        this.listener = null;

        Instance = null;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                this.internalStartMicrophone();
                UnityEngine.Debug.Log("Audio ConnectToSession called");
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                this.internalStopMicrophone();
                UnityEngine.Debug.Log("Audio DisconnectFromSession called");
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                UnityEngine.Debug.Log("Current Volume = " + this.GetVolume());
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                this.MicBoostAmount *= 2.0f;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                this.MicBoost = !this.MicBoost;
                this.MicBoostAmount = DefaultMicBoostAmount;
            }
        }

        internalUpdate();
    }

    private void FixedUpdate()
    {
        internalFixedUpdate();
    }

    private void InitMic()
    {
        internalInitMic();
    }

    private void OnConnected(NetworkConnection connection)
    {
        Profile.BeginRange("MicrophoneController.OnConnected");

        this.circularBuffer.Reset();
        this.connectionVerified = true;
        this.internalStartMicrophone();
        if (this.DebugSpewOn)
        {
            UnityEngine.Debug.Log("MicrophoneController: Connection to session server succeeded!");
        }

        // get the unique client id of this user for outgoing packets
        User localUser = SharingStage.Instance.Manager.GetLocalUser();
        this.clientId = localUser.GetID();
        Profile.EndRange();
    }

    private void OnDisconnected(NetworkConnection connection)
    {
        this.connectionVerified = false;
        this.internalStopMicrophone();
        if (this.DebugSpewOn)
        {
            UnityEngine.Debug.Log("MicrophoneController: Session server disconnected!");
        }
    }

    private void OnConnectedFailed(NetworkConnection connection)
    {
        this.connectionVerified = false;
        this.internalStopMicrophone();
        if (this.DebugSpewOn)
        {
            UnityEngine.Debug.Log("MicrophoneController: Connection to session server failed!");
        }
    }

    /// <summary>
    /// The microphone input bytes are sent from here.
    /// </summary>
    /// <param name="dataCountFloats"></param>
    /// <param name="dataFloats"></param>
    private void Send()
    {
        if (this.connectionVerified && this.listener != null && this.connection != null && this.TransmitData)
        {
            while (this.circularBuffer.FloatCount >= this.dataChunked.Length)
            {
                this.circularBuffer.ReadFloats(this.dataChunked, 0, this.dataChunked.Length);

                if (this.DebugSpewVolumeOn)
                {
                    float averageAmplitude = 0.0f;
                    for (int sample = 0; sample < this.dataChunked.Length; sample++)
                    {
                        averageAmplitude += Math.Abs(this.dataChunked[sample]);
                    }
                    this.lastVolume = averageAmplitude / this.dataChunked.Length;

                    UnityEngine.Debug.Log("vol = " + this.lastVolume);
                }

                SendFixedSizedChunk(this.dataChunked, this.dataChunked.Length);
            }
        }
    }

    private int sequenceNumber = 0;

    private void SendFixedSizedChunk(float[] dataFloats, int dataCountFloats)
    {
        System.DateTime currentTime = System.DateTime.Now;
        float seconds = (float)(currentTime - this.timeOfLastPacketSend).TotalSeconds;
        this.timeOfLastPacketSend = currentTime;
        if (seconds < 10.0)
        {
            if (this.worstTimeBetweenPackets < seconds)
            {
                this.worstTimeBetweenPackets = seconds;
            }

            if (ShowInterPacketTime)
            {
                UnityEngine.Debug.Log("Microphone: Millisecs since last sent: " + seconds * 1000.0 + "  Worst: " + this.worstTimeBetweenPackets * 1000.0);
            }
        }

        // pack the header
        NetworkOutMessage msg = this.connection.CreateMessage((byte)MessageID.AudioSamples);

        msg.Write((byte)5);	// 8 byte header size

        Int32 pack = 0;
        versionPacker.SetBits(ref pack, 1);				// version
        audioStreamCountPacker.SetBits(ref pack, 1);	// AudioStreamCount
        channelCountPacker.SetBits(ref pack, 1);		// ChannelCount
        sampleRatePacker.SetBits(ref pack, sampleRateType);	// SampleRate: 1 = 16000, 3 = 48000
        sampleTypePacker.SetBits(ref pack, 0);			// SampleType
        sampleCountPacker.SetBits(ref pack, dataCountFloats);  // SampleCount (data count is in bytes and the actual data is in floats, so div by 4)
        codecTypePacker.SetBits(ref pack, 0);			// CodecType   
        mutePacker.SetBits(ref pack, this.muteMicrophone ? 1 : 0);
        sequenceNumberPacker.SetBits(ref pack, sequenceNumber++);
        sequenceNumber %= 32;

        msg.Write((int)pack);			// the packed bits

        // This is where stream data starts. Write all data for one stream

        msg.Write((float)0.0f);			// average amplitude.  Not needed in direction from client to server.
        msg.Write((int)this.clientId);	// non-zero client ID for this client.

        // HRTF position bits

        Vector3 cameraPosRelativeToGlobalAnchor = Vector3.zero;
        Vector3 cameraDirectionRelativeToGlobalAnchor = Vector3.zero;

        if (this.GlobalAnchorTransform != null)
        {
            cameraPosRelativeToGlobalAnchor = MathUtils.TransformPointFromTo(
                 null,
                 this.GlobalAnchorTransform,
                 Camera.main.transform.position);

            cameraDirectionRelativeToGlobalAnchor = MathUtils.TransformDirectionFromTo(
                null,
                this.GlobalAnchorTransform,
                Camera.main.transform.position);
        }

        cameraPosRelativeToGlobalAnchor.Normalize();
        cameraDirectionRelativeToGlobalAnchor.Normalize();

        // Camera position
        msg.Write(cameraPosRelativeToGlobalAnchor.x);
        msg.Write(cameraPosRelativeToGlobalAnchor.y);
        msg.Write(cameraPosRelativeToGlobalAnchor.z);

        // HRTF direction bits
        msg.Write(cameraDirectionRelativeToGlobalAnchor.x);
        msg.Write(cameraDirectionRelativeToGlobalAnchor.y);
        msg.Write(cameraDirectionRelativeToGlobalAnchor.z);

        if (this.dataStreamSendBytes == null || this.dataStreamSendBytes.Length != dataCountFloats * 4)
        {
            this.dataStreamSendBytes = new byte[dataCountFloats * 4];
        }
        dataFloats.ToByteArray4(this.dataStreamSendBytes, dataCountFloats);
        msg.WriteArray(this.dataStreamSendBytes, 4 * (uint)dataCountFloats);

        this.connection.Send(msg, MessagePriority.Immediate, MessageReliability.ReliableOrdered, MessageChannel.Audio, true);
    }

#if !UNITY_METRO || UNITY_EDITOR

    // this code only runs on the desktop, not on the veil machine.
    // 
    // the trick in Unity is to start a Microphone recording audio to an audio clip that
    // does not have a file backing it.  In this case, you can grab the audio bits from
    // the audio clips memory buffer (which has a minimum size of 1 second).  
    // write these bytes into our own circular buffer.  Divide the bytes into fixed sized 
    // packets (441 bytes for now which is 10 ms at 44100 hz) and send them over the network.

    private AudioUtil.HiresTimer sw = new AudioUtil.HiresTimer();
    private string selectedDevice;
    private int microphoneBufferSizeInSeconds = 1;
    private int lastSamplePos = 0;
    private bool isRecording;

    private void internalStart()
    {
        // start up the timer
        this.sw.Start();
    }

    private void internalUpdate()
    {
    }

    private void internalDestroy()
    {
        Microphone.End(this.selectedDevice);
    }

    private void internalInitMic()
    {
        //select audio source
        if (!this.audioSource)
        {
            this.audioSource = transform.GetComponent<AudioSource>();
        }

        int iDevices = 0;
        bool firstDevice = true;
        //count amount of devices connected
        foreach (string device in Microphone.devices)
        {
            iDevices++;
            if (firstDevice)
            {
                firstDevice = false;
                this.selectedDevice = device;
            }

            if (this.ShowDeviceName)
            {
                UnityEngine.Debug.Log("Devices number " + iDevices + ", Name" + "=" + device);
            }
        }

        // detect the selected microphone, if any
        if (iDevices > 0)
        {
            this.selectedDevice = Microphone.devices[0].ToString();
            this.GetMicCaps();
            this.GetComponent<AudioSource>().clip = Microphone.Start(this.selectedDevice, true, this.microphoneBufferSizeInSeconds, this.deviceFrequency);
        }
        else
        {
            this.GetComponent<AudioSource>().clip = null;
        }

        if (GetComponent<AudioSource>().clip != null)
        {
            // loop the playing of the recording so it will be real time
            this.GetComponent<AudioSource>().loop = true;

            // if you only need the data stream values check Mute, if you want to hear yourself in-game don't check Mute. 
            this.GetComponent<AudioSource>().mute = true;

            // full volume by default
            this.GetComponent<AudioSource>().volume = 1;
        }
    }

    private void internalFixedUpdate()
    {
        // if we are recording, then get the stream and send the data...
        if (this.isRecording)
        {
            this.internalGetAndSendDataStream();
        }

        // verify that we are recording periodically
        if (this.fixedUpdateCounter++ % 10 == 0)
        {
            this.isRecording = Microphone.IsRecording(this.selectedDevice);
        }
    }

    private void internalGetAndSendDataStream()
    {
        int numSamples = 0;

        if (this.lockObject != null)
        {
            lock (this.lockObject)
            {
                int micBufferSampleSize = this.microphoneBufferSizeInSeconds * this.deviceFrequency;

                numSamples = this.GetComponent<AudioSource>().timeSamples - this.lastSamplePos;
                if (numSamples != 0)
                {
                    if (numSamples < 0)
                    {
                        numSamples += micBufferSampleSize;
                    }
                    float[] dataStreamFloats = new float[numSamples];

                    this.GetComponent<AudioSource>().GetOutputData(dataStreamFloats, 0);
                    this.circularBuffer.WriteFloats(dataStreamFloats, 0, numSamples);
                    this.lastSamplePos = this.GetComponent<AudioSource>().timeSamples;
                }

                // Send from non-locked state
                if (numSamples > 0)
                {
                    this.Send();
                }
            }
        }
    }

    private void internalStartMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            return;
        }

        this.GetComponent<AudioSource>().clip = Microphone.Start(this.selectedDevice, true, this.microphoneBufferSizeInSeconds, this.deviceFrequency);

        if (GetComponent<AudioSource>().clip != null)
        {
            // loop the playing of the recording so it will be realtime
            this.GetComponent<AudioSource>().loop = true;

            // if you only need the data stream values check Mute, if you want to hear yourself ingame don't check Mute. 
            this.GetComponent<AudioSource>().mute = true;

            // full volume by default
            this.GetComponent<AudioSource>().volume = 1;

            //don't do anything until the microphone has started up
            while (!(Microphone.GetPosition(this.selectedDevice) > 0))
            {
                if (this.DebugSpewOn)
                {
                    UnityEngine.Debug.Log("Awaiting connection");
                }
            }

            // playing the clip starts to stream the Microphone data into the audio clips circular buffer.
            this.GetComponent<AudioSource>().Play();
            this.fixedUpdateCounter = 0;
        }
    }

    private void internalStopMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            return;
        }

        this.GetComponent<AudioSource>().Stop();
        Microphone.End(this.selectedDevice);
        this.fixedUpdateCounter = 0;
    }

    private void GetMicCaps()
    {
        Microphone.GetDeviceCaps(this.selectedDevice, out this.minFreq, out this.maxFreq);
        if ((this.minFreq + this.maxFreq) == 0)
        {
            this.maxFreq = AudioConstants.BaseFrequency;
        }
        this.deviceFrequency = this.maxFreq;
    }
#else

    // the Microphone object in Unity does not work correctly in WSA applications.  This
    // is probably because it is being initialized on an MTAThread and not on an STAThread.
    // Just wraping the init function with a Dispatcher to make it STAThread for init doesn't
    // seem to be enough.
    //
    //  So this code calls into the AudioIO_DLL assembly which is based on NAudio.  It has
    // been paired down to just the bare essentials of WASAPI interactions and should be readable 
    // by someone who knows a bunch about audio in WSA apps.  It is not an easy interface so be careful 
    // making changes in interfaces and init functions.  Most of the meaty stuff is wrapped in a Dispatch
    // call to make sure the critical calls are being done on STAThread COM model.

    MicrophoneControl microphoneControl = new MicrophoneControl();

    private void internalStart()
    {
        this.microphoneControl.Callback = callback;
    }

    private void internalInitMic()
    {
        this.MicBoost = true;
    }

    private void internalDestroy()
    {
    }
    
    private void internalUpdate()
    {
        // this calls down into the Microphone processing code and will bubble back up packets into the 
        // callback just below.
        this.microphoneControl.CheckAndSend();
    }

    private void callback(int length, byte[] dataBytes)
    {
        if (this.dataStreamFloats == null || dataBytes.Length / 4 != this.dataStreamFloats.Length)
        {
            try
            {
                // always read the correct size
                this.dataStreamFloats = new float[dataBytes.Length / 4];
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Problem creating a float or byte (4x) array of size: " + dataBytes.Length + ", " + e.ToString());
            }
        }

        float audioValue = 0.0f;
        float gain = 1.0f;

        if (this.MicBoost)
        {
            gain = this.MicBoostAmount;
        }

        for (int i = 0; i < this.dataStreamFloats.Length; i++)
        {
            this.dataStreamFloats[i] = BitConverter.ToSingle(dataBytes, 4 * i) * gain;
            audioValue += Mathf.Abs(this.dataStreamFloats[i]);
        }

        this.circularBuffer.WriteFloats(this.dataStreamFloats, 0, this.dataStreamFloats.Length);

        this.lastVolume = audioValue / this.dataStreamFloats.Length;

        // send it!
        this.Send();
    }

    private void internalFixedUpdate()
    {
    }

    private void internalGetAndSendDataStream()
    {
    }

    private void internalStartMicrophone()
    {
        this.microphoneControl.ConnectToSession("");
    }

    private void internalStopMicrophone()
    {
        this.microphoneControl.DisconnectFromSession();
    }
#endif
}
