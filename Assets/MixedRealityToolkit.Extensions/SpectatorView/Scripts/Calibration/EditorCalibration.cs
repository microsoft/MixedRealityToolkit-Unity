using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class EditorCalibration : MonoBehaviour
    {
        /// <summary>
        /// Time between headset calibration data requests (in seconds).
        /// </summary>
        [Tooltip("Time between headset calibration data requests (in seconds).")]
        [SerializeField]
        private float timeBetweenRequests = 5.0f;

        /// <summary>
        /// Raw Image used to display the last calibration image obtained provided by a headset.
        /// </summary>
        [Tooltip("Raw Image used to display the last calibration image obtained provided by a headset.")]
        [SerializeField]
        private RawImage lastCalibrationImage;

        /// <summary>
        /// Used to setup a network connection.
        /// </summary>
        [Tooltip("Used to setup a network connection.")]
        [SerializeField]
        private MonoBehaviour MatchMakingService;
        private IMatchMakingService matchMakingService;

        /// <summary>
        /// Used to send/receive data related to the calibration process.
        /// </summary>
        [Tooltip("Used to send/receive data related to the calibration process.")]
        [SerializeField]
        private MonoBehaviour NetworkingService;
        private INetworkingService networkingService;

        private Queue<HeadsetCalibrationData> dataQueue = new Queue<HeadsetCalibrationData>();
        private int nextImageId = 0;

        private void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<INetworkingService>(NetworkingService);
            FieldHelper.ValidateType<IMatchMakingService>(MatchMakingService);
#endif
        }

        private void Start()
        {
            networkingService = NetworkingService as INetworkingService;
            networkingService.DataReceived += OnDataReceived;

            matchMakingService = MatchMakingService as IMatchMakingService;
            matchMakingService.Connect();

            HeadsetCalibrationDataHelper.Initialize(out nextImageId);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var request = new HeadsetCalibrationDataRequest();
                request.timestamp = Time.time;
                var payload = request.Serialize();

                if (networkingService.SendData(payload, NetworkPriority.Critical))
                {
                    Debug.Log($"Sent calibration request {request.timestamp}");
                }
                else
                {
                    Debug.LogWarning("Failed to send calibration request");
                }
            }

            while (dataQueue.Count > 0)
            {
                var data = dataQueue.Dequeue();

                if (dataQueue.Count == 0)
                {
                    Texture2D texture = new Texture2D(
                        (int)data.imageData.resolution.Width,
                        (int) data.imageData.resolution.Height,
                        TextureFormat.BGRA32,
                        false);
                    texture.LoadRawTextureData(data.imageData.pixelData);
                    texture.Apply();

                    if (lastCalibrationImage != null)
                    {
                        lastCalibrationImage.texture = texture;
                        lastCalibrationImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
                    }

                    HeadsetCalibrationDataHelper.SavePVImage(texture, nextImageId);
                    HeadsetCalibrationDataHelper.SaveMetaInformation(data, nextImageId);
                    nextImageId++;
                }
            }
        }

        private void OnDataReceived(string playerId, byte[] payload)
        {
            Debug.Log($"Received payload of {payload.Length} bytes");
            HeadsetCalibrationData headsetCalibrationData;
            if (HeadsetCalibrationData.TryDeserialize(payload, out headsetCalibrationData))
            {
                dataQueue.Enqueue(headsetCalibrationData);
            }
        }

        private class HeadsetCalibrationDataHelper
        {
            private const string PVCamDirectoryName = "PV";
            private const string MetadataDirectoryName = "META";
            private const string RootDirectoryName = "Calibration";

            public static void Initialize(out int nextImageId)
            {
                nextImageId = 0;
#if UNITY_EDITOR
                string rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), RootDirectoryName);
                if (Directory.Exists(rootFolder))
                {
                    InitializeDirectory(rootFolder, PVCamDirectoryName, "*.png", out nextImageId);
                    InitializeDirectory(rootFolder, MetadataDirectoryName, "*.json", out nextImageId);
                }
                else
                {
                    Directory.CreateDirectory(rootFolder);
                    Directory.CreateDirectory(Path.Combine(rootFolder, PVCamDirectoryName));
                    Directory.CreateDirectory(Path.Combine(rootFolder, MetadataDirectoryName));
                }
#endif
            }

            private static void InitializeDirectory(string rootFolder, string directoryName, string fileExtensionExpected, out int nextImageId)
            {
                nextImageId = 0;
                string directory = Path.Combine(rootFolder, directoryName);
                if (Directory.Exists(directory))
                {
                    nextImageId = Math.Max(Directory.GetFiles(directory, fileExtensionExpected, SearchOption.TopDirectoryOnly).Length, nextImageId);
                }
                else
                {
                    Directory.CreateDirectory(directory);
                }
            }

            public static void SavePVImage(Texture2D image, int fileID)
            {
                SaveImage(PVCamDirectoryName, image, fileID);
            }

            private static void SaveImage(string directory, Texture2D image, int fileID)
            {
#if UNITY_EDITOR
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), RootDirectoryName, directory, $"{fileID}.png");
                File.WriteAllBytes(path, image.EncodeToPNG());
#endif
            }

            public static void SaveMetaInformation(HeadsetCalibrationData data, int fileID)
            {
#if UNITY_EDITOR
                byte[] temp = data.imageData.pixelData;
                data.imageData.pixelData = null;

                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), RootDirectoryName, MetadataDirectoryName, $"{fileID}.json");
                File.WriteAllBytes(path, data.Serialize());

                data.imageData.pixelData = temp;
#endif
            }
        }
    }
}
