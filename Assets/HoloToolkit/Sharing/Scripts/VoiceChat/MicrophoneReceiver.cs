// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing.VoiceChat
{
    /// <summary>
    /// Receives and plays voice data transmitted through the session server. This data comes from other clients running the MicrophoneTransmitter behaviour.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneReceiver : MonoBehaviour
    {
        private readonly BitManipulator versionExtractor = new BitManipulator(0x7, 0);           // 3 bits, 0 shift
        private readonly BitManipulator audioStreamCountExtractor = new BitManipulator(0x38, 3); // 3 bits, 3 shift
        private readonly BitManipulator channelCountExtractor = new BitManipulator(0x1c0, 6);    // 3 bits, 6 shift
        private readonly BitManipulator sampleRateExtractor = new BitManipulator(0x600, 9);      // 2 bits, 9 shift
        private readonly BitManipulator sampleTypeExtractor = new BitManipulator(0x1800, 11);    // 2 bits, 11 shift
        private readonly BitManipulator sampleCountExtractor = new BitManipulator(0x7fe000, 13); // 10 bits, 13 shift
        private readonly BitManipulator codecTypeExtractor = new BitManipulator(0x1800000, 23);  // 2 bits, 23 shift
        private readonly BitManipulator sequenceNumberExtractor = new BitManipulator(0x7C000000, 26);  // 6 bits, 26 shift

        public Transform GlobalAnchorTransform;

        public class ProminentSpeakerInfo
        {
            public UInt32 SourceId;
            public float AverageAmplitude;
            public Vector3 HrtfPosition;
        }

        /// <summary>
        /// Maximum number of prominent speakers we should ever encounter
        /// </summary>
        public const int MaximumProminentSpeakers = 4;

        /// <summary>
        /// The information for current prominent speakers
        /// </summary>
        private int prominentSpeakerCount;

        /// <summary>
        /// The information for current prominent speakers. NOTE: preallocated to avoid any runtime allocations.
        /// </summary>
        private ProminentSpeakerInfo[] prominentSpeakerList;

        private NetworkConnectionAdapter listener;

        private readonly Mutex audioDataMutex = new Mutex();

        private const float KDropOffMaximum = 5f;
        private const float KPanMaximum = 5f;

        public float DropOffMaximumMetres = 5.0f;
        public float PanMaximumMetres = 5.0f;

        public float MinimumDistance = .01f;

        private byte[] networkPacketBufferBytes;
        private CircularBuffer circularBuffer;

        #region DebugVariables
        private readonly CircularBuffer testCircularBuffer = new CircularBuffer(48000 * 2 * 4 * 3, true);
        private AudioSource testSource;
        public AudioClip TestClip;
        public bool SaveTestClip;
        #endregion

        private void Awake()
        {
            prominentSpeakerList = new ProminentSpeakerInfo[MaximumProminentSpeakers];
            for (int prominentSpeaker = 0; prominentSpeaker < MaximumProminentSpeakers; prominentSpeaker++)
            {
                prominentSpeakerList[prominentSpeaker] = new ProminentSpeakerInfo();
            }

            networkPacketBufferBytes = new byte[4 * MicrophoneTransmitter.AudioPacketSize];
            circularBuffer = new CircularBuffer(48000 * 4);
        }

        private void TryConnect()
        {
            try
            {
                if (listener == null)
                {
                    SharingStage sharingStage = SharingStage.Instance;
                    if (sharingStage && sharingStage.Manager != null)
                    {
                        NetworkConnection connection = SharingStage.Instance.Manager.GetServerConnection();

                        listener = new NetworkConnectionAdapter();
                        listener.ConnectedCallback += OnConnected;
                        listener.DisconnectedCallback += OnDisconnected;
                        listener.ConnectionFailedCallback += OnConnectedFailed;
                        listener.MessageReceivedCallback += OnMessageReceived;

                        connection.AddListener((byte)MessageID.AudioSamples, listener);

                        Debug.Log("SpeakerController Start called");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Exception: " + ex);
            }
        }

        private void OnDestroy()
        {
            if (listener != null)
            {
                listener.ConnectedCallback -= OnConnected;
                listener.DisconnectedCallback -= OnDisconnected;
                listener.ConnectionFailedCallback -= OnConnectedFailed;
                listener.MessageReceivedCallback -= OnMessageReceived;
            }
        }

        private void OnConnected(NetworkConnection connection)
        {
            Profile.BeginRange("SpeakerController.OnConnected");
            InternalStartSpeaker();
            Debug.Log("SpeakerController: Connection to session server succeeded!");
            Profile.EndRange();
        }

        private void OnDisconnected(NetworkConnection connection)
        {
            InternalStopSpeaker();

            prominentSpeakerCount = 0;

            Debug.Log("SpeakerController: Session server disconnected!");
        }

        private void OnConnectedFailed(NetworkConnection connection)
        {
            InternalStopSpeaker();
            Debug.Log("SpeakerController: Connection to session server failed!");
        }

        /// <summary>
        /// Starts playing the audio stream out to the speaker.
        /// </summary>
        private void InternalStartSpeaker()
        {
            GetComponent<AudioSource>().Play();
        }

        /// <summary>
        /// Stops playing the audio stream out to the speaker.
        /// </summary>
        private void InternalStopSpeaker()
        {
            GetComponent<AudioSource>().Stop();
        }

        private void Update()
        {
            TryConnect();

            AudioSource audioSource = GetComponent<AudioSource>();
            GameObject remoteHead = GameObject.Find("mixamorig:Head");
            if (remoteHead)
            {
                transform.parent = remoteHead.transform;
                transform.localPosition = new Vector3();
                transform.localRotation = Quaternion.identity;

                audioSource.spatialize = true;
                audioSource.spatialBlend = 1;
            }
            else
            {
                audioSource.spatialize = false;
                audioSource.spatialBlend = 0;
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
                testSource.PlayClip(TestClip, true);
                SaveTestClip = false;
            }
            #endregion
        }

        /// <summary>
        /// Now that we've gotten a message, examine it and dissect the audio data.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
        {
            // Unused byte headerSize
            message.ReadByte();

            Int32 pack = message.ReadInt32();

            // Unused int version
            versionExtractor.GetBitsValue(pack);
            int audioStreamCount = audioStreamCountExtractor.GetBitsValue(pack);
            int channelCount = channelCountExtractor.GetBitsValue(pack);
            int sampleRate = sampleRateExtractor.GetBitsValue(pack);
            int sampleType = sampleTypeExtractor.GetBitsValue(pack);
            int bytesPerSample = sizeof(float);
            if (sampleType == 1)
            {
                bytesPerSample = sizeof(Int16);
            }

            int sampleCount = sampleCountExtractor.GetBitsValue(pack);
            int codecType = codecTypeExtractor.GetBitsValue(pack);

            // Unused int sequenceNumber
            sequenceNumberExtractor.GetBitsValue(pack);

            if (sampleRate == 0)
            {
                // Unused int extendedSampleRate
                 message.ReadInt32();
            }

            try
            {
                audioDataMutex.WaitOne();

                prominentSpeakerCount = 0;

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

                        Vector3 soundVector = hrtfPosition - cameraPosRelativeToGlobalAnchor;
                        soundVector.Normalize();

                        // x is forward
                        float fltx = (KDropOffMaximum / DropOffMaximumMetres) * Vector3.Dot(soundVector, cameraDirectionRelativeToGlobalAnchor);
                        // y is right
                        Vector3 myRight = Quaternion.Euler(0, 90, 0) * cameraDirectionRelativeToGlobalAnchor;
                        float flty = -(KPanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myRight);
                        // z is up
                        Vector3 myUp = Quaternion.Euler(90, 0, 0) * cameraDirectionRelativeToGlobalAnchor;
                        float fltz = (KPanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myUp);

                        // Hacky distance check so we don't get too close to source.
                        Vector3 flt = new Vector3(fltx, flty, fltz);
                        if (flt.magnitude < (MinimumDistance * KDropOffMaximum))
                        {
                            flt = flt.normalized * MinimumDistance * KDropOffMaximum;
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
                        int totalBytes = size * bytesPerSample;
                        if (networkPacketBufferBytes.Length != totalBytes)
                        {
                            networkPacketBufferBytes = new byte[totalBytes];
                        }
                        message.ReadArray(networkPacketBufferBytes, (uint)(totalBytes));

                        if (codecType != 0)
                        {
                            // in place decompression please - should fill out the data buffer
                            // ...
                        }

                        if (hrtfSourceID > 0)
                        {
                            // TODO hrtf processing here
                        }

                        circularBuffer.Write(networkPacketBufferBytes, 0, networkPacketBufferBytes.Length);
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

        private void OnAudioFilterRead(float[] data, int numChannels)
        {
            try
            {
                audioDataMutex.WaitOne();
                int byteCount = data.Length * 4;
                circularBuffer.Read(data, 0, byteCount);
                if (SaveTestClip)
                {
                    testCircularBuffer.Write(data, 0, byteCount);
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

        private void AddProminentSpeaker(UInt32 sourceID, float averageAmplitude, float posX, float posY, float posZ)
        {
            if (prominentSpeakerCount < MaximumProminentSpeakers)
            {
                ProminentSpeakerInfo prominentSpeakerInfo = prominentSpeakerList[prominentSpeakerCount++];
                prominentSpeakerInfo.SourceId = sourceID;
                prominentSpeakerInfo.AverageAmplitude = averageAmplitude;
                prominentSpeakerInfo.HrtfPosition.x = posX;
                prominentSpeakerInfo.HrtfPosition.y = posY;
                prominentSpeakerInfo.HrtfPosition.z = posZ;
            }
        }

        public int GetNumProminentSpeakers()
        {
            return prominentSpeakerCount;
        }

        public ProminentSpeakerInfo GetProminentSpeaker(int index)
        {
            if (index < prominentSpeakerCount)
            {
                return prominentSpeakerList[index];
            }
            return null;
        }
    }
}