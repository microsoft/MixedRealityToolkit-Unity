// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Interfaces
{
    public enum Priority
    {
        Default,
        Critical
    }

    public delegate void DataHandler(string playerId, byte[] payload);

    public interface INetworkingService
    {
        bool SendData(byte[] data, Priority priority);
        event DataHandler DataReceived;
    }
}
