// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
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
        }

        // The state manager for this event receiver manager
        private StateManager stateManager = null;

        /// <summary>
        /// List of active event receivers for the state events.
        /// </summary>
        public List<BaseEventReceiver> EventReceivers { get; protected set; } = new List<BaseEventReceiver>();

        /// <summary>
        /// Initialize the event receivers for each state that has an existing event configuration.
        /// </summary>
        internal void InitializeEventReceivers()
        {
            foreach (InteractionState state in stateManager.States)
            {
                // Initialize Event Configurations for each state if the configuration exists
                if (IsEventConfigurationValid(state.Name))
                {
                    // If an interactive element component is created via script instead of initialized in the inspector,
                    // an instance of the event config scriptable needs to be created
                    if (state.EventConfiguration == null)
                    {
                        state.EventConfiguration = CreateEventConfigurationInstance(state.Name);
                    }

                    // Initialize runtime event receiver classes for states that have an event configuration 
                    InitializeEventReceiver(state.Name);
                }
            }
        }

        /// <summary>
        /// Invoke a state event with optional event data.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <param name="eventData">The event data for the state event</param>
        public void InvokeStateEvent(string stateName, BaseEventData eventData = null)
        {
            BaseEventReceiver receiver = EventReceivers.Find((eventReceiver) => eventReceiver.Name.StartsWith(stateName));

            if (receiver != null)
            {
                receiver.OnUpdate(stateManager, eventData);
            }
        }

        /// <summary>
        /// Get the event configuration of a state.
        /// </summary>
        /// <param name="stateName">The name of the state that contains the event configuration to be retrieved</param>
        /// <returns>The Interaction Event Configuration of the state</returns>
        public BaseInteractionEventConfiguration GetEventConfiguration(string stateName)
        {
            InteractionState state = stateManager.GetState(stateName);

            if (state == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }

            var eventConfig = state.EventConfiguration;

            return eventConfig;
        }

        /// <summary>
        /// Sets the event configuration for a given state. This method checks if the state has a valid associated event configuration, creates
        /// an instance of the event configuration scriptable object, and initializes the matching runtime class. 
        /// </summary>
        /// <param name="state">This state's event configuration will be set</param>
        /// <returns>The set event configuration for the state.</returns>
        public BaseInteractionEventConfiguration SetEventConfiguration(InteractionState state)
        {
            var eventConfiguration = CreateEventConfigurationInstance(state.Name);

            if (eventConfiguration != null)
            {
                state.EventConfiguration = eventConfiguration;
                InitializeEventReceiver(state.Name);
            }
            else
            {
                Debug.Log($"The event configuration for the {state.Name} was not set because the {state.Name}InteractionEventConfiguration file does not exist.");
            }

            return eventConfiguration;
        }

        // Create an instance of an event configuration scriptable object for a state
        private BaseInteractionEventConfiguration CreateEventConfigurationInstance(string stateName)
        {
            if (IsEventConfigurationValid(stateName))
            {
                var eventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(stateName + "InteractionEventConfiguration");
                return eventConfiguration;
            }
            else
            {
                return null;
            }
        }

        // Initialize an event receiver for a state
        private BaseEventReceiver InitializeEventReceiver(string stateName)
        {
            if (IsEventConfigurationValid(stateName))
            {
                BaseEventReceiver receiver = stateManager.GetState(stateName).EventConfiguration.InitializeRuntimeEventReceiver();

                EventReceivers.Add(receiver);

                return receiver;
            }

            return null;
        }

        // Check if a state has an existing and valid event configuration
        private bool IsEventConfigurationValid(string stateName)
        {
            // Check if there is an existing subclass that starts with the state name
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            bool isValidEventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(stateName)) != null;
            
            return isValidEventConfigType;
        }
    }
}
