// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Uses input and action data to declare a set of states
    /// Maintains a collection of themes that react to state changes and provide sensory feedback
    /// Passes state information and input data on to receivers that detect patterns and does stuff.
    /// </summary>
    [System.Serializable]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/interactable")]
    [AddComponentMenu("Scripts/MRTK/SDK/Interactable")]
    public class Interactable :
        MonoBehaviour,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler,
        IMixedRealityInputHandler,
        IMixedRealitySpeechHandler,
        IMixedRealityTouchHandler,
        IMixedRealityInputHandler<Vector2>,
        IMixedRealityInputHandler<Vector3>,
        IMixedRealityInputHandler<MixedRealityPose>
    {
        /// <summary>
        /// Pointers that are focusing the interactable
        /// </summary>
        public List<IMixedRealityPointer> FocusingPointers => focusingPointers;
        protected readonly List<IMixedRealityPointer> focusingPointers = new List<IMixedRealityPointer>();

        /// <summary>
        /// Input sources that are pressing the interactable
        /// </summary>
        public HashSet<IMixedRealityInputSource> PressingInputSources => pressingInputSources;
        protected readonly HashSet<IMixedRealityInputSource> pressingInputSources = new HashSet<IMixedRealityInputSource>();

        [FormerlySerializedAs("States")]
        [SerializeField]
        [Tooltip("ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be \"DefaultInteractableStates\" object")]
        private States states;

        /// <summary>
        /// ScriptableObject to reference for basic state logic to follow when interacting and transitioning between states. Should generally be "DefaultInteractableStates" object
        /// </summary>
        public States States
        {
            get => states;
            set
            {
                states = value;
                SetupStates();
            }
        }

        /// <summary>
        /// The state logic class for storing and comparing states which determines the current value.
        /// </summary>
        public InteractableStates StateManager { get; protected set; }

        /// <summary>
        /// The Interactable will only respond to input down events fired with the corresponding assigned Input Action.
        /// Available input actions are populated via the Input Actions Profile under the MRTK Input System Profile assigned in the current scene
        /// </summary>
        public MixedRealityInputAction InputAction { get; set; }

        /// <summary>
        /// The id of the selected inputAction, for serialization
        /// </summary>
        [HideInInspector]
        [SerializeField]
        [Tooltip("The Interactable will only respond to input down events fired with the corresponding assigned Input Action." +
        "Available input actions are populated via the Input Actions Profile under the MRTK Input System Profile assigned in the current scene.")]
        private int InputActionId = 0;

        [FormerlySerializedAs("IsGlobal")]
        [SerializeField]
        [Tooltip("If true, this Interactable will listen globally for any IMixedRealityInputHandler input events. These include general input up/down and clicks." +
        "If false, this Interactable will only respond to general input click events if the pointer target is this GameObject's, or one of its children's, collider.")]
        protected bool isGlobal = false;
        /// <summary>
        /// If true, this Interactable will listen globally for any IMixedRealityInputHandler input events. These include general input up/down and clicks.
        /// If false, this Interactable will only respond to general input click events if the pointer target is this GameObject's, or one of its children's, collider.
        /// </summary>
        public bool IsGlobal
        {
            get => isGlobal;
            set
            {
                if (isGlobal != value)
                {
                    isGlobal = value;

                    // If we are active, then register or unregister our the global input handler with the InputSystem
                    // If we are disabled, then we will re-register OnEnable()
                    if (gameObject.activeInHierarchy)
                    {
                        RegisterHandler<IMixedRealityInputHandler>(isGlobal);
                    }
                }
            }
        }

        /// <summary>
        /// A way of adding more layers of states for controls like toggles.
        /// This is capitalized and doesn't match conventions for backwards compatibility
        /// (to not break people using Interactable). We tried using FormerlySerializedAs("Dimensions)
        /// and renaming to "dimensions", however Unity did not properly pick up the former serialization,
        /// so we maintained the old value. See https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6169
        /// </summary>
        [SerializeField]
        protected int Dimensions = 1;
        /// <summary>
        /// A way of adding more layers of states for controls like toggles
        /// </summary>
        public int NumOfDimensions
        {
            get
            {
                EnsureInitialized();
                return Dimensions;
            }
            set
            {
                EnsureInitialized();

                if (Dimensions != value)
                {
                    // Value cannot be negative or zero
                    if (value > 0)
                    {
                        // If we are currently in Toggle mode, we are about to not be
                        // Auto-turn off state
                        if (ButtonMode == SelectionModes.Toggle)
                        {
                            IsToggled = false;
                        }

                        Dimensions = value;

                        CurrentDimension = Mathf.Clamp(CurrentDimension, 0, Dimensions - 1);
                    }
                    else
                    {
                        Debug.LogWarning($"Value {value} for Dimensions property setter cannot be negative or zero.");
                    }
                }
            }
        }

        // cache of current dimension
        [SerializeField]
        protected int dimensionIndex = 0;
        /// <summary>
        /// Current Dimension index based zero and must be less than Dimensions
        /// </summary>
        public int CurrentDimension
        {
            get
            {
                EnsureInitialized();
                return dimensionIndex;
            }
            set
            {
                EnsureInitialized();

                if (dimensionIndex != value)
                {
                    // If valid value and not our current value, then update
                    if (value >= 0 && value < NumOfDimensions)
                    {
                        dimensionIndex = value;

                        // If we are in toggle mode, update IsToggled state based on current dimension
                        // This needs to happen after updating dimensionIndex, since IsToggled.set will call CurrentDimension.set again
                        if (ButtonMode == SelectionModes.Toggle)
                        {
                            IsToggled = dimensionIndex > 0;
                        }

                        UpdateActiveThemes();
                        forceUpdate = true;
                    }
                    else
                    {
                        Debug.LogWarning($"On GameObject {gameObject.name}, value {value} for property setter CurrentDimension cannot be less than 0 and cannot be greater than or equal to Dimensions={NumOfDimensions}.");
                    }
                }
            }
        }

        /// <summary>
        /// Returns the current selection mode of the Interactable based on the number of Dimensions available
        /// </summary>
        /// <remarks>
        /// <para>Returns the following under the associated conditions:</para>
        /// <para>SelectionModes.Invalid => Dimensions less than or equal to 0</para>
        /// <para>SelectionModes.Button => Dimensions == 1</para>
        /// <para>SelectionModes.Toggle => Dimensions == 2</para>
        /// <para>SelectionModes.MultiDimension => Dimensions > 2</para>
        /// </remarks>
        public SelectionModes ButtonMode => ConvertToSelectionMode(NumOfDimensions);

        /// <summary>
        /// The Dimension value to set on start
        /// </summary>
        [FormerlySerializedAs("StartDimensionIndex")]
        [SerializeField]
        private int startDimensionIndex = 0;

        /// <summary>
        /// Is the interactive selectable?
        /// When a multi-dimension button, can the user initiate switching dimensions?
        /// </summary>
        public bool CanSelect = true;

        /// <summary>
        /// Can the user deselect a toggle?
        /// A radial button or tab should set this to false
        /// </summary>
        public bool CanDeselect = true;

        [SpeechKeyword]
        [SerializeField, FormerlySerializedAs("VoiceCommand")]
        [Tooltip("This string keyword is the voice command that will fire a click on this Interactable.")]
        private string voiceCommand = "";

        /// <summary>
        /// This string keyword is the voice command that will fire a click on this Interactable.
        /// </summary>
        public string VoiceCommand
        {
            get => voiceCommand;
            set => voiceCommand = value;
        }

        [SerializeField, FormerlySerializedAs("RequiresFocus")]
        [Tooltip("If true, then the voice command will only respond to voice commands while this Interactable has focus.")]
        public bool voiceRequiresFocus = true;

        /// <summary>
        /// Does the voice command require this to have focus?
        /// Registers as a global listener for speech commands, ignores input events
        /// </summary>
        public bool VoiceRequiresFocus
        {
            get => voiceRequiresFocus;
            set
            {
                if (voiceRequiresFocus != value)
                {
                    voiceRequiresFocus = value;

                    // If we are active, then change global speech registration. 
                    // Register handle if we do not require focus, unregister otherwise
                    if (gameObject.activeInHierarchy)
                    {
                        RegisterHandler<IMixedRealitySpeechHandler>(!voiceRequiresFocus);
                    }
                }
            }
        }

        [FormerlySerializedAs("Profiles")]
        [SerializeField]
        private List<InteractableProfileItem> profiles = new List<InteractableProfileItem>();
        /// <summary>
        /// List of profile configurations that match Visual Themes with GameObjects targets
        /// Setting at runtime will re-create the runtime Theme Engines (i.e ActiveThemes property) being used by this class
        /// </summary>
        public List<InteractableProfileItem> Profiles
        {
            get => profiles;
            set
            {
                profiles = value;
                SetupThemes();
            }
        }

        /// <summary>
        /// Base onclick event
        /// </summary>
        public UnityEvent OnClick = new UnityEvent();

        [SerializeField]
        private List<InteractableEvent> Events = new List<InteractableEvent>();
        /// <summary>
        /// List of events added to this interactable
        /// </summary>
        public List<InteractableEvent> InteractableEvents
        {
            get => Events;
            set
            {
                Events = value;
                SetupEvents();
            }
        }

        [Tooltip("If true, when this component is destroyed, active themes will reset their modified properties to original values on the targeted GameObjects. If false, GameObject properties will remain as-is.")]
        [SerializeField]
        private bool resetOnDestroy = false;

        /// <summary>
        /// If true, when this component is destroyed, active themes will reset their modified properties to original values on the targeted GameObjects. If false, GameObject properties will remain as-is.
        /// </summary>
        public bool ResetOnDestroy
        {
            get => resetOnDestroy;
            set => resetOnDestroy = value;
        }

        private List<InteractableThemeBase> activeThemes = new List<InteractableThemeBase>();

        /// <summary>
        /// The list of running theme instances to receive state changes
        /// When the dimension index changes, activeThemes updates to those assigned to that dimension.
        /// </summary>
        public IReadOnlyList<InteractableThemeBase> ActiveThemes => activeThemes.AsReadOnly();

        /// <summary>
        /// List of (dimension index, InteractableThemeBase) pairs that describe all possible themes the
        /// interactable can have. First element in the tuple represents dimension index for the theme.
        /// This list gets initialized on startup, or whenever the profiles for the interactable changes.
        /// The list of active themes inspects this list to determine which themes to use based on current dimension.
        /// </summary>
        private List<System.Tuple<int, InteractableThemeBase>> allThemeDimensionPairs = new List<System.Tuple<int, InteractableThemeBase>>();

        /// <summary>
        /// How many times this interactable was clicked
        /// </summary>
        /// <remarks>
        /// Useful for checking when a click event occurs.
        /// </remarks>
        public int ClickCount { get; private set; }

        #region States

        // Field just used for serialization to save if the Interactable should start enabled or disabled
        [FormerlySerializedAs("Enabled")]
        [SerializeField]
        [Tooltip("Defines whether the Interactable is enabled or not internally." +
        "This is different than the enabled property at the GameObject/Component level." +
        "When false, Interactable will continue to run in Unity but not respond to Input." +
        "\n\nProperty is useful for disabling UX, such as greying out a button, until a user completes some pre-mandatory step such as fill out their name, etc")]
        private bool enabledOnStart = true;

        /// <summary>
        /// Defines whether the Interactable is enabled or not internally
        /// This is different than the Enabled property at the GameObject/Component level
        /// When false, Interactable will continue to run in Unity but not respond to Input.
        /// </summary>
        /// <remarks>
        /// Property is useful for disabling UX, such as greying out a button, until a user completes some pre-mandatory step such as fill out their name, etc
        /// </remarks>
        public virtual bool IsEnabled
        {
            // Note the inverse setting since targeting "Disable" state but property is concerning "Enabled"
            get
            {
                return !(GetStateValue(InteractableStates.InteractableStateEnum.Disabled) > 0);
            }
            set
            {
                EnsureInitialized();

                if (IsEnabled != value)
                {
                    // If we are disabling input, we should reset our base input tracking states since we will not be responding to input while disabled
                    if (!value)
                    {
                        ResetInputTrackingStates();
                    }

                    SetState(InteractableStates.InteractableStateEnum.Disabled, !value);
                }
            }
        }

        /// <summary>
        /// Has focus
        /// </summary>
        public virtual bool HasFocus
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Focus) > 0;
            set
            {
                EnsureInitialized();

                if (HasFocus != value)
                {
                    if (!value && HasPress)
                    {
                        rollOffTimer = 0;
                    }
                    else
                    {
                        rollOffTimer = RollOffTime;
                    }

                    SetState(InteractableStates.InteractableStateEnum.Focus, value);
                }
            }
        }

        /// <summary>
        /// Currently being pressed
        /// </summary>
        public virtual bool HasPress
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Pressed) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Pressed, value);
        }

        /// <summary>
        /// Targeted means the item has focus and finger is up
        /// Currently not controlled by Interactable directly
        /// </summary>
        public virtual bool IsTargeted
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Targeted) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Targeted, value);
        }

        /// <summary>
        /// State that corresponds to no focus,and finger is up.
        /// Currently not controlled by Interactable directly
        /// </summary>
        public virtual bool IsInteractive
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Interactive) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Interactive, value);
        }

        /// <summary>
        /// State that corresponds to has focus,and finger down.
        /// Currently not controlled by Interactable directly
        /// </summary>
        public virtual bool HasObservationTargeted
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.ObservationTargeted) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.ObservationTargeted, value);
        }

        /// <summary>
        /// State that corresponds to no focus,and finger is down.
        /// Currently not controlled by Interactable directly
        /// </summary>
        public virtual bool HasObservation
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Observation) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Observation, value);
        }

        /// <summary>
        /// The Interactable has been clicked
        /// </summary>
        public virtual bool IsVisited
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Visited) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Visited, value);
        }

        /// <summary>
        /// Determines whether Interactable is toggled or not. If true, CurrentDimension should be 1 and if false, CurrentDimension should be 0
        /// </summary>
        /// <remarks>
        /// Only valid when ButtonMode == SelectionMode.Toggle (i.e Dimensions == 2)
        /// </remarks>
        public virtual bool IsToggled
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Toggled) > 0;
            set
            {
                EnsureInitialized();

                if (IsToggled != value)
                {
                    // We can only change Toggle state if we are in Toggle mode
                    if (ButtonMode == SelectionModes.Toggle)
                    {
                        SetState(InteractableStates.InteractableStateEnum.Toggled, value);

                        CurrentDimension = value ? 1 : 0;
                    }
                    else
                    {
                        Debug.LogWarning($"SetToggled(bool) called, but SelectionMode is set to {ButtonMode}, so Current Dimension was unchanged.");
                    }
                }
            }
        }

        /// <summary>
        /// Currently pressed and some movement has occurred
        /// </summary>
        public virtual bool HasGesture
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Gesture) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Gesture, value);
        }

        /// <summary>
        /// State that corresponds to Gesture reaching max threshold or limits
        /// </summary>
        public virtual bool HasGestureMax
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.GestureMax) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.GestureMax, value);
        }

        /// <summary>
        /// State that corresponds to Interactable is touching another object 
        /// </summary>
        public virtual bool HasCollision
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Collision) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Collision, value);
        }

        /// <summary>
        /// A voice command has just occurred
        /// </summary>
        public virtual bool HasVoiceCommand
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.VoiceCommand) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.VoiceCommand, value);
        }

        /// <summary>
        /// A near interaction touchable is actively being touched
        /// </summary>
        public virtual bool HasPhysicalTouch
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.PhysicalTouch) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.PhysicalTouch, value);
        }

        /// <summary>
        /// State that corresponds to miscellaneous/custom use by consumers
        /// Currently not controlled by Interactable directly
        /// </summary>
        public virtual bool HasCustom
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Custom) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Custom, value);
        }

        /// <summary>
        /// A near interaction grabbable is actively being grabbed
        /// </summary>
        public virtual bool HasGrab
        {
            get => GetStateValue(InteractableStates.InteractableStateEnum.Grab) > 0;
            set => SetState(InteractableStates.InteractableStateEnum.Grab, value);
        }

        #endregion

        protected State lastState;

        // directly manipulate a theme value, skip blending
        protected bool forceUpdate = false;

        /// <summary>
        /// Allows for switching colliders without firing a lose focus immediately for advanced controls like drop-downs
        /// </summary>
        public float RollOffTime { get; protected set; } = 0.25f;
        protected float rollOffTimer = 0.25f;

        protected List<IInteractableHandler> handlers = new List<IInteractableHandler>();

        /// <summary>
        /// A click must occur within this many seconds after an input down
        /// </summary>
        protected float clickTime = 1.5f;
        protected Coroutine clickValidTimer;

        /// <summary>
        /// Amount of time to "simulate" press states for interactions that do not utilize input up/down such as voice command
        /// This allows for visual feedbacks and other typical UX responsiveness and behavior to occur
        /// </summary>
        protected const float globalFeedbackClickTime = 0.3f;
        protected Coroutine globalTimer;

        #region Gesture State Variables

        /// <summary>
        /// The position of the controller when input down occurs.
        /// Used to determine when controller has moved far enough to trigger gesture
        /// </summary>
        protected Vector3? dragStartPosition = null;

        // Input must move at least this distance before a gesture is considered started, for 2D input like thumbstick
        static readonly float gestureStartThresholdVector2 = 0.1f;

        // Input must move at least this distance before a gesture is considered started, for 3D input
        static readonly float gestureStartThresholdVector3 = 0.05f;

        // Input must move at least this distance before a gesture is considered started, for
        // mixed reality pose input. This is the distance and hand or controller needs to move
        static readonly float gestureStartThresholdMixedRealityPose = 0.1f;

        #endregion

        // Track that the GameObject has been activated (i.e Awake() or Initialize() has been called)
        private bool isInitialized = false;

        #region MonoBehaviour Implementation

        /// <inheritdoc/>
        protected virtual void Awake()
        {
            EnsureInitialized();
        }

        /// <inheritdoc/>
        protected virtual void OnEnable()
        {
            if (!VoiceRequiresFocus)
            {
                RegisterHandler<IMixedRealitySpeechHandler>(true);
            }

            if (IsGlobal)
            {
                RegisterHandler<IMixedRealityInputHandler>(true);
            }

            focusingPointers.RemoveAll((focusingPointer) => (focusingPointer.FocusTarget as Interactable) != this);

            if (focusingPointers.Count == 0)
            {
                ResetInputTrackingStates();
            }
        }

        /// <inheritdoc/>
        protected virtual void OnDisable()
        {
            // If we registered to receive global events, remove ourselves when disabled
            if (!VoiceRequiresFocus)
            {
                RegisterHandler<IMixedRealitySpeechHandler>(false);
            }

            if (IsGlobal)
            {
                RegisterHandler<IMixedRealityInputHandler>(false);
            }

            ResetInputTrackingStates();
        }

        /// <inheritdoc/>
        protected virtual void Start()
        {
            InternalUpdate();
        }

        /// <inheritdoc/>
        protected virtual void Update()
        {
            InternalUpdate();
        }

        /// <inheritdoc/>
        protected virtual void OnDestroy()
        {
            if (ResetOnDestroy)
            {
                foreach (var theme in activeThemes)
                {
                    theme.Reset();
                }
            }
        }

        private void InternalUpdate()
        {
            if (rollOffTimer < RollOffTime && HasPress)
            {
                rollOffTimer += Time.deltaTime;

                if (rollOffTimer >= RollOffTime)
                {
                    HasPress = false;
                    HasGesture = false;
                }
            }

            int interactableEventsCount = InteractableEvents.Count;
            for (int i = 0; i < interactableEventsCount; i++)
            {
                InteractableEvent interactableEvent = InteractableEvents[i];
                if (interactableEvent.Receiver != null)
                {
                    interactableEvent.Receiver.OnUpdate(StateManager, this);
                }
            }

            int activeThemesCount = activeThemes.Count;
            for (int i = 0; i < activeThemesCount; i++)
            {
                InteractableThemeBase interactableThemeBase = activeThemes[i];
                if (interactableThemeBase.Loaded)
                {
                    interactableThemeBase.OnUpdate(StateManager.CurrentState().ActiveIndex, forceUpdate);
                }
            }

            if (lastState != StateManager.CurrentState())
            {
                int handlersCount = handlers.Count;
                for (int i = 0; i < handlersCount; i++)
                {
                    IInteractableHandler handler = handlers[i];
                    if (handler != null)
                    {
                        handler.OnStateChange(StateManager, this);
                    }
                }
            }

            if (forceUpdate)
            {
                forceUpdate = false;
            }

            lastState = StateManager.CurrentState();
        }

        #endregion MonoBehaviour Implementation

        #region Interactable Initiation

        /// <summary>
        /// Ensure this Interactable component has initialized. Must be called on Unity main thread. 
        /// Returns true if has already been initialized, false otherwise. If has not been initialized, then will call Initialize()
        /// </summary>
        private bool EnsureInitialized()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                Initialize();

                return false;
            }

            return true;
        }

        protected virtual void Initialize()
        {
            if (States == null)
            {
                States = GetDefaultInteractableStates();
            }

            InputAction = ResolveInputAction(InputActionId);

            RefreshSetup();

            CurrentDimension = startDimensionIndex;
            IsToggled = CurrentDimension > 0;

            IsEnabled = enabledOnStart;
        }

        /// <summary>
        /// Force re-initialization of Interactable from events, themes and state references
        /// </summary>
        /// <remarks>
        /// This recreates the state machine inside Interactable and thus wipes any pre-existing state values held
        /// </remarks>
        public void RefreshSetup()
        {
            EnsureInitialized();

            SetupEvents();
            SetupThemes();
            SetupStates();
        }

        /// <summary>
        /// starts the StateManager
        /// </summary>
        protected virtual void SetupStates()
        {
            // Note that statemanager will clear states by allocating a new object
            // But resetting states directly will call setters which may perform necessary steps to enter appropriate state
            ResetAllStates();

            Debug.Assert(typeof(InteractableStates).IsAssignableFrom(States.StateModelType), $"Invalid state model of type {States.StateModelType}. State model must extend from {typeof(InteractableStates)}");
            StateManager = (InteractableStates)States.CreateStateModel();
        }

        /// <summary>
        /// Creates the event receiver instances from the Events list
        /// </summary>
        protected virtual void SetupEvents()
        {
            for (int i = 0; i < InteractableEvents.Count; i++)
            {
                var receiver = InteractableEvent.CreateReceiver(InteractableEvents[i]);
                if (receiver != null)
                {
                    InteractableEvents[i].Receiver = receiver;
                    InteractableEvents[i].Receiver.Host = this;
                }
                else
                {
                    Debug.LogWarning($"Empty event receiver found on {gameObject.name}, you may want to re-create this asset.");
                }
            }
        }

        /// <summary>
        /// Updates the list of active themes based the current dimensions index
        /// </summary>
        protected virtual void UpdateActiveThemes()
        {
            activeThemes.Clear();

            for (int i = 0; i < allThemeDimensionPairs.Count; i++)
            {
                if (allThemeDimensionPairs[i].Item1 == CurrentDimension)
                {
                    activeThemes.Add(allThemeDimensionPairs[i].Item2);
                }
            }
        }

        /// <summary>
        /// At startup or whenever a profile changes, creates all
        /// possible themes that interactable can be in. We then update
        /// the set of active themes by inspecting this list, looking for
        /// only themes whose index matched CurrentDimensionIndex.
        /// </summary>
        private void SetupThemes()
        {
            allThemeDimensionPairs.Clear();
            // Profiles are one per GameObject/ThemeContainer
            // ThemeContainers are one per dimension
            // ThemeDefinitions are one per desired effect (i.e theme)
            foreach (var profile in Profiles)
            {
                if (profile.Target != null && profile.Themes != null)
                {
                    for (int i = 0; i < profile.Themes.Count; i++)
                    {
                        var themeContainer = profile.Themes[i];
                        if (themeContainer.States.Equals(States))
                        {
                            foreach (var themeDefinition in themeContainer.Definitions)
                            {
                                allThemeDimensionPairs.Add(new System.Tuple<int, InteractableThemeBase>(
                                    i,
                                    InteractableThemeBase.CreateAndInitTheme(themeDefinition, profile.Target)));
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Could not use {themeContainer.name} in Interactable on {gameObject.name} because Theme's States does not match {States.name}");
                        }
                    }
                }
            }
            UpdateActiveThemes();
        }
        #endregion Interactable Initiation

        #region State Utilities

        /// <summary>
        /// Grabs the state value index, returns -1 if no StateManager available
        /// </summary>
        public int GetStateValue(InteractableStates.InteractableStateEnum state)
        {
            EnsureInitialized();

            if (StateManager != null)
            {
                return StateManager.GetStateValue((int)state);
            }

            return -1;
        }

        /// <summary>
        /// a public way to set state directly
        /// </summary>
        public void SetState(InteractableStates.InteractableStateEnum state, bool value)
        {
            EnsureInitialized();

            if (StateManager != null)
            {
                StateManager.SetStateValue(state, value ? 1 : 0);
                UpdateState();
            }
        }

        /// <summary>
        /// runs the state logic and sets state based on the current state values
        /// </summary>
        protected virtual void UpdateState()
        {
            StateManager.CompareStates();
        }

        /// <summary>
        /// Reset the input tracking states directly managed by Interactable such as whether the component has focus or is being grabbed
        /// Useful for when needing to reset input interactions
        /// </summary>
        public void ResetInputTrackingStates()
        {
            EnsureInitialized();

            HasFocus = false;
            HasPress = false;
            HasPhysicalTouch = false;
            HasGrab = false;
            HasGesture = false;
            HasGestureMax = false;
            HasVoiceCommand = false;

            if (globalTimer != null)
            {
                StopCoroutine(globalTimer);
                globalTimer = null;
            }

            dragStartPosition = null;
        }

        /// <summary>
        /// Reset all states in the Interactable and pointer information
        /// </summary>
        public void ResetAllStates()
        {
            EnsureInitialized();

            focusingPointers.Clear();
            pressingInputSources.Clear();

            ResetInputTrackingStates();

            IsEnabled = true;
            HasObservation = false;
            HasObservationTargeted = false;
            IsInteractive = false;
            IsTargeted = false;
            IsToggled = false;
            IsVisited = false;
            HasCollision = false;
            HasCustom = false;
        }

        #endregion State Utilities

        #region Dimensions Utilities

        /// <summary>
        /// Increases the Current Dimension by 1. If at end (i.e Dimensions - 1), then loop around to beginning (i.e 0)
        /// </summary>
        public void IncreaseDimension()
        {
            if (CurrentDimension == NumOfDimensions - 1)
            {
                CurrentDimension = 0;
            }
            else
            {
                CurrentDimension++;
            }
        }

        /// <summary>
        /// Decreases the Current Dimension by 1. If at zero, then loop around to end (i.e Dimensions - 1)
        /// </summary>
        public void DecreaseDimension()
        {
            if (CurrentDimension == 0)
            {
                CurrentDimension = NumOfDimensions - 1;
            }
            else
            {
                CurrentDimension--;
            }
        }

        /// <summary>
        /// Helper method to convert number of dimensions to the appropriate SelectionModes
        /// </summary>
        /// <param name="dimensions">number of dimensions</param>
        /// <returns>SelectionModes for corresponding number of dimensions</returns>
        public static SelectionModes ConvertToSelectionMode(int dimensions)
        {
            if (dimensions <= 0)
            {
                return SelectionModes.Invalid;
            }
            else if (dimensions == 1)
            {
                return SelectionModes.Button;
            }
            else if (dimensions == 2)
            {
                return SelectionModes.Toggle;
            }
            else
            {
                return SelectionModes.MultiDimension;
            }
        }

        #endregion Dimensions Utilities

        #region Events

        /// <summary>
        /// Register OnClick extra handlers
        /// </summary>
        public void AddHandler(IInteractableHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        /// <summary>
        /// Remove onClick handlers
        /// </summary>
        public void RemoveHandler(IInteractableHandler handler)
        {
            if (handlers.Contains(handler))
            {
                handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Event receivers can be used to listen for different
        /// events at runtime. This method allows receivers to be dynamically added at runtime.
        /// </summary>
        /// <returns>The new event receiver</returns>
        public T AddReceiver<T>() where T : ReceiverBase, new()
        {
            var interactableEvent = new InteractableEvent();
            var result = new T();
            result.Event = interactableEvent.Event;
            interactableEvent.Receiver = result;
            InteractableEvents.Add(interactableEvent);
            return result;
        }

        /// <summary>
        /// Returns the first receiver of type T on the interactable,
        /// or null if nothing is found.
        /// </summary>
        public T GetReceiver<T>() where T : ReceiverBase
        {
            for (int i = 0; i < InteractableEvents.Count; i++)
            {
                if (InteractableEvents[i] != null && InteractableEvents[i].Receiver is T receiverT)
                {
                    return receiverT;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all receivers of type T on the interactable.
        /// If nothing is found, returns empty list.
        /// </summary>
        public List<T> GetReceivers<T>() where T : ReceiverBase
        {
            List<T> result = new List<T>();
            for (int i = 0; i < InteractableEvents.Count; i++)
            {
                if (InteractableEvents[i] != null && InteractableEvents[i].Receiver is T receiverT)
                {
                    result.Add(receiverT);
                }
            }
            return result;
        }

        #endregion

        #region Input Timers

        /// <summary>
        /// Starts a timer to check if input is in progress
        ///  - Make sure global pointer events are not double firing
        ///  - Make sure Global Input events are not double firing
        ///  - Make sure pointer events are not duplicating an input event
        /// </summary>
        protected void StartClickTimer(bool isFromInputDown = false)
        {
            if (IsGlobal || isFromInputDown)
            {
                if (clickValidTimer != null)
                {
                    StopClickTimer();
                }

                clickValidTimer = StartCoroutine(InputDownTimer(clickTime));
            }
        }

        protected void StopClickTimer()
        {
            Debug.Assert(clickValidTimer != null, "StopClickTimer called but no click timer is running");
            StopCoroutine(clickValidTimer);
            clickValidTimer = null;
        }

        /// <summary>
        /// A timer for the MixedRealityInputHandlers, clicks should occur within a certain time.
        /// </summary>
        protected IEnumerator InputDownTimer(float time)
        {
            yield return new WaitForSeconds(time);
            clickValidTimer = null;
        }

        /// <summary>
        /// Return true if the interactable can fire a click event.
        /// Clicks can only occur within a short duration of an input down firing.
        /// </summary>
        private bool CanFireClick()
        {
            return clickValidTimer != null;
        }

        #endregion

        #region Interactable Utilities

        private void RegisterHandler<T>(bool enable) where T : IEventSystemHandler
        {
            if (enable)
            {
                CoreServices.InputSystem?.RegisterHandler<T>(this);
            }
            else
            {
                CoreServices.InputSystem?.UnregisterHandler<T>(this);
            }
        }

        /// <summary>
        /// Assigns the InputAction based on the InputActionId
        /// </summary>
        public static MixedRealityInputAction ResolveInputAction(int index)
        {
            if (CoreServices.InputSystem?.InputSystemProfile != null
                && CoreServices.InputSystem.InputSystemProfile.InputActionsProfile != null)
            {
                MixedRealityInputAction[] actions = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions;
                if (actions?.Length > 0)
                {
                    index = Mathf.Clamp(index, 0, actions.Length - 1);
                    return actions[index];
                }
            }

            return default;
        }

        /// <summary>
        /// Based on inputAction and state, should interactable listen to this up/down event.
        /// </summary>
        protected virtual bool ShouldListenToUpDownEvent(InputEventData data)
        {
            if ((HasFocus || IsGlobal) && data.MixedRealityInputAction == InputAction)
            {
                // Special case: Make sure that we are not being focused only by a PokePointer, since PokePointer
                // dispatches touch events and should not be dispatching button presses like select, grip, menu, etc.
                int focusingPointerCount = 0;
                int focusingPokePointerCount = 0;
                for (int i = 0; i < focusingPointers.Count; i++)
                {
                    if (focusingPointers[i].InputSourceParent.SourceId == data.SourceId)
                    {
                        focusingPointerCount++;
                        if (focusingPointers[i] is PokePointer)
                        {
                            focusingPokePointerCount++;
                        }
                    }
                }

                bool onlyFocusedByPokePointer = focusingPointerCount > 0 && focusingPointerCount == focusingPokePointerCount;
                return !onlyFocusedByPokePointer;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the inputeventdata is being dispatched from a near pointer
        /// </summary>
        private bool IsInputFromNearInteraction(InputEventData eventData)
        {
            bool isAnyNearpointerFocusing = false;
            for (int i = 0; i < focusingPointers.Count; i++)
            {
                if (focusingPointers[i].InputSourceParent.SourceId == eventData.InputSource.SourceId && focusingPointers[i] is IMixedRealityNearPointer)
                {
                    isAnyNearpointerFocusing = true;
                    break;
                }
            }
            return isAnyNearpointerFocusing;
        }

        /// <summary>
        /// Based on button settings and state, should this button listen to input?
        /// </summary>
        protected virtual bool CanInteract()
        {
            // Interactable can interact if we are enabled and we are not a toggle button
            // If we are a toggle button, then we can only toggle if CanSelect (to turn on) or CanDeslect (to turn off)
            return IsEnabled &&
                (ButtonMode != SelectionModes.Toggle
                || (CurrentDimension == 0 && CanSelect)
                || (CurrentDimension == 1 && CanDeselect));
        }

        /// <summary>
        /// A public way to trigger or route an onClick event from an external source, like PressableButton
        /// </summary>
        /// <param name="force">Force the click without checking CanInteract(). Does not override IsEnabled and only applies to toggle.</param>
        public void TriggerOnClick(bool force = false)
        {
            if (!IsEnabled || (!force && !CanInteract()))
            {
                return;
            }

            IncreaseDimension();

            SendOnClick(null);

            IsVisited = true;
        }

        /// <summary>
        /// Call onClick methods on receivers or IInteractableHandlers
        /// </summary>
        protected void SendOnClick(IMixedRealityPointer pointer)
        {
            OnClick.Invoke();
            ClickCount++;

            for (int i = 0; i < InteractableEvents.Count; i++)
            {
                if (InteractableEvents[i].Receiver != null)
                {
                    InteractableEvents[i].Receiver.OnClick(StateManager, this, pointer);
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
        /// For input "clicks" that do not have corresponding input up/down tracking such as voice commands
        /// Simulate pressed and start timer to reset states after some click time
        /// </summary>
        protected void StartGlobalVisual(bool voiceCommand = false)
        {
            if (voiceCommand)
            {
                HasVoiceCommand = true;
            }

            IsVisited = true;
            HasFocus = true;
            HasPress = true;

            if (globalTimer != null)
            {
                StopCoroutine(globalTimer);
            }

            globalTimer = StartCoroutine(GlobalVisualReset(globalFeedbackClickTime));
        }

        /// <summary>
        /// Clears up any automated visual states
        /// </summary>
        protected IEnumerator GlobalVisualReset(float time)
        {
            yield return new WaitForSeconds(time);

            HasVoiceCommand = false;
            if (HasFocus && focusingPointers.Count == 0)
            {
                HasFocus = false;
            }

            if (!HasPress)
            {
                HasPress = false;
            }

            globalTimer = null;
        }

        /// <summary>
        /// Public method that can be used to set state of interactable
        /// corresponding to an input going down (select button, menu button, touch) 
        /// </summary>
        public void SetInputDown()
        {
            if (!CanInteract())
            {
                return;
            }

            dragStartPosition = null;

            HasPress = true;

            StartClickTimer(true);
        }

        /// <summary>
        /// Public method that can be used to set state of interactable
        /// corresponding to an input going up.
        /// </summary>
        public void SetInputUp()
        {
            if (!CanInteract())
            {
                return;
            }

            HasPress = false;
            HasGesture = false;

            if (CanFireClick())
            {
                StopClickTimer();

                TriggerOnClick();
                IsVisited = true;
            }
        }

        private void OnInputChangedHelper<T>(InputEventData<T> eventData, Vector3 inputPosition, float gestureDeadzoneThreshold)
        {
            if (!CanInteract())
            {
                return;
            }

            if (ShouldListenToMoveEvent(eventData))
            {
                if (dragStartPosition == null)
                {
                    dragStartPosition = inputPosition;
                }
                else if (!HasGesture)
                {
                    if (Vector3.Distance(dragStartPosition.Value, inputPosition) > gestureStartThresholdVector2)
                    {
                        HasGesture = true;
                    }
                }
            }
        }

        private bool ShouldListenToMoveEvent<T>(InputEventData<T> eventData)
        {
            if ((HasFocus || IsGlobal) && HasPress)
            {
                // Ensure that this move event is from a pointer that is pressing the interactable
                int matchingPointerCount = 0;
                foreach (var pressingInputSource in pressingInputSources)
                {
                    if (pressingInputSource == eventData.InputSource)
                    {
                        matchingPointerCount++;
                    }
                }

                return matchingPointerCount > 0;
            }

            return false;
        }

        /// <summary>
        /// Creates the default States ScriptableObject configured for Interactable
        /// </summary>
        /// <returns>Default Interactable States asset</returns>
        public static States GetDefaultInteractableStates()
        {
            States result = ScriptableObject.CreateInstance<States>();
            InteractableStates allInteractableStates = new InteractableStates();
            result.StateModelType = typeof(InteractableStates);
            result.StateList = allInteractableStates.GetDefaultStates();
            result.DefaultIndex = 0;
            return result;
        }

        /// <summary>
        /// Helper function to create a new Theme asset using Default Interactable States and provided theme definitions
        /// </summary>
        /// <param name="themeDefintions">List of Theme Definitions to associate with Theme asset</param>
        /// <returns>Theme ScriptableObject instance</returns>
        public static Theme GetDefaultThemeAsset(List<ThemeDefinition> themeDefintions)
        {
            // Create the Theme configuration asset
            Theme newTheme = ScriptableObject.CreateInstance<Theme>();
            newTheme.States = GetDefaultInteractableStates();
            newTheme.Definitions = themeDefintions;
            return newTheme;
        }

        #endregion

        #region MixedRealityFocusChangedHandlers

        /// <inheritdoc/>
        public void OnBeforeFocusChange(FocusEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (eventData.NewFocusedObject == null)
            {
                focusingPointers.Remove(eventData.Pointer);
            }
            else if (eventData.NewFocusedObject.transform.IsChildOf(gameObject.transform))
            {
                if (!focusingPointers.Contains(eventData.Pointer))
                {
                    focusingPointers.Add(eventData.Pointer);
                }
            }
            else if (eventData.OldFocusedObject != null
                && eventData.OldFocusedObject.transform.IsChildOf(gameObject.transform))
            {
                focusingPointers.Remove(eventData.Pointer);
            }
        }

        /// <inheritdoc/>
        public void OnFocusChanged(FocusEventData eventData) { }

        #endregion MixedRealityFocusChangedHandlers

        #region MixedRealityFocusHandlers

        /// <inheritdoc/>
        public void OnFocusEnter(FocusEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            Debug.Assert(focusingPointers.Count > 0,
                "OnFocusEnter called but focusingPointers == 0. Most likely caused by the presence of a child object " +
                "that is handling IMixedRealityFocusChangedHandler");

            HasFocus = true;
        }

        /// <inheritdoc/>
        public void OnFocusExit(FocusEventData eventData)
        {
            if (!IsEnabled || !HasFocus)
            {
                return;
            }

            HasFocus = focusingPointers.Count > 0;
        }

        #endregion MixedRealityFocusHandlers

        #region MixedRealityInputHandlers

        /// <inheritdoc/>
        public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

        #endregion MixedRealityInputHandlers

        #region MixedRealityVoiceCommands

        /// <summary>
        /// Voice commands from MixedRealitySpeechCommandProfile, keyword recognized
        /// </summary>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (eventData.Command.Keyword == VoiceCommand && (!VoiceRequiresFocus || HasFocus) && CanInteract())
            {
                StartGlobalVisual(true);
                HasVoiceCommand = true;
                SendVoiceCommands(VoiceCommand, 0, 1);
                TriggerOnClick();
                eventData.Use();
            }
        }

        /// <summary>
        /// call OnVoinceCommand methods on receivers or IInteractableHandlers
        /// </summary>
        protected void SendVoiceCommands(string command, int index, int length)
        {
            for (int i = 0; i < InteractableEvents.Count; i++)
            {
                if (InteractableEvents[i].Receiver != null)
                {
                    InteractableEvents[i].Receiver.OnVoiceCommand(StateManager, this, command, index, length);
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

        #endregion VoiceCommands

        #region MixedRealityTouchHandlers

        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            HasPress = true;
            HasPhysicalTouch = true;
            eventData.Use();
        }

        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            if (!IsEnabled)
            {
                return;
            }

            HasPress = false;
            HasPhysicalTouch = false;
            eventData.Use();
        }

        public void OnTouchUpdated(HandTrackingInputEventData eventData) { }

        #endregion TouchHandlers

        #region MixedRealityInputHandlers

        /// <inheritdoc/>
        public void OnInputUp(InputEventData eventData)
        {
            if (!CanInteract() && !HasPress)
            {
                return;
            }

            if (ShouldListenToUpDownEvent(eventData))
            {
                SetInputUp();
                if (IsInputFromNearInteraction(eventData))
                {
                    HasGrab = false;
                }

                eventData.Use();
            }
            pressingInputSources.Remove(eventData.InputSource);
        }

        /// <inheritdoc/>
        public void OnInputDown(InputEventData eventData)
        {
            if (!CanInteract())
            {
                return;
            }

            if (ShouldListenToUpDownEvent(eventData))
            {
                pressingInputSources.Add(eventData.InputSource);
                SetInputDown();
                HasGrab = IsInputFromNearInteraction(eventData);

                eventData.Use();
            }
        }

        /// <inheritdoc/>
        public void OnInputChanged(InputEventData<Vector2> eventData)
        {
            OnInputChangedHelper(eventData, eventData.InputData, gestureStartThresholdVector2);
        }

        /// <inheritdoc/>
        public void OnInputChanged(InputEventData<Vector3> eventData)
        {
            OnInputChangedHelper(eventData, eventData.InputData, gestureStartThresholdVector3);
        }

        /// <inheritdoc/>
        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            OnInputChangedHelper(eventData, eventData.InputData.Position, gestureStartThresholdMixedRealityPose);
        }

        #endregion InputHandlers

        #region Deprecated

        /// <summary>
        /// Resets input tracking states such as focus or grab that are directly controlled by Interactable
        /// </summary>
        [System.Obsolete("Use ResetInputTrackingStates property instead")]
        public void ResetBaseStates()
        {
            ResetInputTrackingStates();
        }

        /// <summary>
        /// A public way to access the current dimension
        /// </summary>
        [System.Obsolete("Use CurrentDimension property instead")]
        public int GetDimensionIndex()
        {
            return CurrentDimension;
        }

        /// <summary>
        /// a public way to set the dimension index
        /// </summary>
        [System.Obsolete("Use CurrentDimension property instead")]
        public void SetDimensionIndex(int index)
        {
            CurrentDimension = index;
        }

        /// <summary>
        /// Force re-initialization of Interactable from events, themes and state references
        /// </summary>
        [System.Obsolete("Use RefreshSetup() instead")]
        public void ForceUpdateThemes()
        {
            RefreshSetup();
        }

        /// <summary>
        /// Does this interactable require focus
        /// </summary>
        [System.Obsolete("Use IsGlobal instead")]
        public bool FocusEnabled { get { return !IsGlobal; } set { IsGlobal = !value; } }

        /// <summary>
        /// True if Selection is "Toggle" (Dimensions == 2)
        /// </summary>
        [System.Obsolete("Use ButtonMode to test if equal to SelectionModes.Toggle instead")]
        public bool IsToggleButton { get { return NumOfDimensions == 2; } }

        /// <summary>
        /// Is the interactable enabled?
        /// </summary>
        [System.Obsolete("Use IsEnabled instead")]
        public bool Enabled
        {
            get => IsEnabled;
            set => IsEnabled = value;
        }

        /// <summary>
        /// Do voice commands require focus?
        /// </summary>
        [System.Obsolete("Use VoiceRequiresFocus instead")]
        public bool RequiresFocus
        {
            get => VoiceRequiresFocus;
            set => VoiceRequiresFocus = value;
        }

        /// <summary>
        /// Is disabled
        /// </summary>
        [System.Obsolete("Use IsEnabled instead")]
        public bool IsDisabled
        {
            get => !IsEnabled;
            set => IsEnabled = !value;
        }

        /// <summary>
        /// Returns a list of states assigned to the Interactable
        /// </summary>
        [System.Obsolete("Use States.StateList instead")]
        public State[] GetStates()
        {
            if (States != null)
            {
                return States.StateList.ToArray();
            }

            return System.Array.Empty<State>();
        }

        /// <summary>
        /// Handle focus state changes
        /// </summary>
        [System.Obsolete("Use Focus property instead")]
        public virtual void SetFocus(bool focus)
        {
            HasFocus = focus;
        }

        /// <summary>
        /// Change the press state
        /// </summary>
        [System.Obsolete("Use Press property instead")]
        public virtual void SetPress(bool press)
        {
            HasPress = press;
        }

        /// <summary>
        /// Change the disabled state, will override the Enabled property
        /// </summary>
        [System.Obsolete("Use IsEnabled property instead")]
        public virtual void SetDisabled(bool disabled)
        {
            IsEnabled = !disabled;
        }

        /// <summary>
        /// Change the targeted state
        /// </summary>
        [System.Obsolete("Use IsTargeted property instead")]
        public virtual void SetTargeted(bool targeted)
        {
            IsTargeted = targeted;
        }

        /// <summary>
        /// Change the Interactive state
        /// </summary>
        [System.Obsolete("Use IsInteractive property instead")]
        public virtual void SetInteractive(bool interactive)
        {
            IsInteractive = interactive;
        }

        /// <summary>
        /// Change the observation targeted state
        /// </summary>
        [System.Obsolete("Use HasObservationTargeted property instead")]
        public virtual void SetObservationTargeted(bool targeted)
        {
            HasObservationTargeted = targeted;
        }

        /// <summary>
        /// Change the observation state
        /// </summary>
        [System.Obsolete("Use HasObservation property instead")]
        public virtual void SetObservation(bool observation)
        {
            HasObservation = observation;
        }

        /// <summary>
        /// Change the visited state
        /// </summary>
        [System.Obsolete("Use IsVisited property instead")]
        public virtual void SetVisited(bool visited)
        {
            IsVisited = visited;
        }

        /// <summary>
        /// Change the toggled state
        /// </summary>
        [System.Obsolete("Use IsToggled property instead")]
        public virtual void SetToggled(bool toggled)
        {
            IsToggled = toggled;
        }

        /// <summary>
        /// Change the gesture state
        /// </summary>
        [System.Obsolete("Use HasGesture property instead")]
        public virtual void SetGesture(bool gesture)
        {
            HasGesture = gesture;
        }

        /// <summary>
        /// Change the gesture max state
        /// </summary>
        [System.Obsolete("Use HasGestureMax property instead")]
        public virtual void SetGestureMax(bool gesture)
        {
            HasGestureMax = gesture;
        }

        /// <summary>
        /// Change the collision state
        /// </summary>
        [System.Obsolete("Use HasCollision property instead")]
        public virtual void SetCollision(bool collision)
        {
            HasCollision = collision;
        }

        /// <summary>
        /// Change the custom state
        /// </summary>
        [System.Obsolete("Use HasCustom property instead")]
        public virtual void SetCustom(bool custom)
        {
            HasCustom = custom;
        }

        /// <summary>
        /// Change the voice command state
        /// </summary>
        [System.Obsolete("Use HasVoiceCommand property instead")]
        public virtual void SetVoiceCommand(bool voice)
        {
            HasVoiceCommand = voice;
        }

        /// <summary>
        /// Change the physical touch state
        /// </summary>
        [System.Obsolete("Use HasPhysicalTouch property instead")]
        public virtual void SetPhysicalTouch(bool touch)
        {
            HasPhysicalTouch = touch;
        }

        /// <summary>
        /// Change the grab state
        /// </summary>
        [System.Obsolete("Use HasGrab property instead")]
        public virtual void SetGrab(bool grab)
        {
            HasGrab = grab;
        }

        #endregion
    }
}
