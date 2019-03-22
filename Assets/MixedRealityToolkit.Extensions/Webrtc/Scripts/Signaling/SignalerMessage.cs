// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Signaling
{
    /// <summary>
    /// Data that makes up a signaler message
    /// </summary>
    /// <remarks>
    /// Note: the same data is used for transmitting and receiving
    /// </remarks>
    [Serializable]
    public class SignalerMessage : ISerializationCallbackReceiver 
    {
        /// <summary>
        /// Possible message types as-serialized on the wire
        /// </summary>
        public enum WireMessageType
        {
            /// <summary>
            /// An unrecognized message
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// A SDP offer message
            /// </summary>
            Offer,
            /// <summary>
            /// A SDP answer message
            /// </summary>
            Answer,
            /// <summary>
            /// A trickle-ice or ice message
            /// </summary>
            Ice,
            /// <summary>
            /// A built-in protocol extension message for adjusting target
            /// peer id remotely
            /// </summary>
            SetPeer
        }

        /// <summary>
        /// The message type
        /// </summary>
        [NonSerialized]
        public WireMessageType MessageType;

        /// <summary>
        /// Unity serialization field for <see cref="MessageType"/>
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private string Type;

        /// <summary>
        /// The primary message contents
        /// </summary>
        public string Data;

        /// <summary>
        /// The data separator needed for proper ICE serialization
        /// </summary>
        public string IceDataSeparator;

        /// <summary>
        /// The target id to which we send messages
        /// </summary>
        /// <remarks>
        /// This is expected to be set when <see cref="ISignaler.SendMessageAsync(SignalerMessage)"/> is called
        /// </remarks>
        [NonSerialized]
        public string TargetId;

        public void OnBeforeSerialize()
        {
            Type = MessageType.ToString();
        }

        public void OnAfterDeserialize()
        {
            MessageType = (WireMessageType)Enum.Parse(typeof(WireMessageType), Type);
        }
    }
}
