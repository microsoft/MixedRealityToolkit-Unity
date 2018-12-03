// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem.Handlers
{
    public interface IMixedRealityNetworkingHandler<T> : IEventSystemHandler
    {
        void OnDataReceived(BaseNetworkingEventData<T> eventData);
    }
}
