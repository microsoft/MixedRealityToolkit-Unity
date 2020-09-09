// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Manages state values of Interaction States within the Tracked States Scriptable Object.  The Tracked States
    /// scriptable object is contained within a class that derives from BaseInteractionElement.  This class contains helper
    /// methods for setting, getting and creating new Interaction States to track.
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// Create a new state manager with a given states scriptable object.
        /// </summary>
        /// <param name="trackedStates">TrackedStates scriptable object</param>
        /// <param name="interactiveElement">The interactive element source</param>
        public StateManager(TrackedStates trackedStates, BaseInteractiveElement interactiveElementSource)
        {
            states = trackedStates.States;

            interactiveElement = interactiveElementSource;

            InitializeEventReceiverManager();
        }

        // List of states to be tracked by this state manager
        private List<InteractionState> states = null;

        /// <summary>
        /// The read only list of the current Interaction States being tracked.  To modify this list use the AddNewState()
        /// RemoveState() methods, to set the value of a state in this list use SetStateOn/Off() methods.
        /// </summary>
        public IList<InteractionState> States => states.AsReadOnly();

        /// <summary>
        /// The Event Receiver Manager for this State Manager. Each state can contain an event configuration scriptable which defines
        /// the events associated with the state.  The Event Receiver Manager depends on a State Manager.
        /// </summary>
        public EventReceiverManager EventReceiverManager { get; internal set; } = null;

        // The interactive element for this state manager
        private BaseInteractiveElement interactiveElement = null;

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

        // List of all core states
        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        // List of active states, used for tracking the current and previous states
        private List<InteractionState> activeStates = new List<InteractionState>();

        // Initializes the EventReceiverManager and creates the runtime classes for states that contain a valid 
        // configuration
        internal void InitializeEventReceiverManager()
        {
            // Create a new event receiver manager for this state manager
            EventReceiverManager = new EventReceiverManager(this);
            EventReceiverManager.InitializeEventReceivers();
        }

        /// <summary>
        /// Gets a state by using the state name.
        /// </summary>
        /// <param name="stateName">The name of the state to retrieve</param>
        /// <returns>The state contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(string stateName)
        {
            InteractionState interactionState = states.Find((state) => state.Name == stateName);

            return interactionState;
        }

        /// <summary>
        /// Gets a Core Interaction State.
        /// </summary>
        /// <param name="coreState">The CoreInteractionState to retrieve</param>
        /// <returns>The Core Interaction State contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            return GetState(coreState.ToString());
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
        /// Gets and sets a Core Interaction State given the value.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to set</param>
        /// <param name="value">The Core Interaction State's new value</param>
        /// <returns>The Core Interaction State that was set</returns>
        public InteractionState SetState(CoreInteractionState coreState, int value)
        {
            return SetState(coreState.ToString(), value);
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
                if (state.Value != 1)
                {
                    state.Value = 1;
                    state.Active = 1;

                    OnStateActivated.Invoke(state);

                    // Only add the state to activeStates if it is not present 
                    if (!activeStates.Contains(state))
                    {
                        activeStates.Add(state);
                    }

                    InteractionState defaultState = GetState(CoreInteractionState.Default);

                    // If the state getting switched on is NOT the default state, then make sure the default state is off
                    // The default state is only active when ALL other states are not active
                    if (state.Name != "Default" && defaultState.Value == 1)
                    {
                        SetStateOff(CoreInteractionState.Default);
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
        /// Gets and sets a Core Interaction State to On and invokes the OnStateActivated event. Setting a 
        /// state on changes state value to 1.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to on</param>
        /// <returns>The Core Interaction state that was set to on</returns>
        public InteractionState SetStateOn(CoreInteractionState coreState)
        {
            return SetStateOn(coreState.ToString());
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
                if (state.Value != 0)
                {
                    state.Value = 0;
                    state.Active = 0;

                    // If the only state in active states is going to be removed, then activate the default state
                    if (activeStates.Count == 1 && activeStates.First() == state)
                    {
                        SetStateOn(CoreInteractionState.Default);
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
        /// Gets and sets a Core Interaction State to Off and invokes the OnStateDeactivated event.  Setting a 
        /// state off changes the state value to 0.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to off</param>
        /// <returns>The Core Interaction state that was set to off</returns>
        public InteractionState SetStateOff(CoreInteractionState coreState)
        {
            return SetStateOff(coreState.ToString());
        }

        /// <summary>
        /// Adds a new Core Interaction State to track.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to add</param>
        /// <returns>The newly added Core Interaction State</returns>
        public InteractionState AddCoreState(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state == null)
            {
                // Create a new core state
                InteractionState newState = new InteractionState(coreState.ToString());

                states.Add(newState);

                // Set the event configuration if one exists for the core interaction state
                EventReceiverManager.SetEventConfiguration(newState);

                return newState;
            }
            else
            {
                Debug.Log($" The {coreState} state is already being tracked and does not need to be added.");
                return state;
            }
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
                if (stateName != "Default")
                {
                    states.Remove(state);
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
        /// Removes a Core Interaction State. The state will no longer be tracked if it is removed.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to remove</param>
        public void RemoveState(CoreInteractionState coreState)
        {
            RemoveState(coreState.ToString());
        }

        /// <summary>
        /// Check if a state is currently active.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is active, false if the state is not active</returns>
        public bool IsStateActive(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state != null)
            {
                return state.Active == 1;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddNewState(state) to track whether or not it is active.");
            }

            return false;
        }

        /// <summary>
        /// Check if a core state is currently active.
        /// </summary>
        /// <param name="coreState">The name of the core state to check</param>
        /// <returns>True if the state is active, false if the state is not active</returns>
        public bool IsStateActive(CoreInteractionState coreState)
        {
            return IsStateActive(coreState.ToString());
        }


        /// <summary>
        /// Check if a state is currently being tracked.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is being tracked, false if the state is not being tracked</returns>
        public bool IsStateTracking(string stateName)
        {
            InteractionState state = GetState(stateName);

            return state != null;
        }

        /// <summary>
        /// Check if a state is currently being tracked.
        /// </summary>
        /// <param name="coreState">The name of the core state to check</param>
        /// <returns>True if the state is being tracked, false if the state is not being tracked</returns>
        public bool IsStateTracking(CoreInteractionState coreState)
        {
            return IsStateTracking(coreState.ToString());
        }

        /// <summary>
        /// Creates and adds a new state to track given the new state name.
        /// </summary>
        /// <param name="stateName">The name of the state to add</param>
        /// <returns>The new state added</returns>
        public InteractionState AddNewState(string stateName)
        {
            InteractionState state = GetState(stateName);

            if (state == null)
            {
                InteractionState newState = new InteractionState(stateName);
                states.Add(newState);
            }
            else
            {
                Debug.Log($" The {stateName} state is already being tracked and does not need to be added.");
            }

            return state;
        }

        /// <summary>
        /// Create and add a new state given the state name and the associated existing event configuration.
        /// </summary>
        /// <param name="stateName">The name of the state to create</param>
        /// <param name="eventConfiguration">The existing event configuration for the new state</param>
        /// <returns>The new state added</returns>
        public InteractionState AddNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
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
                        newState.EventConfiguration = eventConfiguration;
                    }
                    else
                    {
                        Debug.LogError("The event configuration entered is null and the event configuration was not set");
                    }

                    states.Add(newState);
                    return newState;
                }
                else
                {
                    Debug.LogError($"The state name {stateName} is a defined core state, please use AddCoreState() to add to Tracked States.");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"The {stateName} state is already tracking, please use another name.");
                return state;
            }
        }
    }
}
