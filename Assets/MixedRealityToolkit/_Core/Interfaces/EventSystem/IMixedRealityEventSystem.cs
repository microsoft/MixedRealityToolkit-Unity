// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Events
{
    /// <summary>
    /// Interface used to implement an Event System that is compatible with the Mixed Reality Toolkit.
    /// </summary>
    public interface IMixedRealityEventSystem : IMixedRealityService
    {
        /// <summary>
        /// List of event listeners that are registered to this Event System.
        /// </summary>
        List<GameObject> EventListeners { get; }

        /// <summary>
        /// The main function for handling and forwarding all events to their intended recipients.
        /// <para><remarks>See: https://docs.unity3d.com/Manual/MessagingSystem.html </remarks></para>
        /// </summary>
        /// <typeparam name="T">Event Handler Interface Type</typeparam>
        /// <param name="eventData">Event Data</param>
        /// <param name="eventHandler">Event Handler delegate</param>
        void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler) where T : IEventSystemHandler;

        /// <summary>
        /// Registers a <see cref="GameObject"/> to listen for events from this Event System.
        /// </summary>
        /// <param name="listener"><see cref="GameObject"/> to add to <see cref="EventListeners"/>.</param>
        void Register(GameObject listener);

        /// <summary>
        /// Unregisters a <see cref="GameObject"/> from listening for events from this Event System.
        /// </summary>
        /// <param name="listener"><see cref="GameObject"/> to remove from <see cref="EventListeners"/>.</param>
        void Unregister(GameObject listener);
    }
}