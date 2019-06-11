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
    /// <remarks>
    /// Event handler interfaces may derive from each other. Some events will be raised using a base handler class, and are supposed to trigger on
    /// all derived handler classes too. Example of that is IMixedRealityBaseInputHandler hierarchy.
    /// To support that current implementation registers multiple dictionary entries per handler, one for each level of event handler hierarchy.
    /// Alternative would be to register just one type (the one used for RegisterHandler call) and 
    /// then determine which handlers to call dynamically in 'HandleEvent'.
    /// Implementation was chosen based on performance of 'HandleEvent'. Without determining type it is about 2+ times faster.
    /// There are possible ways to bypass that, but this will make implementation of old API and classes 
    /// that derive from Input System unnecessarily more complicated.
    /// </remarks>
    public abstract class BaseEventSystem : BaseService, IMixedRealityEventSystem
    {
        private static int eventExecutionDepth = 0;
        private readonly Type eventSystemHandlerType = typeof(IEventSystemHandler);

        private enum Action
        {
            Add,
            Remove
        }

        private List<(Action, Type, IEventSystemHandler)> postponedActions = new List<(Action, Type, IEventSystemHandler)>();

        #region IMixedRealityEventSystem Implementation

        /// <inheritdoc />
        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        /// <inheritdoc />
        public Dictionary<Type, HashSet<IEventSystemHandler>> EventHandlersByType { get; } = new Dictionary<Type, HashSet<IEventSystemHandler>>();

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            HashSet<IEventSystemHandler> handlers;
            if (!EventHandlersByType.TryGetValue(typeof(T), out handlers))
            {
                return;
            }

            eventExecutionDepth++;

            foreach (IEventSystemHandler handler in handlers)
            {
                Debug.Assert(handler is T);
                eventHandler.Invoke((T)handler, eventData);
            }

            eventExecutionDepth--;

            if(eventExecutionDepth == 0 && postponedActions.Count > 0)
            {
                foreach(var handler in postponedActions)
                {
                    if (handler.Item1 == Action.Add)
                    {
                        AddHandlerToMap(handler.Item2, handler.Item3);
                    }
                    else if(handler.Item1 == Action.Remove)
                    {
                        RemoveHandlerFromMap(handler.Item2, handler.Item3);
                    }
                }
            }
        }

        /// <inheritdoc />
        public virtual void RegisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            Debug.Assert(typeof(T).IsInterface);
            TraverseEventSystemHandlerHierarchy<T>(handler, RegisterHandler);
        }

        /// <inheritdoc />
        public virtual void UnregisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            Debug.Assert(typeof(T).IsInterface);
            TraverseEventSystemHandlerHierarchy<T>(handler, UnregisterHandler);
        }

        /// <inheritdoc />
        public virtual void Register(GameObject listener)
        {
            // For backward compatibility
            if (!EventListeners.Contains(listener))
            {
                EventListeners.Add(listener);
            }
            
            TraverseEventSystemHandlerComponents(listener, RegisterHandler);
        }

        /// <inheritdoc />
        public virtual void Unregister(GameObject listener)
        {
            // For backward compatibility
            if (EventListeners.Contains(listener))
            {
                EventListeners.Remove(listener);
            }

            TraverseEventSystemHandlerComponents(listener, UnregisterHandler);
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
                postponedActions.Add((Action.Remove, handlerType, handler));
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
                postponedActions.Add((Action.Add, handlerType, handler));
            }
        }

        private void AddHandlerToMap(Type handlerType, IEventSystemHandler handler)
        {
            HashSet<IEventSystemHandler> handlers;

            if (!EventHandlersByType.TryGetValue(handlerType, out handlers))
            {
                handlers = new HashSet<IEventSystemHandler> { handler };
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
            HashSet<IEventSystemHandler> handlers;

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

        private void TraverseEventSystemHandlerHierarchy<T>(IEventSystemHandler handler, Action<Type, IEventSystemHandler> func) where T : IEventSystemHandler
        {
            var handlerType = typeof(T);

            func(handlerType, handler);

            foreach (var iface in handlerType.GetInterfaces())
            {
                if (!iface.Equals(eventSystemHandlerType))
                {
                    func(iface, handler);
                }
            }
        }

        private void TraverseComponentHandlerHierarchy<T>(T component, Action<Type, IEventSystemHandler> func) where T : IEventSystemHandler
        {
            foreach (var iface in component.GetType().GetInterfaces())
            {
                if (eventSystemHandlerType.IsAssignableFrom(iface) && !eventSystemHandlerType.Equals(iface))
                {
                    func(iface, component);
                }
            }
        }

        private void TraverseEventSystemHandlerComponents(GameObject obj, Action<Type, IEventSystemHandler> func)
        {
            foreach (var component in obj.GetComponents<IEventSystemHandler>())
            {
                // Call a function on every interface which inherits from IEventSystemHandler, of every component.
                // This will register all event handlers, which could potentially be called by UnityEngine.EventSystems.ExecuteEvents.Execute.
                // which was previously used to handle events.
                TraverseComponentHandlerHierarchy(component, func);
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
