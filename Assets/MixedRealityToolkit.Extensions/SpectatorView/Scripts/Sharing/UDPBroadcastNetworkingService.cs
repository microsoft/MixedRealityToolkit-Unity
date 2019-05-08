// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !NETFX_CORE
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Sharing
{
    /// <summary>
    /// Delegate used to specify server and client ports to the <see cref="UDPBroadcastNetworkingService"/>
    /// </summary>
    /// <param name="serverPort"></param>
    /// <param name="clientPort"></param>
    public delegate void UDPBroadcastConnectHandler(int serverPort, int clientPort);

    /// <summary>
    /// Interface implemented by a UI component to set server and client ports
    /// </summary>
    public interface IUDPBroadcastNetworkingServiceVisual
    {
        /// <summary>
        /// Event called when new server and client ports have been specified
        /// </summary>
        event UDPBroadcastConnectHandler OnConnect;

        /// <summary>
        /// Called to show the visual
        /// </summary>
        void ShowVisual();

        /// <summary>
        /// Called to hide the visual
        /// </summary>
        void HideVisual();
    }

    /// <summary>
    /// UDP based component that implements
    /// <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IMatchMakingService"/>,
    /// <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.IPlayerService"/>
    /// and <see cref="Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing.INetworkingService"/>
    /// </summary>
    public class UDPBroadcastNetworkingService : MonoBehaviour,
        IMatchMakingService,
        IPlayerService,
        INetworkingService
    {
        #region Helper classes
        private class PlayerData
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

        private class Message
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

        /// <summary>
        /// If true, this service will show a visual (if defined) to obtain server and client ports.
        /// If false, no visual is shown and the service will use the default server and client ports.
        /// </summary>
        [Tooltip("If true, the UDPBroadcastNetworkingService will show a visual (if defined) to obtain server and client ports. If false, the UDPBroadcastNetworking service will use the default ports defined in the editor.")]
        [SerializeField]
        protected bool _useUdpBroadcastVisual;

        /// <summary>
        /// MonoBehaviour that implements  <see cref="IUDPBroadcastNetworkingServiceVisual"/> for the HoloLens experience.
        /// Note: an error is thrown if a MonoBehaviour is specified that doesn't implement  <see cref="IUDPBroadcastNetworkingServiceVisual"/>.
        /// </summary>
        [Tooltip("MonoBehaviour that implements IUDPBroadcastNetworkingServiceVisual for the HoloLens experience. Note: an error is thrown if a MonoBehaviour is specified that doesn't implement IUDPBroadcastNetworkingServiceVisual.")]
        [SerializeField]
        protected MonoBehaviour HoloLensUdpBroadcastVisual;

        /// <summary>
        /// MonoBehaviour that implements <see cref="IUDPBroadcastNetworkingServiceVisual"/> for the Mobile experience.
        /// Note: an error is thrown if a MonoBehaviour is specified that doesn't implement  <see cref="IUDPBroadcastNetworkingServiceVisual"/>.
        /// </summary>
        [Tooltip("MonoBehaviour that implements IUDPBroadcastNetworkingServiceVisual for the Mobile experience. Note: an error is thrown if a MonoBehaviour is specified that doesn't implement IUDPBroadcastNetworkingServiceVisual.")]
        [SerializeField]
        protected MonoBehaviour MobileUdpBroadcastVisual;

        protected IUDPBroadcastNetworkingServiceVisual _udpBroadcastVisual;

        /// <summary>
        /// Default server port value used if <see cref="UDPBroadcastNetworkingService._useUdpBroadcastVisual"/> is set to false
        /// </summary>
        [Tooltip("Default server port value used if UseUdpBroadcastVisual is set to false")]
        [SerializeField]
        protected int _serverBroadcastPort = 48888;

        /// <summary>
        /// Default client port value used if <see cref="UDPBroadcastNetworkingService._useUdpBroadcastVisual"/> is set to false
        /// </summary>
        [Tooltip("Default client port value used if UseUdpBroadcastVisual is set to false")]
        [SerializeField]
        protected int _clientBroadcastPort = 48889;

        /// <summary>
        /// Time inbetween attempted UDP Broadcasts in seconds
        /// </summary>
        [Tooltip("Time inbetween attempted UDP Broadcasts in seconds")]
        [SerializeField]
        float _broadcastInterval = 0.25f;

        /// <summary>
        /// Timeout in seconds for other players in the shared application. If no broadcast is heard for a player within this timeout, the player is considered disconnected.
        /// </summary>
        [Tooltip("Timeout in seconds for other players in the shared application. If no broadcast is heard for a player within this timeout, the player is considered disconnected.")]
        [SerializeField]
        float _disconnectTimeout = 120.0f;

        protected bool _actAsServer = false;
        protected bool _connected = false;

        private UdpClient _senderUdp;
        private List<IPEndPoint> _broadcastIpEndPoints;
        private UdpClient _receiverUdp;
        private int _receiverPort;
        private Dictionary<IPEndPoint, PlayerData> _receiverIpEndPoints;

        private Message _currentMessage = null;
        private float _prevBroadcastTime = 0;

        protected void OnValidate()
        {
#if UNITY_EDITOR
            FieldHelper.ValidateType<IUDPBroadcastNetworkingServiceVisual>(HoloLensUdpBroadcastVisual);
            FieldHelper.ValidateType<IUDPBroadcastNetworkingServiceVisual>(MobileUdpBroadcastVisual);
#endif
        }

        protected void Awake()
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

        protected void Update()
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

        /// <inheritdoc/>
        public event DataHandler DataReceived;

        /// <inheritdoc/>
        public event PlayerConnectedHandler PlayerConnected;

        /// <inheritdoc/>
        public event PlayerDisconnectedHandler PlayerDisconnected;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool IsConnected()
        {
            return _connected;
        }

        /// <inheritdoc/>
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

        private void BroadcastData()
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

        private List<IPEndPoint> GetBroadcastIPEndPoints(int port)
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

        private IPAddress GetBroadcastAddress(IPAddress ipAddress)
        {
            uint orig = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
            IPAddress subnetMaskIp = GetSubnetMask(ipAddress);
            uint mask = BitConverter.ToUInt32(subnetMaskIp.GetAddressBytes(), 0);
            uint broadcast = orig | ~mask;
            return new IPAddress(BitConverter.GetBytes(broadcast));
        }

        private IPAddress GetSubnetMask(IPAddress ipAddress)
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
#endif