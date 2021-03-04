// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// Base class for event receivers.
    /// </summary>
    public abstract class BaseEventReceiver
    {
        /// <summary>
        /// Constructor for an event receiver.
        /// </summary>
        /// <param name="eventConfiguration">The associated serialized event configuration for an event receiver.</param>
        public BaseEventReceiver(BaseInteractionEventConfiguration eventConfiguration)
        {
            EventConfiguration = eventConfiguration;
            StateName = EventConfiguration.StateName;        
        }

        /// <summary>
        /// The event configuration for this event receiver. 
        /// </summary>
        public IStateEventConfig EventConfiguration { get; protected set; } = null;

        /// <summary>
        /// The name of the state this event receiver is watching.
        /// </summary>
        public string StateName { get; protected set; } = null;


        /// <summary>
        /// Update an event receiver. 
        /// </summary>
        public abstract void OnUpdate(StateManager stateManager, BaseEventData eventData);
   
    }
}
