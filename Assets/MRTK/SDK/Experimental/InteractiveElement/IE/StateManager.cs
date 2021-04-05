// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// Manages the state values of Interaction States within BaseInteractiveElement's States list. This class contains helper
    /// methods for setting, getting and creating new Interaction States for the States list.
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// Create a new state manager with a given states scriptable object.
        /// </summary>
        /// <param name="states">List of Interaction States for this state manager to watch</param>
        /// <param name="interactiveElement">The interactive element source</param>
        public StateManager(List<InteractionState> states, BaseInteractiveElement interactiveElementSource)
        {
            interactionStates = states;

            // Add the list of InteractionStates to an internal dictionary
            foreach (InteractionState state in states)
            {
                statesDictionary.Add(state.Name, state);
            }

            InteractiveElement = interactiveElementSource;

            // Create a new event receiver manager for this state manager
            EventReceiverManager = new EventReceiverManager(this);

            // Add listeners to the OnStateActivated and OnStateDeactivated events
            AddStateEventListeners();
        }

        /// <summary>
        /// The Event Receiver Manager for this State Manager. Each state can contain an event configuration scriptable which defines
        /// the events associated with the state.  The Event Receiver Manager depends on a State Manager.
        /// </summary>
        public EventReceiverManager EventReceiverManager { get; internal set; } = null;

        /// <summary>
        /// The Unity Event with the activated state as the event data. This event is invoked when a state is 
        /// set to on.
        /// </summary>
        public InteractionStateActiveEvent OnStateActivated { get; protected set; } = new InteractionStateActiveEvent();

        /// <summary>
        /// The Unity Event with the previous active state and the current active state.  The event is invoked when 
        /// a state is set to off.
        /// </summary>
        public InteractionStateInactiveEvent OnStateDeactivated { get; protected set; } = new InteractionStateInactiveEvent();

        /// <summary>
        /// The read only dictionary for the Interaction States.  To modify this dictionary use the AddNewState()
        /// RemoveState() methods. To set the value of a state in this dictionary use SetStateOn/Off() methods.
        /// </summary>
        public IReadOnlyDictionary<string, InteractionState> States => statesDictionary.ToDictionary((pair) => pair.Key, (pair) => pair.Value);

        // The interactive element for this state manager
        public BaseInteractiveElement InteractiveElement { get; protected set; }

        // Dictionary of the states being watched by this state manager
        private Dictionary<string, InteractionState> statesDictionary = new Dictionary<string, InteractionState>();

        // The List of InteractionStates for this state manager
        private List<InteractionState> interactionStates = null;

        // List of all core states
        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        // List of active states, used for tracking the current and previous states
        private List<InteractionState> activeStates = new List<InteractionState>();

        // State names
        private string defaultStateName = CoreInteractionState.Default.ToString();
        private string touchStateName = CoreInteractionState.Touch.ToString();
        private string selectFarStateName = CoreInteractionState.SelectFar.ToString();
        private string speechKeywordStateName = CoreInteractionState.SpeechKeyword.ToString();

        /// <summary>
        /// Gets a state by using the state name.
        /// </summary>
        /// <param name="stateName">The name of the state to retrieve</param>
        /// <returns>The state contained in the State list, returns null if the state was not found.</returns>
        public InteractionState GetState(string stateName)
        {
            try
            {
                return statesDictionary[stateName];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets and sets state given the state name and state value.
        /// </summary>
        /// <param name="stateName">The name of the state to set</param>
        /// <param name="value">The new state value</param>
        /// <returns>The state that was set</returns>
        public InteractionState SetState(string stateName, int value)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (value > 0)
                {
                    SetStateOn(stateName);
                }
                else
                {
                    SetStateOff(stateName);
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
            }

            return state;
        }

        /// <summary>
        /// Gets and sets a state to On and invokes the OnStateActivated event. Setting a 
        /// state on changes the state value to 1.
        /// </summary>
        /// <param name="stateName">The name of the state to set to on</param>
        /// <returns>The state that was set to on</returns>
        public InteractionState SetStateOn(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                // Only update the state value and invoke events if InteractiveElement is Active
                if (state.Value != 1 && InteractiveElement.Active)
                {
                    state.Value = 1;
                    state.Active = true;

                    OnStateActivated.Invoke(state);

                    // Only add the state to activeStates if it is not present 
                    if (!activeStates.Contains(state))
                    {
                        activeStates.Add(state);
                    }

                    InteractionState defaultState = GetState(defaultStateName);

                    // If the state getting switched on and is NOT the default state, then make sure the default state is off
                    // The default state is only active when ALL other states are not active
                    if (state.Name != defaultStateName && defaultState.Active)
                    {
                        SetStateOff(defaultStateName);
                    }
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
            }

            return state;
        }

        /// <summary>
        /// Gets and sets a state to Off and invokes the OnStateDeactivated event.  Setting a 
        /// state off changes the state value to 0.
        /// </summary>
        /// <param name="stateName">The name of the state to set to off</param>
        /// <returns>The state that was set to off</returns>
        public InteractionState SetStateOff(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                // Only update the state value and invoke events if InteractiveElement is Active
                if (state.Value != 0 && InteractiveElement.Active)
                {
                    state.Value = 0;
                    state.Active = false;

                    // If the only state in active states is going to be removed, then activate the default state
                    if (activeStates.Count == 1 && activeStates.First() == state)
                    {
                        SetStateOn(defaultStateName);
                    }

                    // We need to save the last state active state so we can add transitions 
                    OnStateDeactivated.Invoke(state, activeStates.Last());

                    activeStates.Remove(state);
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
            }

            return state;
        }

        /// <summary>
        /// Removes a state. The state will no longer be tracked if it is removed.
        /// </summary>
        /// <param name="stateName">The name of the state to remove</param>
        public void RemoveState(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (stateName != defaultStateName)
                {
                    // Remove the state from States list to update the changes in the inspector
                    interactionStates.Remove(state);

                    statesDictionary.Remove(state.Name);
                }
                else
                {
                    Debug.LogError($"The {state.Name} state cannot be removed.");
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked and was not removed.");
            }
        }

        /// <summary>
        /// Check if a state is currently active.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is active, false if the state is not active</returns>
        public bool IsStateActive(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state == null)
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddNewState(state) to track whether or not it is active.");

            }

            return state.Active;
        }

        /// <summary>
        /// Check if a state is currently being tracked by this state manager.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is being tracked, false if the state is not being tracked</returns>
        public bool IsStatePresent(string stateName)
        {
            return GetState(stateName) != null;
        }

        /// <summary>
        /// Create and add a new state to track given the new state name.  Also sets the state's event configuration. 
        /// </summary>
        /// <param name="stateName">The name of the state to add</param>
        /// <returns>The new state added</returns>
        public InteractionState AddNewState(string stateName)
        {
            // Check if the state name is an empty string
            if (stateName == string.Empty)
            {
                Debug.LogError("The state name entered is empty, please add characters to the state name.");
                return null;
            }

            // If the state does not exist, then add it
            if (!statesDictionary.ContainsKey(stateName))
            {
                InteractionState newState = new InteractionState(stateName);

                statesDictionary.Add(newState.Name, newState);

                // Add the state to the States list to ensure the inspector displays the new state
                interactionStates.Add(newState);

                // Set the event configuration if one exists for the core interaction state
                EventReceiverManager.SetEventConfiguration(newState);

                // Set special cases for specific states
                SetStateSpecificSettings(newState);

                return newState;
            }
            else
            {
                Debug.Log($" The {stateName} state is already being tracked and does not need to be added.");
                return GetState(stateName);
            }
        }

        /// <summary>
        /// Create and add a new state given the state name and the associated existing event configuration.
        /// </summary>
        /// <param name="stateName">The name of the state to create</param>
        /// <param name="eventConfiguration">The existing event configuration for the new state</param>
        /// <returns>The new state added</returns>
        public InteractionState AddNewStateWithCustomEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            InteractionState state = GetState(stateName);

            if (state == null)
            {
                // Check if the new state name defined is considered a core state
                if (!coreStates.Contains(stateName))
                {
                    InteractionState newState = AddNewState(stateName);

                    if (eventConfiguration != null)
                    {
                        // Set the event configuration if one exists for the core interaction state
                        EventReceiverManager.SetEventConfiguration(newState);
                    }
                    else
                    {
                        Debug.LogError("The event configuration entered is null and the event configuration was not set");
                    }

                    // Add the state to the States list to ensure the inspector displays the new state
                    interactionStates.Add(newState);

                    statesDictionary.Add(newState.Name, newState);
                    return newState;
                }
                else
                {
                    Debug.LogError($"The state name {stateName} is a defined core state, please use AddCoreState() to add to the States list.");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is already tracking, please use another name.");
                return state;
            }
        }

        /// <summary>
        /// Reset all the state values in the list to 0. State values are reset when the Active
        /// property is set to false. 
        /// </summary>
        public void ResetAllStates()
        {
            foreach (KeyValuePair<string, InteractionState> state in statesDictionary)
            {
                // Set all the state values to 0
                SetStateOff(state.Key);
            }
        }

        // Check if a state has additional initialization steps 
        private void SetStateSpecificSettings(InteractionState state)
        {
            // If a near interaction state is added, check if a Near Interaction Touchable component attached to the game object
            if (state.InteractionType == InteractionType.Near)
            {
                // A Near Interaction Touchable component is required for an object to receive touch events
                InteractiveElement.AddNearInteractionTouchable();
            }

            if (state.Name == selectFarStateName)
            {
                // Add listeners that monitor whether or not the SelectFar state's Global property has been changed
                AddGlobalPropertyChangedListeners(selectFarStateName);
            }

            if (state.Name == speechKeywordStateName)
            {
                // Add listeners that monitor whether or not the SpeechKeyword state's Global property has been changed
                AddGlobalPropertyChangedListeners(speechKeywordStateName);
            }
        }

        // Add listeners to the OnStateActivated and OnStateDeactivated events
        private void AddStateEventListeners()
        {
            // Add listeners to invoke a state event.
            OnStateActivated.AddListener((state) =>
            {
                // If the event configuration for a state is of type StateEvents, this means that the state
                // does not have associated dynamic event data. Therefore, the state event can be invoked when
                // the state value changes without passing in event data. 
                if (state.EventConfiguration is StateEvents)
                {
                    EventReceiverManager.InvokeStateEvent(state.Name);
                }
            });

            OnStateDeactivated.AddListener((previousState, currentState) =>
            {
                if (previousState.EventConfiguration is StateEvents)
                {
                    EventReceiverManager.InvokeStateEvent(previousState.Name);
                }
            });
        }

        // Add listeners to the Global Changed event contained in the SelectFarEvents or the SpeechKeywordEvents event configuration 
        internal void AddGlobalPropertyChangedListeners(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                if (state.Name == selectFarStateName)
                {
                    var eventConfiguration = InteractiveElement.GetStateEvents<SelectFarEvents>(selectFarStateName);

                    // If the select far state is added during runtime, then add listeners to keep track of the Global property
                    eventConfiguration.OnGlobalChanged.AddListener(() =>
                    {
                        InteractiveElement.RegisterHandler<IMixedRealityPointerHandler>(eventConfiguration.Global);
                    });
                }
                else if (state.Name == speechKeywordStateName)
                {
                    var eventConfiguration = InteractiveElement.GetStateEvents<SpeechKeywordEvents>(speechKeywordStateName);

                    // If the speech keyword state is added during runtime, then add listeners to keep track of the Global property
                    eventConfiguration.OnGlobalChanged.AddListener(() =>
                    {
                        InteractiveElement.RegisterHandler<IMixedRealitySpeechHandler>(eventConfiguration.Global);
                    });
                }
            }
        }
    }
}
