//
// Copyright (C) Microsoft. All rights reserved.
//

using UnityEngine;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Utilities;
using System;
using System.Diagnostics;

#if !(UNITY_STANDALONE_WIN || (UNITY_EDITOR && UNITY_STANDALONE_WIN) || (UNITY_EDITOR && UNITY_METRO))
using Windows.Media.Devices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using AudioIO_DLL;
#endif

#pragma warning disable 219

public class SpeakerController : MonoBehaviour
{
	public static SpeakerController Instance = null;

	public class ProminentSpeakerInfo
	{
		public UInt32       sourceID;
		public float        averageAmplitude;
		public Vector3      hrtfPosition;

	};



	// this line causes the DLL to be included in the Metro build
	//[DllImport("SOTAHrtf", CallingConvention = CallingConvention.Cdecl)]
	//public static extern int Initialize();

	private float m_DropOffMaximum = 5f;
	private float m_PanMaximum = 5f;

	public float DropOffMaximumMetres = 5.0f;
	public float PanMaximumMetres = 5.0f;

	public float MinimumDistance = .01f;

	public Transform GlobalAnchorTransform;        // In OnSight, this is typically set to TerrainSite.Instance.transform

	/// <summary>
	/// Show information about the audio streams
	/// </summary>
	public bool ShowHRTFInfo = false;
	public bool ShowInfo = false;

	private NetworkConnection connection;
	private NetworkConnectionAdapter listener;

	private const byte speakerPlaybackMessageID = (byte)MessageID.AudioSamples;

	private BitManipulator versionExtractor = new BitManipulator(0x7, 0);           // 3 bits, 0 shift
	private BitManipulator audioStreamCountExtractor = new BitManipulator(0x38, 3); // 3 bits, 3 shift
	private BitManipulator channelCountExtractor = new BitManipulator(0x1c0, 6);    // 3 bits, 6 shift
	private BitManipulator sampleRateExtractor = new BitManipulator(0x600, 9);      // 2 bits, 9 shift
	private BitManipulator sampleTypeExtractor = new BitManipulator(0x1800, 11);    // 2 bits, 11 shift
	private BitManipulator sampleCountExtractor = new BitManipulator(0x7fe000, 13); // 10 bits, 13 shift
	private BitManipulator codecTypeExtractor = new BitManipulator(0x1800000, 23);  // 2 bits, 23 shift
	private BitManipulator sequenceNumberExtractor = new BitManipulator((int)0x7C000000, 26);  // 6 bits, 26 shift

	// bytes are for network only
	private byte[] networkPacketBufferBytes = new byte[4 * 10 * AudioConstants.StandardPacketSize];

	// floats are internal - not sure if 10 standard packet sizes is enough here!
	private float[] outputBuffer = new float[10 * AudioConstants.StandardPacketSize];

	private AudioIO_DLL.Utils.CircularBuffer circularBuffer = new AudioIO_DLL.Utils.CircularBuffer(AudioConstants.QuarterSecond);

	/// <summary>
	/// The information for current prominent speakers
	/// </summary>
	private int prominentSpeakerCount = 0;

	/// <summary>
	/// The information for current prominent speakers. NOTE: preallocated to avoid any runtime allocations.
	/// </summary>
	private ProminentSpeakerInfo[] prominentSpeakerList;

	private void Awake()
	{
		SpeakerController.Instance = this;

		prominentSpeakerList = new ProminentSpeakerInfo[AudioConstants.MaximumProminentSpeakers];
		for(int prominentSpeaker = 0; prominentSpeaker < AudioConstants.MaximumProminentSpeakers; prominentSpeaker++)
		{
			this.prominentSpeakerList[prominentSpeaker] = new ProminentSpeakerInfo();
		}
	}

	private Stopwatch sw = new Stopwatch();
	private AudioClip speakerClip;

