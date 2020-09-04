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
    /// Class for managing the events states within an Interaction Element.
    /// </summary>
    public class EventReceiverManager
    {
        public EventReceiverManager(StateManager interactiveStateManager)
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
                    // This case if for if a component is created via script instead of initialized in the inspector
                    if (state.EventConfiguration == null)
                    {
                        state.EventConfiguration = SetEventConfiguration(state.Name);
                    }

                    // Initialize runtime event receiver classes for states that have an event configuration 
                    BaseEventReceiver eventReceiver = InitializeEventReceiver(state.Name);

                    EventReceivers.Add(eventReceiver);
                }             
            }
        }

        /// <summary>
        /// Invoke a state event with its associated event data.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <param name="eventData">The event data for the state event</param>
        public void InvokeStateEvent(string stateName, BaseEventData eventData)
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
            // Find the event receiver that has the state name in it and return the configuration
            BaseEventReceiver eventReceiver = EventReceivers.Find((receiver) => receiver.Name.StartsWith(stateName));

            if (eventReceiver == null)
            {
                Debug.LogError($"An event configuration for the {stateName} state does not exist");
            }

            return eventReceiver.EventConfiguration;
        }

        // Create an instance of an event configuration scriptable object for a state
        private BaseInteractionEventConfiguration SetEventConfiguration(string stateName)
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

                return receiver;
            }

            return null;
        }

        // Check if a state has an existing and valid event configuration
        private bool IsEventConfigurationValid(string stateName)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(stateName));

            if (eventConfigType.IsNull())
            {
                return false;
            }

            return true;
        }
    }
}
