// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Networking.WebRTC
{
    /// <summary>
    /// The Mixed Reality Toolkit's Web RTC Data provider.
    /// </summary>
    public class MixedRealityWebRtcDataProvider : BaseDataProvider, IMixedRealityNetworkDataProvider<Guid, WebRtcPeerConnection>
    {
        /// <summary>
        /// This is just an example of a single frame from a web rtc data track.
        /// </summary>
        [Serializable]
        private struct Frame
        {
            public Frame(string messageType, byte[] message)
            {
                MessageType = messageType;
                Message = message;
            }

            public string MessageType;
            public byte[] Message;
        }

        /// <inheritdoc />
        public MixedRealityWebRtcDataProvider(string name, uint priority)
            : base(name, priority)
        {
        }

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName => Name;

        #region IEquality Implementation

        /// <summary>
        /// Equality check against each <see cref="IMixedRealityNetworkDataProvider"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True, if the <see cref="IMixedRealityNetworkDataProvider"/>s are the same.</returns>
        public static bool Equals(IMixedRealityNetworkDataProvider left, IMixedRealityNetworkDataProvider right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityNetworkDataProvider)obj);
        }

        private bool Equals(IMixedRealityNetworkDataProvider other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealityNetworkDataProvider Implementation

        private readonly Dictionary<Guid, WebRtcPeerConnection> connections = new Dictionary<Guid, WebRtcPeerConnection>();

        /// <inheritdoc />
        public IReadOnlyDictionary<Guid, WebRtcPeerConnection> Connections => new Dictionary<Guid, WebRtcPeerConnection>(connections);

        /// <inheritdoc />
        public void SendData<T>(T data)
        {
            var messageDataType = typeof(T).AssemblyQualifiedName;
            var messageData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(data));
            var message = new Frame(messageDataType, messageData);

            // TODO Send message data down into the lower level resource api peer connection.
        }

        /// <summary>
        /// TODO: probably won't be this method but I wanted to provide a way to show how network data providers can call into the network system to raise when 
        /// </summary>
        private void RaiseWebRtcDataArrived()
        {
            // TODO get frame data from peer connection.
            Frame frame = new Frame("myType", new byte[0]);
            var messageDataType = Type.GetType(frame.MessageType, false);
            object message = null;

            if (messageDataType != null)
            {
                message = JsonUtility.FromJson(Encoding.ASCII.GetString(frame.Message), messageDataType);
            }
            else
            {
                Debug.LogError($"Failed to get network message type {frame.MessageType}");
            }

            if (message != null)
            {
                MixedRealityToolkit.NetworkingSystem.RaiseDataReceived(message);
            }
        }

        #endregion IMixedRealityNetworkDataProvider Implementation
    }
}