// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        /// <remarks>
        /// This collection is obsolete and is replaced by handler-based internal storage. It will be removed in a future release.
        /// </remarks>
        List<GameObject> EventListeners { get; }

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
        /// Registers the given handler as a global listener for all events handled via the T interface.
        /// T must be an interface type, not a class type, derived from IEventSystemHandler.
        /// </summary>
        /// <remarks>
        /// <para>If you want to register a single C# object as global handler for several event handling interfaces,
        /// you must call this function for each interface type.</para>
        /// </remarks>
        /// <param name="handler">Handler to receive global input events of specified handler type.</param>
        void RegisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler;

        /// <summary>
        /// Unregisters the given handler as a global listener for all events handled via the T interface.
        /// T must be an interface type, not a class type, derived from IEventSystemHandler.
        /// </summary>
        /// <remarks>
        /// <para>If a single C# object listens to global input events for several event handling interfaces,
        /// you must call this function for each interface type.</para>
        /// </remarks>
        /// <param name="handler">Handler to stop receiving global input events of specified handler type.</param>
        void UnregisterHandler<T>(IEventSystemHandler handler) where T : IEventSystemHandler;
    }
}
