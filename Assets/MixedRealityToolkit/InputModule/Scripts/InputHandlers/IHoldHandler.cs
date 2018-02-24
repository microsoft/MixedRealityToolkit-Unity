// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    /// <summary>
    /// Interface to implement to react to hold gestures.
    /// </summary>
    public interface IHoldHandler : IEventSystemHandler
    {
        void OnHoldStarted(HoldEventData eventData);
        void OnHoldCompleted(HoldEventData eventData);
        void OnHoldCanceled(HoldEventData eventData);
    }
}