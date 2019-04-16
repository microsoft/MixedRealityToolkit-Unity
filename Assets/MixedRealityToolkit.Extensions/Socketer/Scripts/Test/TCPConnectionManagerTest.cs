// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Helper class for testing socketer <see cref="TCPConnectionManager"/>
    /// </summary>
    public class TCPConnectionManagerTest : MonoBehaviour
    {
        /// <summary>
        /// Check to run as the server
        /// </summary>
        [Tooltip("Check to run as the server")]
        [SerializeField]
        protected bool runAsServer = false;

        /// <summary>
        /// IP address of the server
        /// </summary>
        [Tooltip("IP address of the server")]
        [SerializeField]
        private string serverAddress = "127.0.0.1";

        /// <summary>
        /// Port for communicating with the server
        /// </summary>
        [Tooltip("Port for communicating with the server")]
        [SerializeField]
        private int serverPort = 7777;

        /// <summary>
        /// Time between broadcasts
        /// </summary>
        [Tooltip("Time between broadcasts")]
        [SerializeField]
        protected float timeBetweenBroadcasts = 1.0f;

        /// <summary>
        /// TCPConnectionManager to use for networking
        /// </summary>
        [Tooltip("TCPConnectionManager to use for networking")]
        [SerializeField]
        protected TCPConnectionManager connectionManager;

        private float lastBroadcast = 0.0f;
        private bool broadcastSent = false;
        private bool broadcastReceived = false;

        private void OnValidate()
        {
#if UNITY_EDITOR
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
#endif
        }

        private void Start()
        {
            connectionManager.OnConnected += OnNetConnected;
            connectionManager.OnDisconnected += OnNetDisconnected;
            connectionManager.OnReceive += OnNetReceived;

            if (runAsServer)
            {
                connectionManager.StartListening(serverPort);
            }
            else
            {
                connectionManager.ConnectTo(serverAddress, serverPort);
            }
        }

        private void Update()
        {
            if (!connectionManager.HasConnections)
            {
                return;
            }

            if (broadcastSent &&
                broadcastReceived)
            {
                Debug.Log("Broadcasts sent and received, attempting to disconnect");
                connectionManager.DisconnectAll();
                Debug.Log("TCPConnectionManager has disconnected");
            }
            else if ((Time.time - lastBroadcast) > timeBetweenBroadcasts)
            {
                var message = runAsServer ? "Message from server" : "Message from client";
                connectionManager.Broadcast(Encoding.ASCII.GetBytes(message));
                broadcastSent = true;

                lastBroadcast = Time.time;
            }
        }

        private void OnDestroy()
        {
            connectionManager.DisconnectAll();
        }

        private void OnNetConnected(SocketEndpoint obj)
        {
            Debug.Log($"TCPConnectionManager Connected:{obj.ToString()}");
        }

        private void OnNetDisconnected(SocketEndpoint obj)
        {
            Debug.Log($"TCPConnectionManager Disconnected:{obj.ToString()}");
        }

        private void OnNetReceived(IncomingMessage obj)
        {
            Debug.Log($"TCPConnectionManager Received:{obj.ToString()}");
            broadcastReceived = true;
        }
    }
}