// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

/// <summary>
/// Provides per-frame data access to simulated motion controller data
/// </summary>
namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Internal class to define current state of a motion controller.
    /// </summary>
    internal class SimulatedMotionControllerState : SimulatedControllerState
    {
        /// <summary>
        /// States of buttons on the motion controller
        /// </summary>
        public SimulatedMotionControllerButtonState ButtonState { get; set; } = new SimulatedMotionControllerButtonState();

        private const float rotationXOffset = -15f;
        private const float rotationYOffset = -10f;

        public SimulatedMotionControllerState(Handedness _handedness) : base(_handedness) { }

        /// <inheritdoc />
        public override void ResetRotation()
        {
            ViewportRotation = Vector3.zero;
        }

        /// <summary>
        /// Resets the states of buttons on the simulated controller.
        /// </summary>
        public void ResetButtonStates()
        {
            ButtonState = new SimulatedMotionControllerButtonState();
        }

        internal MixedRealityPose UpdateControllerPose()
        {
            Vector3 screenPosition = CameraCache.Main.ViewportToScreenPoint(ViewportPosition);
            Vector3 worldPosition = CameraCache.Main.ScreenToWorldPoint(screenPosition + JitterOffset);

            Quaternion localRotation = Quaternion.Euler(ViewportRotation);
            Quaternion worldRotation = CameraCache.Main.transform.rotation * localRotation;
            Vector3 eulerAngles = worldRotation.eulerAngles;

            // Create an offset to rotation to align with the behavior of simulated hand
            int yOffsetSign = handedness == Handedness.Left ? -1 : 1;
            Quaternion modifiedRotation = Quaternion.Euler(eulerAngles.x + rotationXOffset, eulerAngles.y + rotationYOffset * yOffsetSign, eulerAngles.z);
            return new MixedRealityPose(worldPosition, modifiedRotation);
        }

    }

    /// <inheritdoc />
    public class SimulatedMotionControllerDataProvider : SimulatedControllerDataProvider
    {
        // Cached delegates for position and rotation update
        private SimulatedMotionControllerData.MotionControllerPoseUpdater updaterLeft;
        private SimulatedMotionControllerData.MotionControllerPoseUpdater updaterRight;

        public SimulatedMotionControllerDataProvider(MixedRealityInputSimulationProfile _profile) : base(_profile)
        {
            InputStateLeft = new SimulatedMotionControllerState(Handedness.Left);
            InputStateRight = new SimulatedMotionControllerState(Handedness.Right);
        }

        /// <inheritdoc />
        internal override void SimulateInput(ref long lastMotionControllerTrackedTimestamp, SimulatedControllerState state, bool isSimulating, bool isAlwaysVisible, MouseDelta mouseDelta, bool useMouseRotation)
        {
            if (!(state is SimulatedMotionControllerState motionControllerState))
            {
                return;
            }
            bool enableTracking = isAlwaysVisible || isSimulating;
            if (!motionControllerState.IsTracked && enableTracking)
            {
                ResetInput(motionControllerState, isSimulating);
            }
            if (isSimulating)
            {
                motionControllerState.SimulateInput(mouseDelta, useMouseRotation, profile.MouseRotationSensitivity, profile.MouseControllerRotationSpeed, profile.ControllerJitterAmount);

                motionControllerState.ButtonState = new SimulatedMotionControllerButtonState
                {
                    IsSelecting = KeyInputSystem.GetKey(profile.MotionControllerTriggerKey),
                    IsGrabbing = KeyInputSystem.GetKey(profile.MotionControllerGrabKey),
                    IsPressingMenu = KeyInputSystem.GetKey(profile.MotionControllerMenuKey)
                };
            }

            // Update tracked state of a motion controller.
            // If hideTimeout value is null, motion controllers will stay visible after tracking stops.
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            DateTime currentTime = DateTime.UtcNow;
            if (enableTracking)
            {
                motionControllerState.IsTracked = true;
                lastMotionControllerTrackedTimestamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastMotionControllerTrackedTimestamp)).TotalSeconds;
                if (timeSinceTracking > profile.ControllerHideTimeout)
                {
                    motionControllerState.IsTracked = false;
                }
            }
        }

        /// <summary>
        /// Capture a snapshot of simulated motion controller data based on current state.
        /// </summary>
        public void UpdateControllerData(SimulatedMotionControllerData motionControllerDataLeft, SimulatedMotionControllerData motionControllerDataRight, MouseDelta mouseDelta)
        {
            SimulateUserInput(mouseDelta);

            SimulatedMotionControllerState motionControllerStateLeft = InputStateLeft as SimulatedMotionControllerState;
            SimulatedMotionControllerState motionControllerStateRight = InputStateRight as SimulatedMotionControllerState;

            motionControllerStateLeft.Update();
            motionControllerStateRight.Update();

            // Cache the generator delegates so we don't gc alloc every frame
            if (updaterLeft == null)
            {
                updaterLeft = motionControllerStateLeft.UpdateControllerPose;
            }

            if (updaterRight == null)
            {
                updaterRight = motionControllerStateRight.UpdateControllerPose;
            }

            motionControllerDataLeft.Update(motionControllerStateLeft.IsTracked, motionControllerStateLeft.ButtonState, updaterLeft);
            motionControllerDataRight.Update(motionControllerStateRight.IsTracked, motionControllerStateRight.ButtonState, updaterRight);
        }

        internal override void ResetInput(SimulatedControllerState state, bool isSimulating)
        {
            base.ResetInput(state, isSimulating);

            var motionControllerState = state as SimulatedMotionControllerState;

            motionControllerState.ResetButtonStates();
            motionControllerState.ResetRotation();
        }
    }
}
