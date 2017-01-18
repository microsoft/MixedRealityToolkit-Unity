// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Sharing.Utilities;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Receives and plays voice data transmitted through the session server. This data comes from other clients running the MicrophoneTransmitter behaviour.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneReceiver : MonoBehaviour
    {
        private const byte speakerPlaybackMessageID = (byte)MessageID.AudioSamples;

        private BitManipulator versionExtractor = new BitManipulator(0x7, 0);           // 3 bits, 0 shift
        private BitManipulator audioStreamCountExtractor = new BitManipulator(0x38, 3); // 3 bits, 3 shift
        private BitManipulator channelCountExtractor = new BitManipulator(0x1c0, 6);    // 3 bits, 6 shift
        private BitManipulator sampleRateExtractor = new BitManipulator(0x600, 9);      // 2 bits, 9 shift
        private BitManipulator sampleTypeExtractor = new BitManipulator(0x1800, 11);    // 2 bits, 11 shift
        private BitManipulator sampleCountExtractor = new BitManipulator(0x7fe000, 13); // 10 bits, 13 shift
        private BitManipulator codecTypeExtractor = new BitManipulator(0x1800000, 23);  // 2 bits, 23 shift
        private BitManipulator sequenceNumberExtractor = new BitManipulator((int)0x7C000000, 26);  // 6 bits, 26 shift

        public Transform GlobalAnchorTransform;

        public class ProminentSpeakerInfo
        {
            public UInt32 sourceID;
            public float averageAmplitude;
            public Vector3 hrtfPosition;

        };

        /// <summary>
        /// Maximum number of prominent speakers we should ever encounter
        /// </summary>
        public const int MaximumProminentSpeakers = 4;

        /// <summary>
        /// The information for current prominent speakers
        /// </summary>
        private int prominentSpeakerCount = 0;

        /// <summary>
        /// The information for current prominent speakers. NOTE: preallocated to avoid any runtime allocations.
        /// </summary>
        private ProminentSpeakerInfo[] prominentSpeakerList;

        private NetworkConnectionAdapter listener;

        private Mutex audioDataMutex = new Mutex();

        private const float kDropOffMaximum = 5f;
        private const float kPanMaximum = 5f;

        public float DropOffMaximumMetres = 5.0f;
        public float PanMaximumMetres = 5.0f;

        public float MinimumDistance = .01f;

        private byte[] networkPacketBufferBytes;
        private CircularBuffer circularBuffer;

        #region DebugVariables
        private CircularBuffer testCircularBuffer = new CircularBuffer(48000 * 2 * 4 * 3, true);
        private AudioSource testSource;
        public AudioClip testClip;
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
                    var sharingStage = SharingStage.Instance;
                    if (sharingStage && sharingStage.Manager != null)
                    {
                        var connection = SharingStage.Instance.Manager.GetServerConnection();

                        listener = new NetworkConnectionAdapter();
                        listener.ConnectedCallback += OnConnected;
                        listener.DisconnectedCallback += OnDisconnected;
                        listener.ConnectionFailedCallback += OnConnectedFailed;
                        listener.MessageReceivedCallback += OnMessageReceived;

                        connection.AddListener((byte)MessageID.AudioSamples, listener);

                        UnityEngine.Debug.Log("SpeakerController Start called");
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Exception: " + ex.ToString());
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
            internalStartSpeaker();
            UnityEngine.Debug.Log("SpeakerController: Connection to session server succeeded!");
            Profile.EndRange();
        }

        private void OnDisconnected(NetworkConnection connection)
        {
            internalStopSpeaker();

            prominentSpeakerCount = 0;

            UnityEngine.Debug.Log("SpeakerController: Session server disconnected!");
        }

        private void OnConnectedFailed(NetworkConnection connection)
        {
            internalStopSpeaker();
            UnityEngine.Debug.Log("SpeakerController: Connection to session server failed!");
        }

        /// <summary>
        /// Starts playing the audio stream out to the speaker.
        /// </summary>
        private void internalStartSpeaker()
        {
            GetComponent<AudioSource>().Play();
        }

        /// <summary>
        /// Stops playing the audio stream out to the speaker.
        /// </summary>
        private void internalStopSpeaker()
        {
            GetComponent<AudioSource>().Stop();
        }

        private void Update()
        {
            TryConnect();

            var audioSource = GetComponent<AudioSource>();
            var remoteHead = GameObject.Find("mixamorig:Head");
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
                testClip = AudioClip.Create("testclip", testBuffer.Length / 2, 2, 48000, false);
                testClip.SetData(testBuffer, 0);
                if (!testSource)
                {
                    GameObject testObj = new GameObject("testclip");
                    testObj.transform.parent = transform;
                    testSource = testObj.AddComponent<AudioSource>();
                }
                testSource.PlayClip(testClip, true);
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
                                Camera.main.transform.position);
                            cameraDirectionRelativeToGlobalAnchor = MathUtils.TransformDirectionFromTo(
                                null,
                                GlobalAnchorTransform,
                                Camera.main.transform.position);

                        }

                        cameraPosRelativeToGlobalAnchor.Normalize();
                        cameraDirectionRelativeToGlobalAnchor.Normalize();

                        Vector3 soundVector = hrtfPosition - cameraPosRelativeToGlobalAnchor;
                        soundVector.Normalize();

                        // x is forward
                        float fltx = (kDropOffMaximum / DropOffMaximumMetres) * Vector3.Dot(soundVector, cameraDirectionRelativeToGlobalAnchor);
                        // y is right
                        Vector3 myRight = Quaternion.Euler(0, 90, 0) * cameraDirectionRelativeToGlobalAnchor;
                        float flty = -(kPanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myRight);
                        // z is up
                        Vector3 myUp = Quaternion.Euler(90, 0, 0) * cameraDirectionRelativeToGlobalAnchor;
                        float fltz = (kPanMaximum / PanMaximumMetres) * Vector3.Dot(soundVector, myUp);

                        // Hacky distance check so we don't get too close to source.
                        Vector3 flt = new Vector3(fltx, flty, fltz);
                        if (flt.magnitude < (MinimumDistance * kDropOffMaximum))
                        {
                            flt = flt.normalized * MinimumDistance * kDropOffMaximum;
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
                            // hrtf processing here
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
                prominentSpeakerInfo.sourceID = sourceID;
                prominentSpeakerInfo.averageAmplitude = averageAmplitude;
                prominentSpeakerInfo.hrtfPosition.x = posX;
                prominentSpeakerInfo.hrtfPosition.y = posY;
                prominentSpeakerInfo.hrtfPosition.z = posZ;
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
            else
            {
                return null;
            }
        }
    }
}