// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Base class for buttons.
    /// </summary>
    public abstract class Button : MonoBehaviour, IInputHandler, IPointerSpecificFocusable, IHoldHandler, IInputClickHandler
    {
        #region Public Members and Serialized Fields

        [Header("Basic Settings")]
        [SerializeField]
        [Tooltip("Current State of the Button")]
        private ButtonStateEnum buttonState = ButtonStateEnum.Observation;

        /// <summary>
        /// Current Button State.
        /// </summary>
        public ButtonStateEnum ButtonState
        {
            get { return buttonState; }
            set { buttonState = value; }
        }

        [SerializeField]
        [Tooltip("Filter for press info for click or press event")]
        private InteractionSourcePressInfo buttonPressFilter = InteractionSourcePressInfo.Select;

        /// <summary>
        /// Filter to apply for the correct button source.
        /// </summary>
        public InteractionSourcePressInfo ButtonPressFilter
        {
            get { return buttonPressFilter; }
            set { buttonPressFilter = value; }
        }

        [SerializeField]
        [Tooltip("If RequireGaze, then looking away will deselect object")]
        private bool requireGaze = true;

        /// <summary>
        /// If true, the interactable will deselect when you look off of the object.
        /// </summary>
        public bool RequireGaze
        {
            get { return requireGaze; }
            set { requireGaze = value; }
        }

        /// <summary>
        /// Event to receive button state change.
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
        /// Event fired when button interaction canceled.
        /// </summary>
        public event Action<GameObject> OnButtonCanceled;

        #endregion Public Members and Serialized Fields

        #region Private and Protected Members

        /// <summary>
        /// Protected string for the current active gizmo icon
        /// </summary>
        protected string _GizmoIcon;

        /// <summary>
        /// Last state of hands being visible
        /// </summary>
        private bool lastHandVisible = false;

        /// <summary>
        /// State of hands being visible
        /// </summary>
        private bool handVisible { get { return InputManager.Instance.DetectedInputSources.Count > 0; } }

        /// <summary>
        /// State of gaze/focus being on the button
        /// </summary>
        private bool focused = false;

        /// <summary>
        /// Check for disabled state or disabled behavior
        /// </summary>
        private bool isDisabled { get { return ButtonState == ButtonStateEnum.Disabled || !enabled; } }

        #endregion Private and Protected Members

        #region MonoBehaviour Functions

        /// <summary>
        /// Use LateUpdate to check for whether or not the hand is up
        /// </summary>
        public void LateUpdate()
        {
            if (!isDisabled && lastHandVisible != handVisible)
            {
                OnHandVisibleChange(handVisible);
            }
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

        #endregion MonoBehaviour Functions

        /// <summary>
        /// Public function to force a clicked event on a button
        /// </summary>
        public void TriggerClicked()
        {
            DoButtonPressed(true);
        }

        #region Input Interface Functions

        /// <summary>
        /// Handle on input down events from IInputHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputDown(InputEventData eventData)
        {
            if (!isDisabled)
            {
                if (ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonPressed();

                    // Set state to Pressed
                    ButtonStateEnum newState = ButtonStateEnum.Pressed;
                    this.OnStateChange(newState);
                    eventData.Use();
                }
            }
        }

        /// <summary>
        /// Handle on input up events from IInputHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputUp(InputEventData eventData)
        {
            if (!isDisabled)
            {
                if (ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonReleased();
                    eventData.Use();
                }
            }
        }

        /// <summary>
        /// Handle clicked events from IInputClickHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (!isDisabled)
            {
                if (ButtonPressFilter == InteractionSourcePressInfo.None || ButtonPressFilter == eventData.PressType)
                {
                    DoButtonPressed(true);
                    eventData.Use();
                }
            }
        }


        /// <summary>
        /// Handle OnHoldStarted events from IHoldHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldStarted(HoldEventData eventData)
        {
            if (!isDisabled)
            {
                DoButtonPressed();
                eventData.Use();
            }
        }

        /// <summary>
        /// Handle on hold completed events from IHoldHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCompleted(HoldEventData eventData)
        {
            if (!isDisabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonHeld();

                // Unset state from pressed.
                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
                eventData.Use();
            }
        }

        /// <summary>
        /// Handle on hold canceled events from IHoldHandler.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnHoldCanceled(HoldEventData eventData)
        {
            if (!isDisabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonCanceled();
                // Unset state from pressed.

                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
                eventData.Use();
            }
        }

        /// <summary>
        /// Handle on focus enter events from IPointerSpecificFocusable.
        /// </summary>
        public void OnFocusEnter(PointerSpecificEventData eventData)
        {
            if (!isDisabled)
            {
                ButtonStateEnum newState = handVisible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                OnStateChange(newState);

                focused = true;
                eventData.Use();
            }
        }

        /// <summary>
        /// Handle on focus exit events from IPointerSpecificFocusable.
        /// </summary>
        public void OnFocusExit(PointerSpecificEventData eventData)
        {
            if (!isDisabled)
            {
                if (ButtonState == ButtonStateEnum.Pressed)
                {
                    DoButtonCanceled();
                }

                ButtonStateEnum newState = handVisible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;

                if (RequireGaze || ButtonState != ButtonStateEnum.Pressed)
                {
                    OnStateChange(newState);
                }

                focused = false;
                eventData.Use();
            }
        }

        #endregion Input Interface Functions

        #region Button Functions

        /// <summary>
        /// Called when the button is pressed down.
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
        /// Called when the button is released.
        /// </summary>
        protected void DoButtonReleased()
        {
            ButtonStateEnum newState;

            if (focused)
            {
                newState = handVisible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
            }
            else
            {
                newState = handVisible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
            }

            OnStateChange(newState);

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
        /// Called once after the button is held down.
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
        /// Event to fire off when hand/spatial input source visibility changes.
        /// </summary>
        /// <param name="visible">Whether the spatial input source is has become visible.</param>
        public virtual void OnHandVisibleChange(bool visible)
        {
            lastHandVisible = visible;

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
        /// Callback virtual function for when the button state changes.
        /// </summary>
        /// <param name="newState">
        /// A <see cref="ButtonStateEnum"/> for the new button state.
        /// </param>
        public virtual void OnStateChange(ButtonStateEnum newState)
        {
            ButtonState = newState;

            // Send out the action/event for the state change.
            if (StateChange != null)
            {
                StateChange(newState);
            }
        }
    }
}