	private void Start()
	{
		this.sw.Start();

		//SpeakerController.Initialize(); // init the HRTF engine

		Time.fixedDeltaTime = 0.01f; // 10 ms FixedUpdate times

		try
		{
			this.connection = SharingStage.Instance.Manager.GetServerConnection();

			this.listener = new NetworkConnectionAdapter();
			this.listener.ConnectedCallback += OnConnected;
			this.listener.DisconnectedCallback += OnDisconnected;
			this.listener.ConnectionFailedCallback += OnConnectedFailed;
			this.listener.MessageReceivedCallback += OnMessageReceived;

			this.connection.AddListener(SpeakerController.speakerPlaybackMessageID, this.listener);

			UnityEngine.Debug.Log("SpeakerController Start called");
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Exception: " + ex.ToString());
		}

		internalStart();

		//        internalStartSpeaker();
	}

	protected void OnDestroy()
	{
		this.connection.RemoveListener(SpeakerController.speakerPlaybackMessageID, this.listener);

		this.listener.Dispose();
		Instance = null;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown(KeyCode.F8))
			{
				internalStartSpeaker();
				UnityEngine.Debug.Log("Speakers Started");
			}

			if (Input.GetKeyDown(KeyCode.F9))
			{
				internalStopSpeaker();
				UnityEngine.Debug.Log("Speakers Stopped");
			}
		}

		internalUpdate();
	}

	private void OnConnected(NetworkConnection connection)
	{
		Profile.BeginRange("SpeakerController.OnConnected");
		this.internalStartSpeaker();
		UnityEngine.Debug.Log("SpeakerController: Connection to session server succeeded!");
		Profile.EndRange();
	}

	private void OnDisconnected(NetworkConnection connection)
	{
		this.internalStopSpeaker();

		this.prominentSpeakerCount = 0;

		UnityEngine.Debug.Log("SpeakerController: Session server disconnected!");
	}

	private void OnConnectedFailed(NetworkConnection connection)
	{
		this.internalStopSpeaker();
		UnityEngine.Debug.Log("SpeakerController: Connection to session server failed!");
	}


	/// <summary>
	/// Now that we've gotten a message, examine it and dissect the audio data.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="message"></param>
	public void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
	{
		byte headerSize = message.ReadByte();

		Int32 pack = message.ReadInt32();

		int version = this.versionExtractor.GetBitsValue(pack);
		int audioStreamCount = this.audioStreamCountExtractor.GetBitsValue(pack);
		int channelCount = this.channelCountExtractor.GetBitsValue(pack);
		int sampleRate = this.sampleRateExtractor.GetBitsValue(pack);
		int sampleType = this.sampleTypeExtractor.GetBitsValue(pack);
		int bytesPerSample = sizeof(float);
		if (sampleType == 1)
		{
			bytesPerSample = sizeof(Int16);
		}

		int sampleCount = this.sampleCountExtractor.GetBitsValue(pack);
		int codecType = this.codecTypeExtractor.GetBitsValue(pack);
		int sequenceNumber = this.sequenceNumberExtractor.GetBitsValue(pack);

		Int32 extendedSampleRate = 0;
		if (sampleRate == 0)
		{
			extendedSampleRate = message.ReadInt32();
		}

		this.prominentSpeakerCount = 0;

		for (int i = 0; i < audioStreamCount; i++)
		{
			float averageAmplitude = message.ReadFloat();
			UInt32 hrtfSourceID = (UInt32)message.ReadInt32();
			Vector3 hrtfPosition = new Vector3();
			Vector3 hrtfDirection = new Vector3();
			if (hrtfSourceID != 0)
			{
				hrtfPosition.x = message.ReadFloat();
				hrtfPosition.y = message.ReadFloat();
				hrtfPosition.z = message.ReadFloat();

				hrtfDirection.x = message.ReadFloat();
				hrtfDirection.y = message.ReadFloat();
				hrtfDirection.z = message.ReadFloat();

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

				Vector3 soundVector = hrtfPosition - cameraPosRelativeToGlobalAnchor;
				soundVector.Normalize();

				// x is forward
				float fltx = (m_DropOffMaximum / DropOffMaximumMetres) * Vector3.Dot(soundVector, cameraDirectionRelativeToGlobalAnchor);
				// y is right
				Vector3 myRight = Quaternion.Euler(0, 90, 0) * cameraDirectionRelativeToGlobalAnchor;
				float flty = -(m_PanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myRight);
				// z is up
				Vector3 myUp = Quaternion.Euler(90, 0, 0) * cameraDirectionRelativeToGlobalAnchor;
				float fltz = (m_PanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myUp);

				if (this.ShowHRTFInfo)
				{
					UnityEngine.Debug.Log("hrtf = " + fltx + ", " + flty + ", " + fltz);
				}

				// Hacky distance check so we don't get too close to source.
				Vector3 flt = new Vector3(fltx, flty, fltz);
				if (flt.magnitude < (MinimumDistance * m_DropOffMaximum))
				{
					flt = flt.normalized * MinimumDistance * m_DropOffMaximum;
					fltx = flt.x;
					flty = flt.y;
					fltz = flt.z;
				}

				AddProminentSpeaker(hrtfSourceID, averageAmplitude, fltx, flty, fltz);
			}

			for (int j = 0; j < channelCount; j++)
			{
				// if uncompressed, size = sampleCount
				Int16 size = (Int16)sampleCount;
				if (codecType != 0)
				{
					// if compressed, size is first 2 bytes, sampleCount should be number of bytes after decompression
					size = message.ReadInt16();
				}

				// make this array big enough to hold all of the uncompressed data only if the
				// buffer is not the right size, minimize new operations
				if (this.networkPacketBufferBytes.Length != sampleCount * bytesPerSample)
				{
					this.networkPacketBufferBytes = new byte[sampleCount * bytesPerSample];
				}
				message.ReadArray(this.networkPacketBufferBytes, (uint)(size * bytesPerSample));

				if (codecType != 0)
				{
					// in place decompression please - should fill out the data buffer
					// ...
				}

				if (hrtfSourceID > 0)
				{
					// hrtf processing here
				}

				internalStore(this.networkPacketBufferBytes, 0, this.networkPacketBufferBytes.Length);
			}
		}
	}

	private void AddProminentSpeaker(UInt32 sourceID, float averageAmplitude, float posX, float posY, float posZ)
	{
		if (this.prominentSpeakerCount < AudioConstants.MaximumProminentSpeakers)
		{
			ProminentSpeakerInfo prominentSpeakerInfo = this.prominentSpeakerList[this.prominentSpeakerCount++];
			prominentSpeakerInfo.sourceID = sourceID;
			prominentSpeakerInfo.averageAmplitude = averageAmplitude;
			prominentSpeakerInfo.hrtfPosition.x = posX;
			prominentSpeakerInfo.hrtfPosition.y = posY;
			prominentSpeakerInfo.hrtfPosition.z = posZ;
		}
	}


	public int GetNumProminentSpeakers()
	{
		return this.prominentSpeakerCount;
	}

	public ProminentSpeakerInfo GetProminentSpeaker(int index)
	{
		if (index < this.prominentSpeakerCount)
		{
			return this.prominentSpeakerList[index];
		}
		else
		{
			return null;
		}
	}


