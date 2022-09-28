// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    [AddComponentMenu("MRTK/Core/Stateful Interactable")]
    public class StatefulInteractable : MRTKBaseInteractable
    {
        #region Settings

        /// <summary>
        /// Toggle modes for interactables.
        /// </summary>
        public enum ToggleType
        {
            // Interactable will not enter toggle states
            // unless forced by code (ForceSetToggle)
            Button,
            // User can toggle on and off
            Toggle,
            // User can only toggle on, not off. Useful for radio buttons.
            OneWayToggle
        }

        [SerializeField]
        [Tooltip("The toggle behavior of the interactable. Set OneWayToggle for radio buttons.")]
        private ToggleType toggleMode = ToggleType.Button;

        /// <summary>
        /// The toggle behavior of the interactable. Set OneWayToggle for radio buttons.
        /// </summary>
        public ToggleType ToggleMode
        {
            get => toggleMode;
            set => toggleMode = value;
        }

        [SerializeField]
        [Tooltip("The threshold of variable Selectedness at which the interactable will be selected.")]
        private float selectThreshold = 0.9f;

        /// <summary>
        /// The threshold of variable Selectedness at which the interactable will be selected.
        /// </summary>
        public float SelectThreshold { get => selectThreshold; set => selectThreshold = value; }

        [SerializeField]
        [Tooltip("The threshold of variable Selectedness at which the interactable will be deselected.")]
        private float deselectThreshold = 0.1f;

        /// <summary>
        /// The threshold of variable Selectedness at which the interactable will be deselected.
        /// </summary>
        public float DeselectThreshold { get => deselectThreshold; set => deselectThreshold = value; }

        [SerializeField]
        [Tooltip("Should the user be required to fully select and deselect the interactable for Click and Toggle to fire?")]
        private bool triggerOnRelease = true;

        /// <summary>
        /// Should the user be required to fully select and deselect the interactable for Click and Toggle to fire?
        /// </summary>
        public bool TriggerOnRelease { get => triggerOnRelease; set => triggerOnRelease = value; }

        [SerializeField]
        [Tooltip("Should gazing at the object for a certain amount of time select it?")]
        private bool useGazeDwell = false;

        /// <summary>
        /// Should gazing at the object for a certain amount of time select it?
        /// </summary>
        public bool UseGazeDwell { get => useGazeDwell; set => useGazeDwell = value; }

        [SerializeField]
        [Tooltip("Time required for gaze dwell")]
        private float gazeDwellTime = 1.0f;

        /// <summary>
        /// Time required for gaze dwell
        /// </summary>
        public float GazeDwellTime { get => gazeDwellTime; set => gazeDwellTime = value; }

        [SerializeField]
        [Tooltip("Should hovering the object with a far ray for a certain amount of time select it?")]
        private bool useFarDwell = false;

        /// <summary>
        /// Should hovering the object with a far ray for a certain amount of time select it?
        /// </summary>
        public bool UseFarDwell { get => useFarDwell; set => useFarDwell = value; }

        [SerializeField]
        [Tooltip("Time required for far ray dwell")]
        private float farDwellTime = 1.0f;

        /// <summary>
        /// Time required for far ray dwell
        /// </summary>
        public float FarDwellTime { get => farDwellTime; set => farDwellTime = value; }

        [SerializeField]
        [Tooltip("If true, voice command can be used to trigger \"select\" of the interactable")]
        private bool allowSelectByVoice = true;

        /// <summary>
        /// Does the interactable allow triggering select via a voice command?
        /// If true, voice command can be used to trigger "select" on the interactable
        /// </summary>
        public bool AllowSelectByVoice
        {
            get => allowSelectByVoice;
            set
            {
                if (value != allowSelectByVoice)
                {
                    // Unregister and reregister the interactable to update the speech interactor with latest info
                    interactionManager.UnregisterInteractable(this as IXRInteractable);
                    allowSelectByVoice = value;
                    interactionManager.RegisterInteractable(this as IXRInteractable);
                }
            }
        }

        [SerializeField]
        [Tooltip("Speech keyword required for triggering \"select\" on the interactable")]
        private string speechRecognitionKeyword = "select";

        /// <summary>
        /// Speech keyword required for triggering \"select\" on the interactable
        /// </summary>
        public string SpeechRecognitionKeyword
        {
            get => speechRecognitionKeyword;
            set
            {
                if (value != speechRecognitionKeyword)
                {
                    // Unregister and reregister the interactable to update the speech interactor with latest info
                    interactionManager.UnregisterInteractable(this as IXRInteractable);
                    speechRecognitionKeyword = value;
                    interactionManager.RegisterInteractable(this as IXRInteractable);
                }
            }
        }

        [SerializeField]
        [Tooltip("If true, then the voice command will only respond to voice commands while this Interactable has focus.")]
        private bool voiceRequiresFocus = true;

        /// <summary>
        /// Does the voice command require this to have focus?
        /// If true, then the voice command will only respond to voice commands while this Interactable has focus.
        /// </summary>
        public bool VoiceRequiresFocus
        {
            get => voiceRequiresFocus;
        }

        #endregion Settings

        #region Public state

        [Obsolete("Please use Selectedness() instead.")]
        public float PinchAmount => Selectedness();

        [SerializeField]
        [EditableTimedFlag]
        [Tooltip("Is the interactable toggled?")]
        private TimedFlag isToggled = new TimedFlag();

        /// <summary>
        /// Is the interactable toggled?
        /// </summary>
        public TimedFlag IsToggled => isToggled;

        #endregion Public state

        #region Events

        [SerializeField]
        [Tooltip("Fired when the interactable is fully clicked (select + deselect)")]
        [FormerlySerializedAs("OnClicked")]
        private UnityEvent onClicked = new UnityEvent();

        /// <summary>
        /// Fired when the interactable is fully clicked (select + deselect)
        /// </summary>
        public UnityEvent OnClicked => onClicked;

        [SerializeField]
        private UnityEvent onEnabled = new UnityEvent();

        /// <summary>
        /// Fired when the interactable is enabled
        /// </summary>
        public UnityEvent OnEnabled => onEnabled;

        [SerializeField]
        private UnityEvent onDisabled = new UnityEvent();

        /// <summary>
        /// Fired when the interactable is disabled
        /// </summary>
        public UnityEvent OnDisabled => onDisabled;

        #endregion

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            onEnabled.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onDisabled.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();

            firstSelectEntered.AddListener(OnFirstSelectEntered);
            lastSelectExited.AddListener(OnLastSelectExited);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            firstSelectEntered.RemoveListener(OnFirstSelectEntered);
            lastSelectExited.RemoveListener(OnLastSelectExited);
        }

        #endregion MonoBehaviour Implementation

        private static readonly ProfilerMarker StatefulInteractableSelectednessMarker =
            new ProfilerMarker("[MRTK] StatefulInteractable.Selectedness");

        /// <summary>
        /// Derived classes should override this method to
        /// specify custom variable selection math.
        /// The default implementation allows for variable selection
        /// from the <see cref="GazePinchInteractor">, calculated with
        /// <see cref="PinchAmount"/>.
        /// </summary>
        public virtual float Selectedness()
        {
            using (StatefulInteractableSelectednessMarker.Auto())
            {
                float selectedness = 0.0f;

                foreach (var interactor in interactorsHovering)
                {
                    if (interactor is IVariableSelectInteractor variableSelectInteractor)
                    {
                        selectedness = Mathf.Max(selectedness, variableSelectInteractor.SelectProgress);
                    }
                }

                foreach (var interactor in interactorsSelecting)
                {
                    if (interactor is IVariableSelectInteractor variableSelectInteractor)
                    {
                        selectedness = Mathf.Max(selectedness, variableSelectInteractor.SelectProgress);
                    }
                    else
                    {
                        selectedness = 1.0f;
                    }
                }

                return selectedness;
            }
        }

        /// <summary>
        /// Forcibly toggle the interactable.
        /// </summary>
        /// <param name="active">If true, this toggle will be set to active.</param>
        /// <param name="fireEvents">
        /// Set to false if events and timers should not be fired.
        /// Useful when hydrating state from an external source on startup.
        /// </param>
        public void ForceSetToggled(bool active, bool fireEvents = true)
        {
            if (fireEvents)
            {
                IsToggled.Active = active;
            }
            else
            {
                // Hydrate state and don't fire events.
                IsToggled.Initialize(active);
            }
        }

        /// <summary>
        /// Forcibly toggle the interactable and fire the relevant events.
        /// This is a single-arg overload for ForceSetToggled for use
        /// with UnityEvents. Consider using ForceSetToggled(bool, bool) instead,
        /// especially if you'd like to suppress the resulting toggle events.
        /// </summary>
        public void ForceSetToggled(bool active)
        {
            // This will fire toggle events.
            IsToggled.Active = active;
        }


        private static readonly ProfilerMarker OnFirstSelectEnteredPerfMarker =
            new ProfilerMarker("[MRTK] StatefulInteractable.OnFirstSelectEntered");

        /// <inheritdoc />
        protected virtual void OnFirstSelectEntered(SelectEnterEventArgs args)
        {
            using (OnFirstSelectEnteredPerfMarker.Auto())
            {
                if (CanClickOnFirstSelectEntered(args))
                {
                    TryToggle();
                    OnClicked.Invoke();
                }
            }
        }

        private static readonly ProfilerMarker OnLastSelectExitedPerfMarker =
            new ProfilerMarker("[MRTK] StatefulInteractable.OnLastSelectExited");

        /// <inheritdoc />
        protected virtual void OnLastSelectExited(SelectExitEventArgs args)
        {
            using (OnLastSelectExitedPerfMarker.Auto())
            {
                if (CanClickOnLastSelectExited(args))
                {
                    TryToggle();
                    OnClicked.Invoke();
                }
            }
        }

        /// <summary>
        /// This function determines whether the interactable should fire a click event at a given select event.
        /// Subclasses can override this to add additional requirements for full click/toggle activation,
        /// such as rolloff prevention.
        /// </summary>
        /// <returns>True if the interactable should fire click/toggle event from this current select event.</returns>
        internal protected virtual bool CanClickOnFirstSelectEntered(SelectEnterEventArgs args) => !TriggerOnRelease;

        /// <summary>
        /// This function determines whether the interactable should fire a click event at a given deselect event.
        /// Subclasses can override this to add additional requirements for full click/toggle activation,
        /// such as rolloff prevention.
        /// </summary>
        /// <returns>True if the interactable should fire click/toggle event from this current deselect event.</returns>
        internal protected virtual bool CanClickOnLastSelectExited(SelectExitEventArgs args)
        {
            // This check will prevent OnClick from firing when the interactor loses tracking.
            // XRI interactor interfaces don't have a good API for "is this interactor tracked?"
            // Hover-active is a good equivalent, though, as MRTK interactors set hoverActive false
            // when their controller loses tracking.
            if (args.interactorObject is IXRHoverInteractor hoverInteractor)
            {
                return TriggerOnRelease && hoverInteractor.isHoverActive;
            }

            return TriggerOnRelease;
        }

        // Attempt to toggle our own IsToggled state.
        // Will obey ToggleMode.
        private void TryToggle()
        {
            // Check whether we're allowed to toggle.
            if (ToggleMode == ToggleType.Button || (ToggleMode == ToggleType.OneWayToggle && IsToggled.Active))
            {
                return;
            }

            IsToggled.Active = !IsToggled;
        }
    }
}
