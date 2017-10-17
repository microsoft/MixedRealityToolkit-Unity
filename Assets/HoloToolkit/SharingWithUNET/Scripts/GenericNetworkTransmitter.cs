// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// For a UWP application this should allow us to send or receive data given a server IP address.
    /// </summary>
    public class GenericNetworkTransmitter : Singleton<GenericNetworkTransmitter>
    {
        [Tooltip("The connection port on the machine to use.")]
        public int SendConnectionPort = 11000;

        /// <summary>
        /// When data arrives, this event is raised.
        /// </summary>
        /// <param name="data">The data that arrived.</param>
        public delegate void OnDataReady(byte[] data);
        public event OnDataReady DataReadyEvent;

        /// <summary>
        /// The server to connect to when data is needed.
        /// </summary>
        private string serverIp;

        /// <summary>
        /// Tracks if we have a connection request outstanding.
        /// </summary>
        private bool waitingForConnection = false;

        /// <summary>
        /// Keeps the most recent data buffer.
        /// </summary>
        private byte[] mostRecentDataBuffer;

#if !UNITY_EDITOR && UNITY_WSA
        /// <summary>
        /// Tracks the network connection to the remote machine we are sending meshes to.
        /// </summary>
        private StreamSocket networkConnection;

        /// <summary>
        /// If we are running as the server, this is the listener the server will use.
        /// </summary>
        private StreamSocketListener networkListener;

        /// <summary>
        /// If we cannot connect to the server, this is how long we will wait before retrying.
        /// </summary>
        private float timeToDeferFailedConnections = 10.0f;
#endif

        /// <summary>
        /// If someone connects to us, this is the data we will send them.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(byte[] data)
        {
            mostRecentDataBuffer = data;
        }

        /// <summary>
        /// Tells us who to contact if we need data.
        /// </summary>
        /// <param name="newServerIp"></param>
        public void SetServerIp(string newServerIp)
        {
            serverIp = newServerIp.Trim();
        }

        /// <summary>
        /// Requests data from the server and handles getting the data and firing
        /// the dataReadyEvent.
        /// </summary>
        public bool RequestAndGetData()
        {
            return ConnectListener();
        }

        private Queue<Action> DeferredActionQueue = new Queue<Action>();

        private void Update()
        {
            lock (DeferredActionQueue)
            {
                while (DeferredActionQueue.Count > 0)
                {
                    DeferredActionQueue.Dequeue()();
                }
            }
        }

        // A lot of the work done in this class can only be done in UWP. The editor is not a UWP app.
#if !UNITY_EDITOR && UNITY_WSA
        private void RequestDataRetry()
        {
            if (!RequestAndGetData())
            {
                Invoke("RequestDataRetry", timeToDeferFailedConnections);
            }
        }
#endif

        /// <summary>
        /// Configures the network transmitter as the source.
        /// </summary>
        public void ConfigureAsServer()
        {
#if !UNITY_EDITOR && UNITY_WSA
            Task t = new Task(() =>
            {
                networkListener = new StreamSocketListener();
                networkListener.ConnectionReceived += NetworkListener_ConnectionReceived;
                networkListener.BindServiceNameAsync(SendConnectionPort.ToString()).GetResults();
            }
                );
            t.Start();
#else
            Debug.Log("This script is not intended to be run from the Unity Editor");
            // In order to avoid compiler warnings in the Unity Editor we have to access a few of our fields.
            Debug.Log(string.Format("serverIP = {0} waitingForConnection = {1} mostRecentDataBuffer = {2}", serverIp, waitingForConnection, mostRecentDataBuffer == null ? "No there" : "there"));
#endif
        }

        /// <summary>
        /// Connects to the server and requests data.
        /// </summary>
        private bool ConnectListener()
        {
#if !UNITY_EDITOR && UNITY_WSA
            if (waitingForConnection)
            {
                Debug.Log("Not a good time to connect listener");
                return false;
            }

            waitingForConnection = true;
            Debug.Log("Connecting to " + serverIp);
            HostName networkHost = new HostName(serverIp);
            networkConnection = new StreamSocket();

            IAsyncAction outstandingAction = networkConnection.ConnectAsync(networkHost, SendConnectionPort.ToString());
            AsyncActionCompletedHandler aach = new AsyncActionCompletedHandler(RcvNetworkConnectedHandler);
            outstandingAction.Completed = aach;

            return true;
#else
            return false;
#endif
        }

#if !UNITY_EDITOR && UNITY_WSA
        /// <summary>
        /// When a connection is made to us, this call back gets called and
        /// we send our data.
        /// </summary>
        /// <param name="sender">The listener that was connected to.</param>
        /// <param name="args">some args that we don't use.</param>
        private void NetworkListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            // If we have data, send it. 
            if (mostRecentDataBuffer != null)
            {
                IOutputStream stream = args.Socket.OutputStream;
                using (DataWriter writer = new DataWriter(stream))
                {
                    writer.WriteInt32(mostRecentDataBuffer.Length);
                    writer.WriteBytes(mostRecentDataBuffer);
                    writer.StoreAsync().AsTask().Wait();
                    writer.FlushAsync().AsTask().Wait();
                }
            }
            else
            {
                Debug.LogError("No data to send but we've been connected to.  This is unexpected.");
            }
        }

        /// <summary>
        /// When a connection to the server is established and we can start reading the data, this will be called.
        /// </summary>
        /// <param name="asyncInfo">Info about the connection.</param>
        /// <param name="status">Status of the connection</param>
        private async void RcvNetworkConnectedHandler(IAsyncAction asyncInfo, AsyncStatus status)
        {
            // Status completed is successful.
            if (status == AsyncStatus.Completed)
            {
                DataReader networkDataReader;

                // Since we are connected, we can read the data being sent to us.
                using (networkDataReader = new DataReader(networkConnection.InputStream))
                {
                    // read four bytes to get the size.
                    DataReaderLoadOperation drlo = networkDataReader.LoadAsync(4);
                    while (drlo.Status == AsyncStatus.Started)
                    {
                        // just waiting.
                    }

                    int dataSize = networkDataReader.ReadInt32();
                    if (dataSize < 0)
                    {
                        Debug.Log("Super bad super big datasize");
                    }

                    // Need to allocate a new buffer with the dataSize.
                    mostRecentDataBuffer = new byte[dataSize];

                    // Read the data.
                    await networkDataReader.LoadAsync((uint)dataSize);
                    networkDataReader.ReadBytes(mostRecentDataBuffer);

                    // And fire our data ready event.
                    DataReadyEvent?.Invoke(mostRecentDataBuffer);
                }
            }
            else
            {
                Debug.Log("Failed to establish connection for rcv. Error Code: " + asyncInfo.ErrorCode);
                // In the failure case we'll requeue the data and wait before trying again.


                // And set the defer time so the update loop can do the 'Unity things' 
                // on the main Unity thread.
                DeferredActionQueue.Enqueue(() =>
                {
                    Invoke("RequestDataRetry", timeToDeferFailedConnections);
                });
            }

            networkConnection.Dispose();
            waitingForConnection = false;
        }
#endif
    }
}