#if !UNITY_METRO || UNITY_EDITOR
	private bool processAudioEnabled = false;
	//private int countsPerSecond = 0;
	private int starvedCount = 0;
	private int maxStarvedPacketsBeforeSilence = 5;     //duplicate last packet up to this many packets, then switch to silence.
	private float maxTimeBetweenPackets = 0;
	private System.DateTime timeOfLastPacketSend = System.DateTime.MinValue;

	private void internalStart()
	{
		this.speakerClip = AudioClip.Create(
			"chat",
			AudioConstants.BaseFrequency,
			AudioConstants.AudioChannelsPreHRTF,
			AudioConstants.BaseFrequency,
			true);

		this.GetComponent<AudioSource>().clip = this.speakerClip;
		this.GetComponent<AudioSource>().loop = true;
	}

	private void internalUpdate()
	{
	}

	private void OnAudioFilterRead(float[] data, int numChannels)
	{
		if (this.processAudioEnabled)
		{
			int sampleCount = data.Length / numChannels;

			if (this.circularBuffer.FloatCount >= sampleCount)
			{
				int readCount = this.circularBuffer.ReadFloats(this.outputBuffer, 0, sampleCount);
				if (readCount != sampleCount)
				{
					UnityEngine.Debug.Log("(readCount = " + readCount + ") != (Buffer Length =  " + sampleCount + ")");
				}

				this.starvedCount = 0;
			}
			else
			{
				// repeat last packet unless it's been starved for a while, then emit silence.
				if (++this.starvedCount > this.maxStarvedPacketsBeforeSilence)
				{
					for (int sample = 0; sample < this.outputBuffer.Length; sample++)
					{
						this.outputBuffer[sample] = 0.0f;
					}
				}
			}


			int outSamplePos = 0;
			// Fill either with newly fetched data, or contents of last packet already in the buffer
			for (int sample = 0; sample < sampleCount; sample++)
			{
				float sampleValue = this.outputBuffer[sample];
				for (int channel = 0; channel < numChannels; channel++)
				{
					data[outSamplePos++] = sampleValue;
				}
			}

			if (this.ShowInfo)
			{
				System.DateTime currentTime = System.DateTime.Now;
				float seconds = (float)(currentTime - this.timeOfLastPacketSend).TotalSeconds;
				this.timeOfLastPacketSend = currentTime;

				if (seconds < 10.0)
				{
					if (this.maxTimeBetweenPackets < seconds)
					{
						this.maxTimeBetweenPackets = seconds;
					}
				}
				//UnityEngine.Debug.Log("buffer size per channel = " + data.Length / numChannels + ", channels = " + numChannels);

				if (this.sw.ElapsedMilliseconds > 1000)
				{
					UnityEngine.Debug.Log("Connections [maxLatency] [" +
						this.maxTimeBetweenPackets * 1000 + "]");
					this.sw.Reset();
					this.sw.Start();
				}
			}
		}
	}

	/// <summary>
	/// Starts playing the audio stream out to the speaker.
	/// </summary>
	private void internalStartSpeaker()
	{
		this.processAudioEnabled = true;
		this.GetComponent<AudioSource>().Play();
	}

	/// <summary>
	/// Stops playing the audio stream out to the speaker.
	/// </summary>
	private void internalStopSpeaker()
	{
		this.processAudioEnabled = false;
		this.GetComponent<AudioSource>().Stop();
		this.circularBuffer.Reset();
	}

	private void internalStore(byte[] data, int offset, int dataCount)
	{
		// place the data into the buffer here (converts bytes to floats in the process)
		this.circularBuffer.WriteBytes(data, offset, dataCount);
	}
#else
	private SpeakerControl speakerControl = new SpeakerControl();

	private void internalStart()
	{
	}

	private void internalUpdate()
	{
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	private void SetHRTFPosition(float x, float y, float z)
	{
		//this.speakerControl.SetHRTFPosition(x, y, z);
	}

	/// <summary>
	/// Starts playing the audio stream out to the speaker.
	/// </summary>
	public void internalStartSpeaker()
	{
		this.speakerControl.StartSpeakers();
	}

	/// <summary>
	/// Stops playing the audio stream out to the speaker.
	/// </summary>
	public void internalStopSpeaker()
	{
		this.speakerControl.StopSpeakers();
	}

	private void internalStore(byte[] data, int offset, int dataCount)
	{
		this.speakerControl.WriteDataToSpeakers(data, offset, dataCount);
	}
#endif
}
