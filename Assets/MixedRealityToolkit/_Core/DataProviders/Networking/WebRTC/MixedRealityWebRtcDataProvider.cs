// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Extensions;
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
            byte[] messageData;

            if (typeof(T).IsPrimitive ||
                typeof(T) == typeof(string))
            {
                messageData = data.GetBuiltInTypeBytes();
            }
            else
            {
                messageData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(data));
            }

            var message = new Frame(messageDataType, messageData);

            // TODO Send message data down into the lower level resource api peer connection.
        }

        private void OnFrameReceived(Frame frame)
        {
            // TODO get frame data from peer connection.
            var messageDataType = Type.GetType(frame.MessageType, false);

            if (messageDataType != null)
            {
                RaiseWebRtcDataArrived(messageDataType, frame.Message);
            }
            else
            {
                Debug.LogError($"Failed to get network message type {frame.MessageType}");
            }
        }

        /// <summary>
        /// TODO: probably won't be this method but I wanted to provide a way to show how network data providers could call into the network system to raise when data has been received
        /// </summary>
        private static void RaiseWebRtcDataArrived<T>(T type, byte[] data)
        {
            T message;

            try
            {
                if (typeof(T).IsPrimitive ||
                    typeof(T) == typeof(string))
                {
                    message = data.GetBuiltInTypeValueFromBytes<T>();
                }
                else
                {
                    message = JsonUtility.FromJson<T>(Encoding.ASCII.GetString(data));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to convert {type.GetType().Name} message data!\n{e.Message}");
                return;
            }

            MixedRealityToolkit.NetworkingSystem.RaiseDataReceived(message);
        }

        #endregion IMixedRealityNetworkDataProvider Implementation
    }
}