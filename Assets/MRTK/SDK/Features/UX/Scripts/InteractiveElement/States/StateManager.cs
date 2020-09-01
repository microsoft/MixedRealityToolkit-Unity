// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Utilities;
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
        public StateManager(TrackedStates trackedStates) 
        {
            states = trackedStates.States;
        }

        // List of states to be tracked by this state manager
        private List<InteractionState> states = null;

        /// <summary>
        /// The read only list of the current Interaction States being tracked.  To modify this list use the AddNewState()
        /// RemoveState() methods, to set the value of a state in this list use SetStateOn/Off() methods.
        /// </summary>
        public IList<InteractionState> States => states.AsReadOnly();

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

        #region State Methods for CoreInteractionStates 

        /// <summary>
        /// Gets a Core Interaction State.
        /// </summary>
        /// <param name="coreState">The CoreInteractionState to retrieve</param>
        /// <returns>The Core Interaction State contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            InteractionState interactionState = states.Find((state) => state.Name == coreState.ToString());

            if (interactionState != null)
            {
                return interactionState;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets and sets a Core Interaction State given the value.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to set</param>
        /// <param name="value">The Core Interaction State's new value</param>
        /// <returns>The Core Interaction State that was set</returns>
        public InteractionState SetState(CoreInteractionState coreState, int value)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                if (value > 0)
                {
                    SetStateOn(coreState);

                }
                else
                {
                    SetStateOff(coreState);
                }

                return state;
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        /// <summary>
        /// Gets and sets a Core Interaction State to On and invokes the OnStateActivated event. Setting a 
        /// state on changes state value to 1.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to on</param>
        /// <returns>The Core Interaction state that was set to on</returns>
        public InteractionState SetStateOn(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

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

                return state;
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
        }

        /// <summary>
        /// Gets and sets a Core Interaction State to Off and invokes the OnStateDeactivated event.  Setting a 
        /// state off changes the state value to 0.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to off</param>
        /// <returns>The Core Interaction state that was set to off</returns>
        public InteractionState SetStateOff(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

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

                return state;
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
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

                // Set the event configuration if one exists for the core interaction state
                SetEventConfigurationOfCoreState(newState);

                states.Add(newState);

                return newState;
            }
            else
            {
                Debug.Log($" The {coreState} state is already being tracked and does not need to be added.");
                return state;
            }
        }

        /// <summary>
        /// Removes a Core Interaction State. The state will no longer be tracked if it is removed.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to remove</param>
        public void RemoveCoreState(CoreInteractionState coreState)
        {
            InteractionState state = GetState(coreState);

            if (state != null)
            {
                if (coreState != CoreInteractionState.Default)
                {
                    states.Remove(state);
                }
                else
                {
                    Debug.LogError($"The Default state cannot be removed");
                }
            }
            else
            {
                Debug.LogError($"The {coreState} state is not being tracked and was not removed.");

            }
        }

        #endregion


        #region State Methods for Non-CoreInteractionStates

        /// <summary>
        /// Gets a state by using the state name.
        /// </summary>
        /// <param name="stateName">The name of the state to retrieve</param>
        /// <returns>The state contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(string stateName)
        {
            InteractionState interactionState = states.Find((state) => state.Name == stateName);

            if (interactionState != null)
            {
                return interactionState;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets and sets state given the state name and state value.
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="value"></param>
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

                return state;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
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

                return state;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
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

                return state;
            }
            else
            {
                Debug.LogError($"The {stateName} state is not being tracked, add this state using AddState(state) to set it");
                return null;
            }
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
                return state;
            }
            else
            {
                Debug.Log($" The {stateName} state is already being tracked and does not need to be added.");
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

        #endregion

        // Set the event configuration of an existing core state if it exists
        private void SetEventConfigurationOfCoreState(InteractionState coreState)
        {
            var eventConfigurationTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
            var eventConfigType = eventConfigurationTypes.Find(t => t.Name.StartsWith(coreState.ToString()));

            // Check if the core state has a custom event configuration
            if (eventConfigType != null)
            {
                string className = eventConfigType.Name;

                // Set the state event configuration 
                coreState.EventConfiguration = (BaseInteractionEventConfiguration)ScriptableObject.CreateInstance(className);
            }
            else
            {
                Debug.Log($" The {coreState.Name} state does not have an existing event configuration");
            }
        }
    }
}
