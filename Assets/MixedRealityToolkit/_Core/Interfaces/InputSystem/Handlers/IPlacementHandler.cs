// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement reacting to placement of objects.
    /// </summary>
    public interface IPlacementHandler : IEventSystemHandler
    {
        void OnPlacingStarted(PlacementEventData eventData);

        void OnPlacingCompleted(PlacementEventData eventData);
    }
}