// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Networking;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem.Handlers
{
    /// <summary>
    /// Generic networking data handler for consuming network data from the <see cref="IMixedRealityNetworkingSystem"/>
    /// </summary>
    /// <typeparam name="T">The type of data you want to receive.</typeparam>
    public interface IMixedRealityNetworkingHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Is triggered by incoming data and includes eventData as a parameter.
        /// </summary>
        /// <param name="eventData"></param>
        void OnDataReceived(BaseNetworkingEventData<T> eventData);
    }
}