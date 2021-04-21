// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

/// <summary>
/// Provides per-frame data access to simulated hand data
/// 
/// Controls for mouse/keyboard simulation:
/// - Press spacebar to turn right hand on/off
/// - Left mouse button brings index and thumb together
/// - Mouse moves left and right hand.
/// </summary>
namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Internal class to define current gesture and smoothly animate hand data points.
    /// </summary>
    internal class SimulatedHandState : SimulatedControllerState
    {
        // Activate the pinch gesture
        // Pinch is a special gesture that triggers the Select and TriggerPress input actions
        // The pinch action doesn't occur until the gesture is completed.
        public bool IsPinching => gesture == ArticulatedHandPose.GestureId.Pinch && gestureBlending == 1.0f;

        private ArticulatedHandPose.GestureId gesture = ArticulatedHandPose.GestureId.None;
        public ArticulatedHandPose.GestureId Gesture
        {
            get { return gesture; }
            set
            {
                if (value != ArticulatedHandPose.GestureId.None && value != gesture)
                {
                    gesture = value;
                    gestureBlending = 0.0f;
                }
            }
        }

        // Interpolation between current pose and target gesture
        private float gestureBlending = 0.0f;
        public float GestureBlending
        {
            get { return gestureBlending; }
            set
            {
                gestureBlending = Mathf.Clamp(value, gestureBlending, 1.0f);
            }
        }

        private float poseBlending = 0.0f;
        private ArticulatedHandPose pose = new ArticulatedHandPose();

        public SimulatedHandState(Handedness _handedness) : base(_handedness) { }

        public void ResetGesture()
        {
            gestureBlending = 1.0f;

            ArticulatedHandPose gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.Copy(gesturePose);
            }
        }

        public override void ResetRotation()
        {
            ViewportRotation = Vector3.zero;
        }

        internal void FillCurrentFrame(MixedRealityPose[] jointsOut)
        {
            ArticulatedHandPose gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                if (gestureBlending > poseBlending)
                {
                    float range = Mathf.Clamp01(1.0f - poseBlending);
                    float lerpFactor = range > 0.0f ? (gestureBlending - poseBlending) / range : 1.0f;
                    pose.InterpolateOffsets(pose, gesturePose, lerpFactor);
                }
            }
            poseBlending = gestureBlending;

            Vector3 screenPosition = CameraCache.Main.ViewportToScreenPoint(ViewportPosition);
            Vector3 worldPosition = CameraCache.Main.ScreenToWorldPoint(screenPosition + JitterOffset);

            Quaternion worldRotation = CameraCache.Main.transform.rotation * Quaternion.Euler(ViewportRotation);
            pose.ComputeJointPoses(handedness, worldRotation, worldPosition, jointsOut);
        }
    }

    /// <summary>
    /// Produces simulated data every frame that defines joint positions.
    /// </summary>
    public class SimulatedHandDataProvider : SimulatedControllerDataProvider
    {
        // Cached delegates for hand joint generation
        private SimulatedHandData.HandJointDataGenerator generatorLeft;
        private SimulatedHandData.HandJointDataGenerator generatorRight;
        private SimulatedHandData.HandJointDataGenerator generatorGaze;

        public SimulatedHandDataProvider(MixedRealityInputSimulationProfile _profile) : base(_profile)
        {
            InputStateLeft = new SimulatedHandState(Handedness.Left);
            InputStateRight = new SimulatedHandState(Handedness.Right);
            InputStateGaze = new SimulatedHandState(Handedness.None);

            SimulatedHandState handStateLeft = InputStateLeft as SimulatedHandState;
            SimulatedHandState handStateRight = InputStateRight as SimulatedHandState;
            SimulatedHandState handStateGaze = InputStateGaze as SimulatedHandState;

            handStateLeft.Gesture = profile.DefaultHandGesture;
            handStateRight.Gesture = profile.DefaultHandGesture;
            handStateGaze.Gesture = profile.DefaultHandGesture;
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        public bool UpdateHandData(SimulatedHandData handDataLeft, SimulatedHandData handDataRight, SimulatedHandData handDataGaze, MouseDelta mouseDelta)
        {
            SimulateUserInput(mouseDelta);

            SimulatedHandState handStateLeft = InputStateLeft as SimulatedHandState;
            SimulatedHandState handStateRight = InputStateRight as SimulatedHandState;
            SimulatedHandState handStateGaze = InputStateGaze as SimulatedHandState;

            handStateLeft.Update();
            handStateRight.Update();
            handStateGaze.Update();

            bool handDataChanged = false;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = handStateLeft.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = handStateRight.FillCurrentFrame;
            }

            if (generatorGaze == null)
            {
                generatorGaze = handStateGaze.FillCurrentFrame;
            }

            handDataChanged |= handDataLeft.Update(handStateLeft.IsTracked, handStateLeft.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.Update(handStateRight.IsTracked, handStateRight.IsPinching, generatorRight);
            handDataChanged |= handDataGaze.Update(handStateGaze.IsTracked, handStateGaze.IsPinching, generatorGaze);

            return handDataChanged;
        }

        /// <summary>
        /// Update hand state based on keyboard and mouse input
        /// </summary>
        protected override void SimulateUserInput(MouseDelta mouseDelta)
        {
            base.SimulateUserInput(mouseDelta);

            SimulatedHandState handStateLeft = InputStateLeft as SimulatedHandState;
            SimulatedHandState handStateRight = InputStateRight as SimulatedHandState;
            SimulatedHandState handStateGaze = InputStateGaze as SimulatedHandState;

            // This line explicitly uses unscaledDeltaTime because we don't want input simulation
            // to lag when the time scale is set to a value other than 1. Input should still continue
            // to move freely.
            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.unscaledDeltaTime;
            handStateLeft.GestureBlending += gestureAnimDelta;
            handStateRight.GestureBlending += gestureAnimDelta;
            handStateGaze.GestureBlending = 1.0f;
        }

        /// Apply changes to one hand and update tracking
        internal override void SimulateInput(
            ref long lastHandTrackedTimestamp,
            SimulatedControllerState state,
            bool isSimulating,
            bool isAlwaysVisible,
            MouseDelta mouseDelta,
            bool useMouseRotation)
        {
            var handState = state as SimulatedHandState;
            bool enableTracking = isAlwaysVisible || isSimulating;
            if (!handState.IsTracked && enableTracking)
            {
                ResetInput(handState, isSimulating);
            }

            if (isSimulating)
            {
                handState.SimulateInput(mouseDelta, useMouseRotation, profile.MouseRotationSensitivity, profile.MouseControllerRotationSpeed, profile.ControllerJitterAmount);

                if (isAlwaysVisible)
                {
                    // Toggle gestures on/off
                    handState.Gesture = ToggleGesture(handState.Gesture);
                }
                else
                {
                    // Enable gesture while mouse button is pressed
                    handState.Gesture = SelectGesture();
                }
            }

            // Update tracked state of a hand.
            // If hideTimeout value is null, hands will stay visible after tracking stops.
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            DateTime currentTime = DateTime.UtcNow;
            if (enableTracking)
            {
                handState.IsTracked = true;
                lastHandTrackedTimestamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastHandTrackedTimestamp)).TotalSeconds;
                if (timeSinceTracking > profile.ControllerHideTimeout)
                {
                    handState.IsTracked = false;
                }
            }
        }

        internal override void ResetInput(SimulatedControllerState state, bool isSimulating)
        {
            base.ResetInput(state, isSimulating);

            var handState = state as SimulatedHandState;

            handState.Gesture = profile.DefaultHandGesture;
            handState.ResetGesture();
            handState.ResetRotation();
        }

        /// <summary>
        /// Gets the currently active gesture, according to the mouse configuration and mouse button that is down.
        /// </summary>
        private ArticulatedHandPose.GestureId SelectGesture()
        {
            // Each check needs to verify that both:
            // 1) The corresponding mouse button is down (meaning the gesture, if defined, should be used)
            // 2) The gesture is defined.
            // If only #1 is checked and #2 is not checked, it's possible to "miss" transitions in cases where the user has
            // the left mouse button down and then while it is down, presses the right button, and then lifts the left.
            // It's not until both mouse buttons lift in that case, that the state finally "rests" to the DefaultHandGesture.
            if (KeyInputSystem.GetKey(profile.InteractionButton) && profile.LeftMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return profile.LeftMouseHandGesture;
            }
            else if (KeyInputSystem.GetKey(profile.MouseLookButton) && profile.RightMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return profile.RightMouseHandGesture;
            }
            else if (KeyInputSystem.GetKey(KeyBinding.FromMouseButton(KeyBinding.MouseButton.Middle)) && profile.MiddleMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return profile.MiddleMouseHandGesture;
            }
            else
            {
                return profile.DefaultHandGesture;
            }
        }

        private ArticulatedHandPose.GestureId ToggleGesture(ArticulatedHandPose.GestureId gesture)
        {
            // See comments in SelectGesture for why both the button down and gesture are checked.
            if (KeyInputSystem.GetKeyDown(profile.InteractionButton) && profile.LeftMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return (gesture != profile.LeftMouseHandGesture ? profile.LeftMouseHandGesture : profile.DefaultHandGesture);
            }
            else if (KeyInputSystem.GetKeyDown(profile.MouseLookButton) && profile.RightMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return (gesture != profile.RightMouseHandGesture ? profile.RightMouseHandGesture : profile.DefaultHandGesture);
            }
            else if (KeyInputSystem.GetKeyDown(KeyBinding.FromMouseButton(KeyBinding.MouseButton.Middle)) && profile.MiddleMouseHandGesture != ArticulatedHandPose.GestureId.None)
            {
                return (gesture != profile.MiddleMouseHandGesture ? profile.MiddleMouseHandGesture : profile.DefaultHandGesture);
            }
            else
            {
                // 'None' will not change the gesture
                return ArticulatedHandPose.GestureId.None;
            }
        }

        #region Obsolete Fields
        [Obsolete("Use InputStateLeft instead.")]
        internal SimulatedHandState HandStateLeft
        {
            get => InputStateLeft as SimulatedHandState;
            set { InputStateLeft = value; }
        }
        [Obsolete("Use InputStateRight instead.")]
        internal SimulatedHandState HandStateRight
        {
            get => InputStateRight as SimulatedHandState;
            set { InputStateRight = value; }
        }
        [Obsolete("Use InputStateGaze instead.")]
        internal SimulatedHandState HandStateGaze
        {
            get => InputStateGaze as SimulatedHandState;
            set { InputStateGaze = value; }
        }
        #endregion
    }
}
