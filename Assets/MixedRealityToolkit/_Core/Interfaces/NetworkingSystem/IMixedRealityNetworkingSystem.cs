// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem
{
    public interface IMixedRealityNetworkingSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// The <see cref="IMixedRealityNetworkDataProvider"/>s detected by the system.
        /// </summary>
        HashSet<IMixedRealityNetworkDataProvider> NetworkDataProviders { get; }

        /// <summary>
        /// Request a new <see cref="IMixedRealityEventSource.SourceId"/> for the <see cref="IMixedRealityNetworkDataProvider"/>
        /// </summary>
        /// <returns>A new unique source id for the data provider.</returns>
        uint RequestNetworkDataProviderSourceId();

        /// <summary>
        /// Raise that a <see cref="IMixedRealityNetworkDataProvider"/> has been detected by the system.
        /// </summary>
        /// <param name="networkDataProvider"></param>
        void RaiseNetworkDataProviderDetected(IMixedRealityNetworkDataProvider networkDataProvider);

        /// <summary>
        /// Raise that a <see cref="IMixedRealityNetworkDataProvider"/> has been lost by the system.
        /// </summary>
        /// <param name="networkDataProvider"></param>
        void RaiseNetworkDataProviderLost(IMixedRealityNetworkDataProvider networkDataProvider);

        /// <summary>
        /// Send data out over the wire to whomever is listening.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        void SendData<T>(T data);

        /// <summary>
        /// Raised when data has been received from an <see cref="IMixedRealityNetworkDataProvider"/>. It's up to the <see cref="IMixedRealityNetworkingSystem"/> to forward this data to whomever needs it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        void OnDataReceived<T>(T data);
    }
}