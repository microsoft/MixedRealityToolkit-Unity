// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Uses input and action data to declare a set of states
    /// Maintains a collection of themes that react to state changes and provide scensory feedback
    /// Passes state information and input data on to receivers that detect patterns and does stuff.
    /// </summary>
    
    // TODO: How to handle cycle buttons
    // TODO: plumb for gestures
    // TODO: Add way to protect the defaultTheme from being edited and encourage users to create a new theme, maybe include a create/duplicate button
    // TODO: Make sure all shader values are batched by theme
    // TODO: Setup pointer and pointer data and list of actions to handle pointer action combos, like grip and stick input

    // BUG: Adding dimensions resets all themes to use default them
    // BUG: Removing events or themes removes the wrong one

    [System.Serializable]

    public class Interactable : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityFocusHandler, IMixedRealityPointerHandler, IMixedRealitySpeechHandler // TEMP , IInputClickHandler, IFocusable, IInputHandler
    {
        /// <summary>
        /// Setup the input system
        /// </summary>
        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

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
        public bool RequiresGaze = true;

        /// <summary>
        /// Does this interactable require focus
        /// </summary>
        public bool FocusEnabled { get { return !IsGlobal; } set { IsGlobal = !value; } }

        // list of profiles can match themes with gameObjects
        public List<ProfileItem> Profiles = new List<ProfileItem>();
        // Base onclick event
        public UnityEvent OnClick;
        // list of events added to this interactable
        public List<InteractableEvent> Events = new List<InteractableEvent>();
        // the list of running theme instances to receive state changes
        public List<ThemeBase> runningThemesList = new List<ThemeBase>();

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
        public bool HasCustom { get; private set; }

        // internal cached states
        protected State lastState;
        protected bool wasDisabled = false;

        // cache of current dimenion
        protected int dimensionIndex = 0;

        // allows for switching colliders without firing a lose focus imediately
        // for advanced controls like drop-downs
        protected float rollOffTime = 0.25f;
        protected float rollOffTimer = 0.25f;

        #region InspectorHelpers
        /// <summary>
        /// Gets a list of input actions, used by the inspector
        /// </summary>
        /// <returns></returns>
        public static string[] GetInputActions()
        {
            MixedRealityInputAction[] actions = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;

            List<string> list = new List<string>();
            for (int i = 0; i < actions.Length; i++)
            {
                list.Add(actions[i].Description);
            }

            return list.ToArray();
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
            InputSystem.Register(gameObject);
        }

        private void OnDisable()
        {
            InputSystem.Unregister(gameObject);
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
                //print(name + " - State Change: " + StateManager.CurrentState());
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
            InteractableEvent.EventLists lists = InteractableEvent.GetEventTypes();
            
            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].Receiver = InteractableEvent.GetReceiver(Events[i], lists);
                //Events[i].Settings = InteractableEvent.GetSettings(Events[i].Receiver);
                // apply settings
            }
        }

        /// <summary>
        /// Creates the list of theme instances based on all the theme settings
        /// </summary>
        protected virtual void SetupThemes()
        {
            ProfileItem.ThemeLists lists = ProfileItem.GetThemeTypes();
            runningThemesList = new List<ThemeBase>();
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
                        List<ThemePropertySettings> tempSettings = new List<ThemePropertySettings>();
                        for (int n = 0; n < theme.Settings.Count; n++)
                        {
                            ThemePropertySettings settings = theme.Settings[n];

                            settings.Theme = ProfileItem.GetTheme(settings, Profiles[i].Target, lists);
                            
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
            if(!focus && HasPress)
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

        #region MixedRealityFocusHandlers

        public void OnFocusEnter(FocusEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            AddPointer(eventData.Pointer);
            SetFocus(pointers.Count > 0);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            if (!CanInteract() && !HasFocus)
            {
                return;
            }

            RemovePointer(eventData.Pointer);
            SetFocus(pointers.Count > 0);
        }

        public void OnBeforeFocusChange(FocusEventData eventData)
        {
            //do nothing
        }

        public void OnFocusChanged(FocusEventData eventData)
        {
            //do nothing
        }

        #endregion MixedRealityFocusHandlers

        #region MixedRealityInputHandlers

        public void OnInputUp(InputEventData eventData)
        {
            //ignore for now
            return;

            if (!CanInteract() && !HasPress)
            {
                return;
            }

            print("Input Up: " + pointers.Count + " / " + name + " / " + HasFocus);
            
            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(false);
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            //ignore for now
            return;

            if (!CanInteract())
            {
                return;
            }

            print("Input Down: " + pointers.Count + " / " + name + " / " + HasFocus);
            if(ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(true);
            }
        }

        public void OnInputPressed(InputEventData<float> eventData)
        {
            // ignore for now - using pointer events,
            // but may need to come back to these events for menu and trigger filtering

            //print("PRESSED: " + eventData.InputData + " / " + eventData.MixedRealityInputAction.Description + " / " + name);

            if (!CanInteract() || true)
            {
                return;
            }

            if (StateManager != null)
            {
                if (eventData == null && (HasFocus || IsGlobal)) // handle brute force
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    IncreaseDimensionIndex();
                    OnClick.Invoke();
                }
                else if (eventData != null && ShouldListen(eventData.MixedRealityInputAction))
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    IncreaseDimensionIndex();
                    OnClick.Invoke();
                }
            }
        }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            //not yet
        }

        #endregion MixedRealityInputHandlers

        #region MixedRealityPointerHandlers

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if ((!CanInteract() && !HasPress))
            {
                return;
            }

            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(false);
            }
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(true);
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            //print(eventData.MixedRealityInputAction.Description);

            if (StateManager != null)
            {
                if (eventData != null && ShouldListen(eventData.MixedRealityInputAction))
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    IncreaseDimensionIndex();
                    print("CLICK invoked!");
                    OnClick.Invoke();
                }
                else if (eventData == null && (HasFocus || IsGlobal)) // handle brute force
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    IncreaseDimensionIndex();
                    OnClick.Invoke();
                }
            }
        }

        #endregion MixedRealityPointerHandlers

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
        /// internal deminsion cycling
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
            
            if(currentIndex != dimensionIndex)
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
            MixedRealityInputAction[] actions = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;
            index = Mathf.Clamp(index, 0, actions.Length - 1);
            return actions[index];
        }

        /// <summary>
        /// Get the themes based on the current dimesionIndex
        /// </summary>
        protected void FilterThemesByDimensions()
        {
            runningThemesList = new List<ThemeBase>();

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

            if (Dimensions > 1 && ((dimensionIndex != Dimensions -1 & !CanSelect) || (dimensionIndex == Dimensions - 1 & !CanDeselect)) )
            {
                return false;
            }

            return true;
        }

        #endregion InteractableUtilities
        
        /// <summary>
        /// Voice commands, keyword recognized
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.RecognizedText == VoiceCommand && (!RequiresGaze || HasFocus) && Enabled)
            {
                OnPointerClicked(null);
            }
        }
    }
}
