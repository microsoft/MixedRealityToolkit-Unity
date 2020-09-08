// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for event receivers.
    /// </summary>
    public abstract class BaseEventReceiver
    {
        /// <summary>
        /// Constructor for an event receiver.
        /// </summary>
        /// <param name="eventConfiguration">The associated event configuration scriptable for an event receiver.</param>
        /// <param name="name">The name of the event receiver. The name of an event receiver is the state name + "Receiver".</param>
        public BaseEventReceiver(BaseInteractionEventConfiguration eventConfiguration, string name)
        {
            EventConfiguration = eventConfiguration;
            Name = name;
        }

        /// <summary>
        /// The name of the event receiver. The event receiver is named after the associated state. For example, the 
        /// event receiver for the Focus state is named "FocusReceiver".
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// The event configuration for this event receiver.  The event configuration is a scriptable object that contains
        /// state events. 
        /// </summary>
        public BaseInteractionEventConfiguration EventConfiguration { get; protected set; } = null;

        /// <summary>
        /// Update an event receiver. 
        /// </summary>
        public abstract void OnUpdate(StateManager state, BaseEventData eventData);
    }
}
