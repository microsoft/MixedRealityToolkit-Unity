// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem
{
    /// <summary>
    /// The base interface to define networking data providers, such as WebRTC, Photon, uNet, etc.
    /// </summary>
    public interface IMixedRealityNetworkDataProvider : IMixedRealityEventSource, IMixedRealityDataProvider
    {
        /// <summary>
        /// Send this data over the wire.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        void SendData<T>(T data);
    }

    /// <summary>
    /// The interface to define network data providers that keep a list of active connections.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TConnection"></typeparam>
    public interface IMixedRealityNetworkDataProvider<TKey, TConnection> : IMixedRealityNetworkDataProvider
    {
        /// <summary>
        /// The current connections of the network data provider.
        /// </summary>
        IReadOnlyDictionary<TKey, TConnection> Connections { get; }
    }
}