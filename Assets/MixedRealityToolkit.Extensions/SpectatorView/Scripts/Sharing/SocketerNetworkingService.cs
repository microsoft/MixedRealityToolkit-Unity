// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Networking class based on Socketer's <see cref="Experimental.Socketer.NetworkConnectionManager"/>
    /// </summary>
    public class SocketerNetworkingService : MonoBehaviour,
        IMatchMakingService,
        IPlayerService,
        INetworkingService
    {
        /// <summary>
        /// TCPConnectionManager used for communicating with other devices
        /// </summary>
        [Tooltip("TCPConnectionManager used for communicating with other devices")]
        [SerializeField]
        protected TCPConnectionManager net = null;

        /// <summary>
        /// IPAddress for the user (HoloLens & HoloLens 2)
        /// </summary>
        [Tooltip("IPAddress for the user (HoloLens & HoloLens 2)")]
        [SerializeField]
        protected string userAddress = "127.0.0.1";

        /// <summary>
        /// Port for the user (HoloLens & HoloLens 2)
        /// </summary>
        [Tooltip("Port for the user (HoloLens & HoloLens 2)")]
        [SerializeField]
        protected int userPort = 7997;

        /// <summary>
        /// Timeout interval for establishing connections
        /// </summary>
        [Tooltip("Timeout interval for establishing connections")]
        [SerializeField]
        protected TimeSpan timeoutInterval = TimeSpan.Zero;

        /// <inheritdoc />
        public event PlayerConnectedHandler PlayerConnected;

        /// <inheritdoc />
        public event PlayerDisconnectedHandler PlayerDisconnected;

        /// <inheritdoc />
        public event DataHandler DataReceived;

        private bool runAsUser = false;

        /// <summary>
        /// This dictionary maps ip addresses to unique player ids
        /// </summary>
        private Dictionary<string, string> playerIds = new Dictionary<string, string>();

        private void Start()
        {
#if UNITY_WSA && !UNITY_EDITOR
            runAsUser = true;
#elif UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            runAsUser = false;
#endif
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (net == null)
            {
                Debug.LogError("TCPConnectionManager not set for SocketNetworkingService. Unable to establish connection");
                return;
            }
            else
            {
                net.DisconnectAll();
            }

            net.OnConnected += OnConnected;
            net.OnDisconnected += OnDisconnected;
            net.OnReceive += OnReceive;

            if (runAsUser)
            {
                net.StartListening(userPort);
            }
            else
            {
                net.ConnectTo(userAddress, userPort);
            }
        }

        private void OnConnected(SocketEndpoint obj)
        {
            string playerId = string.Empty;
            lock(playerIds)
            {
                if (!playerIds.ContainsKey(obj.Address))
                {
                    playerIds.Add(obj.Address, Guid.NewGuid().ToString());
                }

                playerId = playerIds[obj.Address];
            }

            if (playerId != string.Empty)
            {
                PlayerConnected?.Invoke(playerId);
            }
        }

        private void OnDisconnected(SocketEndpoint obj)
        {
            string playerId = string.Empty;
            lock (playerIds)
            {
                if (playerIds.ContainsKey(obj.Address))
                {
                    playerId = playerIds[obj.Address];
                }
            }

            if (playerId != string.Empty)
            {
                PlayerDisconnected?.Invoke(playerId);
            }
        }

        private void OnReceive(IncomingMessage obj)
        {
            string playerId = string.Empty;

            lock(playerIds)
            {
                if (playerIds.ContainsKey(obj.Endpoint.Address))
                {
                    playerId = playerIds[obj.Endpoint.Address];
                }
            }

            if (playerId != string.Empty)
            {
                DataReceived?.Invoke(playerId, obj.Data);
            }
            else
            {
                Debug.LogWarning("Payload received from an unknown player");
            }
        }

        /// <inheritdoc />
        public bool Disconnect()
        {
            if (net != null)
            {
                net.DisconnectAll();
                net.OnConnected -= OnConnected;
                net.OnDisconnected -= OnDisconnected;
                net.OnReceive -= OnReceive;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool IsConnected()
        {
            return (net != null) && (net.HasConnections);
        }

        /// <inheritdoc />
        public bool SendData(byte[] data, NetworkPriority priority = NetworkPriority.Default)
        {
            if (net != null &&
                net.HasConnections)
            {
                net.Broadcast(data);
                return true;
            }
            else
            {
                Debug.LogWarning("Payload not sent, no connection established");
                return false;
            }
        }
    }
}

