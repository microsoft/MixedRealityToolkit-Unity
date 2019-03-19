// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces;
using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Networking
{
    public delegate void UDPBroadcastConnectHandler(int serverPort, int clientPort);

    public interface IUDPBroadcastNetworkingServiceVisual
    {
        event UDPBroadcastConnectHandler OnConnect;
        void ShowVisual();
        void HideVisual();
    }

    public class UDPBroadcastNetworkingService : MonoBehaviour,
        IMatchMakingService,
        IPlayerService,
        INetworkingService
    {
        #region Helper classes
        class PlayerData
        {
            public string Id { get; set; }
            public float LastMessageTimestamp { get; set; }

            public PlayerData()
            {
                Id = Guid.NewGuid().ToString();
                LastMessageTimestamp = Time.time;
            }

            public PlayerData(string id, float lastMessageTimestamp)
            {
                Id = id;
                LastMessageTimestamp = LastMessageTimestamp;
            }
        }

        class Message
        {
            public byte[] Data { get; internal set; }
            public NetworkPriority Priority { get; internal set; }
            public bool Sent { get; set; }
            public Message(byte[] data, NetworkPriority priority)
            {
                Data = data;
                Priority = priority;
                Sent = false;
            }
        }
        #endregion

        [SerializeField] bool _useUdpBroadcastVisual;
        [SerializeField] MonoBehaviour HoloLensUdpBroadcastVisual;
        [SerializeField] MonoBehaviour MobileUdpBroadcastVisual;
        IUDPBroadcastNetworkingServiceVisual _udpBroadcastVisual;

        [SerializeField] int _serverBroadcastPort = 48888;
        [SerializeField] int _clientBroadcastPort = 48889;
        [SerializeField] float _broadcastInterval = 0.25f;
        [SerializeField] float _disconnectTimeout = 120.0f;

        bool _actAsServer = false;
        bool _connected = false;

        UdpClient _senderUdp;
        List<IPEndPoint> _broadcastIpEndPoints;
        UdpClient _receiverUdp;
        int _receiverPort;
        Dictionary<IPEndPoint, PlayerData> _receiverIpEndPoints;

        Message _currentMessage = null;
        float _prevBroadcastTime = 0;

        void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<IUDPBroadcastNetworkingServiceVisual>(HoloLensUdpBroadcastVisual);
            FieldHelper.ValidateType<IUDPBroadcastNetworkingServiceVisual>(MobileUdpBroadcastVisual);
#endif
        }

        void Awake()
        {
            // TODO - update here if future scenario requires device other than hololens to act as server
#if UNITY_WSA
            _actAsServer = true;
            _udpBroadcastVisual = HoloLensUdpBroadcastVisual as IUDPBroadcastNetworkingServiceVisual;
#elif UNITY_ANDROID || UNITY_IOS
            _actAsServer = false;
            _udpBroadcastVisual = MobileUdpBroadcastVisual as IUDPBroadcastNetworkingServiceVisual;
#endif

            if (_useUdpBroadcastVisual)
            {
                if (_udpBroadcastVisual == null)
                {
                    Debug.LogError("Error: UdpBroadcastVisual not specified, network connection attempts will fail");
                }

                _udpBroadcastVisual.OnConnect += OnVisualConnect;
            }
        }

        void Update()
        {
            var diff = Time.time - _prevBroadcastTime;
            if (_senderUdp != null &&
                _currentMessage != null &&
                diff > _broadcastInterval)
            {
                BroadcastData();
                _prevBroadcastTime = Time.time;
            }

            if (_receiverIpEndPoints != null)
            {
                if (_receiverUdp != null &&
                    _receiverUdp.Available > 0)
                {
                    IPEndPoint endPoint = null;
                    var rawData = _receiverUdp.Receive(ref endPoint);

                    PlayerData playerData = null;
                    if (!_receiverIpEndPoints.TryGetValue(endPoint, out playerData))
                    {
                        playerData = new PlayerData();
                        _receiverIpEndPoints.Add(endPoint, playerData);
                        Debug.Log("Received data from new player: " + playerData.ToString());
                        PlayerConnected?.Invoke(playerData.Id);
                    }

                    _receiverIpEndPoints[endPoint].LastMessageTimestamp = Time.time;
                    DataReceived?.Invoke(playerData.Id, rawData);
                }

                List<IPEndPoint> itemsToRemove = new List<IPEndPoint>();
                foreach (var playerPair in _receiverIpEndPoints)
                {
                    if ((Time.time - playerPair.Value.LastMessageTimestamp) > _disconnectTimeout)
                    {
                        Debug.Log("Player (" + playerPair.Value.Id + ") timed out. Timeout set at " + _disconnectTimeout + " seconds");
                        PlayerDisconnected?.Invoke(playerPair.Value.Id);
                        itemsToRemove.Add(playerPair.Key);
                    }
                }

                foreach (var ipEndpoint in itemsToRemove)
                {
                    _receiverIpEndPoints.Remove(ipEndpoint);
                }
            }
        }

        public event DataHandler DataReceived;
        public event PlayerConnectedHandler PlayerConnected;
        public event PlayerDisconnectedHandler PlayerDisconnected;

        public void Connect()
        {
            if (_useUdpBroadcastVisual)
            {
                if (_udpBroadcastVisual == null)
                {
                    Debug.LogError("Error: UdpBroadcastVisual not specified, unable to setup network connection");
                    return;
                }

                _udpBroadcastVisual.ShowVisual();
            }
            else
            {
                ConnectImpl();
            }
        }

        private void OnVisualConnect(int serverPort, int clientPort)
        {
            if (_useUdpBroadcastVisual)
            {
                _serverBroadcastPort = serverPort;
                _clientBroadcastPort = clientPort;
                ConnectImpl();

                if (_connected &&
                    _udpBroadcastVisual != null)
                {
                    _udpBroadcastVisual.HideVisual();
                }
            }
        }

        private void ConnectImpl()
        {
            Debug.Log("Connecting. Acting as server: " + _actAsServer.ToString());

            _senderUdp = new UdpClient();
            _senderUdp.EnableBroadcast = true;
            var senderPort = _actAsServer ?
                _serverBroadcastPort :
                _clientBroadcastPort;
            _broadcastIpEndPoints = GetBroadcastIPEndPoints(senderPort);
            Debug.Log("Broadcasting messages on port: " + senderPort);

            _receiverPort = _actAsServer ?
                _clientBroadcastPort :
                _serverBroadcastPort;
            _receiverUdp = new UdpClient(_receiverPort);
            Debug.Log("Receiving messages on port: " + _receiverPort);

            _receiverIpEndPoints = new Dictionary<IPEndPoint, PlayerData>();

            _connected = true;
        }

        public bool Disconnect()
        {
            if (!_connected)
                return false;

            if (_senderUdp != null)
                _senderUdp.Close();
            _senderUdp = null;

            if (_receiverUdp != null)
                _receiverUdp.Close();
            _receiverUdp = null;

            return true;
        }

        public bool IsConnected()
        {
            return _connected;
        }

        public bool SendData(byte[] data, NetworkPriority priority)
        {
            if (!_connected)
            {
                return false;
            }

            if (_currentMessage != null &&
                _currentMessage.Priority == NetworkPriority.Critical &&
                !_currentMessage.Sent)
            {
                Debug.Log("Critical priority message not yet sent, unable to send new payload");
                return false;
            }

            _currentMessage = new Message(data, priority);
            return true;
        }

        void BroadcastData()
        {
            if (_broadcastIpEndPoints != null &&
                _connected &&
                _currentMessage != null &&
                _currentMessage.Data != null)
            {
                foreach (var endpoint in _broadcastIpEndPoints)
                {
                    try
                    {
                        if (_currentMessage.Data.Length != _senderUdp.Send(_currentMessage.Data, _currentMessage.Data.Length, endpoint))
                        {
                            Debug.LogError("Failed to send payload (" + endpoint.Address.ToString() + ", " + endpoint.Port + "): " + _currentMessage.Data.Length + " Bytes");
                        }
                        else
                        {
                            _currentMessage.Sent = true;
                        }
                    }
                    catch
                    {
                        Debug.LogError("Exception thrown sending payload (" + endpoint.Address.ToString() + ", " + endpoint.Port + "): " + _currentMessage.Data.Length + " Bytes");
                    }
                }
            }
        }

        List<IPEndPoint> GetBroadcastIPEndPoints(int port)
        {
            List<IPEndPoint> result = new List<IPEndPoint>();
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    // Convert ip address to broadcast ip address
                    var broadcastIp = GetBroadcastAddress(ip);
                    result.Add(new IPEndPoint(broadcastIp, port));
                    Debug.Log("Found broadcast ip end point: " + broadcastIp.ToString() + " port: " + port);
                }
            }

            // This end point appears needed for an android device to hear broadcasts on its own hotspot network
            result.Add(new IPEndPoint(IPAddress.Broadcast, port));

            return result;
        }

        IPAddress GetBroadcastAddress(IPAddress ipAddress)
        {
            uint orig = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
            IPAddress subnetMaskIp = GetSubnetMask(ipAddress);
            uint mask = BitConverter.ToUInt32(subnetMaskIp.GetAddressBytes(), 0);
            uint broadcast = orig | ~mask;
            return new IPAddress(BitConverter.GetBytes(broadcast));
        }

        IPAddress GetSubnetMask(IPAddress ipAddress)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation info in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (info.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ipAddress.Equals(info.Address))
                        {
                            Debug.Log("Found subnet mask: " + info.IPv4Mask.ToString() + " for ip address: " + ipAddress.ToString());
                            return info.IPv4Mask;
                        }
                    }
                }
            }

            Debug.LogError("Unable to find subnet mask.");
            return IPAddress.Broadcast;
        }
    }
}
