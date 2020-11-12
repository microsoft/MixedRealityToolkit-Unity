// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Manages the events contained in states within an Interaction Element.
    /// </summary>
    public class EventReceiverManager
    {
        /// <summary>
        /// Constructor for the event receiver manager. 
        /// </summary>
        /// <param name="interactiveStateManager">The state manager for this event receiver manager</param>
        internal EventReceiverManager(StateManager interactiveStateManager)
        {
            stateManager = interactiveStateManager;
            InitializeEventReceivers();
        }

        /// <summary>
        /// Dictionary of active event receivers for the state events.
        /// </summary>
        public Dictionary<string, BaseEventReceiver> EventReceivers { get; protected set; } = new Dictionary<string, BaseEventReceiver>();

        // The state manager for this event receiver manager
        private StateManager stateManager = null;

        /// <summary>
        /// Initialize the event receivers for each state.
        /// </summary>
        internal void InitializeEventReceivers()
        {
            foreach (KeyValuePair<string,InteractionState> state in stateManager.States)
            {
                // If an interactive element component is created via script instead of initialized in the inspector,
                // an instance of the event configuration needs to be created
                if (state.Value.EventConfiguration == null)
                {
                    state.Value.EventConfiguration = CreateEventConfigurationInstance(state.Key);
                }

                // Initialize runtime event receiver classes for the states 
                InitializeAndAddEventReceiver(state.Key);  
            }
        }

        /// <summary>
        /// Invoke a state event with optional event data.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <param name="eventData">The event data for the state event (optional)</param>
        public void InvokeStateEvent(string stateName, BaseEventData eventData = null)
        {
            BaseEventReceiver receiver = EventReceivers[stateName];

            if (receiver != null)
            {
                receiver.OnUpdate(stateManager, eventData);
            }
            else
            {
                Debug.LogError($"The event receiver for the {stateName} state does not exist");
            }
        }

        /// <summary>
        /// Get the event configuration of a state.
        /// </summary>
        /// <param name="stateName">The name of the state that contains the event configuration to be retrieved</param>
        /// <returns>The Interaction Event Configuration of the state</returns>
        internal BaseInteractionEventConfiguration GetEventConfiguration(string stateName)
        {
            InteractionState state = stateManager.GetState(stateName);

            if (state == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }

            var eventConfig = state.EventConfiguration;

            return (BaseInteractionEventConfiguration)eventConfig;
        }

        /// <summary>
        /// Sets the event configuration for a given state. This method checks if the state has a valid associated event configuration, creates
        /// an instance of the event configuration class, and initializes the matching runtime class. 
        /// </summary>
        /// <param name="state">This state's event configuration will be set</param>
        /// <returns>The set event configuration for the state.</returns>
        internal BaseInteractionEventConfiguration SetEventConfiguration(InteractionState state)
        {
            var eventConfiguration = CreateEventConfigurationInstance(state.Name);

            if (eventConfiguration != null)
            {
                state.EventConfiguration = eventConfiguration;

                InitializeAndAddEventReceiver(state.Name);
            }
            else
            {
                Debug.Log($"The event configuration for the {state.Name} was not set.");
            }

            return eventConfiguration;
        }

        // Create an instance of an event configuration for a state
        private BaseInteractionEventConfiguration CreateEventConfigurationInstance(string stateName)
        {
            BaseInteractionEventConfiguration eventConfiguration;

            // Check if the state has an associated event configuration by state name.
            // For example, the Focus state is associated with the FocusEvents class which has BaseInteractionEventConfiguration as its base class.
            // The FocusEvents class contains unity events with FocusEventData.
            // This pattern continues with states that have events with specific event data, i.e. the Touch state
            // is associated with the serialized class TouchEvents which contians unity events with TouchEventData
            var eventConfigTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            Type eventConfigType = eventConfigTypes.Find((type) => type.Name.StartsWith(stateName));

            if (eventConfigType != null)
            {
                eventConfiguration = Activator.CreateInstance(eventConfigType) as BaseInteractionEventConfiguration;
            }
            else
            {
                // If a state does not have an associated event configuration class, then create an instance of the 
                // StateEvents class which contains the OnStateOn and OnStateOff unity events. These unity events do not have event data.
                eventConfiguration = Activator.CreateInstance(typeof(StateEvents)) as BaseInteractionEventConfiguration;
            }

            eventConfiguration.StateName = stateName;

            return eventConfiguration;
        }

        // Initialize a runtime event receiver via the state's event configuration and add it to the event receiver dictionary
        private BaseEventReceiver InitializeAndAddEventReceiver(string stateName)
        {
            InteractionState state = stateManager.GetState(stateName);

            BaseInteractionEventConfiguration eventConfiguration = (BaseInteractionEventConfiguration)state.EventConfiguration;
            
            // Find the associated event receiver for the state if it has one
            var eventReceiverTypes = TypeCacheUtility.GetSubClasses<BaseEventReceiver>();
            Type eventReceiver = eventReceiverTypes.Find((type) => type.Name.StartsWith(stateName));

            if (eventReceiver != null)
            {
                eventConfiguration.EventReceiver = Activator.CreateInstance(eventReceiver, new object[] { eventConfiguration }) as BaseEventReceiver;
            }
            else
            {
                eventConfiguration.EventReceiver = Activator.CreateInstance(typeof(StateReceiver), new object[] { eventConfiguration }) as BaseEventReceiver;
            }

            EventReceivers.Add(stateName, eventConfiguration.EventReceiver);

            return eventConfiguration.EventReceiver;
        }
    }
}
