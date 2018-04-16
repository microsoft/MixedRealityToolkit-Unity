using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal
{
    public class EventManager : BaseManager, IEventSystemManager
    {
        #region IEventSystemManager Implementation

        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        /// <summary>
        /// The main function for handling and forwarding all events to their intended recipients.
        /// <para><remarks>See: https://docs.unity3d.com/Manual/MessagingSystem.html </remarks></para>
        /// </summary>
        /// <typeparam name="T">Event Handler Interface Type</typeparam>
        /// <param name="eventData">Event Data</param>
        /// <param name="eventHandler">Event Handler delegate</param>
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            for (int i = 0; i < EventListeners.Count; i++)
            {
                ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events.
        /// </summary>
        /// <param name="listener"><see cref="GameObject"/> to add to <see cref="EventListeners"/>.</param>
        public virtual void Register(GameObject listener)
        {
            Debug.Assert(!EventListeners.Contains(listener), $"{listener.name} is already registered to receive events!");
            EventListeners.Add(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to events.
        /// </summary>
        /// <param name="listener"><see cref="GameObject"/> to remove from <see cref="EventListeners"/>.</param>
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
