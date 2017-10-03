// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for fake input source information, which gives details about current source state and position.
    /// </summary>
    [RequireComponent(typeof(EditorInputControl))]
    public class EditorInputSource : BaseInputSource
    {
        // TODO: add thumbstick, touchpad, and trigger axis support.

        public class ButtonStates
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

        public Vector3 ControllerPosition;
        public Quaternion ControllerRotation;

        public Ray? PointingRay;

        public ButtonStates CurrentButtonStates;

        private uint controllerId;

        private EditorInputControl manualController;

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

            SupportedInputInfo supportedInputInfo = SupportedInputInfo.None;

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
                isPressed = CurrentButtonStates.IsGrasped;
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
                isPressed = CurrentButtonStates.IsMenuButtonDown;
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

            manualController = GetComponent<EditorInputControl>();

            CurrentButtonStates = new ButtonStates();
            currentlyVisible = false;
            visibilityChanged = false;
            controllerId = (uint)Random.value;
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateControllerData();
            SendControllerVisibilityEvents();
        }

        protected override void OnEnableAfterStart()
        {
            base.OnEnableAfterStart();

            ConnectController();
        }

        protected override void OnDisableAfterStart()
        {
            DisconnectController();

            base.OnDisableAfterStart();
        }
#endif

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
            float time;

            if (manualController.UseUnscaledTime)
            {
                time = Time.unscaledTime;
            }
            else
            {
                time = Time.time;
            }

            CurrentButtonStates.SelectButtonStateChanged = (CurrentButtonStates.IsSelectButtonDown != source.selectPressed);
            CurrentButtonStates.IsSelectButtonDown = source.selectPressed;

            if (CurrentButtonStates.SelectButtonStateChanged && source.selectPressed)
            {
                CurrentButtonStates.SelectDownStartTime = time;
                CurrentButtonStates.CumulativeDelta = Vector3.zero;
            }

            if (SupportsPosition)
            {
                Vector3 controllerPosition;
                if (source.sourcePose.TryGetPosition(out controllerPosition))
                {
                    CurrentButtonStates.CumulativeDelta += controllerPosition - ControllerPosition;
                    ControllerPosition = controllerPosition;
                }
            }

            if (SupportsRotation)
            {
                Quaternion controllerRotation;
                if (source.sourcePose.TryGetRotation(out controllerRotation))
                {
                    ControllerRotation = controllerRotation;
                }
            }

            if (SupportsRay)
            {
                PointingRay = source.sourcePose.PointerRay;
            }

            if (SupportsMenuButton)
            {
                CurrentButtonStates.MenuButtonStateChanged = (CurrentButtonStates.IsMenuButtonDown != source.menuPressed);
                CurrentButtonStates.IsMenuButtonDown = source.menuPressed;
            }

            if (SupportsGrasp)
            {
                CurrentButtonStates.GraspStateChanged = (CurrentButtonStates.IsGrasped != source.grasped);
                CurrentButtonStates.IsGrasped = source.grasped;
            }

            SendControllerStateEvents(time);
        }

        /// <summary>
        /// Sends the events for controller state changes.
        /// </summary>
        private void SendControllerStateEvents(float time)
        {
            // TODO: Send other new input manager events relating to source updates.
#if UNITY_WSA
            if (CurrentButtonStates.SelectButtonStateChanged)
            {
                if (CurrentButtonStates.IsSelectButtonDown)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Select);
                }
                // New up presses require sending different events depending on whether it's also a click, hold, or manipulation.
                else
                {
                    // A gesture is always either a click, a hold or a manipulation.
                    if (CurrentButtonStates.ManipulationInProgress)
                    {
                        InputManager.Instance.RaiseManipulationCompleted(this, controllerId, CurrentButtonStates.CumulativeDelta);
                        CurrentButtonStates.ManipulationInProgress = false;
                    }
                    // Clicks and holds are based on time, and both are overruled by manipulations.
                    else if (CurrentButtonStates.HoldInProgress)
                    {
                        InputManager.Instance.RaiseHoldCompleted(this, controllerId);
                        CurrentButtonStates.HoldInProgress = false;
                    }
                    else
                    {
                        // We currently only support single taps in editor.
                        InputManager.Instance.RaiseInputClicked(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Select, 1);
                    }
                    InputManager.Instance.RaiseSourceUp(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Select);
                }
            }
            // If the select state hasn't changed, but it's down, that means it might
            // trigger a hold or a manipulation (or a hold and then a manipulation).
            else if (CurrentButtonStates.IsSelectButtonDown)
            {
                if (!CurrentButtonStates.ManipulationInProgress)
                {
                    // Manipulations are triggered by the amount of movement since select was pressed down.
                    if (CurrentButtonStates.CumulativeDelta.magnitude > manipulationStartMovementThreshold)
                    {
                        // Starting a manipulation will cancel an existing hold.
                        if (CurrentButtonStates.HoldInProgress)
                        {
                            InputManager.Instance.RaiseHoldCanceled(this, controllerId);
                            CurrentButtonStates.HoldInProgress = false;
                        }

                        InputManager.Instance.RaiseManipulationStarted(this, controllerId);
                        CurrentButtonStates.ManipulationInProgress = true;
                    }
                    // Holds are triggered by time.
                    else if (!CurrentButtonStates.HoldInProgress && (time - CurrentButtonStates.SelectDownStartTime >= MaxClickDuration))
                    {
                        InputManager.Instance.RaiseHoldStarted(this, controllerId);
                        CurrentButtonStates.HoldInProgress = true;
                    }
                }
                else
                {
                    InputManager.Instance.RaiseManipulationUpdated(this, controllerId, CurrentButtonStates.CumulativeDelta);
                }
            }

            if (CurrentButtonStates.MenuButtonStateChanged)
            {
                if (CurrentButtonStates.IsMenuButtonDown)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Menu);
                }
                else
                {
                    InputManager.Instance.RaiseSourceUp(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Menu);
                }
            }

            if (CurrentButtonStates.GraspStateChanged)
            {
                if (CurrentButtonStates.IsGrasped)
                {
                    InputManager.Instance.RaiseSourceDown(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Grasp);
                }
                else
                {
                    InputManager.Instance.RaiseSourceUp(this, controllerId, (InteractionSourcePressInfo)InteractionSourcePressType.Grasp);
                }
            }
#endif
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