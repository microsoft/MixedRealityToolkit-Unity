using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private float lastRequest = 0;

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
        }

        private void Update()
        {
            if ((Time.time - lastRequest) > timeBetweenRequests)
            {
                var request = new HeadsetCalibrationDataRequest();
                request.timestamp = Time.time;
                var payload = request.Serialize();

                if(networkingService.SendData(payload, NetworkPriority.Critical))
                {
                    Debug.Log("Sent calibration request");
                }
                else
                {
                    Debug.LogWarning("Failed to send calibration request");
                }

                lastRequest = Time.time;
            }

            while(dataQueue.Count > 0)
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
    }
}
