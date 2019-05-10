using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Storage;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.HolographicCamera
{
    /// <summary>
    /// Loads calibration data from the Pictures library on the device and transfers that data
    /// to the compositor upon connection.
    /// </summary>
    [RequireComponent(typeof(TCPConnectionManager))]
    public class CalibrationDataProvider : MonoBehaviour
    {
        private TCPConnectionManager tcpConnectionManager;

        private void Awake()
        {
            tcpConnectionManager = GetComponent<TCPConnectionManager>();
            tcpConnectionManager.OnConnected += TcpConnectionManager_OnConnected;
        }

        private void OnDestroy()
        {
            tcpConnectionManager.OnConnected -= TcpConnectionManager_OnConnected;
        }

        private void TcpConnectionManager_OnConnected(SocketEndpoint obj)
        {
            SendCalibrationDataAsync();
        }

#if !UNITY_EDITOR && UNITY_WSA
        private async void SendCalibrationDataAsync()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                StorageFile file = (await KnownFolders.PicturesLibrary.TryGetItemAsync(@"CalibrationData.json").AsTask()) as StorageFile;
                if (file != null)
                {
                    string contents = await FileIO.ReadTextAsync(file);
                    message.Write("CalibrationData");
                    message.Write(contents);
                    tcpConnectionManager.Broadcast(memoryStream.ToArray());
                }
            }
        }
#else
        private void SendCalibrationDataAsync()
        {
        }
#endif
    }
}