// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Base Event System that can be inherited from to give other system features event capabilities.
    /// </summary>
    public abstract class BaseEventSystem : BaseService, IMixedRealityEventSystem
    {
        // Utility flag controlling error messages in 'Destroy' method for reporting dangling event handlers.
        // This may generate false warnings in usual Unity play mode due to arbitrary order 
        // of disabling and destroying components. It is enabled by tests and can be enabled for debugging purposes.
        // Variable is static to be shared between all event system instances.
        public static bool enableDanglingHandlerDiagnostics = false;

        // Tracks the number of HandleEvent calls in flight - while HandleEvent is happening,
        // set of registered listeners isn't safe to modify because doing so would cause an
        // update on a collection that is being iterated over. Note that this also could be worked
        // around by snapshotting the listener collection prior to making callouts, but this would
        // also incur memory allocation on each event.
        private int eventExecutionDepth = 0;
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

        public struct EventHandlerEntry
        {
            public IEventSystemHandler handler;

            // Cached value, whether handler is implemented by a unity component, and this component's object
            // is in EventListeners collection.
            // Cached for performance reasons.
            public bool parentObjectIsInObjectCollection;

            public EventHandlerEntry(IEventSystemHandler h, bool isParentListener = false)
            {
                handler = h;
                parentObjectIsInObjectCollection = isParentListener;
            }

            // For better diagnostics in Unit tests
            public override string ToString()
            {
                return $"{handler}. Parent object registered: {parentObjectIsInObjectCollection}";
            }
        }

        /// <summary>
        /// List of all event handlers grouped by type that are registered to this Event System.
        /// </summary>
        public Dictionary<Type, List<EventHandlerEntry>> EventHandlersByType { get; } = new Dictionary<Type, List<EventHandlerEntry>>();

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
                // Ensure client code does not put the event dispatch system into a bad state.
                // Note that ExecuteEvents.Execute internally safeguards against exceptions, but
                // this is another layer to ensure that nothing below this layer can affect the state
                // of our eventExecutionDepth tracker.
                try
                {
                    ExecuteEvents.Execute(EventListeners[i], eventData, eventHandler);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            // Send events to all handlers registered via RegisterHandler API.
            if (EventHandlersByType.TryGetValue(typeof(T), out List<EventHandlerEntry> handlers))
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    var handlerEntry = handlers[i];

                    // If handler's parent is in object collection (traversed above), it has already received an event.
                    if (handlerEntry.parentObjectIsInObjectCollection)
                    {
                        continue;
                    }

                    // Ensure client code does not put the event dispatch system into a bad state.
                    try
                    {
                        eventHandler.Invoke((T)handlerEntry.handler, eventData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }

            eventExecutionDepth--;

            if (eventExecutionDepth == 0)
            {
                int postponedActionsCount = postponedActions.Count;
                int postponedObjectActionsCount = postponedObjectActions.Count;

                if (postponedActionsCount <= 0 && postponedObjectActionsCount <= 0)
                {
                    return;
                }

                for (int i = 0; i < postponedActionsCount; i++)
                {
                    Tuple<Action, Type, IEventSystemHandler> handler = postponedActions[i];
                    if (handler.Item1 == Action.Add)
                    {
                        AddHandlerToMap(handler.Item2, handler.Item3);
                    }
                    else if (handler.Item1 == Action.Remove)
                    {
                        RemoveHandlerFromMap(handler.Item2, handler.Item3);
                    }
                }

                for (int i = 0; i < postponedObjectActionsCount; i++)
                {
                    Tuple<Action, GameObject> obj = postponedObjectActions[i];
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

                postponedActions.Clear();
                postponedObjectActions.Clear();
            }
        }

        /// <inheritdoc />
        public virtual void RegisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            if (handler == null)
            {
                return;
            }

            // #if due to Microsoft.MixedReality.Toolkit.ReflectionExtensions overload of Type.IsInterface
#if WINDOWS_UWP && !ENABLE_IL2CPP
            Debug.Assert(typeof(T).IsInterface(), "RegisterHandler must be called with an interface as a generic parameter.");
#else
            Debug.Assert(typeof(T).IsInterface, "RegisterHandler must be called with an interface as a generic parameter.");
#endif
            Debug.Assert(typeof(T).IsAssignableFrom(handler.GetType()), "Handler passed to RegisterHandler doesn't implement a type given as generic parameter.");

            DebugUtilities.LogVerboseFormat("Registering handler {0} against system {1}", handler.ToString().Trim(), this);

            TraverseEventSystemHandlerHierarchy<T>(handler, RegisterHandler);
        }

        /// <inheritdoc />
        public virtual void UnregisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler
        {
            if (handler == null)
            {
                return;
            }

            // #if due to Microsoft.MixedReality.Toolkit.ReflectionExtensions overload of Type.IsInterface
#if WINDOWS_UWP && !ENABLE_IL2CPP
            Debug.Assert(typeof(T).IsInterface(), "UnregisterHandler must be called with an interface as a generic parameter.");
#else
            Debug.Assert(typeof(T).IsInterface, "UnregisterHandler must be called with an interface as a generic parameter.");
#endif
            Debug.Assert(typeof(T).IsAssignableFrom(handler.GetType()), "Handler passed to UnregisterHandler doesn't implement a type given as generic parameter.");

            DebugUtilities.LogVerboseFormat("Unregistering handler {0} against system {1}", handler, this);

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
                    // Due to how events are sent to game objects, if any of registered handlers sits on a 
                    // registered object it will receive any event passed to this object.
                    // We need to mark such handlers, so they don't receive their events twice.
                    // It can be checked in HandleEvent with less code, but this becomes a 
                    // performance bottleneck with many handlers in the system

                    bool report = false;
                    foreach (var typeEntry in EventHandlersByType)
                    {
                        for (int index = 0; index < typeEntry.Value.Count; index++)
                        {
                            var handlerEntry = typeEntry.Value[index];

                            var comp = handlerEntry.handler as Component;

                            if (comp != null && comp.gameObject == listener)
                            {
                                handlerEntry.parentObjectIsInObjectCollection = true;
                                typeEntry.Value[index] = handlerEntry;
                                report = true;
                            }
                        }
                    }

                    if (report)
                    {
                        WarnAboutConflictingApis(listener.name);
                    }

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
                    // Reset cached flags in handler collection as object will not intercept the events anymore.
                    // This is a slow loop, which is here to maintain backward compatibility and enable co-existing of
                    // new and old API.
                    foreach (var typeEntry in EventHandlersByType)
                    {
                        for (int index = 0; index < typeEntry.Value.Count; index++)
                        {
                            var handlerEntry = typeEntry.Value[index];

                            // if cache flag is true, handler is guaranteed to be a unity component.
                            if (handlerEntry.parentObjectIsInObjectCollection && (handlerEntry.handler as Component).gameObject == listener)
                            {
                                // Don't need to report, because it was reported during registration
                                handlerEntry.parentObjectIsInObjectCollection = false;
                                typeEntry.Value[index] = handlerEntry;
                            }
                        }
                    }

                    EventListeners.Remove(listener);
                }
            }
            else
            {
                postponedObjectActions.Add(Tuple.Create(Action.Remove, listener));
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (!enableDanglingHandlerDiagnostics)
            {
                return;
            }

            foreach (var listener in EventListeners)
            {
                Debug.LogError($"Event system {Name} is destroyed, while still having a registered listener. " +
                    "Make sure that all global event listeners have been unregistered before destroying the event system. " +
                    $"Dangling listener: object {listener.name}");
            }

            foreach (var typeEntry in EventHandlersByType)
            {
                for (int index = 0; index < typeEntry.Value.Count; index++)
                {
                    var handlerEntry = typeEntry.Value[index];
                    Debug.LogError($"Event system {Name} is being destroyed while still having a registered listener. " +
                        "Make sure that all global event listeners have been unregistered before destroying the event system. " +
                        $"Dangling listener: handler {handlerEntry.handler}");
                }
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
            bool isParentObjectRegistered = false;

            var componentHandler = handler as Component;
            if (componentHandler != null && EventListeners.Contains(componentHandler.gameObject))
            {
                isParentObjectRegistered = true;
                WarnAboutConflictingApis(componentHandler.gameObject.name);
            }

            List<EventHandlerEntry> handlers;
            if (!EventHandlersByType.TryGetValue(handlerType, out handlers))
            {
                handlers = new List<EventHandlerEntry> { new EventHandlerEntry(handler, isParentObjectRegistered) };
                EventHandlersByType.Add(handlerType, handlers);
                return;
            }

            bool handlerExists = false;
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].handler == handler)
                {
                    handlerExists = true;
                    break;
                }
            }

            if (!handlerExists)
            {
                handlers.Add(new EventHandlerEntry(handler, isParentObjectRegistered));
            }
        }

        /// <inheritdoc />
        private void RemoveHandlerFromMap(Type handlerType, IEventSystemHandler handler)
        {
            List<EventHandlerEntry> handlers;
            if (!EventHandlersByType.TryGetValue(handlerType, out handlers))
            {
                return;
            }

            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].handler == handler)
                {
                    handlers.RemoveAt(i);
                }
            }

            if (handlers.Count == 0)
            {
                EventHandlersByType.Remove(handlerType);
            }
        }

        #endregion Registration helpers

        #region Utilities

        private static readonly ProfilerMarker TraverseEventSystemHandlerHierarchyPerfMarker = new ProfilerMarker("[MRTK] BaseEventSystem.TraverseEventSystemHandlerHierarchy");

        /// <summary>
        /// Utility function for registering parent interfaces of a given handler.
        /// </summary>
        /// <remarks>
        /// <para>Event handler interfaces may derive from each other. Some events will be raised using a base handler class, and are supposed to trigger on
        /// all derived handler classes too. Example of that is IMixedRealityBaseInputHandler hierarchy.
        /// To support that current implementation registers multiple dictionary entries per handler, one for each level of event handler hierarchy.
        /// Alternative would be to register just one root type and 
        /// then determine which handlers to call dynamically in 'HandleEvent'.
        /// Implementation was chosen based on performance of 'HandleEvent'. Without determining type it is about 2+ times faster.
        /// There are possible ways to bypass that, but this will make implementation of classes 
        /// that derive from Input System unnecessarily more complicated.</para>
        /// </remarks>
        private void TraverseEventSystemHandlerHierarchy<T>(IEventSystemHandler handler, Action<Type, IEventSystemHandler> func) where T : IEventSystemHandler
        {
            using (TraverseEventSystemHandlerHierarchyPerfMarker.Auto())
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
        }

        private void WarnAboutConflictingApis(string objectName)
        {
            Debug.LogError("Detected simultaneous usage of IMixedRealityEventSystem.Register and IMixedRealityEventSystem.RegisterHandler " +
                $"on the same game object '{objectName}' for global input events registration. This is a compatibility behavior which might " +
                "cause performance issues. It is recommended to remove or replace usages of 'Register/Unregister' methods with 'RegisterHandler/UnregisterHandler'.");
        }

        #endregion Utilities

        // Example Event Pattern #############################################################

        // public void RaiseGenericEvent(IEventSource eventSource)
        // {
        //     genericEventData.Initialize(eventSource);
        //     HandleEvent(genericEventData, GenericEventHandler);
        // }

        // private static readonly ExecuteEvents.EventFunction<IEventHandler> GenericEventHandler =
        //     delegate (IEventHandler handler, BaseEventData eventData)
        //     {
        //         var casted = ExecuteEvents.ValidateEventData<GenericBaseEventData>(eventData);
        //         handler.OnEventRaised(casted);
        //     };

        // Example Event Pattern #############################################################
    }
}
