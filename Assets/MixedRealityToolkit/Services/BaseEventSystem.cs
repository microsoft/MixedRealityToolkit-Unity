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
        #region IMixedRealityEventSystem Implementation

        private static int eventExecutionDepth = 0;
        private readonly Type eventSystemHandlerType = typeof(IEventSystemHandler);

        /// <inheritdoc />
        public List<GameObject> EventListeners { get; } = new List<GameObject>();

        public Dictionary<Type, HashSet<IEventSystemHandler>> HandlerListeners { get; } = new Dictionary<Type, HashSet<IEventSystemHandler>>();

        private List<(Type, IEventSystemHandler)> handlersToAdd    = new List<(Type, IEventSystemHandler)>();
        private List<(Type, IEventSystemHandler)> handlersToRemove = new List<(Type, IEventSystemHandler)>();

        /// <inheritdoc />
        public virtual void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler
        {
            Debug.Assert(!eventData.used);

            HashSet<IEventSystemHandler> handlers;
            if (!HandlerListeners.TryGetValue(typeof(T), out handlers))
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

            if(eventExecutionDepth == 0)
            {
                foreach(var handler in handlersToRemove)
                {
                    Unregister(handler.Item1, handler.Item2);
                }

                foreach (var handler in handlersToAdd)
                {
                    Register(handler.Item1, handler.Item2);
                }
            }
        }

        public virtual void RegisterHandler<T>(T handler) where T : IEventSystemHandler
        {
            RegisterHandler(typeof(T), handler);

            foreach (var iface in typeof(T).GetInterfaces())
            {
                if (!iface.Equals(typeof(IEventSystemHandler)))
                {
                    RegisterHandler(iface, handler);
                }
            }
        }

        private void RegisterHandler(Type handlerType, IEventSystemHandler handler)
        {
            if (eventExecutionDepth == 0)
            {
                Register(handlerType, handler);
            }
            else
            {
                handlersToAdd.Add((handlerType, handler));
            }
        }

        private void Register(Type handlerType, IEventSystemHandler handler)
        {
            HashSet<IEventSystemHandler> handlers;

            if (!HandlerListeners.TryGetValue(handlerType, out handlers))
            {
                handlers = new HashSet<IEventSystemHandler> { handler };
                HandlerListeners.Add(handlerType, handlers);
                return;
            }

            if(!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }
        public virtual void UnregisterHandler<T>(T handler) where T : IEventSystemHandler
        {
            UnregisterHandler(typeof(T), handler);

            foreach (var iface in typeof(T).GetInterfaces())
            {
                if (!iface.Equals(eventSystemHandlerType))
                {
                    UnregisterHandler(iface, handler);
                }
            }
        }

        private void UnregisterHandler(Type handlerType, IEventSystemHandler handler)
        {
            if (eventExecutionDepth == 0)
            {
                Unregister(handlerType, handler);
            }
            else
            {
                handlersToRemove.Add((handlerType, handler));
            }
        }

        /// <inheritdoc />
        private void Unregister(Type handlerType, IEventSystemHandler handler)
        {
            HashSet<IEventSystemHandler> handlers;

            if (!HandlerListeners.TryGetValue(handlerType, out handlers))
            {
                return;
            }

            if(handlers.Contains(handler))
            {
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    HandlerListeners.Remove(handlerType);
                }
            }
        }

        public virtual void Register(GameObject listener)
        {
            if (!EventListeners.Contains(listener))
            {
                EventListeners.Add(listener);
            }

            foreach (var component in listener.GetComponents<IEventSystemHandler>())
            {
                var ifaces = component.GetType().GetInterfaces();
                foreach (var iface in ifaces)
                {
                    if (eventSystemHandlerType.IsAssignableFrom(iface) && !eventSystemHandlerType.Equals(iface))
                    {
                        RegisterHandler(iface, component);
                    }
                }
            }
        }

        public virtual void Unregister(GameObject listener)
        {
            if (EventListeners.Contains(listener))
            {
                EventListeners.Remove(listener);
            }

            foreach (var component in listener.GetComponents<IEventSystemHandler>())
            {
                var ifaces = component.GetType().GetInterfaces();
                foreach (var iface in ifaces)
                {
                    if (eventSystemHandlerType.IsAssignableFrom(iface) && !eventSystemHandlerType.Equals(iface))
                    {
                        UnregisterHandler(iface, component);
                    }
                }
            }
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
