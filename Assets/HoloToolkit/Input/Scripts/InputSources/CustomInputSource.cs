// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for fake input source information, which gives details about current source state and position.
    /// </summary>
    [RequireComponent(typeof(CustomInputControl))]
    public class CustomInputSource : BaseInputSource
    {
        // TODO: add thumbstick, touchpad, and trigger axis support.
        [Serializable]
        private class ButtonStates
        {
            public ButtonStates()
            {
                IsSelectButtonDown = false;
                SelectButtonStateChanged = false;

                IsMenuButtonDown = false;
                MenuButtonStateChanged = false;

                IsGrasped = false;
                GraspStateChanged = false;

                ManipulationInProgress = false;
                HoldInProgress = false;
                CumulativeDelta = Vector3.zero;
            }

            public bool IsSelectButtonDown;
            public bool SelectButtonStateChanged;
            public float SelectDownStartTime;

            public bool IsMenuButtonDown;
            public bool MenuButtonStateChanged;

            public bool IsGrasped;
            public bool GraspStateChanged;

            public bool ManipulationInProgress;
            public bool HoldInProgress;
            public Vector3 CumulativeDelta;
        }

        public bool SupportsPosition;
        public bool SupportsRotation;
        public bool SupportsRay;
        public bool SupportsMenuButton;
        public bool SupportsGrasp;
        public bool RaiseEventsBasedOnVisibility;
        public InteractionSourceInfo SourceKind;

        //Navigation Gesture Emulation vars
        Vector3 NavigatorValues = Vector3.zero; //holds the navigation gesture values [-1,1]
        Vector2 railUsedCurrently = Vector2.one;
        bool    isNavigatorUsingRails = false;

        public Vector3 ControllerPosition;
        public Quaternion ControllerRotation;

        public Ray? PointingRay;

        [SerializeField]
        private ButtonStates currentButtonStates;

        private uint controllerId;

        private CustomInputControl manualController;

        private bool currentlyVisible;
        private bool visibilityChanged;

        /// <summary>
        /// The maximum interval between button down and button up that will result in a clicked event.
        /// </summary>
        private const float MaxClickDuration = 0.5f;

        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to signal intent to start a manipulation. This is a distance, but not a distance in any one direction.")]
        private float manipulationStartMovementThreshold = 0.03f;

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            var supportedInputInfo = SupportedInputInfo.None;

            if (SupportsPosition)
            {
                supportedInputInfo |= SupportedInputInfo.Position;
            }

            if (SupportsRotation)
            {
                supportedInputInfo |= SupportedInputInfo.Rotation;
            }

            if (SupportsRay)
            {
                supportedInputInfo |= SupportedInputInfo.Pointing;
            }

            if (SupportsMenuButton)
            {
                supportedInputInfo |= SupportedInputInfo.Menu;
            }

            if (SupportsGrasp)
            {
                supportedInputInfo |= SupportedInputInfo.Grasp;
            }

            return supportedInputInfo;
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            sourceKind = SourceKind;
            return true;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsPosition)
            {
                position = ControllerPosition;
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsRotation)
            {
                rotation = ControllerRotation;
                return true;
            }

            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsRay && (PointingRay != null))
            {
                pointingRay = (Ray)PointingRay;
                return true;
            }

            pointingRay = default(Ray);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsPosition)
            {
                position = ControllerPosition;
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsRotation)
            {
                rotation = ControllerRotation;
                return true;
            }

            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            isPressed = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            isPressed = false;
            isTouched = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedAmount)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            isPressed = false;
            pressedAmount = 0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool isPressed)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsGrasp)
            {
                isPressed = currentButtonStates.IsGrasped;
                return true;
            }

            isPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool isPressed)
        {
            Debug.Assert(sourceId == controllerId, "Controller data requested for a mismatched source ID.");

            if (SupportsMenuButton)
            {
                isPressed = currentButtonStates.IsMenuButtonDown;
                return true;
            }

            isPressed = false;
            return false;
        }

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            manualController = GetComponent<CustomInputControl>();

            currentButtonStates = new ButtonStates();
            currentlyVisible = false;
            visibilityChanged = false;
            controllerId = (uint)Random.value;

            InteractionInputSource inputSource = FindObjectOfType<InteractionInputSource>();
            if (inputSource != null)
            {
                isNavigatorUsingRails = inputSource.UseRailsNavigation;
            }
        }

        private void Update()
        {
            if (!Application.isEditor) { return; }

            UpdateControllerData();
            SendControllerVisibilityEvents();
        }

        private void OnEnable()
        {
            if (!Application.isEditor) { return; }

            ConnectController();
        }

        private void OnDisable()
        {
            if (!Application.isEditor) { return; }

            DisconnectController();
        }

        private void ConnectController()
        {
            if (!RaiseEventsBasedOnVisibility)
            {
                InputManager.Instance.RaiseSourceDetected(this, controllerId);
            }
        }

        private void DisconnectController()
        {
            if (!RaiseEventsBasedOnVisibility)
            {
                InputManager.Instance.RaiseSourceLost(this, controllerId);
            }
        }

        /// <summary>
        /// Update the controller data for the currently detected controllers.
        /// </summary>
        private void UpdateControllerData()
        {
            bool doUpdateState = !RaiseEventsBasedOnVisibility;

            if (manualController.ControllerInView)
            {
                if (!currentlyVisible)
                {
                    visibilityChanged = true;
                }

                currentlyVisible = true;
                doUpdateState = true;
            }
            else
            {
                if (currentlyVisible)
                {
                    visibilityChanged = true;
                }

                currentlyVisible = false;
            }

            if (doUpdateState)
            {
                UpdateControllerState(manualController.ControllerSourceState);
            }
        }

        /// <summary>
        /// Updates the controller state information.
        /// </summary>
        /// <param name="source">Input source to use to update the position.</param>
        private void UpdateControllerState(DebugInteractionSourceState source)
        {
            float time = manualController.UseUnscaledTime ? Time.unscaledTime : Time.time;

            currentButtonStates.SelectButtonStateChanged = (currentButtonStates.IsSelectButtonDown != source.SelectPressed);
            currentButtonStates.IsSelectButtonDown = source.SelectPressed;

            if (currentButtonStates.SelectButtonStateChanged && source.SelectPressed)
            {
                currentButtonStates.SelectDownStartTime = time;
                currentButtonStates.CumulativeDelta = Vector3.zero;
            }

            if (SupportsPosition)
            {
                Vector3 controllerPosition;
                if (source.SourcePose.TryGetPosition(out controllerPosition))
                {
                    currentButtonStates.CumulativeDelta += controllerPosition - ControllerPosition;
                    ControllerPosition = controllerPosition;
                }
            }

            if (SupportsRotation)
            {
                Quaternion controllerRotation;
                if (source.SourcePose.TryGetRotation(out controllerRotation))
                {
                    ControllerRotation = controllerRotation;
                }
            }

            if (SupportsRay)
            {
                PointingRay = source.SourcePose.PointerRay;
            }

            if (SupportsMenuButton)
            {
                currentButtonStates.MenuButtonStateChanged = (currentButtonStates.IsMenuButtonDown != source.MenuPressed);
                currentButtonStates.IsMenuButtonDown = source.MenuPressed;
            }

            if (SupportsGrasp)
            {
                currentButtonStates.GraspStateChanged = (currentButtonStates.IsGrasped != source.Grasped);
                currentButtonStates.IsGrasped = source.Grasped;
            }

            SendControllerStateEvents(time);
        }

        /// <summary>
        /// Sends the events for controller state changes.
        /// </summary>
        private void SendControllerStateEvents(float time)
        {
            // TODO: Send other new input manager events relating to source updates.
            if (currentButtonStates.SelectButtonStateChanged)
            {
                if (currentButtonStates.IsSelectButtonDown)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, InteractionSourcePressInfo.Select);
                }
                // New up presses require sending different events depending on whether it's also a click, hold, or manipulation.
                else
                {
                    // A gesture is always either a click, a hold or a manipulation.
                    if (currentButtonStates.ManipulationInProgress)
                    {
                        InputManager.Instance.RaiseManipulationCompleted(this, controllerId, currentButtonStates.CumulativeDelta);
                        currentButtonStates.ManipulationInProgress = false;

                        //Navigation Gesture Emulation
                        InputManager.Instance.RaiseNavigationCompleted(this, controllerId, NavigatorValues);
                        NavigatorValues = Vector3.zero;
                        railUsedCurrently = Vector2.one;
                    }
                    // Clicks and holds are based on time, and both are overruled by manipulations.
                    else if (currentButtonStates.HoldInProgress)
                    {
                        InputManager.Instance.RaiseHoldCompleted(this, controllerId);
                        currentButtonStates.HoldInProgress = false;
                    }
                    else
                    {
                        // We currently only support single taps in editor.
                        InputManager.Instance.RaiseInputClicked(this, controllerId, InteractionSourcePressInfo.Select, 1);
                    }

                    InputManager.Instance.RaiseSourceUp(this, controllerId, InteractionSourcePressInfo.Select);
                }
            }
            // If the select state hasn't changed, but it's down, that means it might
            // trigger a hold or a manipulation (or a hold and then a manipulation).
            else if (currentButtonStates.IsSelectButtonDown)
            {
                if (!currentButtonStates.ManipulationInProgress)
                {
                    // Manipulations are triggered by the amount of movement since select was pressed down.
                    if (currentButtonStates.CumulativeDelta.magnitude > manipulationStartMovementThreshold)
                    {
                        // Starting a manipulation will cancel an existing hold.
                        if (currentButtonStates.HoldInProgress)
                        {
                            InputManager.Instance.RaiseHoldCanceled(this, controllerId);
                            currentButtonStates.HoldInProgress = false;
                        }

                        InputManager.Instance.RaiseManipulationStarted(this, controllerId);
                        currentButtonStates.ManipulationInProgress = true;

                        //Navigation Gesture Emulation
                        InputManager.Instance.RaiseNavigationStarted(this, controllerId);
                        NavigatorValues = Vector3.zero;
                        if (isNavigatorUsingRails)
                        {
                            railUsedCurrently = (currentButtonStates.CumulativeDelta.x >= manipulationStartMovementThreshold) ? new Vector2(1, 0) : new Vector2(0, 1);
                        }
                    }
                    // Holds are triggered by time.
                    else if (!currentButtonStates.HoldInProgress && (time - currentButtonStates.SelectDownStartTime >= MaxClickDuration))
                    {
                        InputManager.Instance.RaiseHoldStarted(this, controllerId);
                        currentButtonStates.HoldInProgress = true;
                    }
                }
                else
                {
                    InputManager.Instance.RaiseManipulationUpdated(this, controllerId, currentButtonStates.CumulativeDelta);

                    //Navigation Gesture Emulation
                    NavigatorValues.x = Mathf.Clamp(currentButtonStates.CumulativeDelta.x*5, -1.0f, 1.0f) * railUsedCurrently.x;
                    NavigatorValues.y = Mathf.Clamp(currentButtonStates.CumulativeDelta.y*5, -1.0f, 1.0f) * railUsedCurrently.y;
                    InputManager.Instance.RaiseNavigationUpdated(this, controllerId, NavigatorValues);
                }
            }

            if (currentButtonStates.MenuButtonStateChanged)
            {
                if (currentButtonStates.IsMenuButtonDown)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, InteractionSourcePressInfo.Menu);
                }
                else
                {
                    InputManager.Instance.RaiseSourceUp(this, controllerId, InteractionSourcePressInfo.Menu);
                }
            }

            if (currentButtonStates.GraspStateChanged)
            {
                if (currentButtonStates.IsGrasped)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, InteractionSourcePressInfo.Grasp);
                }
                else
                {
                    InputManager.Instance.RaiseSourceUp(this, controllerId, InteractionSourcePressInfo.Grasp);
                }
            }
        }

        /// <summary>
        /// Sends the events for hand visibility changes &amp; controller connect/disconnect.
        /// </summary>
        private void SendControllerVisibilityEvents()
        {
            // Send event for new hands that were added
            if (RaiseEventsBasedOnVisibility && visibilityChanged)
            {
                if (currentlyVisible)
                {
                    InputManager.Instance.RaiseSourceDetected(this, controllerId);
                }
                else
                {
                    InputManager.Instance.RaiseSourceLost(this, controllerId);
                }

                visibilityChanged = false;
            }
        }
    }
}
