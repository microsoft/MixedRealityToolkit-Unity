// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface used to implement an Event System that is compatible with the Mixed Reality Toolkit.
    /// </summary>
    public interface IMixedRealityEventSystem : IMixedRealityService
    {
        /// <summary>
        /// List of event listeners that are registered to this Event System.
        /// </summary>
        [Obsolete("EventListeners is replaced by HandlerEventListeners and will be removed in a future release.")]
        List<GameObject> EventListeners { get; }

        /// <summary>
        /// List of event handlers that are registered to this Event System.
        /// </summary>
        Dictionary<Type, HashSet<IEventSystemHandler>> HandlerEventListeners { get; }

        /// <summary>
        /// The main function for handling and forwarding all events to their intended recipients.
        /// </summary>
        /// <remarks>See: https://docs.unity3d.com/Manual/MessagingSystem.html </remarks>
        /// <typeparam name="T">Event Handler Interface Type</typeparam>
        /// <param name="eventData">Event Data</param>
        /// <param name="eventHandler">Event Handler delegate</param>
        void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler;

        /// <summary>
        /// Registers a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to listen for events from this Event System.
        /// </summary>
        /// <param name="listener"><see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to add to <see cref="EventListeners"/>.</param>
        [Obsolete("Register using a game object causes all components of this object to receive global events of all types. " +
            "Use RegisterHandler<> methods instead to avoid unexpected behavior.")]
        void Register(GameObject listener);

        /// <summary>
        /// Unregisters a <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> from listening for events from this Event System.
        /// </summary>
        /// <param name="listener"><see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to remove from <see cref="EventListeners"/>.</param>
        [Obsolete("Unregister using a game object will disable listening of global events for all components of this object. " +
            "Use UnregisterHandler<> methods instead to avoid unexpected behavior.")]
        void Unregister(GameObject listener);

        /// <summary>
        /// Registers an event Handler of a given type to listen for its events from this Event System.
        /// </summary>
        /// <remarks>
        /// Should be called with explicit template type like RegisterHandler<ISpeechEventHandler>(this)
        /// </remarks>
        /// <param name="handler">Handler to add to <see cref="HandlerEventListeners"/>.</param>
        void RegisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler;

        /// <summary>
        /// Registers all event handlers, implemented by a given component in this Event System.
        /// </summary>
        /// <remarks>
        /// Should be called without explicit template type like RegisterAllHandlers(this)
        /// </remarks>
        /// <param name="handler">Handler to add to <see cref="HandlerEventListeners"/>.</param>
        void RegisterAllHandlers<T>(T component) where T : IEventSystemHandler;

        /// <summary>
        /// Unregisters an event Handler of a given type from listening for its events from this Event System.
        /// </summary>
        /// <remarks>
        /// Should be called with explicit template type like UnregisterHandler<ISpeechEventHandler>(this)
        /// </remarks>
        /// <param name="handler">Handler to remove from <see cref="HandlerEventListeners"/>.</param>
        void UnregisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler;

        /// <summary>
        /// Unregisters all event handlers, implemented by a given component in this Event System.
        /// </summary>
        /// <remarks>
        /// Should be called without explicit template type like UnregisterAllHandlers(this)
        /// </remarks>
        /// <param name="handler">Handler to add to <see cref="HandlerEventListeners"/>.</param>
        void UnregisterAllHandlers<T>(T component) where T : IEventSystemHandler;
    }
}