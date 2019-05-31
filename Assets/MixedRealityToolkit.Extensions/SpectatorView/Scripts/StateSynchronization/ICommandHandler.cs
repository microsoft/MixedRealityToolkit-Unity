// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface ICommandHandler
    {
        void OnConnected(SocketEndpoint endpoint);
        void OnDisconnected(SocketEndpoint endpoint);
        void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize);
    }
}
