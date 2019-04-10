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
    /// Helper class for testing socketer <see cref="NetworkConnectionManager"/>
    /// </summary>
    public class NetworkConnectionManagerTest : MonoBehaviour
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

        private NetworkConnectionManager net;
        private float lastBroadcast = 0.0f;

        private void OnValidate()
        {
#if UNITY_EDITOR
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
            PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
#endif
        }

        private void Start()
        {
            net = new NetworkConnectionManager(TimeSpan.Zero);
            net.OnConnected += OnNetConnected;
            net.OnDisconnected += OnNetDisconnected;
            net.OnReceive += OnNetReceived;

            if (runAsServer)
            {
                net.StartListening(serverPort);
            }
            else
            {
                net.ConnectTo(serverAddress, serverPort);
            }
        }

        private void Update()
        {
            if ((Time.time - lastBroadcast) > timeBetweenBroadcasts)
            {
                if (net.HasConnections)
                {
                    var message = runAsServer ? "Message from server" : "Message from client";
                    net.Broadcast(Encoding.ASCII.GetBytes(message));
                }
                else
                {
                    Debug.Log("Message not broadcasted, connection not yet established");
                }


                lastBroadcast = Time.time;
            }

            net.Update();
        }

        private void OnDestroy()
        {
            net.DisconnectAll();
        }

        private void OnNetConnected(SocketEndpoint obj)
        {
            Debug.Log($"NetworkConnectionManager Connected:{obj.ToString()}");
        }

        private void OnNetDisconnected(SocketEndpoint obj)
        {
            Debug.Log($"NetworkConnectionManager Disconnected:{obj.ToString()}");
        }

        private void OnNetReceived(IncomingMessage obj)
        {
            Debug.Log($"NetworkConnectionManager Received:{obj.ToString()}");
        }
    }
}