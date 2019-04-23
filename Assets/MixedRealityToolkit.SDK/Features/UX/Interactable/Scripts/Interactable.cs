// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Uses input and action data to declare a set of states
    /// Maintains a collection of themes that react to state changes and provide sensory feedback
    /// Passes state information and input data on to receivers that detect patterns and does stuff.
    /// </summary>

    // TODO: How to handle cycle buttons
    // TODO: plumb for gestures
    // TODO: Add way to protect the defaultTheme from being edited and encourage users to create a new theme, maybe include a create/duplicate button
    // TODO: Make sure all shader values are batched by theme

    [System.Serializable]

    public class Interactable :
        MonoBehaviour,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler,
        IMixedRealityInputHandler,
        IMixedRealityPointerHandler,
        IMixedRealitySpeechHandler,
        IMixedRealityTouchHandler
    {
        /// <summary>
        /// Setup the input system
        /// </summary>
        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>());

        // list of pointers
        protected List<IMixedRealityPointer> pointers = new List<IMixedRealityPointer>();
        public List<IMixedRealityPointer> Focusers => pointers;

        // is the interactable enabled?
        public bool Enabled = true;
        // a collection of states and basic state logic
        public States States;
        // the state logic for comparing state
        public InteractableStates StateManager;
        // which action is this interactable listening for
        public MixedRealityInputAction InputAction;

        // the id of the selected inputAction, for serialization
        [HideInInspector]
        public int InputActionId;
        // is the interactable listening to global events
        public bool IsGlobal = false;
        // a way of adding more layers of states for toggles
        public int Dimensions = 1;
        // is the interactive selectable
        public bool CanSelect = true;
        // can deselect a toggle, a radial button or tab would set this to false
        public bool CanDeselect = true;
        // a voice command to fire a click event
        public string VoiceCommand = "";
        // does the voice command require this to have focus?
        public bool RequiresFocus = true;

        /// <summary>
        /// Does this interactable require focus
        /// </summary>
        public bool FocusEnabled { get { return !IsGlobal; } set { IsGlobal = !value; } }

        // list of profiles can match themes with gameObjects
        public List<InteractableProfileItem> Profiles = new List<InteractableProfileItem>();
        // Base onclick event
        public UnityEvent OnClick;
        // list of events added to this interactable
        public List<InteractableEvent> Events = new List<InteractableEvent>();
        // the list of running theme instances to receive state changes
        public List<InteractableThemeBase> runningThemesList = new List<InteractableThemeBase>();

        // the list of profile settings, so theme values are not directly effected
        protected List<ProfileSettings> runningProfileSettings = new List<ProfileSettings>();
        // directly manipulate a theme value, skip blending
        protected bool forceUpdate = false;

        // basic button states
        public bool HasFocus { get; private set; }
        public bool HasPress { get; private set; }
        public bool IsDisabled { get; private set; }

        // advanced button states from InteractableStates.InteractableStateEnum
        public bool IsTargeted { get; private set; }
        public bool IsInteractive { get; private set; }
        public bool HasObservationTargeted { get; private set; }
        public bool HasObservation { get; private set; }
        public bool IsVisited { get; private set; }
        public bool IsToggled { get; private set; }
        public bool HasGesture { get; private set; }
        public bool HasGestureMax { get; private set; }
        public bool HasCollision { get; private set; }
        public bool HasVoiceCommand { get; private set; }
        public bool HasPhysicalTouch { get; private set; }
        public bool HasCustom { get; private set; }

        // internal cached states
        protected State lastState;
        protected bool wasDisabled = false;

        // cache of current dimension
        protected int dimensionIndex = 0;

        // allows for switching colliders without firing a lose focus immediately
        // for advanced controls like drop-downs
        protected float rollOffTime = 0.25f;
        protected float rollOffTimer = 0.25f;

        // cache voice commands
        protected string[] voiceCommands;

        // IInteractableEvents
        protected List<IInteractableHandler> handlers = new List<IInteractableHandler>();
        protected Coroutine globalTimer;
        protected float clickTime = 0.3f;
        protected Coroutine inputTimer;

        protected MixedRealityInputAction pointerInputAction;

        // order = pointer , input
        protected int[] GlobalClickOrder = new int[] { 0, 0 };

        public void AddHandler(IInteractableHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void RemoveHandler(IInteractableHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }

        #region InspectorHelpers
        public static bool TryGetInputActions(out string[] descriptionsArray)
        {
            if (!MixedRealityToolkit.IsInitialized || !MixedRealityToolkit.Instance.HasActiveProfile)
            {
                descriptionsArray = null;
                return false;
            }

            MixedRealityInputAction[] actions = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;

            descriptionsArray = new string[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                descriptionsArray[i] = actions[i].Description;
            }

            return true;
        }

        /// <summary>
        /// Returns a list of states assigned to the Interactable
        /// </summary>
        /// <returns></returns>
        public State[] GetStates()
        {
            if (States != null)
            {
                return States.GetStates();
            }

            return new State[0];
        }
        #endregion InspectorHelpers

        #region MonoBehaviorImplimentation

        protected virtual void Awake()
        {
            //State = new InteractableStates(InteractableStates.Default);
            InputAction = ResolveInputAction(InputActionId);
            SetupEvents();
            SetupThemes();
            SetupStates();
        }

        private void OnEnable()
        {
            if (IsGlobal)
            {
                InputSystem.Register(gameObject);
            }
        }

        private void OnDisable()
        {
            if (IsGlobal)
            {
                InputSystem.Unregister(gameObject);
            }
        }

        protected virtual void Update()
        {
            if (rollOffTimer < rollOffTime && HasPress)
            {
                rollOffTimer += Time.deltaTime;

                if (rollOffTimer >= rollOffTime)
                {
                    SetPress(false);
                }
            }

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnUpdate(StateManager, this);
                }
            }

            for (int i = 0; i < runningThemesList.Count; i++)
            {
                if (runningThemesList[i].Loaded)
                {
                    runningThemesList[i].OnUpdate(StateManager.CurrentState().ActiveIndex, forceUpdate);
                }
            }

            if (lastState != StateManager.CurrentState())
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    if (handlers[i] != null)
                    {
                        handlers[i].OnStateChange(StateManager, this);
                    }
                }
            }

            if (forceUpdate)
            {
                forceUpdate = false;
            }

            if (IsDisabled == Enabled)
            {
                SetDisabled(!Enabled);
            }

            lastState = StateManager.CurrentState();
        }

        #endregion MonoBehaviorImplimentation

        #region InteractableInitiation

        /// <summary>
        /// starts the StateManager
        /// </summary>
        protected virtual void SetupStates()
        {
            StateManager = States.SetupLogic();
        }

        /// <summary>
        /// Creates the event receiver instances from the Events list
        /// </summary>
        protected virtual void SetupEvents()
        {
            InteractableTypesContainer interactableTypes = InteractableEvent.GetEventTypes();

            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].Receiver = InteractableEvent.GetReceiver(Events[i], interactableTypes);
                Events[i].Receiver.Host = this;
            }
        }

        /// <summary>
        /// Creates the list of theme instances based on all the theme settings
        /// </summary>
        protected virtual void SetupThemes()
        {
            runningThemesList = new List<InteractableThemeBase>();
            runningProfileSettings = new List<ProfileSettings>();
            for (int i = 0; i < Profiles.Count; i++)
            {
                ProfileSettings profileSettings = new ProfileSettings();
                List<ThemeSettings> themeSettingsList = new List<ThemeSettings>();
                for (int j = 0; j < Profiles[i].Themes.Count; j++)
                {
                    Theme theme = Profiles[i].Themes[j];
                    ThemeSettings themeSettings = new ThemeSettings();
                    if (Profiles[i].Target != null && theme != null)
                    {
                        List<InteractableThemePropertySettings> tempSettings = new List<InteractableThemePropertySettings>();
                        for (int n = 0; n < theme.Settings.Count; n++)
                        {
                            InteractableThemePropertySettings settings = theme.Settings[n];
                            settings.Theme = InteractableProfileItem.GetTheme(settings, Profiles[i].Target);

                            // add themes to theme list based on dimension
                            if (j == dimensionIndex)
                            {
                                runningThemesList.Add(settings.Theme);
                            }

                            tempSettings.Add(settings);
                        }

                        themeSettings.Settings = tempSettings;
                        themeSettingsList.Add(themeSettings);
                    }
                }

                profileSettings.ThemeSettings = themeSettingsList;
                runningProfileSettings.Add(profileSettings);
            }
        }

        #endregion InteractableInitiation

        #region SetButtonStates

        /// <summary>
        /// Grabs the state value index
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int GetStateValue(InteractableStates.InteractableStateEnum state)
        {
            return StateManager.GetStateValue((int)state);
        }

        /// <summary>
        /// Handle focus state changes
        /// </summary>
        /// <param name="focus"></param>
        public virtual void SetFocus(bool focus)
        {
            HasFocus = focus;
            if (!focus && HasPress)
            {
                rollOffTimer = 0;
            }
            else
            {
                rollOffTimer = rollOffTime;
            }

            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Focus, focus ? 1 : 0);
            UpdateState();
        }

        public virtual void SetPress(bool press)
        {
            HasPress = press;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Pressed, press ? 1 : 0);
            UpdateState();
        }

        public virtual void SetDisabled(bool disabled)
        {
            IsDisabled = disabled;
            Enabled = !disabled;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Disabled, disabled ? 1 : 0);
            UpdateState();
        }

        public virtual void SetTargeted(bool targeted)
        {
            IsTargeted = targeted;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Targeted, targeted ? 1 : 0);
            UpdateState();
        }

        public virtual void SetInteractive(bool interactive)
        {
            IsInteractive = interactive;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Interactive, interactive ? 1 : 0);
            UpdateState();
        }

        public virtual void SetObservationTargeted(bool targeted)
        {
            HasObservationTargeted = targeted;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.ObservationTargeted, targeted ? 1 : 0);
            UpdateState();
        }

        public virtual void SetObservation(bool observation)
        {
            HasObservation = observation;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Observation, observation ? 1 : 0);
            UpdateState();
        }

        public virtual void SetVisited(bool visited)
        {
            IsVisited = visited;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, visited ? 1 : 0);
            UpdateState();
        }

        public virtual void SetToggled(bool toggled)
        {
            IsToggled = toggled;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Toggled, toggled ? 1 : 0);
            UpdateState();
        }

        public virtual void SetGesture(bool gesture)
        {
            HasGesture = gesture;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Gesture, gesture ? 1 : 0);
            UpdateState();
        }

        public virtual void SetGestureMax(bool gesture)
        {
            HasGestureMax = gesture;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.GestureMax, gesture ? 1 : 0);
            UpdateState();
        }

        public virtual void SetCollision(bool collision)
        {
            HasCollision = collision;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Collision, collision ? 1 : 0);
            UpdateState();
        }

        public virtual void SetCustom(bool custom)
        {
            HasCustom = custom;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Custom, custom ? 1 : 0);
            UpdateState();
        }

        public virtual void SetVoiceCommand(bool voice)
        {
            HasVoiceCommand = voice;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Custom, voice ? 1 : 0);
            UpdateState();
        }

        public virtual void SetPhysicalTouch(bool touch)
        {
            HasPhysicalTouch = touch;
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.PhysicalTouch, touch ? 1 : 0);
            UpdateState();
        }



        /// <summary>
        /// a public way to set state directly
        /// </summary>
        /// <param name="state"></param>
        /// <param name="value"></param>
        public void SetState(InteractableStates.InteractableStateEnum state, bool value)
        {
            if (StateManager != null)
            {
                StateManager.SetStateValue(state, value ? 1 : 0);
            }

            UpdateState();
        }

        /// <summary>
        /// runs the state logic and sets state based on the current state values
        /// </summary>
        protected virtual void UpdateState()
        {
            StateManager.CompareStates();
        }

        #endregion SetButtonStates

        #region PointerManagement

        /// <summary>
        /// Adds a pointer to pointers, means a pointer is giving focus
        /// </summary>
        /// <param name="pointer"></param>
        private void AddPointer(IMixedRealityPointer pointer)
        {
            if (!pointers.Contains(pointer))
            {
                pointers.Add(pointer);
            }
        }

        /// <summary>
        /// Removes a pointer, lost focus
        /// </summary>
        /// <param name="pointer"></param>
        private void RemovePointer(IMixedRealityPointer pointer)
        {
            pointers.Remove(pointer);
        }

        #endregion PointerManagement

        #region MixedRealityFocusChangedHandlers

        public void OnBeforeFocusChange(FocusEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            if (eventData.NewFocusedObject == gameObject)
            {
                AddPointer(eventData.Pointer);
            }
            else if (eventData.OldFocusedObject == gameObject)
            {
                RemovePointer(eventData.Pointer);
            }
        }

        public void OnFocusChanged(FocusEventData eventData) {}

        #endregion MixedRealityFocusChangedHandlers

        #region MixedRealityFocusHandlers

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (CanInteract())
            {
                SetFocus(pointers.Count > 0);
            }
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (!CanInteract() && !HasFocus)
            {
                return;
            }

            SetFocus(pointers.Count > 0);
        }

        #endregion MixedRealityFocusHandlers

        #region MixedRealityPointerHandlers

        /// <summary>
        /// pointer up event has fired
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            pointerInputAction = eventData.MixedRealityInputAction;
            if ((!CanInteract() && !HasPress))
            {
                return;
            }

            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(false);
                eventData.Use();
            }
        }

        /// <summary>
        /// Pointer down event has fired
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            pointerInputAction = eventData.MixedRealityInputAction;
            if (!CanInteract())
            {
                return;
            }

            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(true);
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            // let the Input Handlers know what the pointer action is
            if (eventData != null)
            {
                pointerInputAction = eventData.MixedRealityInputAction;
            }

            // check to see if is global or focus - or - if is global, pointer event does not fire twice  - or - input event is not taking these actions already
            if (!CanInteract() || (IsGlobal && (inputTimer != null || GlobalClickOrder[1] == 1)))
            {
                return;
            }

            if (StateManager != null)
            {
                if (eventData != null && ShouldListen(eventData.MixedRealityInputAction))
                {
                    if (GlobalClickOrder[1] == 0)
                    {
                        GlobalClickOrder[0] = 1;
                    }
                    IncreaseDimensionIndex();
                    SendOnClick(eventData.Pointer);
                    SetVisited(true);
                    StartInputTimer(false);
                    eventData.Use();
                }
                else if (eventData == null && (HasFocus || IsGlobal)) // handle brute force
                {
                    if (GlobalClickOrder[1] == 0)
                    {
                        GlobalClickOrder[0] = 1;
                    }
                    IncreaseDimensionIndex();
                    StartGlobalVisual(false);
                    SendOnClick(null);
                    SetVisited(true);
                    StartInputTimer(false);
                }
                else if (eventData == null && HasPhysicalTouch) // handle touch interactions
                {
                    if (GlobalClickOrder[1] == 0)
                    {
                        GlobalClickOrder[0] = 1;
                    }
                    IncreaseDimensionIndex();
                    StartGlobalVisual(false);
                    SendOnClick(null);
                    SetVisited(true);
                    StartInputTimer(false);
                }
            }
        }

        /// <summary>
        /// Starts a timer to check if input is in progress
        ///  - Make sure global pointer events are not double firing
        ///  - Make sure Global Input events are not double firing
        ///  - Make sure pointer events are not duplicating an input event
        /// </summary>
        /// <param name="isInput"></param>
        protected void StartInputTimer(bool isInput = false)
        {
            if (IsGlobal || isInput)
            {
                if (inputTimer != null)
                {
                    StopCoroutine(inputTimer);
                    inputTimer = null;
                }

                inputTimer = StartCoroutine(InputDownTimer(clickTime));
            }
        }

        #endregion MixedRealityPointerHandlers

        #region MixedRealityInputHandlers

        /// <summary>
        /// Used for click events for actions not processed by pointer events
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputUp(InputEventData eventData)
        {
            // check global and focus
            if (!CanInteract())
            {
                return;
            }

            if (StateManager != null)
            {
                // check if the InputAction matches - and - if the pointer event did not fire first or is handling these actions, 
                if (eventData != null && ShouldListen(eventData.MixedRealityInputAction) && inputTimer != null && (eventData.MixedRealityInputAction != pointerInputAction || pointerInputAction == MixedRealityInputAction.None))
                {
                    if (GlobalClickOrder[0] == 0)
                    {
                        GlobalClickOrder[1] = 1;
                    }
                    StopCoroutine(inputTimer);
                    inputTimer = null;
                    SetPress(false);

                    IncreaseDimensionIndex();
                    SendOnClick(null);
                    SetVisited(true);

                    eventData.Use();
                }
            }
        }

        /// <summary>
        /// Used to handle global events really, using pointer events for most things
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputDown(InputEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            if (StateManager != null)
            {
                if (eventData != null && ShouldListen(eventData.MixedRealityInputAction) && (eventData.MixedRealityInputAction != pointerInputAction || pointerInputAction == MixedRealityInputAction.None))
                {
                    StartInputTimer(true);
                    SetPress(true);
                    eventData.Use();
                }
            }
        }

        public void OnInputPressed(InputEventData<float> eventData)
        {
        }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            // ignore
        }

        #endregion MixedRealityInputHandlers

        #region DimensionsUtilities

        /// <summary>
        /// A public way to access the current dimension
        /// </summary>
        /// <returns></returns>
        public int GetDimensionIndex()
        {
            return dimensionIndex;
        }

        /// <summary>
        /// a public way to increase a dimension, for cycle button
        /// </summary>
        public void IncreaseDimension()
        {
            IncreaseDimensionIndex();
        }

        /// <summary>
        /// a public way to decrease the dimension
        /// </summary>
        public void DecreaseDimension()
        {
            int index = dimensionIndex;
            if (index > 0)
            {
                index--;
            }
            else
            {
                index = Dimensions - 1;
            }

            SetDimensionIndex(index);
        }

        /// <summary>
        /// a public way to set the dimension index
        /// </summary>
        /// <param name="index"></param>
        public void SetDimensionIndex(int index)
        {
            int currentIndex = dimensionIndex;
            if (index < Dimensions)
            {
                dimensionIndex = index;

                if (currentIndex != dimensionIndex)
                {
                    FilterThemesByDimensions();
                    forceUpdate = true;
                }
            }
        }

        /// <summary>
        /// internal dimension cycling
        /// </summary>
        protected void IncreaseDimensionIndex()
        {
            int currentIndex = dimensionIndex;

            if (dimensionIndex < Dimensions - 1)
            {
                dimensionIndex++;
            }
            else
            {
                dimensionIndex = 0;
            }

            if (currentIndex != dimensionIndex)
            {
                FilterThemesByDimensions();
                forceUpdate = true;
            }
        }

        #endregion DimensionsUtilities

        #region InteractableUtilities

        /// <summary>
        /// Assigns the InputAction based on the InputActionId
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static MixedRealityInputAction ResolveInputAction(int index)
        {
            MixedRealityInputAction[] actions = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;
            index = Mathf.Clamp(index, 0, actions.Length - 1);
            return actions[index];
        }

        /// <summary>
        /// Get the themes based on the current dimesionIndex
        /// </summary>
        protected void FilterThemesByDimensions()
        {
            runningThemesList = new List<InteractableThemeBase>();

            for (int i = 0; i < runningProfileSettings.Count; i++)
            {
                ProfileSettings settings = runningProfileSettings[i];
                ThemeSettings themeSettings = settings.ThemeSettings[dimensionIndex];
                for (int j = 0; j < themeSettings.Settings.Count; j++)
                {
                    runningThemesList.Add(themeSettings.Settings[j].Theme);
                }
            }
        }

        /// <summary>
        /// Based on inputAction and state, should this interaction listen to this input?
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual bool ShouldListen(MixedRealityInputAction action)
        {
            bool isListening = HasFocus || IsGlobal;
            return action == InputAction && isListening;
        }

        /// <summary>
        /// Based on button settings and state, should this button listen to input?
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanInteract()
        {
            if (!Enabled)
            {
                return false;
            }

            if (Dimensions > 1 && ((dimensionIndex != Dimensions - 1 & !CanSelect) || (dimensionIndex == Dimensions - 1 & !CanDeselect)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// call onClick methods on receivers or IInteractableHandlers
        /// </summary>
        protected void SendOnClick(IMixedRealityPointer pointer)
        {
            OnClick.Invoke();

            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnClick(StateManager, this, pointer);
                }
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i] != null)
                {
                    handlers[i].OnClick(StateManager, this, pointer);
                }
            }
        }

        /// <summary>
        /// sets some visual states for automating button events like clicks from a keyword
        /// </summary>
        /// <param name="voiceCommand"></param>
        protected void StartGlobalVisual(bool voiceCommand = false)
        {
            if (voiceCommand)
            {
                StateManager.SetStateValue(InteractableStates.InteractableStateEnum.VoiceCommand, 1);
            }

            SetVisited(true);
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Focus, 1);
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Pressed, 1);
            UpdateState();

            if (globalTimer != null)
            {
                StopCoroutine(globalTimer);
            }

            globalTimer = StartCoroutine(GlobalVisualReset(clickTime));
        }

        /// <summary>
        /// Clears up any automated visual states
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected IEnumerator GlobalVisualReset(float time)
        {
            yield return new WaitForSeconds(time);

            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.VoiceCommand, 0);
            if (!HasFocus)
            {
                StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Focus, 0);
            }

            if (!HasPress)
            {
                StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Pressed, 0);
            }

            UpdateState();

            globalTimer = null;
        }

        /// <summary>
        /// A timer for the MixedRealityInputHandlers, clicks should occur within a certain time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected IEnumerator InputDownTimer(float time)
        {
            yield return new WaitForSeconds(time);
            inputTimer = null;
        }

        #endregion InteractableUtilities

        #region VoiceCommands

        /// <summary>
        /// Voice commands from MixedRealitySpeechCommandProfile, keyword recognized
        /// requires isGlobal
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.Command.Keyword == VoiceCommand && (!RequiresFocus || HasFocus) && Enabled)
            {
                StartGlobalVisual(true);
                IncreaseDimensionIndex();
                OnPointerClicked(null);
                eventData.Use();
            }

            // TODO(https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/3767): Need to merge this
            // work below with the code above.
            // if (Enabled && ShouldListen(eventData.MixedRealityInputAction))
            // {
            //     StartGlobalVisual(true);                
            //     IncreaseDimensionIndex();
            //     SendVoiceCommands(eventData.RecognizedText, 0, 1);
            //     SendOnClick(null);
            //     eventData.Use();
            // }
        }

        /// <summary>
        /// call OnVoinceCommand methods on receivers or IInteractableHandlers
        /// </summary>
        protected void SendVoiceCommands(string command, int index, int length)
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnVoiceCommand(StateManager, this, command, index, length);
                }
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i] != null)
                {
                    handlers[i].OnVoiceCommand(StateManager, this, command, index, length);
                }
            }
        }

        /// <summary>
        /// checks the voiceCommand array for a keyword and returns it's index
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected int GetVoiceCommandIndex(string command)
        {
            if (voiceCommands.Length > 1)
            {
                for (int i = 0; i < voiceCommands.Length; i++)
                {
                    if (command == voiceCommands[i])
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        #endregion VoiceCommands

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            SetPress(true);
            eventData.Use();
        }

        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            SetPress(false);
            eventData.Use();
        }

        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData) { }
    }
}
