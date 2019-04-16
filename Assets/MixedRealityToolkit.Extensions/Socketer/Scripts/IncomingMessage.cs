// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer
{
    /// <summary>
    /// Helper class that wraps an incoming message
    /// </summary>
    public class IncomingMessage
    {
        public IncomingMessage(SocketEndpoint endpoint, byte[] data, int size)
        {
            Endpoint = endpoint;
            Data = data;
            Size = size;
        }

        /// <summary>
        /// The endpoint that the message was received from
        /// </summary>
        public SocketEndpoint Endpoint { get; private set; }

        /// <summary>
        /// The data provided in the message
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The size of the data provided in the message
        /// </summary>
        public int Size { get; private set; }
    }
}
