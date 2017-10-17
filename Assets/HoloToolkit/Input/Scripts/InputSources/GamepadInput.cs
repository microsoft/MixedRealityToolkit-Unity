// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// GamepadInput class maps Xbox gamepad buttons to the GestureRecognizer.
    /// Gamepad button A press and release maps to tap gesture.
    /// Gamepad button A pressed maps to hold started, completed, canceled gesture.
    /// Gamepad button A pressed plus left joystick rotate maps to navigation gesture.
    /// </summary>
    [Obsolete("Please use GamePadInputSource or XboxControllerInputSource")]
    public class GamepadInput : BaseInputSource
    {
        [Tooltip("Game pad button to press for air tap.")]
        public string GamePadButtonA = "Fire1";

        [Tooltip("Change this value to give a different source id to your controller.")]
        public uint GamePadId = 50000;

        [Tooltip("Elapsed time for hold started gesture in seconds.")]
        public float HoldStartedInterval = 2.0f;
        [Tooltip("Elapsed time for hold completed gesture in seconds.")]
        public float HoldCompletedInterval = 3.0f;

        [Tooltip("Name of the joystick axis that navigates around X.")]
        public string NavigateAroundXAxisName = "ControllerLeftStickX";

        [Tooltip("Name of the joystick axis that navigates around Y.")]
        public string NavigateAroundYAxisName = "ControllerLeftStickY";

        bool isAPressed = false;
        bool holdStarted = false;
        bool raiseOnce = false;
        bool navigationStarted = false;
        bool navigationCompleted = false;

        private InputManager inputManager;

        enum GestureState
        {
            APressed,
            NavigationStarted,
            NavigationCompleted,
            HoldStarted,
            HoldCompleted,
            HoldCanceled
        }

        GestureState currentGestureState;

        protected virtual void Start()
        {
            if (InputManager.IsInitialized)
            {
                inputManager = InputManager.Instance;
            }

            if (inputManager == null)
            {
                Debug.LogError("Ensure your scene has the InputManager prefab.");
                Destroy(this);
            }
        }

        private void Update()
        {
#if UNITY_WSA
            if (InteractionManager.numSourceStates > 0)
            {
                return;
            }
#endif

            HandleGamepadAPressed();
        }

        private void HandleGamepadAPressed()
        {
            // TODO: Should this handle Submit from Edit > ProjectSettings > Input ?
            if (Input.GetButtonDown(GamePadButtonA))
            {
                inputManager.RaiseSourceDown(this, GamePadId, InteractionSourcePressInfo.Select);
                isAPressed = true;
                navigationCompleted = false;
                currentGestureState = GestureState.APressed;
            }

            if (isAPressed)
            {
                HandleNavigation();

                if (!holdStarted && !raiseOnce && !navigationStarted)
                {
                    // Raise hold started when user has held A down for certain interval.
                    Invoke("HandleHoldStarted", HoldStartedInterval);
                }

                // Check if we get a subsequent release on A.
                HandleGamepadAReleased();
            }
        }

        private void HandleNavigation()
        {
            if (navigationCompleted)
            {
                return;
            }

            float displacementAlongX = 0.0f;
            float displacementAlongY = 0.0f;

            try
            {
                displacementAlongX = Input.GetAxis(NavigateAroundXAxisName);
                displacementAlongY = Input.GetAxis(NavigateAroundYAxisName);
            }
            catch (Exception)
            {
                Debug.LogWarningFormat("Ensure you have Edit > ProjectSettings > Input > Axes set with values: {0} and {1}",
                    NavigateAroundXAxisName, NavigateAroundYAxisName);
                navigationCompleted = true; // just so we don't keep logging the same message.
                return;
            }

            if (displacementAlongX != 0.0f || displacementAlongY != 0.0f || navigationStarted)
            {
                Vector3 normalizedOffset = new Vector3(displacementAlongX,
                    displacementAlongY,
                    0.0f);

                if (!navigationStarted)
                {
                    currentGestureState = GestureState.NavigationStarted;
                    navigationStarted = true;
                    // Raise navigation started event.
                    inputManager.RaiseNavigationStarted(this, GamePadId);
                }

                // Raise navigation updated event.
                inputManager.RaiseNavigationUpdated(this, GamePadId, normalizedOffset);
            }
        }

        private void HandleGamepadAReleased()
        {
            if (Input.GetButtonUp(GamePadButtonA))
            {
                inputManager.RaiseSourceUp(this, GamePadId, InteractionSourcePressInfo.Select);

                switch (currentGestureState)
                {
                    case GestureState.NavigationStarted:
                        navigationCompleted = true;
                        CancelInvoke("HandleHoldStarted");
                        CancelInvoke("HandleHoldCompleted");
                        inputManager.RaiseNavigationCompleted(this, GamePadId, Vector3.zero);
                        Reset();
                        break;

                    case GestureState.HoldStarted:
                        CancelInvoke("HandleHoldCompleted");
                        inputManager.RaiseHoldCanceled(this, GamePadId);
                        Reset();
                        break;

                    case GestureState.HoldCompleted:
                        inputManager.RaiseHoldCompleted(this, GamePadId);
                        Reset();
                        break;

                    default:
                        CancelInvoke("HandleHoldStarted");
                        CancelInvoke("HandleHoldCompleted");
                        inputManager.RaiseInputClicked(this, GamePadId, InteractionSourcePressInfo.Select, 1);
                        Reset();
                        break;
                }
            }
        }

        private void Reset()
        {
            isAPressed = false;
            holdStarted = false;
            raiseOnce = false;
            navigationStarted = false;
        }

        private void HandleHoldStarted()
        {
            if (raiseOnce || currentGestureState == GestureState.HoldStarted || currentGestureState == GestureState.NavigationStarted)
            {
                return;
            }

            holdStarted = true;

            currentGestureState = GestureState.HoldStarted;
            inputManager.RaiseHoldStarted(this, GamePadId);
            raiseOnce = true;

            Invoke("HandleHoldCompleted", HoldCompletedInterval);
        }

        private void HandleHoldCompleted()
        {
            currentGestureState = GestureState.HoldCompleted;
        }

        #region BaseInputSource Events
        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            // Since the game pad is not a 3dof or 6dof controller.
            return SupportedInputInfo.None;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            pointingRay = new Ray(Vector3.zero, Vector3.zero);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
            sourceKind = InteractionSourceInfo.Controller;
            return true;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
        {
            isPressed = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
        {
            isPressed = false;
            isTouched = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedAmount)
        {
            isPressed = false;
            pressedAmount = 0.0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }
        #endregion BaseInputSource Events
    }
}