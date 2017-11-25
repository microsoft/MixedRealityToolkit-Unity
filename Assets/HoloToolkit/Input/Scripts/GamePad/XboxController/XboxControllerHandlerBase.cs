// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerHandlerBase : GamePadHandlerBase, IXboxControllerHandler
    {
        protected enum GestureState
        {
            SelectButtonPressed,
            NavigationStarted,
            NavigationCompleted,
            HoldStarted,
            HoldCompleted,
            HoldCanceled
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

        protected GestureState CurrentGestureState;

        protected bool HoldStarted;
        protected bool RaiseOnce;
        protected bool NavigationStarted;
        protected bool NavigationCompleted;
        protected Vector3 NormalizedOffset;

        protected Coroutine HandStartedRoutine;
        protected Coroutine HoldCompletedRoutine;

        public virtual void OnXboxInputUpdate(XboxControllerEventData eventData)
        {
            if (XboxControllerMapping.GetButton_Down(SelectButton, eventData))
            {
                InputManager.Instance.RaiseSourceDown(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select);
            }

            if (XboxControllerMapping.GetButton_Pressed(SelectButton, eventData))
            {
                HandleNavigation(eventData);

                if (!HoldStarted && !RaiseOnce && !NavigationStarted)
                {
                    HandStartedRoutine = StartCoroutine(HandleHoldStarted(eventData));
                }
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
            InputManager.Instance.RaiseSourceUp(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select);

            switch (CurrentGestureState)
            {
                case GestureState.NavigationStarted:
                    NavigationCompleted = true;
                    StopCoroutine(HandStartedRoutine);
                    StopCoroutine(HoldCompletedRoutine);
                    InputManager.Instance.RaiseNavigationCompleted(eventData.InputSource, eventData.SourceId, Vector3.zero);
                    break;
                case GestureState.HoldStarted:
                    StopCoroutine(HandStartedRoutine);
                    InputManager.Instance.RaiseHoldCanceled(eventData.InputSource, eventData.SourceId);
                    break;
                case GestureState.HoldCompleted:
                    InputManager.Instance.RaiseHoldCompleted(eventData.InputSource, eventData.SourceId);
                    break;
                default:
                    StopCoroutine(HandStartedRoutine);
                    StopCoroutine(HoldCompletedRoutine);
                    InputManager.Instance.RaiseInputClicked(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select, 1);
                    break;
            }

            Reset();
        }

        protected void Reset()
        {
            HoldStarted = false;
            RaiseOnce = false;
            NavigationStarted = false;
        }

        protected virtual IEnumerator HandleHoldStarted(XboxControllerEventData eventData)
        {
            yield return new WaitForSeconds(HoldStartedInterval);

            if (RaiseOnce || CurrentGestureState == GestureState.HoldStarted || CurrentGestureState == GestureState.NavigationStarted)
            {
                yield break;
            }

            HoldStarted = true;

            CurrentGestureState = GestureState.HoldStarted;
            InputManager.Instance.RaiseHoldStarted(eventData.InputSource, eventData.SourceId);
            RaiseOnce = true;

            HoldCompletedRoutine = StartCoroutine(HandleHoldCompleted());
        }

        protected virtual IEnumerator HandleHoldCompleted()
        {
            yield return new WaitForSeconds(HoldCompletedInterval);

            CurrentGestureState = GestureState.HoldCompleted;
        }

        protected virtual void HandleNavigation(XboxControllerEventData eventData)
        {
            if (NavigationCompleted) { return; }

            float displacementAlongX = XboxControllerMapping.GetAxis(HorizontalNavigationAxis, eventData);
            float displacementAlongY = XboxControllerMapping.GetAxis(VerticalNavigationAxis, eventData);

            if (displacementAlongX == 0.0f && displacementAlongY == 0.0f && !NavigationStarted) { return; }

            NormalizedOffset.x = displacementAlongX;
            NormalizedOffset.y = displacementAlongY;
            NormalizedOffset.z = 0f;

            if (!NavigationStarted)
            {
                CurrentGestureState = GestureState.NavigationStarted;
                NavigationStarted = true;

                // Raise navigation started event.
                InputManager.Instance.RaiseNavigationStarted(eventData.InputSource, eventData.SourceId);
            }

            // Raise navigation updated event.
            InputManager.Instance.RaiseNavigationUpdated(eventData.InputSource, eventData.SourceId, NormalizedOffset);
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
