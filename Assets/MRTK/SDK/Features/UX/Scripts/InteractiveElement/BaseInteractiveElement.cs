// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for an Interactive Element.  Contains state management methods, event management and the state setting logic for 
    /// some Core Interaction States.
    /// </summary>
    public abstract class BaseInteractiveElement :
        MonoBehaviour,
        IMixedRealityFocusHandler,
        IMixedRealityTouchHandler
    {
        [SerializeField]
        [Tooltip("Whether or not this interactive element will react to input and update internally. If true, the " +
            "object will react to input and update internally.  If false, the object will not update internally " +
            "and not react to input, i.e. state values will not be updated.")]
        private bool active = true;

        /// <summary>
        /// Whether or not this interactive element will react to input and update internally. If true, the 
        /// object will react to input and update internally.  If false, the object will not update internally 
        /// and not react to input, i.e. state values will not be updated.
        /// </summary>
        public bool Active
        {
            get => active;
            set 
            {
                ResetAllStates();
                active = value; 
            }
        }

        [SerializeField]
        [Tooltip("A list of the interaction states for this interactive element.")]
        private List<InteractionState> states = new List<InteractionState>();

        /// <summary>
        /// A list of the interaction states for this interactive element.
        /// </summary>
        public List<InteractionState> States
        {
            get => states; 
            set => states = value; 
        }

        /// <summary>
        /// Entry point for state management. Contains methods for state setting, getting and creating.
        /// </summary>
        public StateManager StateManager { get; protected set; }

        /// <summary>
        /// Manages the associated state events contained in a state. 
        /// </summary>
        public EventReceiverManager EventReceiverManager => StateManager.EventReceiverManager;

        // Core State Names
        protected string DefaultStateName = CoreInteractionState.Default.ToString();
        protected string FocusStateName = CoreInteractionState.Focus.ToString();
        protected string FocusNearStateName = CoreInteractionState.FocusNear.ToString();
        protected string FocusFarStateName = CoreInteractionState.FocusFar.ToString();
        protected string TouchStateName = CoreInteractionState.Touch.ToString();

        public virtual void OnValidate()
        {
            // Populate the States list with the initial states when this component is initialized via inspector
            PopulateInitialStates();
        }

        // Initialize the State Manager in Awake because the State Visualizer depends on the initialization of these elements
        private void Awake()
        {
            // Populate the States list with the initial states when this component is initialized via script instead of the inspector
            PopulateInitialStates();

            // Initializes the state dictionary in the StateManager with the states defined in the States list
            StateManager = new StateManager(States, this);

            // Initially set the default state to on
            SetStateOn(DefaultStateName);
        }

        // Add the Default and the Focus state as the initial states in the States list
        private void PopulateInitialStates()
        {
            if (States.Count == 0)
            {
                States.Add(new InteractionState(DefaultStateName));
                States.Add(new InteractionState(FocusStateName));
            }
        }

        #region Focus

        public void OnFocusEnter(FocusEventData eventData)
        {
            // Set the FocusNear and FocusFar state depending on the type of pointer 
            // currently active
            if (eventData.Pointer is IMixedRealityNearPointer)
            {
                SetStateAndInvokeEvent(FocusNearStateName, 1, eventData);
            }
            else if (!(eventData.Pointer is IMixedRealityNearPointer))
            {
                SetStateAndInvokeEvent(FocusFarStateName, 1, eventData);
            }

            SetStateAndInvokeEvent(FocusStateName, 1, eventData);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            // Set the Focus, FocusNear, and FocusFar states off 
            SetStateAndInvokeEvent(FocusNearStateName, 0, eventData);
            SetStateAndInvokeEvent(FocusFarStateName, 0, eventData);
            SetStateAndInvokeEvent(FocusStateName, 0, eventData);
        }

        #endregion

        #region Touch

        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            SetStateAndInvokeEvent(TouchStateName, 1, eventData);
        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            SetStateAndInvokeEvent(TouchStateName, 0, eventData);
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if (IsStatePresent(TouchStateName))
            {
                EventReceiverManager.InvokeStateEvent(TouchStateName, eventData);
            }
        }

        #endregion

        /// <summary>
        /// Sets a state to a given state value and invokes an event with associated event data. 
        /// </summary>
        /// <param name="stateName">The name of the state to set</param>
        /// <param name="stateValue">The state value. A value of 0 = set the state off, 1 = set the state on</param>
        /// <param name="eventData">Event data to pass into the event</param>
        public void SetStateAndInvokeEvent(string stateName, int stateValue, BaseEventData eventData)
        {
            if (IsStatePresent(stateName))
            {
                StateManager.SetState(stateName, stateValue);

                EventReceiverManager.InvokeStateEvent(stateName, eventData);
            }
        }

        #region Event Utilities

        public T GetStateEvents<T>(string stateName) where T : BaseInteractionEventConfiguration
        {
            return EventReceiverManager.GetEventConfiguration(stateName) as T;
        }

        #endregion

        #region State Utilities

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
        /// Gets a state by using the state name.
        /// </summary>
        /// <param name="stateName">The name of the state to retrieve</param>
        /// <returns>The state contained in the States list.</returns>
        public InteractionState GetState(string stateName)
        {
            return StateManager.GetState(stateName);
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
            StateManager.AddNewStateWithCustomEventConfiguration(stateName, eventConfiguration);
        }

        /// <summary>
        /// Checks if a state is currently in the States list and is being tracked by the state manager.
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is being tracked, false if the state is not being tracked</returns>
        public bool IsStatePresent(string stateName)
        {
            return StateManager.IsStatePresent(stateName);
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
        /// Reset all the state values in the list to 0. State values are reset when the Active
        /// property is set to false. 
        /// </summary>
        public void ResetAllStates()
        {
            StateManager.ResetAllStates();
        }

        #endregion

        /// <summary>
        /// Used for setting the event configuration for a new state when the state is added via inspector.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        public void SetEventConfigurationInstance(string stateName)
        {
            InteractionState state = States.Find((interactionState) => interactionState.Name == stateName);
            
            // Set the new Interaction Type and configuration
            state.SetEventConfiguration(stateName);
            state.SetInteractionType(stateName);
        }

        /// <summary>
        /// Add a Near Interaction Touchable component to the current game object if the Touch state is 
        /// added to the States list. A Near Interaction Touchable component is required for an object to detect
        /// touch input events. 
        /// A Near Interaction Touchable Volume component is attached by default because it detects touch input
        /// on the entire surface area of a collider.  While a Near Interaction Touchable component
        /// will be attached if the object is a button because touch input is only detected within the area of a plane. 
        /// </summary>
        public void AddNearInteractionTouchable()
        {
            if (gameObject.GetComponent<BaseNearInteractionTouchable>() == null)
            {
                // Add a Near Interaction Touchable Volume by default because it detects touch on the 
                // entire surface area of a collider. 
                gameObject.AddComponent<NearInteractionTouchableVolume>();

                // Add a Near Interaction Touchable if the object is a button.
                // A Near Interaction Touchable detects touch input within the area of a plane and not the 
                // entire surface area of an object.
            }
        }

        /// <summary>
        /// Checks if a state is currently in the State list. This method is specifically used for checking the 
        /// contents of the States list during edit mode as the State Manager contains runtime methods. 
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <returns>True if the state is in the States list. False, if the state could not be found.</returns>
        public bool IsStatePresentEditMode(string stateName)
        {
            return States.Find((state) => state.Name == stateName) != null;
        }
    }
}