// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Sharing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.Examples.Sharing
{
    /// <summary>
    /// Test class for demonstrating how to send custom messages between clients.
    /// </summary>
    public class CustomMessages : Singleton<CustomMessages>
    {
        /// <summary>
        /// Message enum containing our information bytes to share.
        /// The first message type has to start with UserMessageIDStart
        /// so as not to conflict with MixedRealityToolkit internal messages.
        /// </summary>
        public enum TestMessageID : byte
        {
            HeadTransform = MessageID.UserMessageIDStart,
            Max
        }

        public enum UserMessageChannels
        {
            Anchors = MessageChannel.UserMessageChannelStart
        }

        /// <summary>
        /// Cache the local user's ID to use when sending messages
        /// </summary>
        public long LocalUserID
        {
            get; set;
        }

        public delegate void MessageCallback(NetworkInMessage msg);
        private Dictionary<TestMessageID, MessageCallback> messageHandlers = new Dictionary<TestMessageID, MessageCallback>();
        public Dictionary<TestMessageID, MessageCallback> MessageHandlers
        {
            get
            {
                return messageHandlers;
            }
        }

        /// <summary>
        /// Helper object that we use to route incoming message callbacks to the member
        /// functions of this class
        /// </summary>
        private NetworkConnectionAdapter connectionAdapter;

        /// <summary>
        /// Cache the connection object for the sharing service
        /// </summary>
        private NetworkConnection serverConnection;

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }
        }

        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;
            InitializeMessageHandlers();
        }

        private void InitializeMessageHandlers()
        {
            SharingStage sharingStage = SharingStage.Instance;

            if (sharingStage == null)
            {
                Debug.Log("Cannot Initialize CustomMessages. No SharingStage instance found.");
                return;
            }

            serverConnection = sharingStage.Manager.GetServerConnection();
            if (serverConnection == null)
            {
                Debug.Log("Cannot initialize CustomMessages. Cannot get a server connection.");
                return;
            }

            connectionAdapter = new NetworkConnectionAdapter();
            connectionAdapter.MessageReceivedCallback += OnMessageReceived;

            // Cache the local user ID
            LocalUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

            for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
            {
                if (MessageHandlers.ContainsKey((TestMessageID)index) == false)
                {
                    MessageHandlers.Add((TestMessageID)index, null);
                }

                serverConnection.AddListener(index, connectionAdapter);
            }
        }

        private NetworkOutMessage CreateMessage(byte messageType)
        {
            NetworkOutMessage msg = serverConnection.CreateMessage(messageType);
            msg.Write(messageType);
            // Add the local userID so that the remote clients know whose message they are receiving
            msg.Write(LocalUserID);
            return msg;
        }

        public void SendHeadTransform(Vector3 position, Quaternion rotation)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)TestMessageID.HeadTransform);

                AppendTransform(msg, position, rotation);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (serverConnection != null)
            {
                for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
                {
                    serverConnection.RemoveListener(index, connectionAdapter);
                }
                connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
            }
        }

        private void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
        {
            byte messageType = msg.ReadByte();
            MessageCallback messageHandler = MessageHandlers[(TestMessageID)messageType];
            if (messageHandler != null)
            {
                messageHandler(msg);
            }
        }

        #region HelperFunctionsForWriting

        private void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
        {
            AppendVector3(msg, position);
            AppendQuaternion(msg, rotation);
        }

        private void AppendVector3(NetworkOutMessage msg, Vector3 vector)
        {
            msg.Write(vector.x);
            msg.Write(vector.y);
            msg.Write(vector.z);
        }

        private void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
        {
            msg.Write(rotation.x);
            msg.Write(rotation.y);
            msg.Write(rotation.z);
            msg.Write(rotation.w);
        }

        #endregion

        #region HelperFunctionsForReading

        public Vector3 ReadVector3(NetworkInMessage msg)
        {
            return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        public Quaternion ReadQuaternion(NetworkInMessage msg)
        {
            return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        #endregion
    }
}