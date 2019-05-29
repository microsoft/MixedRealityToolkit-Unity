// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    public sealed class SocketEndpoint
    {
        private enum ConnectionState
        {
            Connected,
            Disconnected
        }

        /// <summary>
        /// IP address for the socket endpoint
        /// </summary>
        public string Address { get; private set; }

        private readonly SocketerClient socketerClient;
        private readonly int sourceId;
        private ConcurrentQueue<IncomingMessage> incomingQueue;
        private DateTime lastActiveTimestamp;
        private TimeSpan timeoutInterval;

        private ConnectionState State { get; set; } = ConnectionState.Connected;

        /// <summary>
        /// Returns true if the socket endpoint is connected, otherwise false
        /// </summary>
        public bool IsConnected
        {
            get { return State == ConnectionState.Connected; }
        }

        /// <summary>
        /// Call to set the socket endpoint state to disconnected
        /// </summary>
        public void Disconnect()
        {
            if (State != ConnectionState.Disconnected)
            {
                State = ConnectionState.Disconnected;
                this.socketerClient.Disconnect(sourceId);
            }
        }

        /// <summary>
        /// Checks whether the associated client is still active. If not, the client is disconnected.
        /// </summary>
        /// <param name="currentTime">Time to use relative to last active timestamp to determine whether to disconnect</param>
        public void CheckConnectionTimeout(DateTime currentTime)
        {
            if (timeoutInterval != TimeSpan.Zero && currentTime - lastActiveTimestamp > timeoutInterval)
            {
                this.socketerClient.Disconnect(sourceId);
            }
        }

        public SocketEndpoint(SocketerClient socketerClient, TimeSpan timeoutInterval, string address, int sourceId = 0)
        {
            this.socketerClient = socketerClient;
            this.timeoutInterval = timeoutInterval;
            this.sourceId = sourceId;
            this.Address = address;
            this.lastActiveTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Call to start enqueuing incoming messages
        /// </summary>
        /// <param name="incomingQueue">Queue used for enqueuing messages</param>
        public void QueueIncomingMessages(ConcurrentQueue<IncomingMessage> incomingQueue)
        {
            this.incomingQueue = incomingQueue;
            socketerClient.Message += Socket_Message;
        }

        /// <summary>
        /// Call to stop enqueuing incoming messages
        /// </summary>
        public void StopIncomingMessageQueue()
        {
            socketerClient.Message -= Socket_Message;
            this.incomingQueue = null;
        }

        /// <summary>
        /// Call to send data to this endpoint
        /// </summary>
        /// <param name="data">data to send</param>
        public void Send(byte[] data)
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Attempted to send message to disconnected SocketEndpoint.");
                return;
            }

            try
            {
                socketerClient.SendNetworkMessage(data, sourceId);
            }
            catch
            {
                socketerClient.Disconnect(sourceId);
            }
        }

        private void Socket_Message(SocketerClient arg1, MessageEvent e)
        {
            // This event is sent to all socket endpoints. Make sure this message matches the server (connectionId == 0) or the correct client (sourceId == e.SourceId)
            if (sourceId == 0 || sourceId == e.SourceId)
            {
                lastActiveTimestamp = DateTime.UtcNow;
                IncomingMessage incomingMessage = new IncomingMessage(this, e.Message, e.Message.Length);
                incomingQueue.Enqueue(incomingMessage);
            }
        }
    }
}
