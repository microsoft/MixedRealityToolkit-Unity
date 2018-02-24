// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using System;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.GamePad
{
    public class XboxControllerHandlerBase : GamePadHandlerBase, IXboxControllerHandler
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
                GamePadName = eventData.GamePadName;
            }

            if (XboxControllerMapping.GetButton_Down(SelectButton, eventData))
            {
                CurrentGestureState = GestureState.SelectButtonPressed;

                InputManager.Instance.RaiseSourceDown(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select);

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
            InputManager.Instance.RaiseSourceUp(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select);

            if (HoldStartedRoutine != null)
            {
                StopCoroutine(HoldStartedRoutine);
            }

            switch (CurrentGestureState)
            {
                case GestureState.NavigationStarted:
                    InputManager.Instance.RaiseNavigationCompleted(eventData.InputSource, eventData.SourceId, Vector3.zero);
                    break;
                case GestureState.HoldStarted:
                    InputManager.Instance.RaiseHoldCompleted(eventData.InputSource, eventData.SourceId);
                    break;
                default:
                    InputManager.Instance.RaiseInputClicked(eventData.InputSource, eventData.SourceId, InteractionSourcePressInfo.Select, 1);
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

            InputManager.Instance.RaiseHoldStarted(eventData.InputSource, eventData.SourceId);
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
                    InputManager.Instance.RaiseHoldCanceled(eventData.InputSource, eventData.SourceId);
                }

                CurrentGestureState = GestureState.NavigationStarted;

                // Raise navigation started event.
                InputManager.Instance.RaiseNavigationStarted(eventData.InputSource, eventData.SourceId);
            }
            else
            {
                // Raise navigation updated event.
                InputManager.Instance.RaiseNavigationUpdated(eventData.InputSource, eventData.SourceId, NormalizedOffset);
            }
        }
    }
}
