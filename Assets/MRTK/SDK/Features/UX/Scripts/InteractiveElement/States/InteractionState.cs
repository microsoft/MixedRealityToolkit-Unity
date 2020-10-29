// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The container that represents a single Interaction State. This class is utilited in the BaseInteractiveElement MonoBehaviour.
    /// </summary>
    [System.Serializable]
    public class InteractionState
    {
        /// <summary>
        /// Constructor for an Interaction State.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        public InteractionState(string stateName)
        {
            Name = stateName;
            SetEventConfiguration(Name);
        }

        [SerializeField]
        [Tooltip("The name of the state")]
        private string stateName;

        /// <summary>
        /// The name of the state.
        /// </summary>
        public string Name
        {
            get => stateName;
            internal set => stateName = value;
        }

        [SerializeField]
        [Tooltip("The value of the state. The value will be 0 if the state is off, 1 if the state is on.")]
        private int stateValue = 0;

        /// <summary>
        /// The value of the state. The value will be 0 if the state is off, 1 if the state is on.
        /// </summary>
        public int Value
        {
            get => stateValue;
            internal set => stateValue = value;
        }

        [SerializeField]
        [Tooltip("Whether or not the state is currently active.")]
        private bool active = false;

        /// <summary>
        /// Whether or not the state is currently active.
        /// </summary>
        public bool Active
        {
            get => active;
            internal set => active = value;
        }

        [SerializeReference]
        [Tooltip("The event configuration for this state. ")]
        private IStateEventConfig eventConfiguration = null;

        /// <summary>
        /// The event configuration for this state. 
        /// </summary>
        public IStateEventConfig EventConfiguration
        {
            get => eventConfiguration;
            internal set => eventConfiguration = value;
        }

        // Set the event configuration for a new state
        private void SetEventConfiguration(string stateName)
        {
            if (EventConfiguration == null)
            {
                BaseInteractionEventConfiguration eventConfiguration;

                // Find matching event configuration by state name
                var eventConfigTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
                Type eventConfigType = eventConfigTypes.Find((type) => type.Name.StartsWith(stateName));

                if (eventConfigType != null)
                {
                    // If a state has an associated event configuration class, then create an instance with the matching type
                    eventConfiguration = Activator.CreateInstance(eventConfigType) as BaseInteractionEventConfiguration;
                }
                else
                {
                    // If the state does not have a specific event configuration class type, create the an instance of StateEvents.  
                    // StateEvents is the default type for a state's event configuration when a matching event configuration type does not exist.
                    eventConfiguration = Activator.CreateInstance(typeof(StateEvents)) as BaseInteractionEventConfiguration;
                }

                eventConfiguration.StateName = stateName;
                EventConfiguration = eventConfiguration;
            }
        }
    }
}