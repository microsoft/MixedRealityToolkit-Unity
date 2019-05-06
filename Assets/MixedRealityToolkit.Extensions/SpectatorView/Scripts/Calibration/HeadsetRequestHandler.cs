// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class HeadsetRequestHandler : MonoBehaviour
    {
        /// <summary>
        /// Used to obtain marker, headset and pv camera data.
        /// </summary>
        [Tooltip("Used to obtain marker, headset and pv camera data.")]
        [SerializeField]
        private HeadsetCalibration headsetCalibration;

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

        /// <summary>
        /// Used to display the last request timestamp.
        /// </summary>
        [Tooltip("Used to display the last request timestamp.")]
        [SerializeField]
        private Text lastRequestTimestampText;

        private bool updateData = false;
        private HeadsetCalibrationDataRequest request = null;

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<IMatchMakingService>(MatchMakingService);
            FieldHelper.ValidateType<INetworkingService>(NetworkingService);
        }
#endif

        private void Start()
        {
            networkingService = NetworkingService as INetworkingService;
            networkingService.DataReceived += OnDataReceived;

            matchMakingService = MatchMakingService as IMatchMakingService;
            matchMakingService.Connect();

            headsetCalibration.Updated += OnHeadsetCalibrationUpdated;
        }

        private void Update()
        {
            if (updateData)
            {
                updateData = false;

                if (headsetCalibration == null)
                {
                    Debug.LogWarning("HeadsetCalibration field is not set, unable to create headset calibration data payload");
                    return;
                }

                if (request != null &&
                    lastRequestTimestampText != null)
                {
                    lastRequestTimestampText.text = $"Last Request Timestamp: {request.timestamp}";
                }

                headsetCalibration.UpdateHeadsetCalibrationData();
            }
        }

        private void OnDataReceived(string playerId, byte[] payload)
        {
            if (HeadsetCalibrationDataRequest.TryDeserialize(payload, out request))
            {
                Debug.Log("Headset calibration data requested");
                updateData = true;
            }
            else
            {
                Debug.LogWarning("Received network payload that wasn't a headset calibration data request");
                request = null;
            }
        }

        private void OnHeadsetCalibrationUpdated(byte[] payload)
        {
            if (networkingService != null)
            {
                if (networkingService.SendData(payload, NetworkPriority.Critical))
                {
                    Debug.Log($"Headset calibration data sent:{payload.Length} bytes");
                }
                else
                {
                    Debug.LogWarning($"Failed to send payload:{payload.Length} bytes");
                }
            }
        }
    }
}
