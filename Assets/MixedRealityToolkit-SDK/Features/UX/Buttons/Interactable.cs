using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity
{
    [System.Serializable]

    public class Interactable : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityFocusHandler, IMixedRealitySpeechHandler // TEMP , IInputClickHandler, IFocusable, IInputHandler
    {

        // TODO: Has bug where the disabled state is getting set for all buttons!!!!!!!!!!!!
        private static IMixedRealityInputSystem inputSystem = null;
        protected static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        public bool Enabled = true;
        public States States;
        public InteractableStates StateManager;
        public MixedRealityInputAction InputAction;
        [HideInInspector]
        public int InputActionId;
        public bool IsGlobal = false;
        public int Dimensions = 1;
        public string VoiceCommand = "";
        public bool RequiresGaze = true;
        public List<ProfileItem> Profiles = new List<ProfileItem>();
        public UnityEvent OnClick;
        public List<InteractableEvent> Events = new List<InteractableEvent>();
        
        public List<ThemeBase> runningThemesList = new List<ThemeBase>();

        public bool HasFocus { get; private set; }
        public bool HasPress { get; private set; }
        public bool IsDisabled { get; private set; }

        public bool FocusEnabled
        {
            get
            {
                return !IsGlobal;
            }

            set
            {
                IsGlobal = !value;
            }
        }

        public List<IMixedRealityPointer> Focusers => pointers;

        private State lastState;
        private bool wasDisabled = false;
        private List<IMixedRealityPointer> pointers = new List<IMixedRealityPointer>();

        // these should get simplified and moved
        // create a ScriptableObject for managing states!!!!
        
        public State[] GetStates()
        {
            if (States != null)
            {
                return States.GetStates();
            }

            //InteractableStates states = new InteractableStates(InteractableStates.Default);
            //return states.GetStates();
            
            return new State[0];
        }

        protected virtual void Awake()
        {
            //State = new InteractableStates(InteractableStates.Default);
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

        protected virtual void SetupStates()
        {
            StateManager = States.SetupLogic();
        }

        protected virtual void SetupEvents()
        {
            InteractableEvent.EventLists lists = InteractableEvent.GetEventTypes();
            
            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].Receiver = InteractableEvent.GetReceiver(Events[i], lists);
                // apply settings
            }
        }

        protected virtual void SetupThemes()
        {
            ProfileItem.ThemeLists lists = ProfileItem.GetThemeTypes();
            runningThemesList = new List<ThemeBase>();

            for (int i = 0; i < Profiles.Count; i++)
            {
                for (int j = 0; j < Profiles[i].Themes.Count; j++)
                {
                    Theme theme = Profiles[i].Themes[j];
                    if (Profiles[i].Target != null) {
                        for (int n = 0; n < theme.Settings.Count; n++)
                        {
                            ThemePropertySettings settings = theme.Settings[n];

                            settings.Theme = ProfileItem.GetTheme(settings, Profiles[i].Target, lists);

                            theme.Settings[n] = settings;
                            if (theme != null)
                            {
                                runningThemesList.Add(settings.Theme);
                            }
                        }

                        if (theme != null)
                        {
                            Profiles[i].Themes[j] = theme;
                        }
                    }
                }
            }
        }

        //collider checks and other alerts

        public int GetStateValue(InteractableStates.InteractableStateEnum state)
        {
            return StateManager.GetStateValue((int)state);
        }

        // state management
        public virtual void SetFocus(bool focus)
        {
            HasFocus = focus;
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
            StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Disabled, disabled ? 1 : 0);
            UpdateState();
        }

        private void AddPointer(IMixedRealityPointer pointer)
        {
            if (!pointers.Contains(pointer))
            {
                pointers.Add(pointer);
            }
        }

        private void RemovePointer(IMixedRealityPointer pointer)
        {
            pointers.Remove(pointer);
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            AddPointer(eventData.Pointer);
            //print("Enter: " + pointers.Count);
            SetFocus(pointers.Count > 0);
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            RemovePointer(eventData.Pointer);
            //print("Exit: " + pointers.Count);
            SetFocus(pointers.Count > 0);
        }

        public void OnBeforeFocusChange(FocusEventData eventData)
        {
            //throw new NotImplementedException();
        }

        public void OnFocusChanged(FocusEventData eventData)
        {
            //throw new NotImplementedException();
        }

        public void OnInputUp(InputEventData eventData)
        {
            print("Up: " + pointers.Count);
            if (ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(false);
            }
        }

        public void OnInputDown(InputEventData eventData)
        {
            print("Down: " + pointers.Count);
            if(ShouldListen(eventData.MixedRealityInputAction))
            {
                SetPress(true);
            }
        }

        public void OnInputPressed(InputEventData<float> eventData)
        {
            if (StateManager != null)
            {
                State[] states = StateManager.GetStates();
                if (StateManager.CurrentState() != StateManager.GetState(InteractableStates.InteractableStateEnum.Disabled) && ShouldListen(eventData.MixedRealityInputAction))
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    OnClick.Invoke();
                }
            }
        }

        public void OnPositionInputChanged(InputEventData<Vector2> eventData)
        {
            //throw new NotImplementedException();
        }

        public virtual void OnInputClicked(UnityEngine.Object eventData)// TEMP
        {
            if (StateManager != null)
            {
                State[] states = StateManager.GetStates();
                if (true)// TEMP StateManager.CurrentState() != StateManager.GetState(InteractableStates.InteractableStateEnum.Disabled) && eventData.PressType == ButtonPressFilter)
                {
                    StateManager.SetStateValue(InteractableStates.InteractableStateEnum.Visited, 1);
                    OnClick.Invoke();
                }
            }
        }

        protected virtual bool ShouldListen(MixedRealityInputAction action)
        {
            return action == InputAction;
        }

        protected virtual void UpdateState()
        {
            StateManager.CompareStates();
        }

        protected virtual void Update()
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].Receiver != null)
                {
                    Events[i].Receiver.OnUpdate(StateManager);
                    ReceiverBase reciever = Events[i].Receiver;
                }
            }

            for (int i = 0; i < runningThemesList.Count; i++)
            {
                if (runningThemesList[i].Loaded)
                {
                    runningThemesList[i].OnUpdate(StateManager.CurrentState().ActiveIndex);
                }
            }

            if (lastState != StateManager.CurrentState())
            {

            }

            if(IsDisabled == Enabled)
            {
                SetDisabled(!Enabled);
            }

            lastState = StateManager.CurrentState();
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.RecognizedText == VoiceCommand && (!RequiresGaze || HasFocus) && Enabled)
            {
                //OnInputClicked(null); 
            }
        }
    }
}
