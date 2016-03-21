// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using Windows.Foundation;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// RemoteMeshSource will try to send meshes from the HoloLens to a remote system that is running the Unity editor.
    /// </summary>
    public class RemoteMeshSource : Singleton<RemoteMeshSource>
    {
        [Tooltip("The IPv4 Address of the machine running the Unity editor. Copy and paste this value from RemoteMeshTarget.")]
        public string ServerIP;

        [Tooltip("The connection port on the machine to use.")]
        public int ConnectionPort = 11000;

#if !UNITY_EDITOR 
        /// <summary>
        /// Tracks the network connection to the remote machine we are sending meshes to.
        /// </summary>
        private StreamSocket networkConnection;

        /// <summary>
        /// Tracks if we are currently sending a mesh.
        /// </summary>
        private bool Sending = false;

        /// <summary>
        /// Temporary buffer for the data we are sending.
        /// </summary>
        private byte[] nextDataBufferToSend;
 
        /// <summary>
        /// A queue of data buffers to send.
        /// </summary>
        private Queue<byte[]> dataQueue = new Queue<byte[]>();

        /// <summary>
        /// If we cannot connect to the server, we will wait before trying to reconnect.
        /// </summary>
        private float deferTime = 0.0f;
 
        /// <summary>
        /// If we cannot connect to the server, this is how long we will wait before retrying.
        /// </summary>
        private float timeToDeferFailedConnections = 10.0f;
   
        public void Update()
        {
            // Check to see if deferTime has been set.  
            // DeferUpdates will set the Sending flag to true for 
            // deferTime seconds.  
            if (deferTime > 0.0f)
            {
                DeferUpdates(deferTime);
                deferTime = 0.0f;
            }

            // If we aren't sending a mesh, but we have a mesh to send, send it.
            if (!Sending && dataQueue.Count > 0)
            {
                byte[] nextPacket = dataQueue.Dequeue();
                SendDataOverNetwork(nextPacket);
            }
        }

        /// <summary>
        /// Handles waiting for some amount of time before trying to reconnect.
        /// </summary>
        /// <param name="timeout">Time in seconds to wait.</param>
        void DeferUpdates(float timeout)
        {
            Sending = true;
            Invoke("EnableUpdates", timeout);
        }

        /// <summary>
        /// Stops waiting to reconnect.
        /// </summary>
        void EnableUpdates()
        {
            Sending = false;
        }

        /// <summary>
        /// Queues up a data buffer to send over the network.
        /// </summary>
        /// <param name="dataBufferToSend">The data buffer to send.</param>
        public void SendData(byte[] dataBufferToSend)
        {
            dataQueue.Enqueue(dataBufferToSend);
        }

        /// <summary>
        /// Sends the data over the network.
        /// </summary>
        /// <param name="dataBufferToSend">The data buffer to send.</param>
        private void SendDataOverNetwork(byte[] dataBufferToSend)
        {
            if (Sending)
            {
                // This shouldn't happen, but just in case.
                Debug.Log("one at a time please");
                return;
            }

            // Track that we are sending a data buffer.
            Sending = true;

            // Set the next buffer to send when the connection is made.
            nextDataBufferToSend = dataBufferToSend;

            // Setup a connection to the server.
            HostName networkHost = new HostName(ServerIP.Trim());
            networkConnection = new StreamSocket();

            // Connections are asynchronous.  
            // !!! NOTE These do not arrive on the main Unity Thread. Most Unity operations will throw in the callback !!!
            IAsyncAction outstandingAction = networkConnection.ConnectAsync(networkHost, ConnectionPort.ToString());
            AsyncActionCompletedHandler aach = new AsyncActionCompletedHandler(NetworkConnectedHandler);
            outstandingAction.Completed = aach;
        }

        /// <summary>
        /// Called when a connection attempt complete, successfully or not.  
        /// !!! NOTE These do not arrive on the main Unity Thread. Most Unity operations will throw in the callback !!!
        /// </summary>
        /// <param name="asyncInfo">Data about the async operation.</param>
        /// <param name="status">The status of the operation.</param>
        public void NetworkConnectedHandler(IAsyncAction asyncInfo, AsyncStatus status)
        {
            // Status completed is successful.
            if (status == AsyncStatus.Completed)
            {
                DataWriter networkDataWriter;
                
                // Since we are connected, we can send the data we set aside when establishing the connection.
                using(networkDataWriter = new DataWriter(networkConnection.OutputStream))
                {
                    // Write how much data we are sending.
                    networkDataWriter.WriteInt32(nextDataBufferToSend.Length);

                    // Then write the data.
                    networkDataWriter.WriteBytes(nextDataBufferToSend);

                    // Again, this is an async operation, so we'll set a callback.
                    DataWriterStoreOperation dswo = networkDataWriter.StoreAsync();
                    dswo.Completed = new AsyncOperationCompletedHandler<uint>(DataSentHandler);
                }
            }
            else
            {
                Debug.Log("Failed to establish connection. Error Code: " + asyncInfo.ErrorCode);
                // In the failure case we'll requeue the data and wait before trying again.
                networkConnection.Dispose();

                // Didn't send, so requeue the data.
                dataQueue.Enqueue(nextDataBufferToSend);

                // And set the defer time so the update loop can do the 'Unity things' 
                // on the main Unity thread.
                deferTime = timeToDeferFailedConnections;
            }
        }

        /// <summary>
        /// Called when sending data has completed.
        /// !!! NOTE These do not arrive on the main Unity Thread. Most Unity operations will throw in the callback !!!
        /// </summary>
        /// <param name="operation">The operation in flight.</param>
        /// <param name="status">The status of the operation.</param>
        public void DataSentHandler(IAsyncOperation<uint> operation, AsyncStatus status)
        {
            // If we failed, requeue the data and set the deferral time.
            if (status == AsyncStatus.Error)
            {
                // didn't send, so requeue
                dataQueue.Enqueue(nextDataBufferToSend);
                deferTime = timeToDeferFailedConnections;
            }
            else
            {
                // If we succeeded, clear the sending flag so we can send another mesh
                Sending = false;
            }

            // Always disconnect here since we will reconnect when sending the next mesh.  
            networkConnection.Dispose();
        }
#endif
    }
}
