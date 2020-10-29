// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for an Interaction Element.  Contains state management methods, event management and the state setting logic for 
    /// some Core Interaction States.
    /// </summary>
    public abstract class BaseInteractiveElement :
        MonoBehaviour,
        IMixedRealityFocusHandler
    {

        [SerializeField]
        [Tooltip("A list of the interaction states for this interactive element.")]
        private List<InteractionState> states = new List<InteractionState>();

        /// <summary>
        /// A list of the interaction states for this interactive element.
        /// </summary>
        public List<InteractionState> States
        {
            get { return states; }
            set { states = value; }
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

        // List of all core states
        private string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

        private void OnValidate()
        {
            // Populate the States list with the initial states when this component is initialized via inspector
            if (States.Count == 0)
            {
                States.Add(new InteractionState("Default"));
                States.Add(new InteractionState("Focus"));
            }
        }

        // Initialize the State Manager in Awake because the State Visualizer depends on the initialization of these elements
        private void Awake()
        {
            // Populate the States list with the initial states when this component is initialized via script instead of the inspector
            if (States.Count == 0)
            {
                States.Add(new InteractionState("Default"));
                States.Add(new InteractionState("Focus"));
            }

            InitializeStateManager();

            // Initially set the default state to on
            SetStateOn(DefaultStateName);
        }

        /// <summary>
        /// Initializes the StateList in the StateManager with the states defined in the States list
        /// </summary>
        private void InitializeStateManager()
        {
            StateManager = new StateManager(States, this);

            StateManager.OnStateActivated.AddListener((state) =>
            {
                if (!coreStates.Contains(state.Name))
                {
                    EventReceiverManager.InvokeStateEvent(state.Name);
                }
            });

            StateManager.OnStateDeactivated.AddListener((previousState, currentState) =>
            {
                if (!coreStates.Contains(previousState.Name))
                {
                    EventReceiverManager.InvokeStateEvent(previousState.Name);
                }
            });
        }

        #region Focus

        public void OnFocusEnter(FocusEventData eventData)
        {
            SetStateAndInvokeEvent(FocusStateName, 1, eventData);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            SetStateAndInvokeEvent(FocusStateName, 0, eventData);
        }

        #endregion

        public void SetStateAndInvokeEvent(string stateName, int value, BaseEventData eventData)
        {
            if (IsStateTracking(stateName))
            {
                StateManager.SetState(stateName, value);

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
        /// Checks if a state is currently being tracked by the state manager.
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

        #endregion

        /// <summary>
        /// Used for setting the event configuration for a new state when the state is added via inspector.
        /// </summary>
        /// <param name="stateName"></param>
        public void SetEventConfigurationInstance(string stateName)
        {
            InteractionState state = States.Find((interactionState) => interactionState.Name == stateName);
            BaseInteractionEventConfiguration eventConfiguration;

            if (state != null)
            {
                var eventConfigTypes = TypeCacheUtility.GetSubClasses<BaseInteractionEventConfiguration>();
                Type eventConfigType = eventConfigTypes.Find((type) => type.Name.StartsWith(stateName));

                if (eventConfigType != null)
                {
                    eventConfiguration = Activator.CreateInstance(eventConfigType) as BaseInteractionEventConfiguration;
                }
                else
                {
                    eventConfiguration = Activator.CreateInstance(typeof(StateEvents)) as BaseInteractionEventConfiguration;
                }

                state.Name = stateName;
                eventConfiguration.StateName = state.Name;
                state.EventConfiguration = eventConfiguration;
            }
            else
            {
                Debug.LogError($"{stateName} is not contianted in the States list");
            }
        }
    }
}