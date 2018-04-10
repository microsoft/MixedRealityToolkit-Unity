// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Base class for buttons.
    /// </summary>
    public abstract class Button : MonoBehaviour, IInputHandler, IPointerSpecificFocusable, IHoldHandler, ISourceStateHandler, IInputClickHandler
    {
        #region Public Members

        /// <summary>
        /// Current Button State
        /// </summary>
        [Header("Basic Settings")]
        [SerializeField]
        [Tooltip("Current State of the Button")]
        private ButtonStateEnum buttonState = ButtonStateEnum.Observation;
        public ButtonStateEnum ButtonState
        {
            get { return buttonState; }
            set { buttonState = value; }
        }

        /// <summary>
        /// Filter to apply for the correct button source
        /// </summary>
        [SerializeField]
        [Tooltip("Filter for press info for click or press event")]
        private InteractionSourcePressInfo buttonPressFilter = InteractionSourcePressInfo.Select;
        public InteractionSourcePressInfo ButtonPressFilter
        {
            get { return buttonPressFilter; }
            set { buttonPressFilter = value; }
        }

        /// <summary>
        /// If true the interactable will deselect when you look off of the object
        /// </summary>
        [SerializeField]
        [Tooltip("If RequireGaze then looking away will deselect object")]
        private bool requireGaze = true;
        public bool RequireGaze
        {
            get { return requireGaze; }
            set { requireGaze = value; }
        }

        /// <summary>
        /// Event to receive button state change
        /// </summary>
        public event Action<ButtonStateEnum> StateChange;

        /// <summary>
        /// Event fired when tap interaction received.
        /// </summary>
        public event Action<GameObject> OnButtonPressed;

        /// <summary>
        /// Event fired when released interaction received.
        /// </summary>
        public event Action<GameObject> OnButtonReleased;

        /// <summary>
        /// Event fired when click interaction received.
        /// </summary>
        public event Action<GameObject> OnButtonClicked;

        /// <summary>
        /// Event fired when hold interaction initiated.
        /// </summary>
        public event Action<GameObject> OnButtonHeld;

        /// <summary>
        /// Event fired when hold interaction canceled.
        /// </summary>
        public event Action<GameObject> OnButtonCanceled;

        #endregion

        #region Private and Protected Members
        /// <summary>
        /// Protected string for the current active gizmo icon
        /// </summary>
        protected string _GizmoIcon;

        /// <summary>
        /// Last state of hands being visible
        /// </summary>
        private bool _bLastHandVisible = false;

        /// <summary>
        /// State of hands being visible
        /// </summary>
        private bool _bHandVisible = false;

        /// <summary>
        /// State of hands being visible
        /// </summary>
        private bool _bFocused = false;

        /// <summary>
        /// Count of visible hands
        /// </summary>
        private int _handCount = 0;

        /// <summary>
        /// Check for disabled state or disabled behavior
        /// </summary>
        private bool m_disabled { get { return ButtonState == ButtonStateEnum.Disabled || !enabled; } }

        #endregion

        /// <summary>
        /// Public function to force a clicked event on a button
        /// </summary>
        public void TriggerClicked()
        {
            DoButtonPressed(true);
        }

        #region Input Interface Functions
        /// <summary>
        /// Handle input down events from IInputSource.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputDown(InputEventData eventData)
        {
            if (enabled && !m_disabled)
            {
                if(ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonPressed();

                    // Set state to Pressed
                    ButtonStateEnum newState = ButtonStateEnum.Pressed;
                    this.OnStateChange(newState);
                }
            }
        }

        /// <summary>
        /// Handle on input up events from IInputSource
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputUp(InputEventData eventData)
        {
            if (enabled && !m_disabled)
            {
                if (ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonReleased();
                }
            }
        }

        /// <summary>
        /// Handle clicked event
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (enabled && !m_disabled)
            {
                if (ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonPressed(true);
                }
            }
        }


        /// <summary>
        /// Handle On Hold started from IHoldSource
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldStarted(HoldEventData eventData)
        {
            if (!m_disabled)
            {
                DoButtonPressed();
            }
        }

        /// <summary>
        /// Handle On Hold started from IHoldSource
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCompleted(HoldEventData eventData)
        {
            if (!m_disabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonHeld();

                // Unset state from pressed.
                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
            }
        }

                /// <summary>
        /// Handle On Hold started from IHoldSource
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCanceled(HoldEventData eventData)
        {
            if (!m_disabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonCanceled();
                // Unset state from pressed.

                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// FocusManager SendMessage("FocusEnter") receiver.
        /// </summary>
        public void OnFocusEnter(PointerSpecificEventData eventData)
        {
            if (!m_disabled)
            {
                ButtonStateEnum newState = _bHandVisible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                this.OnStateChange(newState);

                _bFocused = true;
            }
        }

        /// <summary>
        /// FocusManager SendMessage("FocusExit") receiver.
        /// </summary>
        public void OnFocusExit(PointerSpecificEventData eventData)
        {
             if (!m_disabled) // && FocusManager.Instance.IsFocused(this))
            {
                if (ButtonState == ButtonStateEnum.Pressed)
                {
                    DoButtonCanceled();
                }

                ButtonStateEnum newState = _bHandVisible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;

                if (RequireGaze || ButtonState != ButtonStateEnum.Pressed)
                {
                    this.OnStateChange(newState);
                }

                _bFocused = false;
            }
        }

        /// <summary>
        /// On Source detected see if it is a hand and increment hand count and set visibility
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSourceDetected(SourceStateEventData eventData)
        {
            InteractionSourceInfo sourceInfo;
            if (eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceInfo))
            {
                if (sourceInfo == InteractionSourceInfo.Hand)
                {
                    _handCount++;
                    _bHandVisible = true;
                }
            }
        }

        /// <summary>
        ///  On Source lost decrement hand count and set visibility
        /// </summary>
        /// <param name="eventData"></param>
        public void OnSourceLost(SourceStateEventData eventData)
        {
            InteractionSourceInfo sourceInfo;
            if (eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceInfo))
            {
                if (sourceInfo == InteractionSourceInfo.Hand)
                {
                    _handCount--;
                    _bHandVisible = _handCount <= 0;
                }
            }
        }
        #endregion

        /// <summary>
        /// Called when button is pressed down.
        /// </summary>
        protected void DoButtonPressed(bool bRelease = false)
        {
            ButtonStateEnum newState = ButtonStateEnum.Pressed;
            this.OnStateChange(newState);

            if (OnButtonPressed != null)
            {
                OnButtonPressed(gameObject);
            }

            if(OnButtonClicked != null)
            {
                OnButtonClicked(gameObject);
            }

            if (bRelease)
            {
                StartCoroutine(DelayedRelease(0.2f));
            }
        }

        /// <summary>
        /// Called when button is released.
        /// </summary>
        protected void DoButtonReleased()
        {
            ButtonStateEnum newState;

            if(_bFocused)
            {
                newState = _bHandVisible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
            }
            else
            {
                newState = _bHandVisible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
            }

            this.OnStateChange(newState);

            if (OnButtonReleased != null)
            {
                OnButtonReleased(gameObject);
            }
        }

        /// <summary>
        /// Delayed function to release button works for click events
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DelayedRelease(float delay)
        {
            yield return new WaitForSeconds(delay);
            DoButtonReleased();
        }

        /// <summary>
        /// Called while button is pressed down.
        /// </summary>
        protected void DoButtonHeld()
        {
            if (OnButtonHeld != null)
            {
                OnButtonHeld(gameObject);
            }
        }

        /// <summary>
        /// Called when something interrupts the button pressed state.
        /// </summary>
        protected void DoButtonCanceled()
        {
            if (OnButtonCanceled != null)
            {
                OnButtonCanceled(gameObject);
            }
        }

        /// <summary>
        /// Use LateUpdate to check for whether or not the hand is up
        /// </summary>
        public void LateUpdate()
        {
            if (!m_disabled && _bLastHandVisible != _bHandVisible)
            {
                OnHandVisibleChange(_bHandVisible);
            }
        }

        /// <summary>
        /// Event to fire off when hand visibility changes
        /// </summary>
        /// <param name="visible"></param>
        public virtual void OnHandVisibleChange(bool visible)
        {
            _bLastHandVisible = visible;

            ButtonStateEnum newState = ButtonState;

            switch (ButtonState)
            {
                case ButtonStateEnum.Interactive:
                {
                    newState = visible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
                    break;
                }
                case ButtonStateEnum.Targeted:
                {
                    newState = visible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                    break;
                }
                case ButtonStateEnum.Observation:
                {
                    newState = visible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
                    break;
                }
                case ButtonStateEnum.ObservationTargeted:
                {
                    newState = visible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                    break;
                }
            }

            OnStateChange(newState);
        }

        /// <summary>
        /// Ensures the button returns to a neutral state when disabled
        /// </summary>
        public virtual void OnDisable()
        {
            if (ButtonState != ButtonStateEnum.Disabled)
            {
                OnStateChange(ButtonStateEnum.Observation);
            }
        }

        /// <summary>
        /// Callback virtual function for when the button state changes
        /// </summary>
        /// <param name="newState">
        /// A <see cref="ButtonStateEnum"/> for the new button state.
        /// </param>
        public virtual void OnStateChange(ButtonStateEnum newState)
        {
            ButtonState = newState;

            // Send out the action/event for the state change
            if (StateChange != null)
            {
                StateChange(newState);
            }
        }
    }
}