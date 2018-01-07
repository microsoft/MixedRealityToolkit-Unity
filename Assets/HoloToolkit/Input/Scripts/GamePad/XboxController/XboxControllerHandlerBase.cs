// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerHandlerBase : GamePadHandlerBase
    {
        protected enum GestureState
        {
            SelectButtonUnpressed,
            SelectButtonPressed,
            NavigationStarted,
            HoldStarted
        }

        [SerializeField]
        [Tooltip("Elapsed time for hold started gesture in seconds.")]
        protected float HoldStartedInterval = 2.0f;

        [SerializeField]
        [Tooltip("Elapsed time for hold completed gesture in seconds.")]
        protected float HoldCompletedInterval = 3.0f;

        [SerializeField]
        [Tooltip("The action button that is used to select.  Analogous to air tap on HoloLens and trigger press with motion controllers.")]
        protected XboxControllerMappingTypes SelectButton = XboxControllerMappingTypes.XboxA;

        [SerializeField]
        [Tooltip("The Horizontal Axis that navigation events take place")]
        protected XboxControllerMappingTypes HorizontalNavigationAxis = XboxControllerMappingTypes.XboxLeftStickHorizontal;

        [Tooltip("The Vertical Axis that navigation events take place")]
        protected XboxControllerMappingTypes VerticalNavigationAxis = XboxControllerMappingTypes.XboxLeftStickVertical;

        protected GestureState CurrentGestureState = GestureState.SelectButtonUnpressed;

        protected Vector3 NormalizedOffset;

        protected Coroutine HoldStartedRoutine;

        public virtual void OnXboxInputUpdate(XboxControllerEventData eventData)
        {
            if (string.IsNullOrEmpty(GamePadName))
            {
                GamePadName = eventData.InputSource.Name;
            }

            if (XboxControllerMapping.GetButton_Down(SelectButton, eventData))
            {
                CurrentGestureState = GestureState.SelectButtonPressed;

                InputManager.Instance.RaisePointerDown(eventData.InputSource);

                HoldStartedRoutine = StartCoroutine(HandleHoldStarted(eventData));
            }

            if (XboxControllerMapping.GetButton_Pressed(SelectButton, eventData))
            {
                HandleNavigation(eventData);
            }

            if (XboxControllerMapping.GetButton_Up(SelectButton, eventData))
            {
                HandleSelectButtonReleased(eventData);
            }

            // Consume this event
            eventData.Use();
        }

        protected virtual void HandleSelectButtonReleased(XboxControllerEventData eventData)
        {
            InputManager.Instance.RaisePointerUp(eventData.InputSource);

            if (HoldStartedRoutine != null)
            {
                StopCoroutine(HoldStartedRoutine);
            }

            switch (CurrentGestureState)
            {
                case GestureState.NavigationStarted:
                    InputManager.Instance.RaiseNavigationCompleted(eventData.InputSource, Vector3.zero);
                    break;
                case GestureState.HoldStarted:
                    InputManager.Instance.RaiseHoldCompleted(eventData.InputSource);
                    break;
                default:
                    InputManager.Instance.RaiseInputClicked(eventData.InputSource, 1);
                    break;
            }

            CurrentGestureState = GestureState.SelectButtonUnpressed;
        }

        protected virtual IEnumerator HandleHoldStarted(XboxControllerEventData eventData)
        {
            yield return new WaitForSeconds(HoldStartedInterval);

            if (CurrentGestureState == GestureState.HoldStarted || CurrentGestureState == GestureState.NavigationStarted)
            {
                yield break;
            }

            CurrentGestureState = GestureState.HoldStarted;

            InputManager.Instance.RaiseHoldStarted(eventData.InputSource);
        }

        protected virtual void HandleNavigation(XboxControllerEventData eventData)
        {
            float displacementAlongX = XboxControllerMapping.GetAxis(HorizontalNavigationAxis, eventData);
            float displacementAlongY = XboxControllerMapping.GetAxis(VerticalNavigationAxis, eventData);

            if (displacementAlongX == 0.0f && displacementAlongY == 0.0f && CurrentGestureState != GestureState.NavigationStarted) { return; }

            NormalizedOffset.x = displacementAlongX;
            NormalizedOffset.y = displacementAlongY;
            NormalizedOffset.z = 0f;

            if (CurrentGestureState != GestureState.NavigationStarted)
            {
                if (CurrentGestureState == GestureState.HoldStarted)
                {
                    InputManager.Instance.RaiseHoldCanceled(eventData.InputSource);
                }

                CurrentGestureState = GestureState.NavigationStarted;

                // Raise navigation started event.
                InputManager.Instance.RaiseNavigationStarted(eventData.InputSource);
            }
            else
            {
                // Raise navigation updated event.
                InputManager.Instance.RaiseNavigationUpdated(eventData.InputSource, NormalizedOffset);
            }
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Up")]
        protected static bool OnButton_Up(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Up(buttonType, eventData);
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Pressed")]
        protected static bool OnButton_Pressed(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Pressed(buttonType, eventData);
        }

        [Obsolete("Use XboxControllerMapping.GetButton_Down")]
        protected static bool OnButton_Down(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            return XboxControllerMapping.GetButton_Down(buttonType, eventData);
        }
    }
}
