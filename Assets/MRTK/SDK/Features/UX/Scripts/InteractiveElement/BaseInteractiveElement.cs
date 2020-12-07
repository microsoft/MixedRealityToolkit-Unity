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

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.SDK.Editor")]
namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// Base class for an Interactive Element.  Contains state management methods, event management and the state setting logic for 
    /// some Core Interaction States.
    /// </summary>
    public abstract class BaseInteractiveElement :
        MonoBehaviour,
        IMixedRealityFocusHandler,
        IMixedRealityTouchHandler,
        IMixedRealityPointerHandler
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
        protected string SelectFarStateName = CoreInteractionState.SelectFar.ToString();
        protected string ClickedStateName = CoreInteractionState.Clicked.ToString();
        protected string ToggleOnStateName = CoreInteractionState.ToggleOn.ToString();
        protected string ToggleOffStateName = CoreInteractionState.ToggleOff.ToString();

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

        public virtual void Start()
        {
            // If the SelectFar state is in the States list at start and the Global property is true, then
            // register the IMixedRealityPointerHandler for global usage
            RegisterSelectFarHandler(true);

            // If the SelectFar state is present, add listeners for the Global property for runtime property modification
            StateManager.AddSelectFarListeners();

            // If the Toggle states are present, ensure the set up is correct and check initial values
            if (IsStatePresent(ToggleOnStateName) || IsStatePresent(ToggleOffStateName))
            {
                // Ensure both the ToggleOn and ToggleOff states are added if either state is present
                // The Toggle behavior only works if both the ToggleOn and ToggleOff states are present
                AddToggleStates();

                var toggleOn = GetStateEvents<ToggleOnEvents>(ToggleOnStateName);

                // Set the initial toggle states according to the value of ToggleOn's IsActiveOnStart property
                ForceSetToggleStates(toggleOn.IsSelectedOnStart);
            }
        }

        private void OnDisable()
        {
            // Unregister the IMixedRealityPointerHandler if it was registered
            RegisterSelectFarHandler(false);
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
            EventReceiverManager.InvokeStateEvent(TouchStateName, eventData);
        }

        #endregion

        #region SelectFar

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            SetStateAndInvokeEvent(SelectFarStateName, 1, eventData);
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            EventReceiverManager.InvokeStateEvent(SelectFarStateName, eventData);
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            EventReceiverManager.InvokeStateEvent(SelectFarStateName, eventData);

            TriggerClickedState();

            SetToggleStates();
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            SetStateAndInvokeEvent(SelectFarStateName, 0, eventData);
        }

        #endregion

        #region Event Utilities

        /// <summary>
        /// Sets a state to a given state value and invokes an event with associated event data. 
        /// </summary>
        /// <param name="stateName">The name of the state to set</param>
        /// <param name="stateValue">The state value. A value of 0 = set the state off, 1 = set the state on</param>
        /// <param name="eventData">Event data to pass into the event</param>
        public void SetStateAndInvokeEvent(string stateName, int stateValue, BaseEventData eventData = null)
        {
            if (IsStatePresent(stateName))
            {
                StateManager.SetState(stateName, stateValue);

                EventReceiverManager.InvokeStateEvent(stateName, eventData);
            }
        }

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

        #region Button Setting Utilities

        /// <summary>
        /// Set the Clicked state which triggers the OnClicked event.  The click behavior in the
        /// state management system is expressed by setting the Clicked state to on and then immediately setting
        /// it to off. 
        /// 
        /// Note: Due to the fact that a click is triggered by setting the Clicked state to on and 
        /// then immediately off, the cyan active state highlight in the inspector will not be visible.
        /// </summary>
        public void TriggerClickedState()
        {
            // Set the Clicked state to on, invokes the OnClicked event
            SetStateAndInvokeEvent(ClickedStateName, 1);

            // Set the Clicked state to off
            SetStateAndInvokeEvent(ClickedStateName, 0);
        }

        /// <summary>
        /// Add the ToggleOn and ToggleOff state.
        /// </summary>
        public void AddToggleStates()
        {
            if (!IsStatePresent(ToggleOnStateName))
            {
                StateManager.AddNewState(ToggleOnStateName);
            }

            if (!IsStatePresent(ToggleOffStateName))
            {
                StateManager.AddNewState(ToggleOffStateName);
            }
        }

        /// <summary>
        /// Set the toggle based on the current values of the ToggleOn and ToggleOff states. 
        /// </summary>
        public void SetToggleStates()
        {
            if (IsStatePresent(ToggleOnStateName) && IsStatePresent(ToggleOffStateName))
            {
                bool setToggleOn = StateManager.GetState(ToggleOnStateName).Value > 0;

                SetToggles(!setToggleOn);
            }
        }

        /// <summary>
        /// Force set the toggle states either on or off. 
        /// </summary>
        /// <param name="setToggleOn">If true, the toggle will be set to on. If false, the toggle will be set to off.</param>
        public void ForceSetToggleStates(bool setToggleOn)
        {
            if (IsStatePresent(ToggleOnStateName) && IsStatePresent(ToggleOffStateName))
            {
                SetToggles(setToggleOn);
            }
        }

        #endregion

        #region Helper Methods

        protected void SetToggles(bool setToggleOn)
        {
            if (setToggleOn)
            {
                SetStateAndInvokeEvent(ToggleOffStateName, 0);
                SetStateAndInvokeEvent(ToggleOnStateName, 1);
            }
            else
            {
                SetStateAndInvokeEvent(ToggleOnStateName, 0);
                SetStateAndInvokeEvent(ToggleOffStateName, 1);
            }
        }

        /// <summary>
        /// Used for setting the event configuration for a new state when the state is added via inspector.
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        internal void SetEventConfigurationInstance(string stateName)
        {
            InteractionState state = States.Find((interactionState) => interactionState.Name == stateName);
            
            // Set the new Interaction Type and configuration
            state.SetEventConfiguration(stateName);
            state.SetInteractionType(stateName);
        }

        /// <summary>
        /// Checks if a state is currently in the State list. This method is specifically used for checking the 
        /// contents of the States list during edit mode as the State Manager contains runtime methods. 
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <returns>True if the state is in the States list. False, if the state could not be found.</returns>
        internal bool IsStatePresentEditMode(string stateName)
        {
            return States.Find((state) => state.Name == stateName) != null;
        }

        /// <summary>
        /// Add a Near Interaction Touchable component to the current game object if the Touch state is 
        /// added to the States list. A Near Interaction Touchable component is required for an object to detect
        /// touch input events. 
        /// A Near Interaction Touchable Volume component is attached by default because it detects touch input
        /// on the entire surface area of a collider.  While a Near Interaction Touchable component
        /// will be attached if the object is a button because touch input is only detected within the area of a plane. 
        /// </summary>
        internal void AddNearInteractionTouchable()
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
        /// Register the IMixedRealityPointerHandler for global input when the SelectFar state is
        /// present on Start and the Global property is true.
        /// </summary>
        internal void RegisterSelectFarHandler(bool register)
        {
            if (IsStatePresent(SelectFarStateName))
            {
                var selectFarEvents = GetStateEvents<SelectFarEvents>(SelectFarStateName);

                // Check if far select has the Global property enabled
                if (selectFarEvents.Global)
                {
                    RegisterHandler<IMixedRealityPointerHandler>(register);
                }
            }
        }

        /// <summary>
        /// Helper method for registering an IEventSystemHandler.
        /// </summary>
        internal void RegisterHandler<T>(bool register) where T : IEventSystemHandler
        {
            if (register)
            {
                CoreServices.InputSystem?.RegisterHandler<T>(this);
            }
            else
            {
                CoreServices.InputSystem?.UnregisterHandler<T>(this);
            }
        }

        #endregion
    }
}