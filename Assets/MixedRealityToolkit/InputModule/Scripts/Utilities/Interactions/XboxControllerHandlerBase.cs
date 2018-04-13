//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License. See LICENSE in the project root for license information.

//using System;
//using System.Collections;
//using UnityEngine;

//namespace MixedRealityToolkit.InputModule.Utilities.Interations
//{
//    public class XboxControllerHandlerBase : GamePadHandlerBase
//    {
//        protected enum GestureState
//        {
//            SelectButtonUnpressed,
//            SelectButtonPressed,
//            NavigationStarted,
//            HoldStarted
//        }

//        [SerializeField]
//        [Tooltip("Elapsed time for hold started gesture in seconds.")]
//        protected float HoldStartedInterval = 2.0f;

//        [SerializeField]
//        [Tooltip("Elapsed time for hold completed gesture in seconds.")]
//        protected float HoldCompletedInterval = 3.0f;

//        [SerializeField]
//        [Tooltip("The action button that is used to select.  Analogous to air tap on HoloLens and trigger press with motion controllers.")]
//        protected XboxControllerInputType SelectButton = XboxControllerInputType.XboxA;

//        [SerializeField]
//        [Tooltip("The Horizontal Axis that navigation events take place")]
//        protected XboxControllerInputType HorizontalNavigationAxis = XboxControllerInputType.XboxLeftStickHorizontal;

//        [Tooltip("The Vertical Axis that navigation events take place")]
//        protected XboxControllerInputType VerticalNavigationAxis = XboxControllerInputType.XboxLeftStickVertical;

//        protected GestureState CurrentGestureState = GestureState.SelectButtonUnpressed;

//        protected Vector3 NormalizedOffset;

//        protected Coroutine HoldStartedRoutine;

//        public virtual void OnXboxInputUpdate(XboxControllerEventData eventData)
//        {
//            if (XboxControllerMapping.GetButton_Down(SelectButton, eventData))
//            {
//                CurrentGestureState = GestureState.SelectButtonPressed;

//                InputManager.Instance.RaisePointerDown(eventData.InputSource);

//                HoldStartedRoutine = StartCoroutine(HandleHoldStarted(eventData));
//            }

//            if (XboxControllerMapping.GetButton_Pressed(SelectButton, eventData))
//            {
//                HandleNavigation(eventData);
//            }

//            if (XboxControllerMapping.GetButton_Up(SelectButton, eventData))
//            {
//                HandleSelectButtonReleased(eventData);
//            }

//            // Consume this event
//            eventData.Use();
//        }

//        protected virtual void HandleSelectButtonReleased(XboxControllerEventData eventData)
//        {
//            InputManager.Instance.RaisePointerUp(eventData.InputSource);

//            if (HoldStartedRoutine != null)
//            {
//                StopCoroutine(HoldStartedRoutine);
//            }

//            switch (CurrentGestureState)
//            {
//                case GestureState.NavigationStarted:
//                    InputManager.Instance.RaiseNavigationCompleted(eventData.InputSource, Vector3.zero);
//                    break;
//                case GestureState.HoldStarted:
//                    InputManager.Instance.RaiseHoldCompleted(eventData.InputSource);
//                    break;
//                default:
//                    InputManager.Instance.RaiseInputClicked(eventData.InputSource, 1);
//                    break;
//            }

//            CurrentGestureState = GestureState.SelectButtonUnpressed;
//        }

//        protected virtual IEnumerator HandleHoldStarted(XboxControllerEventData eventData)
//        {
//            yield return new WaitForSeconds(HoldStartedInterval);

//            if (CurrentGestureState == GestureState.HoldStarted || CurrentGestureState == GestureState.NavigationStarted)
//            {
//                yield break;
//            }

//            CurrentGestureState = GestureState.HoldStarted;

//            InputManager.Instance.RaiseHoldStarted(eventData.InputSource);
//        }

//        protected virtual void HandleNavigation(XboxControllerEventData eventData)
//        {
//            float displacementAlongX = XboxControllerMapping.GetAxis(HorizontalNavigationAxis);
//            float displacementAlongY = XboxControllerMapping.GetAxis(VerticalNavigationAxis);

//            if (displacementAlongX == 0.0f && displacementAlongY == 0.0f && CurrentGestureState != GestureState.NavigationStarted) { return; }

//            NormalizedOffset.x = displacementAlongX;
//            NormalizedOffset.y = displacementAlongY;
//            NormalizedOffset.z = 0f;

//            if (CurrentGestureState != GestureState.NavigationStarted)
//            {
//                if (CurrentGestureState == GestureState.HoldStarted)
//                {
//                    InputManager.Instance.RaiseHoldCanceled(eventData.InputSource);
//                }

//                CurrentGestureState = GestureState.NavigationStarted;

//                // Raise navigation started event.
//                InputManager.Instance.RaiseNavigationStarted(eventData.InputSource);
//            }
//            else
//            {
//                // Raise navigation updated event.
//                InputManager.Instance.RaiseNavigationUpdated(eventData.InputSource, NormalizedOffset);
//            }
//        }
//    }
//}
