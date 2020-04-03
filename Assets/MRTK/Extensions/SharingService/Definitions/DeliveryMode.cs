// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// This enum matches Photon's delivery mode values.
    /// These are common modes and will be expected from other implementations.
    /// </summary>
    public enum DeliveryMode
    {
        /// <summary>
        /// The operation/message gets sent just once without acknowledgement or repeat.
        /// The sequence (order) of messages is guaranteed.
        /// </summary>
        Unreliable = 0,
        /// <summary>
        /// The operation/message asks for an acknowledgment. It's resent until an ACK arrived.
        /// The sequence (order) of messages is guaranteed.
        /// </summary>
        Reliable = 1,
        /// <summary>
        /// The operation/message gets sent once (unreliable) and might arrive out of order.
        /// </summary>
        UnreliableUnsequenced = 2,
        /// <summary>
        /// The operation/message asks for an acknowledgment. It's resent until an ACK arrived
        /// </summary>
        ReliableUnsequenced = 3
    }
}