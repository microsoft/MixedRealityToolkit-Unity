// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if (UNITY_WSA || WINDOWS_UWP) && !UNITY_EDITOR
#define NETFX_CORE
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

#if !NETFX_CORE
using System.Threading;
#else
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.Networking.Connectivity;
using System.Threading;
#endif

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Sends and receives packets over the network, using the chosen protocol.
    /// </summary>
    public class Socketer : MonoBehaviour
    {
        /// <summary>
        /// The protocol to use:  TCP or UDP.
        /// </summary>
        public SocketerClient.Protocol Protocol;
        /// <summary>
        /// Whether this object should listen for incoming connections/packets, or send them.
        /// TCP Socketers are able to both send and recieve once a connection is intitiated.
        /// </summary>
        public SocketerClient.ProtocolDirection Direction;
        /// <summary>
        /// The IP address to connect to, if the Direction is Sender.  Ignored if Listener.
        /// </summary>
        public string Host;
        /// <summary>
        /// The port to use.  Suggested values are between 10000 and 40000, and must agree
        /// with the Socketer on the other end.
        /// </summary>
        public int Port;

        /// <summary>
        /// Fired when a message arrives.  Runs on the Update thread.
        /// </summary>
        public event Action<Socketer, MessageEvent> Message;
        /// <summary>
        /// Fired when a TCP connection completes a connection.  The string is the Host of the
        /// other Socketer.
        /// Note:  this may fire on a thread other than the Update thread.
        /// </summary>
        public event Action<Socketer, int, string> Connected;
        /// <summary>
        /// Fired when a TCP connection loses a connection.  The string is the Host of the
        /// other Socketer.
        /// Note:  this may fire on a thread other than the Update thread.
        /// </summary>
        public event Action<Socketer, int, string> Disconnected;

        private SocketerClient socketer;
        private Queue<MessageEvent> stateQueue = new Queue<MessageEvent>();

        internal const UInt32 HandshakeCode = 9999;

        /// <summary>
        /// Gets the IP of this machine.  Suitable for use as a Host on another Socketer.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            return SocketerClient.GetLocalIPAddress();
        }

        /// <summary>
        /// Sends a byte array to the other side.  Works for all Socketers except UDP Listeners.
        /// If the other Socketer is not listening, the messages are lost.
        /// </summary>
        /// <param name="message">Message contents.</param>
        public void SendNetworkMessage(byte[] message)
        {
            if (socketer != null)
            {
                socketer.SendNetworkMessage(message);
            }
        }

        /// <summary>
        /// Sends a byte array to the other side.  Works for all Socketers except UDP Listeners.
        /// If the other Socketer is not listening, the messages are lost.
        /// </summary>
        /// <param name="message">Message contents.</param>
        public void SendNetworkMessage(string message)
        {
            if (socketer != null)
            {
                socketer.SendNetworkMessage(message);
            }
        }

        private void OnEnable()
        {
            if (Direction == SocketerClient.ProtocolDirection.Listener)
            {
                socketer = SocketerClient.CreateListener(Protocol, Port);
            }
            else
            {
                socketer = SocketerClient.CreateSender(Protocol,
                    string.IsNullOrEmpty(Host) ? "127.0.0.1" : Host, Port);
            }
            socketer.Connected += Socketer_Connected;
            socketer.Disconnected += Socketer_Disconnected;
            socketer.Message += Socketer_Message;
            socketer.Start();
        }

        private void OnDisable()
        {
            socketer.Stop();
            socketer.Connected -= Socketer_Connected;
            socketer.Disconnected -= Socketer_Disconnected;
            socketer.Message -= Socketer_Message;
            socketer = null;
        }

        private void Socketer_Connected(SocketerClient sender, int sourceId, string remoteHost)
        {
            Connected?.Invoke(this, sourceId, remoteHost);
        }

        private void Socketer_Disconnected(SocketerClient sender, int sourceId, string remoteHost)
        {
            Disconnected?.Invoke(this, sourceId, remoteHost);
        }

        private void Socketer_Message(SocketerClient sender, MessageEvent e)
        {
            // messages come in on a different thread, so we move them to the update thread
            lock (stateQueue)
            {
                stateQueue.Enqueue(e);
            }
        }

        // Update is called once per frame
        void Update()
        {
            while (true)
            {
                MessageEvent e;

                lock (stateQueue)
                {
                    if (stateQueue.Count <= 0) break;
                    e = stateQueue.Dequeue();
                }

                Message?.Invoke(this, e);
            }
        }
    }
}
