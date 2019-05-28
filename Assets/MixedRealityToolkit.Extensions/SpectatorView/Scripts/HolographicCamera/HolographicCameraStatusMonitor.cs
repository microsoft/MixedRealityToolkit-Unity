// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_WSA
using UnityEngine.XR.WSA;

#if !UNITY_EDITOR
using Windows.Networking;
using Windows.Networking.Connectivity;
#endif
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.HolographicCamera
{
    [RequireComponent(typeof(TCPConnectionManager))]
    [RequireComponent(typeof(HolographicCameraOriginAnchor))]
    public class HolographicCameraStatusMonitor : MonoBehaviour
    {
#if UNITY_WSA
        TCPConnectionManager tcpConnectionManager = null;
        HolographicCameraOriginAnchor originAnchor = null;

        private byte[] previousStatusMessage = null;

        private void Awake()
        {
            tcpConnectionManager = GetComponent<TCPConnectionManager>();
            tcpConnectionManager.OnConnected += TcpConnectionManager_OnConnected;
            originAnchor = GetComponent<HolographicCameraOriginAnchor>();
        }

        private void OnDestroy()
        {
            tcpConnectionManager.OnConnected -= TcpConnectionManager_OnConnected;
        }

        private void TcpConnectionManager_OnConnected(SocketEndpoint obj)
        {
            // Reset the cached status message each time we reconnect
            // so that new state is sent out to the new connection.
            previousStatusMessage = null;

            SendDeviceInfo();
        }

        private void Update()
        {
            if (!tcpConnectionManager.HasConnections)
            {
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write(StateSynchronizationObserver.StatusCommand);
                message.Write(WorldManager.state == PositionalLocatorState.Active);
                message.Write(originAnchor.IsAnchorLocated);
                message.Write(originAnchor.IsDetectingMarker);

                byte[] newMessage = memoryStream.ToArray();
                if (previousStatusMessage == null || !newMessage.SequenceEqual<byte>(previousStatusMessage))
                {
                    tcpConnectionManager.Broadcast(newMessage);
                    previousStatusMessage = newMessage;
                }
            }
        }

        private void SendDeviceInfo()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write(StateSynchronizationObserver.DeviceInfoCommand);
                message.Write(GetMachineName());
                message.Write(GetIPAddress());

                tcpConnectionManager.Broadcast(memoryStream.ToArray());
            }
        }

        private string GetMachineName()
        {
#if UNITY_EDITOR
            return System.Environment.MachineName;
#else
            HostName localName = NetworkInformation.GetHostNames().FirstOrDefault(n => n.Type == HostNameType.DomainName);
            if (localName == null)
            {
                return "UnknownDevice";
            }
            else
            {
                 return localName.RawName;
            }
#endif
        }

        private string GetIPAddress()
        {
            string ipAddress = "Unknown IP";

#if !UNITY_EDITOR && WINDOWS_UWP
            var icp = NetworkInformation.GetInternetConnectionProfile();
            if (icp != null && icp.NetworkAdapter != null)
            {
                HostName localName = NetworkInformation.GetHostNames().FirstOrDefault(n =>
                            n.Type == HostNameType.Ipv4 &&
                            n.IPInformation != null &&
                            n.IPInformation.NetworkAdapter != null &&
                            n.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);
                if (localName != null)
                    ipAddress = localName.ToString();
            }
#endif
            return ipAddress;
        }
#endif
    }
}