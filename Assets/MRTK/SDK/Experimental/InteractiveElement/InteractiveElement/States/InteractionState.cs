// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The container that represents a single Interaction State. This class is utilized by the BaseInteractiveElement MonoBehaviour.
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
            SetInteractionType(Name);
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

        [SerializeField]
        [Tooltip("The type of interaction (Near, Far, Both, Other) this state is associated with.")]
        private InteractionType interactionType = InteractionType.Other;

        /// <summary>
        /// The type of interaction (Near, Far, Both, None) this state is associated with.
        /// </summary>
        public InteractionType InteractionType
        {
            get => interactionType;
            internal set => interactionType = value;
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

        private const string Near = "Near";
        private const string Far = "Far";

        // List of all core state names
        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        // Set the event configuration for a new state
        internal void SetEventConfiguration(string stateName)
        {
            if (EventConfiguration == null)
            {
                BaseInteractionEventConfiguration eventConfiguration;

                string subStateName = GetSubStateName();

                // Find matching event configuration by state name
                var eventConfigTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();

                Type eventConfigType;

                try
                {
                    eventConfigType = eventConfigTypes?.Find((type) => type.Name.StartsWith(subStateName));

                }
                catch
                {
                    eventConfigType = null;
                }

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

        // Set the InteractionType for a state based on the state name 
        internal void SetInteractionType(string stateName)
        {
            string touchStateName = CoreInteractionState.Touch.ToString();
            string focusStateName = CoreInteractionState.Focus.ToString();

            if (stateName.Contains(Far))
            {
                InteractionType = InteractionType.Far;
            }
            // The Touch state is a special case because it does not contain "Near" in the state name but 
            // the InteractionType is Near
            else if (stateName.Contains(Near) || stateName == touchStateName)
            {
                InteractionType = InteractionType.Near;
            }
            // Special case for Focus as that state supports near and far interaction without "Near" or "Far" in the name
            else if (stateName == focusStateName)
            {
                InteractionType = InteractionType.NearAndFar;
            }
            else 
            {
                InteractionType = InteractionType.Other;
            }  
        }

        // Trim the name of a state if it contains "Near" or "Far" if the current state contains "Focus" in the name
        internal string GetSubStateName()
        {
            string focusStateName = CoreInteractionState.Focus.ToString();

            string subStateName = Name;

            if (subStateName.Contains(focusStateName))
            {
                // If the state name contains Near, then remove "Near" and return the remaining sub-string
                if (subStateName.Contains(Near))
                {
                    subStateName = stateName.Replace(Near, "");
                }
                else if (stateName.Contains(Far))
                {
                    subStateName = stateName.Replace(Far, "");
                }
            }

            return subStateName;
        }
    }
}