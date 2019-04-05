// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Base Event System that can be inherited from to give other system features event capabilities.
    /// </summary>
    public abstract class BaseEventSystem : BaseService, IMixedRealityEventSystem
    {
        #region IMixedRealityEventSystem Implementation

        private static int eventExecutionDepth = 0;
        private readonly WaitUntil doneExecutingEvents = new WaitUntil(() => eventExecutionDepth == 0);

        /// <inheritdoc />
        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);
            eventExecutionDepth++;

            for (int i = EventListeners.Count - 1; i >= 0; i--)
            {
                ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
            }

            eventExecutionDepth--;
        }

        /// <inheritdoc />
        public virtual async void Register(GameObject listener)
        {
            if (EventListeners.Contains(listener)) { return; }

            if (eventExecutionDepth > 0)
            {
                await doneExecutingEvents;
            }

            EventListeners.Add(listener);
        }

        /// <inheritdoc />
        public virtual async void Unregister(GameObject listener)
        {
            if (!EventListeners.Contains(listener)) { return; }

            if (eventExecutionDepth > 0)
            {
                await doneExecutingEvents;
            }

            EventListeners.Remove(listener);
        }

        #endregion IMixedRealityEventSystem Implementation

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
