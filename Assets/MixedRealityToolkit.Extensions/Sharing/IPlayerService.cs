// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public delegate void PlayerConnectedHandler(string playerId);
    public delegate void PlayerDisconnectedHandler(string playerId);

    public interface IPlayerStateObserver
    {
        void PlayerConnected(string playerId);
        void PlayerDisconnected(string playerId);
    }

    public interface IPlayerService
    {
        event PlayerConnectedHandler PlayerConnected;
        event PlayerDisconnectedHandler PlayerDisconnected;
    }
}

