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
    /// <summary>
    /// An extended version of <see cref="Microsoft.MixedReality.Toolkit.MRTKBaseInteractable">MRTKBaseInteractable</see> that adds additional
    /// functionality such as speech support, gaze support, and toggle behaviors.
    /// </summary>
    [AddComponentMenu("MRTK/Core/Stateful Interactable")]
    public class StatefulInteractable : MRTKBaseInteractable
    {
        #region Settings

        /// <summary>
        /// Toggle modes for interactables.
        /// </summary>
        public enum ToggleType
        {
            /// <summary>
            /// The interactable will not enter toggle states unless forces by code 
            /// using the <see cref="ForceSetToggled" /> function.
            /// </summary>
            Button,

            /// <summary>
            /// The user can toggle on and off the interactable.
            /// </summary>
            Toggle,
            
            /// <summary>
            /// The User can only toggle on the interactable, but not toggle off. 
            /// This value is useful for radio buttons.
            /// </summary>
            OneWayToggle
        }

        /// <summary>
        /// The toggle behavior of the interactable. Set OneWayToggle for radio buttons.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("toggleMode"),
            Tooltip("The toggle behavior of the interactable. Set OneWayToggle for radio buttons.")]
        public ToggleType ToggleMode { get; set; } = ToggleType.Button;

        /// <summary>
        /// The threshold of variable Selectedness at which the interactable will be selected.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("selectThreshold"),
            Tooltip("The threshold of variable Selectedness at which the interactable will be selected.")]
        public float SelectThreshold { get; set; } = 0.9f;

        /// <summary>
        /// The threshold of variable Selectedness at which the interactable will be deselected.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("deselectThreshold"),
            Tooltip("The threshold of variable Selectedness at which the interactable will be deselected.")]
        public float DeselectThreshold { get; set; } = 0.1f;

        /// <summary>
        /// Should the user be required to fully select and deselect the interactable for Click and Toggle to fire?
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("triggerOnRelease"),
            Tooltip("Should the user be required to fully select and deselect the interactable for Click and Toggle to fire?")]
        public bool TriggerOnRelease { get; set; } = true;

        /// <summary>
        /// Should gazing at the object for a certain amount of time select it?
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("useGazeDwell"),
            Tooltip("Should gazing at the object for a certain amount of time select it?")]
        public bool UseGazeDwell { get; set; } = false;

        /// <summary>
        /// Time required for gaze dwell
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("gazeDwellTime"), Tooltip("Time required for gaze dwell")]
        public float GazeDwellTime { get; set; } = 1.0f;

        /// <summary>
        /// Should hovering the object with a far ray for a certain amount of time select it?
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("useFarDwell"), Tooltip("Should hovering the object with a far ray for a certain amount of time select it?")]
        public bool UseFarDwell { get; set; } = false;

        /// <summary>
        /// Time required for far ray dwell
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("farDwellTime"), Tooltip("Time required for far ray dwell")]
        public float FarDwellTime { get; set; } = 1.0f;

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
        
        /// <summary>
        /// Does the voice command require this to have focus?
        /// If true, then the voice command will only respond to voice commands while this Interactable has focus.
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("voiceRequiresFocus"),
            Tooltip("If true, then the voice command will only respond to voice commands while this Interactable has focus.")]
        public bool VoiceRequiresFocus { get; private set; } = true;

        /// <summary>
		/// Does the interactable require the interactor to hover over it?
        /// If true, then the OnClick event will only get fired while this Interactable is being hovered.
        /// </summary>
        [field: SerializeField, Tooltip("If true, then the OnClick event will only get fired while this Interactable is being hovered.")]
        public bool SelectRequiresHover { get; private set; } = false;

        #endregion Settings

        #region Public state

        /// <summary>
        /// Is the interactable toggled?
        /// </summary>
        [field: SerializeField, EditableTimedFlag, FormerlySerializedAs("isToggled"), Tooltip("Is the interactable toggled?")]
        public TimedFlag IsToggled { get; private set; } = new TimedFlag();

        #endregion Public state

        #region Events

        /// <summary>
        /// Fired when the interactable is fully clicked (select + deselect)
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("onClicked"), Tooltip("Fired when the interactable is fully clicked")]
        public UnityEvent OnClicked { get; private set; } = new UnityEvent();

        /// <summary>
        /// Fired when the interactable is enabled
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("onEnabled"), Tooltip("Fired when the interactable is enabled")]
        public UnityEvent OnEnabled { get; private set; } = new UnityEvent();

        /// <summary>
        /// Fired when the interactable is disabled
        /// </summary>
        [field: SerializeField, FormerlySerializedAs("onDisabled"), Tooltip("Fired when the interactable is disabled")]
        public UnityEvent OnDisabled { get; private set; } = new UnityEvent();

        #endregion

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            base.OnEnable();
            OnEnabled.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnDisabled.Invoke();
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
        /// from the <see cref="GazePinchInteractor"/>, calculated with
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
            return TriggerOnRelease && IsRegistered() && IsInteractorTracked() && IsTargetValid();

            // This check will prevent OnClick from firing when the interactable or interactor was unregistered.
            bool IsRegistered()
            {
                return !args.isCanceled;
            }

            // This check will prevent OnClick from firing when the interactor loses tracking.
            // XRI interactor interfaces don't have a good API for "is this interactor tracked?"
            // Hover-active is a good equivalent, though, as MRTK interactors set hoverActive false
            // when their controller loses tracking.
            bool IsInteractorTracked()
            {
                return !(args.interactorObject is IXRHoverInteractor hoverInteractor) ||
                       hoverInteractor.isHoverActive;
            }

            // This check will prevent OnClick from firing when the interactable is not being hovered.
            bool IsTargetValid()
            {
                return !SelectRequiresHover ||
                       !(args.interactableObject is IXRHoverInteractable hoverInteractable) ||
                       hoverInteractable.isHovered;
            }
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
