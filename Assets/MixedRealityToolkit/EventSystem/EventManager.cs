using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal
{
    public class EventManager
    {
        public static readonly List<GameObject> EventListeners = new List<GameObject>();

        private static GenericBaseEventData genericEventData;

        static EventManager()
        {
            Setup();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Setup()
        {
            genericEventData = new GenericBaseEventData(EventSystem.current);
        }

        public static void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
            where T : IEventSystemHandler
        {
            DebugUtilities.DebugAssert(!eventData.used);

            for (int i = 0; i < EventListeners.Count; i++)
            {
                ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
            }
        }

        /// <summary>
        /// Register a <see cref="GameObject"/> to listen to events.
        /// </summary>
        /// <param name="listener"></param>
        public static void Register(GameObject listener)
        {
            DebugUtilities.DebugAssert(!EventListeners.Contains(listener), $"{listener.name} is already registered to receive events!");
            EventListeners.Add(listener);
        }

        /// <summary>
        /// Unregister a <see cref="GameObject"/> from listening to events.
        /// </summary>
        /// <param name="listener"></param>
        public static void Unregister(GameObject listener)
        {
            DebugUtilities.DebugAssert(EventListeners.Contains(listener), $"{listener.name} was never registered!");
            EventListeners.Remove(listener);
        }

        public static void RaiseGenericEvent(IEventSource eventSource)
        {
            genericEventData.Initialize(eventSource);
            HandleEvent(genericEventData, GenericEventHandler);
        }

        private static readonly ExecuteEvents.EventFunction<IEventHandler> GenericEventHandler =
            delegate (IEventHandler handler, BaseEventData eventData)
            {
                var casted = ExecuteEvents.ValidateEventData<GenericBaseEventData>(eventData);
                handler.OnEventRaised(casted);
            };
    }
}
