// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public delegate void ConnectedEventHandler(SocketEndpoint endpoint);
    public delegate void DisconnectedEventHandler(SocketEndpoint endoint);
    public delegate void CommandHandler(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize);

    public interface ICommandRegistry
    {
        event ConnectedEventHandler Connected;
        event DisconnectedEventHandler Disconnected;

        void RegisterCommandHandler(string command, CommandHandler handler);
        void UnregisterCommandHandler(string command, CommandHandler handler);
    }
}
