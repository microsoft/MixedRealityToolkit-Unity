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
    public class DeviceInfoBroadcaster : MonoBehaviour
    {
        [SerializeField]
        private TCPConnectionManager connectionManager = null;

#if UNITY_WSA
        private void Awake()
        {
            connectionManager.OnConnected += TcpConnectionManager_OnConnected;
        }

        private void OnDestroy()
        {
            connectionManager.OnConnected -= TcpConnectionManager_OnConnected;
        }

        private void TcpConnectionManager_OnConnected(SocketEndpoint obj)
        {
            SendDeviceInfo();
        }

        private void SendDeviceInfo()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write(DeviceInfoObserver.DeviceInfoCommand);
                message.Write(GetMachineName());
                message.Write(GetIPAddress());

                connectionManager.Broadcast(memoryStream.ToArray());
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