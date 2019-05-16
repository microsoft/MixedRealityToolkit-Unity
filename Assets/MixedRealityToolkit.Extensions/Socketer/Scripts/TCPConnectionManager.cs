// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Helper class for setting up a TCP based network connection
    /// </summary>
    public class TCPConnectionManager : MonoBehaviour
    {
        /// <summary>
        /// Called when a client or server connection is established and the connection manager is using the TCP protocol.
        /// </summary>
        public event Action<SocketEndpoint> OnConnected;
        /// <summary>
        /// Called when a client or server connection is disconnected and the connection manager is using the TCP protocol.
        /// </summary>
        public event Action<SocketEndpoint> OnDisconnected;
        /// <summary>
        /// Called when a data payload is received
        /// </summary>
        public event Action<IncomingMessage> OnReceive;

        private readonly TimeSpan timeoutInterval = TimeSpan.Zero;
        private readonly ConcurrentQueue<SocketEndpoint> newConnections = new ConcurrentQueue<SocketEndpoint>();
        private readonly ConcurrentQueue<SocketEndpoint> oldConnections = new ConcurrentQueue<SocketEndpoint>();
        private readonly ConcurrentDictionary<int, SocketEndpoint> serverConnections = new ConcurrentDictionary<int, SocketEndpoint>();
        private readonly ConcurrentQueue<IncomingMessage> inputMessageQueue = new ConcurrentQueue<IncomingMessage>();
        private SocketEndpoint clientConnection;
        private SocketerClient client;

        private SocketerClient server;

        /// <summary>
        /// Returns true if any server or client connections exist, otherwise false
        /// </summary>
        public bool HasConnections => (serverConnections.Count > 0 || clientConnection != null);

        /// <summary>
        /// Returns true if a connection is being attempted, otherwise false
        /// </summary>
        public bool IsConnecting => client != null;

        /// <summary>
        /// Returns the number of bytes currently queued for the socketer server
        /// </summary>
        public int OutputBytesQueued => SocketerClient.OutputQueueLength;

        /// <summary>
        /// Call to begin acting as a server listening on the provided port
        /// </summary>
        /// <param name="port">port to listen on</param>
        public void StartListening(int port)
        {
            if (server == null)
            {
                server = DoStartListening(port);
            }
        }

        private SocketerClient DoStartListening(int port)
        {
            Debug.Log("Listening on port " + port);
            SocketerClient newServer = SocketerClient.CreateListener(SocketerClient.Protocol.TCP, port);
            newServer.Connected += OnServerConnected;
            newServer.Disconnected += OnServerDisconnected;
            newServer.Start();
            return newServer;
        }

        /// <summary>
        /// Call to stop acting as a server
        /// </summary>
        public void StopListening()
        {
            DoStopListening(ref server);
        }

        protected void DoStopListening(ref SocketerClient listener)
        {
            if (listener != null)
            {
                Debug.Log("Stopped listening on port " + listener.Port);
                listener.Stop();
                listener = null;
            }
        }

        /// <summary>
        /// Call to start acting as a client connected to the provided server and port
        /// </summary>
        /// <param name="serverAddress">server to connect to</param>
        /// <param name="port">port to use for communication</param>
        public void ConnectTo(string serverAddress, int port)
        {
            Debug.LogFormat($"Connecting to {serverAddress}:{port}");
            client = SocketerClient.CreateSender(SocketerClient.Protocol.TCP, serverAddress, port);
            client.Connected += OnClientConnected;
            client.Disconnected += OnClientDisconnected;
            client.Start();
        }

        private void OnServerConnected(SocketerClient client, int sourceId, string clientAddress)
        {
            Debug.Log("Server connected to " + clientAddress);
            SocketEndpoint socketEndpoint = new SocketEndpoint(client, timeoutInterval, clientAddress, sourceId);
            serverConnections[sourceId] = socketEndpoint;
            socketEndpoint.QueueIncomingMessages(inputMessageQueue);
            newConnections.Enqueue(socketEndpoint);
        }

        protected virtual void OnServerDisconnected(SocketerClient client, int sourceId, string clientAddress)
        {
            SocketEndpoint socketEndpoint;
            if (serverConnections.TryRemove(sourceId, out socketEndpoint))
            {
                Debug.Log("Server disconnected from " + clientAddress);
                socketEndpoint.StopIncomingMessageQueue();
                oldConnections.Enqueue(socketEndpoint);
            }
        }

        private void OnClientConnected(SocketerClient client, int sourceId, string hostAddress)
        {
            Debug.Log("Client connected to " + hostAddress);
            SocketEndpoint socketEndpoint = new SocketEndpoint(client, timeoutInterval, hostAddress, sourceId);
            clientConnection = socketEndpoint;
            socketEndpoint.QueueIncomingMessages(inputMessageQueue);
            newConnections.Enqueue(socketEndpoint);
        }

        private void OnClientDisconnected(SocketerClient client, int sourceId, string hostAddress)
        {
            if (clientConnection != null)
            {
                Debug.Log("Client disconnected");
                clientConnection.StopIncomingMessageQueue();
                oldConnections.Enqueue(clientConnection);
                clientConnection = null;
            }
        }

        private void Update()
        {
            DateTime utcNow = DateTime.UtcNow;
            if (clientConnection != null)
            {
                clientConnection.CheckConnectionTimeout(utcNow);
            }

            foreach (SocketEndpoint endpoint in serverConnections.Values)
            {
                endpoint.CheckConnectionTimeout(utcNow);
            }

            SocketEndpoint connection;
            while (newConnections.TryDequeue(out connection))
            {
                OnConnected?.Invoke(connection);
            }

            while (oldConnections.TryDequeue(out connection))
            {
                OnDisconnected?.Invoke(connection);
            }

            while (inputMessageQueue.TryDequeue(out IncomingMessage resultPack))
            {
                if (resultPack == null)
                    break;

                OnReceive?.Invoke(resultPack);
            }
        }

        /// <summary>
        /// Call to broadcast the provided data to all connected clients/servers
        /// </summary>
        /// <param name="data">data to send</param>
        public void Broadcast(byte[] data)
        {
            foreach (SocketEndpoint endpoint in serverConnections.Values)
            {
                endpoint.Send(data);
            }

            if (clientConnection != null)
            {
                clientConnection.Send(data);
            }
        }

        /// <summary>
        /// Disconnect all connections
        /// </summary>
        public void DisconnectAll()
        {
            // Make sure the client stops before attempting to disconnect
            // anything else. Otherwise, a race condition could cause the client
            // to automatically reconnect to the disconnected endpoints.
            if (client != null)
            {
                client.Stop();
                client = null;
            }

            if (clientConnection != null)
            {
                clientConnection.Disconnect();
                clientConnection = null;
            }

            foreach (SocketEndpoint endpoint in serverConnections.Values)
            {
                endpoint.Disconnect();
            }
            serverConnections.Clear();
        }
    }
}