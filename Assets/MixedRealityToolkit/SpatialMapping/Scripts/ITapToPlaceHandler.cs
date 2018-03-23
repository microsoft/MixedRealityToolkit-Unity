// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace MixedRealityToolkit.SpatialMapping
{
    /// <summary>
    /// Interface to implement reacting to placement of objects.
    /// </summary>
    public interface ITapToPlaceHandler : IEventSystemHandler
    {
        void OnPlacingStarted(TapToPlaceEventData eventData);

        void OnPlacingCompleted(TapToPlaceEventData eventData);
    }
}
