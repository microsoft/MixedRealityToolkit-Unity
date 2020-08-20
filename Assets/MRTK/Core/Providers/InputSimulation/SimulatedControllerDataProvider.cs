// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

/// <summary>
/// Provides per-frame data access to simulated controller data
/// </summary>
namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Internal class to define current state of a controller.
    /// </summary>
    internal abstract class SimulatedControllerState
    {
        protected Handedness handedness = Handedness.None;
        public Handedness Handedness => handedness;

        // Show a tracked controller
        public bool IsTracked = false;
        
        // Position of the controller in viewport space
        public Vector3 ViewportPosition = Vector3.zero;
        // Rotation of the controller relative to the camera
        public Vector3 ViewportRotation = Vector3.zero;
        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset = Vector3.zero;

        protected float viewportPositionZTarget;
        protected readonly float smoothScrollSpeed = 5f;

        public SimulatedControllerState(Handedness _handedness)
        {
            handedness = _handedness;
        }

        public void SimulateInput(MouseDelta mouseDelta, bool useMouseRotation, float rotationSensitivity, float rotationScale, float noiseAmount)
        {
            if (useMouseRotation)
            {
                Vector3 rotationDeltaEulerAngles = Vector3.zero;
                rotationDeltaEulerAngles.x += -mouseDelta.screenDelta.y * rotationSensitivity;
                rotationDeltaEulerAngles.y += mouseDelta.screenDelta.x * rotationSensitivity;
                rotationDeltaEulerAngles.z += mouseDelta.screenDelta.z * rotationSensitivity;
                rotationDeltaEulerAngles *= rotationScale;

                ViewportRotation = ViewportRotation + rotationDeltaEulerAngles;
            }
            else
            {
                ViewportPosition.x += mouseDelta.viewportDelta.x;
                ViewportPosition.y += mouseDelta.viewportDelta.y;
                viewportPositionZTarget += mouseDelta.viewportDelta.z;
            }

            JitterOffset = Random.insideUnitSphere * noiseAmount;
        }

        /// <summary>
        /// Resets simulated controller position.
        /// </summary>
        /// <param name="resetTo">The position to reset controller to.</param>
        public void ResetPosition(Vector3 resetTo)
        {
            ViewportPosition = resetTo;
            viewportPositionZTarget = ViewportPosition.z;
        }

        /// <summary>
        /// Resets simulated controller rotation.
        /// </summary>
        public abstract void ResetRotation();

        /// <summary>
        /// Update information about the controller state or position.
        /// </summary>
        internal void Update()
        {
            ViewportPosition.z = Mathf.Lerp(ViewportPosition.z, viewportPositionZTarget, smoothScrollSpeed * Time.deltaTime);
        }

    }

    /// <summary>
    /// Produces simulated data every frame that defines the position and rotation of the simulated controller.
    /// </summary>
    public abstract class SimulatedControllerDataProvider
    {
        protected MixedRealityInputSimulationProfile profile;

        /// <summary>
        /// If true then the left controller is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleLeft = false;
        /// <summary>
        /// If true then the right controller is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleRight = false;

        internal SimulatedControllerState InputStateLeft;
        internal SimulatedControllerState InputStateRight;
        internal SimulatedControllerState InputStateGaze;

        private bool isSimulatingGaze => !IsSimulatingLeft && !IsSimulatingRight && !IsAlwaysVisibleLeft && !IsAlwaysVisibleRight && !DeviceUtility.IsPresent;
        /// <summary>
        /// Left controller is controlled by user input.
        /// </summary>
        public bool IsSimulatingLeft { get; private set; } = false;
        /// <summary>
        /// Right controller is controlled by user input.
        /// </summary>
        public bool IsSimulatingRight { get; private set; } = false;

        // Most recent time controller control was enabled,
        protected float lastSimulationLeft = -1.0e6f;
        protected float lastSimulationRight = -1.0e6f;
        protected float lastSimulationGaze = -1.0e6f;
        // Last timestamp when controllers were tracked
        protected long lastInputTrackedTimestampLeft = 0;
        protected long lastInputTrackedTimestampRight = 0;
        protected long lastInputTrackedTimestampGaze = 0;

        protected static readonly KeyBinding cancelRotationKey = KeyBinding.FromKey(KeyCode.Escape);
        protected readonly MouseRotationProvider mouseRotation = new MouseRotationProvider();

        public SimulatedControllerDataProvider(MixedRealityInputSimulationProfile _profile)
        {
            profile = _profile;
        }

        /// <summary>
        /// Update controller state based on keyboard and mouse input
        /// </summary>
        protected virtual void SimulateUserInput(MouseDelta mouseDelta)
        {
            float time = Time.time;

            if (KeyInputSystem.GetKeyDown(profile.ToggleLeftControllerKey))
            {
                IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
            }
            if (KeyInputSystem.GetKeyDown(profile.ToggleRightControllerKey))
            {
                IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
            }

            if (!Application.isFocused && !KeyInputSystem.SimulatingInput)
            {
                IsSimulatingLeft = false;
                IsSimulatingRight = false;
            }
            else
            {
                if (KeyInputSystem.GetKeyDown(profile.LeftControllerManipulationKey))
                {
                    IsSimulatingLeft = true;
                    if (lastSimulationLeft > 0.0f && time - lastSimulationLeft <= profile.DoublePressTime)
                    {
                        IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
                    }
                    lastSimulationLeft = time;
                }
                if (KeyInputSystem.GetKeyUp(profile.LeftControllerManipulationKey))
                {
                    IsSimulatingLeft = false;
                }

                if (KeyInputSystem.GetKeyDown(profile.RightControllerManipulationKey))
                {
                    IsSimulatingRight = true;
                    if (lastSimulationRight > 0.0f && time - lastSimulationRight <= profile.DoublePressTime)
                    {
                        IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
                    }
                    lastSimulationRight = time;
                }
                if (KeyInputSystem.GetKeyUp(profile.RightControllerManipulationKey))
                {
                    IsSimulatingRight = false;
                }
                if (isSimulatingGaze)
                {
                    lastSimulationGaze = time;
                }   
            }

            mouseRotation.Update(profile.ControllerRotateButton, cancelRotationKey, false);

            SimulateInput(ref lastInputTrackedTimestampLeft, InputStateLeft, IsSimulatingLeft, IsAlwaysVisibleLeft, mouseDelta, mouseRotation.IsRotating);
            SimulateInput(ref lastInputTrackedTimestampRight, InputStateRight, IsSimulatingRight, IsAlwaysVisibleRight, mouseDelta, mouseRotation.IsRotating);
            SimulateInput(ref lastInputTrackedTimestampGaze, InputStateGaze, isSimulatingGaze, false, mouseDelta, mouseRotation.IsRotating);

        }

        /// Apply changes to one controller and update tracking
        internal abstract void SimulateInput(
            ref long lastHandTrackedTimestamp,
            SimulatedControllerState state,
            bool isSimulating,
            bool isAlwaysVisible,
            MouseDelta mouseDelta,
            bool useMouseRotation);

        /// <summary>
        /// Reset the controller to its default state.
        /// </summary>
        public void ResetInput(Handedness handedness)
        {
            if (handedness == Handedness.Left)
            {
                ResetInput(InputStateLeft, IsSimulatingLeft);
            }
            else
            {
                ResetInput(InputStateRight, IsSimulatingRight);
            }
        }

        internal virtual void ResetInput(SimulatedControllerState state, bool isSimulating)
        {
            if (isSimulating)
            {
                // Start at current mouse position
                Vector3 mousePos = UnityEngine.Input.mousePosition;
                state.ResetPosition(CameraCache.Main.ScreenToViewportPoint(new Vector3(mousePos.x, mousePos.y, profile.DefaultControllerDistance)));
            }
            else
            {
                state.ResetPosition(new Vector3(0.5f, 0.5f, profile.DefaultControllerDistance));
            }
        }
    }
}
