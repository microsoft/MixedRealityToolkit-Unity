// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    public interface IMixedRealityEventSystem : IMixedRealityManager
    {
        List<GameObject> EventListeners { get; }
        void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler;
        void Register(GameObject listener);
        void Unregister(GameObject listener);
    }
}