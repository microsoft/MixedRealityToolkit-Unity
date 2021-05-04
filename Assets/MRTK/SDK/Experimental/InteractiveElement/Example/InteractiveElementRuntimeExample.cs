// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement.Examples
{
    /// <summary>
    /// Contains examples on how to add states with event listeners to an Interactive Element.
    /// </summary>
    public class InteractiveElementRuntimeExample : MonoBehaviour
    {
        private InteractiveElement interactiveElement;

        void Start()
        {
            interactiveElement = gameObject.AddComponent<InteractiveElement>();

            // Interactive Element is initialized with the Default and Focus state

            AddStateActivatedListeners();

            AddFocusFarState();

            AddSelectFarState();

            CreateNewState();
        }

        /// <summary>
        /// Add State Activated and State Deactivated event listeners.
        /// </summary>
        public void AddStateActivatedListeners()
        {
            interactiveElement.StateManager.OnStateActivated.AddListener((state) =>
            {
                Debug.Log($"{state.Name} is now active");
            });

            interactiveElement.StateManager.OnStateDeactivated.AddListener((stateDeactivated, activeState) =>
            {
                Debug.Log($"{stateDeactivated.Name} is no longer active");
            });
        }

        /// <summary>
        /// Add Touch state with event listeners.
        /// </summary>
        public void AddTouchState()
        {
            interactiveElement.AddNewState("Touch");

            TouchEvents touchEvents = interactiveElement.GetStateEvents<TouchEvents>("Touch");

            touchEvents.OnTouchStarted.AddListener((touchData) =>
            {
                Debug.Log($"{gameObject.name} Touch Started");
            });
        }

        /// <summary>
        /// Add SelectFar state with event listeners.
        /// </summary>
        public void AddSelectFarState()
        {
            interactiveElement.AddNewState("SelectFar");

            SelectFarEvents selectFarEvents = interactiveElement.GetStateEvents<SelectFarEvents>("SelectFar");

            selectFarEvents.OnSelectClicked.AddListener((pointerEventData) =>
            {
                Debug.Log($"{gameObject.name} Far Interaction Click");
            });
        }

        /// <summary>
        /// Add Default state event listeners. The Default state is present by default
        /// and does not need to be added. 
        /// </summary>
        public void AddDefaultStateListeners()
        {
            StateEvents defaultEvents = interactiveElement.GetStateEvents<StateEvents>("Default");

            defaultEvents.OnStateOn.AddListener(() =>
            {
                Debug.Log($"{gameObject.name} Default State On");
            });

            defaultEvents.OnStateOff.AddListener(() =>
            {
                Debug.Log($"{gameObject.name} Default State Off");
            });
        }

        /// <summary>
        /// Add FocusNear state with event listeners.
        /// </summary>
        public void AddFocusNearState()
        {
            interactiveElement.AddNewState("FocusNear");

            FocusEvents focusNearEvents = interactiveElement.GetStateEvents<FocusEvents>("FocusNear");

            focusNearEvents.OnFocusOn.AddListener((pointerEventData) =>
            {
                Debug.Log($"{gameObject.name} Near Interaction Focus");
            });
        }

        /// <summary>
        /// Add FocusFar state with event listeners.
        /// </summary>
        public void AddFocusFarState()
        {
            interactiveElement.AddNewState("FocusFar");

            FocusEvents focusFarEvents = interactiveElement.GetStateEvents<FocusEvents>("FocusFar");

            focusFarEvents.OnFocusOn.AddListener((pointerEventData) =>
            {
                Debug.Log($"{gameObject.name} Far Interaction Focus");
            });
        }

        /// <summary>
        /// Add Clicked state with event listeners.
        /// </summary>
        public void AddClickedState()
        {
            interactiveElement.AddNewState("Clicked");

            ClickedEvents clickedEvent = interactiveElement.GetStateEvents<ClickedEvents>("Clicked");

            clickedEvent.OnClicked.AddListener(() =>
            {
                Debug.Log($"{gameObject.name} Clicked");
            });

            // Note:
            // To customize the timing of when the clicked state is triggered use: 
            // interactiveElement.TriggerClickedState();
        }

        /// <summary>
        /// Add the Toggle states with event listeners.
        /// </summary>
        public void AddToggleStates()
        {
            // Adds both the ToggleOn and ToggleOff state
            interactiveElement.AddToggleStates();

            // Toggle On Events
            ToggleOnEvents toggleOnEvent = interactiveElement.GetStateEvents<ToggleOnEvents>("ToggleOn");

            toggleOnEvent.OnToggleOn.AddListener(() =>
            {
                Debug.Log($"{gameObject.name} Toggled On");
            });

            // Toggle Off Events
            ToggleOffEvents toggleOffEvent = interactiveElement.GetStateEvents<ToggleOffEvents>("ToggleOff");

            toggleOffEvent.OnToggleOff.AddListener(() =>
            {
                Debug.Log($"{gameObject.name} Toggled Off");
            });

            // Note:
            // To customize the timing of when the toggle states are changed use: 
            // interactiveElement.SetToggleStates();
        }

        /// <summary>
        /// Add the SpeechKeyword state with event listeners.
        /// </summary>
        public void AddSpeechKeywordState()
        {
            interactiveElement.AddNewState("SpeechKeyword");

            SpeechKeywordEvents speechKeywordEvents = interactiveElement.GetStateEvents<SpeechKeywordEvents>("SpeechKeyword");

            KeywordEvent keywordEvent = new KeywordEvent() { Keyword = "Change" };

            // Any new keyword MUST be registered in the speech command profile prior to runtime 
            // To register a keyword:
            // 1. Select the MixedRealityToolkit game object
            // 2. Select Copy and Customize at the top of the profile
            // 3. Navigate to the Input section and select Clone to enable modification of the Input profile
            // 4. Scroll down to the Speech section in the Input profile and clone the Speech Profile
            // 5. Select Add a New Speech Command

            keywordEvent.OnKeywordRecognized.AddListener(() =>
            {
                Debug.Log($"The Change Keyword was recognized");
            });

            speechKeywordEvents.Keywords.Add(keywordEvent);

            speechKeywordEvents.OnAnySpeechKeywordRecognized.AddListener((speechEventData) =>
            {
                Debug.Log($"{speechEventData.Command.Keyword} recognized");
            });
        }

        /// <summary>
        /// Create a state with a new name.  This state will be initialized with the StateEvents
        /// event configuration that contains the OnStateOn and OnStateOff events.
        /// </summary>
        public void CreateNewState()
        {
            interactiveElement.AddNewState("MyNewState");

            // A new state is initialized with a the default StateEvents configuration which contains the 
            // OnStateOn and OnStateOff events

            StateEvents myNewStateEvents = interactiveElement.GetStateEvents<StateEvents>("MyNewState");

            myNewStateEvents.OnStateOn.AddListener(() =>
            {
                Debug.Log($"MyNewState is On");
            });

            // Creating a new state with a custom event configuration requires the creation of 2 new files:
            // 1. A receiver file 
            // 2. An event configuration file
            // 
            // An example of a new state with a custom event configuration is the Keyboard state.
            // Example file names are KeyboardReceiver.cs + KeyboardEvents.cs files
        }
    }
}
