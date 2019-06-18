// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
        private static int eventExecutionDepth = 0;
        private readonly Type eventSystemHandlerType = typeof(IEventSystemHandler);

        private enum Action
        {
            Add,
            Remove
        }

        // Lists for handlers which are added/removed during event dispatching.
        // Game objects and handlers are processed independently, so can be kept in separate lists.
        private List<Tuple<Action, Type, IEventSystemHandler>> postponedActions = new List<Tuple<Action, Type, IEventSystemHandler>>();
        private List<Tuple<Action, GameObject>> postponedObjectActions = new List<Tuple<Action, GameObject>>();

        /// <inheritdoc />
        public Dictionary<Type, List<IEventSystemHandler>> EventHandlersByType { get; } = new Dictionary<Type, List<IEventSystemHandler>>();

    #region IMixedRealityEventSystem Implementation

        /// <inheritdoc />
        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            eventExecutionDepth++;

            // This sends the event to every component that implements the corresponding event handling interface,
            // regardless of whether it was the one registering the object as global listener or not.
            // This behavior is kept for backwards compatibility. Will be removed together with the IMixedRealityEventSystem.Register(GameObject listener) interface.
            for (int i = EventListeners.Count - 1; i >= 0; i--)
            {
                ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
            }

            // Send events to all handlers registered via RegisterHandler API.
            List<IEventSystemHandler> handlers;
            if (EventHandlersByType.TryGetValue(typeof(T), out handlers))
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    var handler = handlers[i];
                    // If handler is a unity component, need to make sure, that it didn't receive an event
                    // when we were sending it to Game objects above.
                    // Do that by checking if its parent is registered in EventListeners.
                    var componentHandler = handler as Component;
                    if (componentHandler && EventListeners.Contains(componentHandler.gameObject))
                    {
                        continue;
                    }

                    eventHandler.Invoke((T)handler, eventData);
                }
            }

            eventExecutionDepth--;

            if (eventExecutionDepth == 0 && (postponedActions.Count > 0 || postponedObjectActions.Count > 0))
            {
                foreach (var handler in postponedActions)
                {
                    if (handler.Item1 == Action.Add)
                    {
                        AddHandlerToMap(handler.Item2, handler.Item3);
                    }
                    else if (handler.Item1 == Action.Remove)
                    {
                        RemoveHandlerFromMap(handler.Item2, handler.Item3);
                    }
                }

                foreach (var obj in postponedObjectActions)
                {
                    if (obj.Item1 == Action.Add)
                    {
                        // Can call it here, because guaranteed that eventExecutionDepth is 0
                        Register(obj.Item2);
                    }
                    else if (obj.Item1 == Action.Remove)
                    {
                        Unregister(obj.Item2);
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual void RegisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            if(handler == null)
            {
                return;
            }

            // #if due to Microsoft.MixedReality.Toolkit.ReflectionExtensions overload of Type.IsInterface
            #if WINDOWS_UWP && !ENABLE_IL2CPP
                Debug.Assert(typeof(T).IsInterface());
            #else
                Debug.Assert(typeof(T).IsInterface);
            #endif
            Debug.Assert(typeof(T).IsAssignableFrom(handler.GetType()));

            TraverseEventSystemHandlerHierarchy<T>(handler, RegisterHandler);
        }

        /// <inheritdoc />
        public virtual void UnregisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            if(handler == null)
            {
                return;
            }

            // #if due to Microsoft.MixedReality.Toolkit.ReflectionExtensions overload of Type.IsInterface
            #if WINDOWS_UWP && !ENABLE_IL2CPP
                Debug.Assert(typeof(T).IsInterface());
            #else
                Debug.Assert(typeof(T).IsInterface);
            #endif
            Debug.Assert(typeof(T).IsAssignableFrom(handler.GetType()));

            TraverseEventSystemHandlerHierarchy<T>(handler, UnregisterHandler);
        }

        /// <inheritdoc />
        public virtual void Register(GameObject listener)
        {
            // Because components on an object can change during its lifetime, we can't enumerate all handlers on an object 
            // at this point in time and register them via the new API.
            // This forces us to store an object and use ExecuteEvents traversal at time of handling events.
            if (eventExecutionDepth == 0)
            {
                if (!EventListeners.Contains(listener))
                {
                    EventListeners.Add(listener);
                }
            }
            else
            {
                postponedObjectActions.Add(Tuple.Create(Action.Add, listener));
            }
        }

        /// <inheritdoc />
        public virtual void Unregister(GameObject listener)
        {
            if (eventExecutionDepth == 0)
            {
                if (EventListeners.Contains(listener))
                {
                    EventListeners.Remove(listener);
                }
            }
            else
            {
                postponedObjectActions.Add(Tuple.Create(Action.Remove, listener));
            }
        }

    #endregion IMixedRealityEventSystem Implementation

    #region Registration helpers

        private void UnregisterHandler(Type handlerType, IEventSystemHandler handler)
        {
            if (eventExecutionDepth == 0)
            {
                RemoveHandlerFromMap(handlerType, handler);
            }
            else
            {
                postponedActions.Add(Tuple.Create(Action.Remove, handlerType, handler));
            }
        }

        private void RegisterHandler(Type handlerType, IEventSystemHandler handler)
        {
            if (eventExecutionDepth == 0)
            {
                AddHandlerToMap(handlerType, handler);
            }
            else
            {
                postponedActions.Add(Tuple.Create(Action.Add, handlerType, handler));
            }
        }

        private void AddHandlerToMap(Type handlerType, IEventSystemHandler handler)
        {
            List<IEventSystemHandler> handlers;

            if (!EventHandlersByType.TryGetValue(handlerType, out handlers))
            {
                handlers = new List<IEventSystemHandler> { handler };
                EventHandlersByType.Add(handlerType, handlers);
                return;
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <inheritdoc />
        private void RemoveHandlerFromMap(Type handlerType, IEventSystemHandler handler)
        {
            List<IEventSystemHandler> handlers;

            if (!EventHandlersByType.TryGetValue(handlerType, out handlers))
            {
                return;
            }

            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    EventHandlersByType.Remove(handlerType);
                }
            }
        }

    #endregion Registration helpers

    #region Utilities

        /// <summary>
        /// Utility function for registering parent interfaces of a given handler.
        /// </summary>
        /// <remarks>
        /// Event handler interfaces may derive from each other. Some events will be raised using a base handler class, and are supposed to trigger on
        /// all derived handler classes too. Example of that is IMixedRealityBaseInputHandler hierarchy.
        /// To support that current implementation registers multiple dictionary entries per handler, one for each level of event handler hierarchy.
        /// Alternative would be to register just one root type and 
        /// then determine which handlers to call dynamically in 'HandleEvent'.
        /// Implementation was chosen based on performance of 'HandleEvent'. Without determining type it is about 2+ times faster.
        /// There are possible ways to bypass that, but this will make implementation of classes 
        /// that derive from Input System unnecessarily more complicated.
        /// </remarks>
        private void TraverseEventSystemHandlerHierarchy<T>(IEventSystemHandler handler, Action<Type, IEventSystemHandler> func) where T : IEventSystemHandler
        {
            var handlerType = typeof(T);

            // Need to call on handlerType first, because GetInterfaces below will only return parent types.
            func(handlerType, handler);

            foreach (var iface in handlerType.GetInterfaces())
            {
                if (!iface.Equals(eventSystemHandlerType))
                {
                    func(iface, handler);
                }
            }
        }

    #endregion Utilities
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
