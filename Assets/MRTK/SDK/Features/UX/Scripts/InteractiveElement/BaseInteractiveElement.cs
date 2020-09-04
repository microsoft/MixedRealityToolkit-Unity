// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for an Interaction Element.  Contains state management methods, event management and the state setting logic for
    /// Core Interaction States.
    /// </summary>
    public abstract class BaseInteractiveElement :
        MonoBehaviour,
        IMixedRealityFocusHandler
    {
        [SerializeField]
        [Tooltip("ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be \"DefaultInteractableStates\" object")]
        private TrackedStates trackedStates;

        /// <summary>
        /// Scriptable Object that contains the list of states to track.
        /// </summary>
        public TrackedStates TrackedStates
        {
            get => trackedStates;
            set => trackedStates = value;
        }

        /// <summary>
        /// Entry point for state management. Contains methods for state setting, getting and creating.
        /// </summary>
        public StateManager StateManager { get; protected set; }

        /// <summary>
        /// Entry point for event management. 
        /// </summary>
        public EventReceiverManager EventReceiverManager { get; protected set; }

        // Initialize the State Manager and the Event Manager in Awake because 
        // the States Visualizer depends on the initialization of these elements
        private void Awake()
        {
            InitializeStateManager();

            InitializeEventReceiverManager();

            // Initially set the default state to on
            SetStateOn(CoreInteractionState.Default);
        }

        /// <summary>
        /// Initializes the StateList in the StateManager with the states defined in the tracked states scriptable object.
        /// </summary>
        private void InitializeStateManager()
        {
            // Create an instance of the Tracked States scriptable object if this class is initialized via script
            // instead of the inspector 
            if (TrackedStates == null)
            {
                TrackedStates = ScriptableObject.CreateInstance<TrackedStates>();
            }

            StateManager = new StateManager(TrackedStates);
        }

        /// <summary>
        /// Initializes the EventReceiverManager and creates the runtime classes for states that contain a valid 
        /// configuration.
        /// </summary>
        private void InitializeEventReceiverManager()
        {
            EventReceiverManager = new EventReceiverManager(StateManager);

            // Create runtime classes for each state that has a valid associated event configuration
            EventReceiverManager.InitializeEventReceivers();
        }

        #region Focus

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (IsStateTracking("Focus"))
            {
                SetStateOn(CoreInteractionState.Focus);

                // Invoke the state event with the Focus Event Data
                EventReceiverManager.InvokeStateEvent("Focus", eventData);
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (IsStateTracking("Focus"))
            {
                SetStateOff(CoreInteractionState.Focus);

                // Invoke the state event with the Focus Event Data
                EventReceiverManager.InvokeStateEvent("Focus", eventData);
            }
        }

        #endregion


        #region State Utilities

        /// <summary>
        /// Gets and sets a Core Interaction State to On and invokes the OnStateActivated event. Setting a 
        /// state on changes state value to 1.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to on</param>
        /// <returns>The Core Interaction state that was set to on</returns>
        public void SetStateOn(CoreInteractionState coreState)
        {
            StateManager.SetStateOn(coreState);
        }

        /// <summary>
        /// Gets and sets a state to On and invokes the OnStateActivated event. Setting a 
        /// state on changes the state value to 1.
        /// </summary>
        /// <param name="stateName">The name of the state to set to on</param>
        /// <returns>The state that was set to on</returns>
        public void SetStateOn(string stateName)
        {
            StateManager.SetStateOn(stateName);
        }

        /// <summary>
        /// Gets and sets a Core Interaction State to Off and invokes the OnStateDeactivated event.  Setting a 
        /// state off changes the state value to 0.
        /// </summary>
        /// <param name="coreState">The Core Interaction state to set to off</param>
        /// <returns>The Core Interaction state that was set to off</returns>
        public void SetStateOff(CoreInteractionState coreState)
        {
            StateManager.SetStateOff(coreState);
        }

        /// <summary>
        /// Gets and sets a state to Off and invokes the OnStateDeactivated event.  Setting a 
        /// state off changes the state value to 0.
        /// </summary>
        /// <param name="stateName">The name of the state to set to off</param>
        /// <returns>The state that was set to off</returns>
        public void SetStateOff(string stateName)
        {
             StateManager.SetStateOff(stateName);
        }

        /// <summary>
        /// Gets a Core Interaction State.
        /// </summary>
        /// <param name="coreState">The CoreInteractionState to retrieve</param>
        /// <returns>The Core Interaction State contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(CoreInteractionState coreState)
        {
            return StateManager.GetState(coreState);
        }

        /// <summary>
        /// Gets a state by using the state name.
        /// </summary>
        /// <param name="stateName">The name of the state to retrieve</param>
        /// <returns>The state contained in the Tracked States scriptable object.</returns>
        public InteractionState GetState(string stateName)
        {
            return StateManager.GetState(stateName);
        }

        /// <summary>
        /// Adds a new Core Interaction State to track.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to add</param>
        /// <returns>The newly added Core Interaction State</returns>
        public InteractionState AddCoreState(CoreInteractionState state)
        {
            return StateManager.AddCoreState(state);
        }

        /// <summary>
        /// Creates and adds a new state to track given the new state name.
        /// </summary>
        /// <param name="stateName">The name of the state to add</param>
        /// <returns>The new state added</returns>
        public InteractionState AddNewState(string stateName)
        {
            return StateManager.AddNewState(stateName);
        }

        /// <summary>
        /// Removes a Core Interaction State. The state will no longer be tracked if it is removed.
        /// </summary>
        /// <param name="coreState">The Core Interaction State to remove</param>
        public void RemoveCoreState(CoreInteractionState state)
        {
            StateManager.RemoveState(state);
        }

        /// <summary>
        /// Removes a state. The state will no longer be tracked if it is removed.
        /// </summary>
        /// <param name="stateName">The name of the state to remove</param>
        public void RemoveState(string stateName)
        {
            StateManager.RemoveState(stateName);
        }

        /// <summary>
        /// Create and add a new state given the state name and the associated existing event configuration.
        /// </summary>
        /// <param name="stateName">The name of the state to create</param>
        /// <param name="eventConfiguration">The existing event configuration for the new state</param>
        /// <returns>The new state added</returns>
        public void AddNewStateWithEventConfiguration(string stateName, BaseInteractionEventConfiguration eventConfiguration)
        {
            StateManager.AddNewStateWithEventConfiguration(stateName, eventConfiguration);
        }

        /// <summary>
        /// Check if a state is currently being tracked.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is being tracked, false if the state is not being tracked</returns>
        public bool IsStateTracking(string stateName)
        {
            return StateManager.IsStateTracking(stateName);
        }

        /// <summary>
        /// Check if a state is currently active.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is active, false if the state is not active</returns>
        public bool IsStateActive(string stateName)
        {
            return StateManager.IsStateActive(stateName);
        }

        /// <summary>
        /// Check if a core state is currently active.
        /// </summary>
        /// <param name="coreState">The name of the core state to check</param>
        /// <returns>True if the state is active, false if the state is not active</returns>
        public bool IsStateActive(CoreInteractionState coreState)
        {
            return StateManager.IsStateActive(coreState);
        }

        #endregion
    }
}