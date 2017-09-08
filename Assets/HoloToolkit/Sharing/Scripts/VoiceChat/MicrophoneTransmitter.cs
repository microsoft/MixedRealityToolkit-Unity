// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Sharing.VoiceChat
{
    /// <summary>
    /// Transmits data from your microphone to other clients connected to a SessionServer. Requires any receiving client to be running the MicrophoneReceiver script.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneTransmitter : MonoBehaviour
    {
        /// <summary>
        /// Which type of microphone/quality to access
        /// </summary>
        public MicStream.StreamCategory Streamtype = MicStream.StreamCategory.HIGH_QUALITY_VOICE;

        /// <summary>
        /// You can boost volume here as desired. 1 is default but probably too quiet. You can change during operation. 
        /// </summary>
        public float InputGain = 2;

        /// <summary>
        /// Whether or not to send the microphone data across the network
        /// </summary>
        public bool ShouldTransmitAudio = true;

        /// <summary>
        /// Whether other users should be able to hear the transmitted audio
        /// </summary>
        public bool Mute;

        public Transform GlobalAnchorTransform;

        public bool ShowInterPacketTime;

        private DateTime timeOfLastPacketSend;
        private float worstTimeBetweenPackets;

        private int sequenceNumber;

        private int sampleRateType = 3; // 48000Hz

        private AudioSource audioSource;

        private bool hasServerConnection;
        private bool micStarted;

        public const int AudioPacketSize = 960;
        private CircularBuffer micBuffer = new CircularBuffer(AudioPacketSize * 10 * 2 * 4, true);
        private byte[] packetSamples = new byte[AudioPacketSize * 4];

        // bit packers
        private readonly BitManipulator versionPacker = new BitManipulator(0x7, 0);           // 3 bits, 0 shift
        private readonly BitManipulator audioStreamCountPacker = new BitManipulator(0x38, 3); // 3 bits, 3 shift
        private readonly BitManipulator channelCountPacker = new BitManipulator(0x1c0, 6);    // 3 bits, 6 shift
        private readonly BitManipulator sampleRatePacker = new BitManipulator(0x600, 9);      // 2 bits, 9 shift
        private readonly BitManipulator sampleTypePacker = new BitManipulator(0x1800, 11);    // 2 bits, 11 shift
        private readonly BitManipulator sampleCountPacker = new BitManipulator(0x7fe000, 13); // 10 bits, 13 shift
        private readonly BitManipulator codecTypePacker = new BitManipulator(0x1800000, 23);  // 2 bits, 23 shift
        private readonly BitManipulator mutePacker = new BitManipulator(0x2000000, 25);  // 1 bits, 25 shift
        private readonly BitManipulator sequenceNumberPacker = new BitManipulator(0x7C000000, 26);  // 6 bits, 26 shift

        private readonly Mutex audioDataMutex = new Mutex();

        #region DebugVariables
        public bool HearSelf;

        private readonly CircularBuffer testCircularBuffer = new CircularBuffer(48000 * 2 * 4 * 3, true);
        private AudioSource testSource;
        public AudioClip TestClip;
        public bool SaveTestClip;
        #endregion

        private NetworkConnection GetActiveConnection()
        {
            NetworkConnection connection = null;
            var stage = SharingStage.Instance;
            if (stage && stage.Manager != null)
            {
                connection = stage.Manager.GetServerConnection();
            }
            if (connection == null || !connection.IsConnected())
            {
                return null;
            }
            return connection;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            int errorCode = MicStream.MicInitializeCustomRate((int)Streamtype, AudioSettings.outputSampleRate);
            CheckForErrorOnCall(errorCode);
            if (errorCode == 0 || errorCode == (int)MicStream.ErrorCodes.ALREADY_RUNNING)
            {
                if (CheckForErrorOnCall(MicStream.MicSetGain(InputGain)))
                {
                    audioSource.volume = HearSelf ? 1.0f : 0.0f;
                    micStarted = CheckForErrorOnCall(MicStream.MicStartStream(false, false));
                }
            }
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            try
            {
                audioDataMutex.WaitOne();

                if (micStarted && hasServerConnection)
                {
                    if (CheckForErrorOnCall(MicStream.MicGetFrame(buffer, buffer.Length, numChannels)))
                    {
                        int dataSize = buffer.Length * 4;
                        if (micBuffer.Write(buffer, 0, dataSize) != dataSize)
                        {
                            Debug.LogError("Send buffer filled up. Some audio will be lost.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                audioDataMutex.ReleaseMutex();
            }
        }

        private void Update()
        {
            CheckForErrorOnCall(MicStream.MicSetGain(InputGain));
            audioSource.volume = HearSelf ? 1.0f : 0.0f;

            try
            {
                audioDataMutex.WaitOne();

                var connection = GetActiveConnection();
                hasServerConnection = (connection != null);
                if (hasServerConnection)
                {
                    while (micBuffer.UsedCapacity >= 4 * AudioPacketSize)
                    {
                        TransmitAudio(connection);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                audioDataMutex.ReleaseMutex();
            }

            #region debuginfo
            if (SaveTestClip && testCircularBuffer.UsedCapacity == testCircularBuffer.TotalCapacity)
            {
                float[] testBuffer = new float[testCircularBuffer.UsedCapacity / 4];
                testCircularBuffer.Read(testBuffer, 0, testBuffer.Length * 4);
                testCircularBuffer.Reset();
                TestClip = AudioClip.Create("testclip", testBuffer.Length / 2, 2, 48000, false);
                TestClip.SetData(testBuffer, 0);
                if (!testSource)
                {
                    GameObject testObj = new GameObject("testclip");
                    testObj.transform.parent = transform;
                    testSource = testObj.AddComponent<AudioSource>();
                }
                testSource.PlayClip(TestClip);
                SaveTestClip = false;
            }
            #endregion
        }

        private void TransmitAudio(NetworkConnection connection)
        {
            micBuffer.Read(packetSamples, 0, 4 * AudioPacketSize);
            SendFixedSizedChunk(connection, packetSamples, packetSamples.Length);

            if (SaveTestClip)
            {
                testCircularBuffer.Write(packetSamples, 0, packetSamples.Length);
            }
        }

        private void SendFixedSizedChunk(NetworkConnection connection, byte[] data, int dataSize)
        {
            DateTime currentTime = DateTime.Now;
            float seconds = (float)(currentTime - timeOfLastPacketSend).TotalSeconds;
            timeOfLastPacketSend = currentTime;
            if (seconds < 10.0)
            {
                if (worstTimeBetweenPackets < seconds)
                {
                    worstTimeBetweenPackets = seconds;
                }

                if (ShowInterPacketTime)
                {
                    Debug.LogFormat("Microphone: Millisecs since last sent: {0}, Worst: {1}",
                        (seconds * 1000.0).ToString(CultureInfo.InvariantCulture),
                        (worstTimeBetweenPackets * 1000.0).ToString(CultureInfo.InvariantCulture));
                }
            }

            int clientId = SharingStage.Instance.Manager.GetLocalUser().GetID();

            // pack the header
            NetworkOutMessage msg = connection.CreateMessage((byte)MessageID.AudioSamples);

            int dataCountFloats = dataSize / 4;

            msg.Write((byte)5); // 8 byte header size

            Int32 pack = 0;
            versionPacker.SetBits(ref pack, 1);                   // version
            audioStreamCountPacker.SetBits(ref pack, 1);          // AudioStreamCount
            channelCountPacker.SetBits(ref pack, 1);              // ChannelCount
            sampleRatePacker.SetBits(ref pack, sampleRateType);   // SampleRate: 1 = 16000, 3 = 48000
            sampleTypePacker.SetBits(ref pack, 0);                // SampleType
            sampleCountPacker.SetBits(ref pack, dataCountFloats); // SampleCount (data count is in bytes and the actual data is in floats, so div by 4)
            codecTypePacker.SetBits(ref pack, 0);                 // CodecType
            mutePacker.SetBits(ref pack, Mute ? 1 : 0);
            sequenceNumberPacker.SetBits(ref pack, sequenceNumber++);
            sequenceNumber %= 32;

            msg.Write(pack); // the packed bits

            // This is where stream data starts. Write all data for one stream

            msg.Write(0.0f);     // average amplitude.  Not needed in direction from client to server.
            msg.Write(clientId); // non-zero client ID for this client.

            // HRTF position bits

            Vector3 cameraPosRelativeToGlobalAnchor = Vector3.zero;
            Vector3 cameraDirectionRelativeToGlobalAnchor = Vector3.zero;

            if (GlobalAnchorTransform != null)
            {
                cameraPosRelativeToGlobalAnchor = MathUtils.TransformPointFromTo(
                    null,
                    GlobalAnchorTransform,
                    CameraCache.Main.transform.position);

                cameraDirectionRelativeToGlobalAnchor = MathUtils.TransformDirectionFromTo(
                    null,
                    GlobalAnchorTransform,
                    CameraCache.Main.transform.position);
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

            msg.WriteArray(data, (uint)dataCountFloats * 4);

            connection.Send(msg, MessagePriority.Immediate, MessageReliability.ReliableOrdered, MessageChannel.Audio, true);
        }

        private void OnDestroy()
        {
            CheckForErrorOnCall(MicStream.MicDestroy());
        }

        private bool CheckForErrorOnCall(int returnCode)
        {
            return MicStream.CheckForErrorOnCall(returnCode);
        }

#if DOTNET_FX
        // on device, deal with all the ways that we could suspend our program in as few lines as possible
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                CheckForErrorOnCall(MicStream.MicPause());
            }
            else
            {
                CheckForErrorOnCall(MicStream.MicResume());
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            OnApplicationPause(!focused);
        }

        private void OnDisable()
        {
            OnApplicationPause(true);
        }

        private void OnEnable()
        {
            OnApplicationPause(false);
        }
#endif
    }
}