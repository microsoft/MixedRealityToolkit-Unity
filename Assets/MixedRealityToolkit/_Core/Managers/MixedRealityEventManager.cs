// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// Event System Manager that can be inherited from to give other managers event capabilities.
    /// </summary>
    public abstract class MixedRealityEventManager : BaseManager, IMixedRealityEventSystem
    {
        #region IEventSystemManager Implementation

        /// <inheritdoc />
        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            for (int i = 0; i < EventListeners.Count; i++)
            {
                ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
            }
        }

        /// <inheritdoc />
        public virtual void Register(GameObject listener)
        {
            Debug.Assert(!EventListeners.Contains(listener), $"{listener.name} is already registered to receive events!");
            EventListeners.Add(listener);
        }

        /// <inheritdoc />
        public virtual void Unregister(GameObject listener)
        {
            Debug.Assert(EventListeners.Contains(listener), $"{listener.name} was never registered!");
            EventListeners.Remove(listener);
        }

        #endregion IEventSystemManager Implementation

        // Example Event Pattern #############################################################

        //public void RaiseGenericEvent(IEventSource eventSource)
        //{
        //    genericEventData.Initialize(eventSource);
        //    HandleEvent(genericEventData, GenericEventHandler);
        //}

        //private static readonly ExecuteEvents.EventFunction<IEventHandler> GenericEventHandler =
        //    delegate (IEventHandler handler, BaseEventData eventData)
        //    {
        //        var casted = ExecuteEvents.ValidateEventData<GenericBaseEventData>(eventData);
        //        handler.OnEventRaised(casted);
        //    };

        // Example Event Pattern #############################################################
    }
}
