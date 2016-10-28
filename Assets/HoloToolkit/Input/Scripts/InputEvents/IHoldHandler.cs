//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
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