// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

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
    internal class SimulatedHandState
    {
        private Handedness handedness = Handedness.None;
        public Handedness Handedness => handedness;

        // Show a tracked hand device
        public bool IsTracked = false;
        // Activate the pinch gesture
        public bool IsPinching
        {
            get { return gesture == ArticulatedHandPose.GestureId.Pinch; }
        }

        // Position of the hand in viewport space
        public Vector3 ViewportPosition = Vector3.zero;
        // Rotation of the hand relative to the camera
        public Vector3 ViewportRotation = Vector3.zero;
        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset = Vector3.zero;

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
        private float viewportPositionZTarget;
        private readonly float smoothScrollSpeed = 5f;

        public SimulatedHandState(Handedness _handedness)
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

            JitterOffset = UnityEngine.Random.insideUnitSphere * noiseAmount;
        }

        public void ResetGesture()
        {
            gestureBlending = 1.0f;

            ArticulatedHandPose gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.Copy(gesturePose);
            }
        }

        /// <summary>
        /// Resets simulated hand position.
        /// </summary>
        /// <param name="resetTo">The position to reset hand to.</param>
        public void ResetPosition(Vector3 resetTo)
        {
            ViewportPosition = resetTo;
            viewportPositionZTarget = ViewportPosition.z;
        }

        public void ResetRotation()
        {
            // Use wrist joint rotation as the default
            Quaternion rotationRef = pose.GetLocalJointPose(TrackedHandJoint.Wrist, handedness).Rotation;
            ViewportRotation = rotationRef.eulerAngles;
        }

        /// <summary>
        /// Update information about the hand state or position.
        /// </summary>
        internal void Update()
        {
            ViewportPosition.z = Mathf.Lerp(ViewportPosition.z, viewportPositionZTarget, smoothScrollSpeed * Time.deltaTime);
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

            // Apply rotation relative to the wrist joint
            Quaternion rotationRef = pose.GetLocalJointPose(TrackedHandJoint.Wrist, handedness).Rotation;
            Quaternion localRotation = Quaternion.Euler(ViewportRotation) * Quaternion.Inverse(rotationRef);

            Quaternion worldRotation = CameraCache.Main.transform.rotation * localRotation;
            pose.ComputeJointPoses(handedness, worldRotation, worldPosition, jointsOut);
        }
    }

    /// <summary>
    /// Produces simulated data every frame that defines joint positions.
    /// </summary>
    public class SimulatedHandDataProvider
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        protected MixedRealityInputSimulationProfile profile;

        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleLeft = false;
        /// <summary>
        /// If true then the hand is always visible, regardless of simulating.
        /// </summary>
        public bool IsAlwaysVisibleRight = false;

        internal SimulatedHandState HandStateLeft;
        internal SimulatedHandState HandStateRight;
        internal SimulatedHandState HandStateGaze;

        // If true then hands are controlled by user input
        private bool isSimulatingLeft = false;
        private bool isSimulatingRight = false;
        private bool isSimulatingGaze => !isSimulatingLeft && !isSimulatingRight && !IsAlwaysVisibleLeft && !IsAlwaysVisibleRight && !XRDevice.isPresent;
        /// <summary>
        /// Left hand is controlled by user input.
        /// </summary>
        public bool IsSimulatingLeft => isSimulatingLeft;
        /// <summary>
        /// Right hand is controlled by user input.
        /// </summary>
        public bool IsSimulatingRight => isSimulatingRight;

        // Most recent time hand control was enabled,
        private float lastSimulationLeft = -1.0e6f;
        private float lastSimulationRight = -1.0e6f;
        private float lastSimulationGaze = -1.0e6f;
        // Last timestamp when hands were tracked
        private long lastHandTrackedTimestampLeft = 0;
        private long lastHandTrackedTimestampRight = 0;
        private long lastHandTrackedTimestampGaze = 0;
        // Cached delegates for hand joint generation
        private SimulatedHandData.HandJointDataGenerator generatorLeft;
        private SimulatedHandData.HandJointDataGenerator generatorRight;
        private SimulatedHandData.HandJointDataGenerator generatorGaze;

        private static readonly KeyBinding cancelRotationKey = KeyBinding.FromKey(KeyCode.Escape);
        private readonly MouseRotationProvider mouseRotation = new MouseRotationProvider();

        public SimulatedHandDataProvider(MixedRealityInputSimulationProfile _profile)
        {
            profile = _profile;

            HandStateLeft = new SimulatedHandState(Handedness.Left);
            HandStateRight = new SimulatedHandState(Handedness.Right);
            HandStateGaze = new SimulatedHandState(Handedness.None);

            HandStateLeft.Gesture = profile.DefaultHandGesture;
            HandStateRight.Gesture = profile.DefaultHandGesture;
            HandStateGaze.Gesture = profile.DefaultHandGesture;
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        public bool UpdateHandData(SimulatedHandData handDataLeft, SimulatedHandData handDataRight, SimulatedHandData handDataGaze, MouseDelta mouseDelta)
        {
            SimulateUserInput(mouseDelta);

            HandStateLeft.Update();
            HandStateRight.Update();
            HandStateGaze.Update();

            bool handDataChanged = false;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = HandStateLeft.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = HandStateRight.FillCurrentFrame;
            }

            if (generatorGaze == null)
            {
                generatorGaze = HandStateGaze.FillCurrentFrame;
            }

            handDataChanged |= handDataLeft.Update(HandStateLeft.IsTracked, HandStateLeft.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.Update(HandStateRight.IsTracked, HandStateRight.IsPinching, generatorRight);
            handDataChanged |= handDataGaze.Update(HandStateGaze.IsTracked, HandStateGaze.IsPinching, generatorGaze);

            return handDataChanged;
        }

        /// <summary>
        /// Update hand state based on keyboard and mouse input
        /// </summary>
        private void SimulateUserInput(MouseDelta mouseDelta)
        {
            float time = Time.time;

            if (KeyInputSystem.GetKeyDown(profile.ToggleLeftHandKey))
            {
                IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
            }
            if (KeyInputSystem.GetKeyDown(profile.ToggleRightHandKey))
            {
                IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
            }

            if (!Application.isFocused && !KeyInputSystem.SimulatingInput)
            {
                isSimulatingLeft = false;
                isSimulatingRight = false;
            }
            else
            {
                if (KeyInputSystem.GetKeyDown(profile.LeftHandManipulationKey))
                {
                    isSimulatingLeft = true;
                    if (lastSimulationLeft > 0.0f && time - lastSimulationLeft <= profile.DoublePressTime)
                    {
                        IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
                    }
                    lastSimulationLeft = time;
                }
                if (KeyInputSystem.GetKeyUp(profile.LeftHandManipulationKey))
                {
                    isSimulatingLeft = false;
                }

                if (KeyInputSystem.GetKeyDown(profile.RightHandManipulationKey))
                {
                    isSimulatingRight = true;
                    if (lastSimulationRight > 0.0f && time - lastSimulationRight <= profile.DoublePressTime)
                    {
                        IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
                    }
                    lastSimulationRight = time;
                }
                if (KeyInputSystem.GetKeyUp(profile.RightHandManipulationKey))
                {
                    isSimulatingRight = false;
                }
                if(isSimulatingGaze)
                    lastSimulationGaze = time;
            }

            mouseRotation.Update(profile.HandRotateButton, cancelRotationKey, false);

            SimulateHandInput(ref lastHandTrackedTimestampLeft, HandStateLeft, isSimulatingLeft, IsAlwaysVisibleLeft, mouseDelta, mouseRotation.IsRotating);
            SimulateHandInput(ref lastHandTrackedTimestampRight, HandStateRight, isSimulatingRight, IsAlwaysVisibleRight, mouseDelta, mouseRotation.IsRotating);
            SimulateHandInput(ref lastHandTrackedTimestampGaze, HandStateGaze, isSimulatingGaze, false, mouseDelta, mouseRotation.IsRotating);

            // This line explicitly uses unscaledDeltaTime because we don't want input simulation
            // to lag when the time scale is set to a value other than 1. Input should still continue
            // to move freely.
            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.unscaledDeltaTime;
            HandStateLeft.GestureBlending += gestureAnimDelta;
            HandStateRight.GestureBlending += gestureAnimDelta;
            HandStateGaze.GestureBlending += gestureAnimDelta;
        }

        /// Apply changes to one hand and update tracking
        private void SimulateHandInput(
            ref long lastHandTrackedTimestamp,
            SimulatedHandState state,
            bool isSimulating,
            bool isAlwaysVisible,
            MouseDelta mouseDelta,
            bool useMouseRotation)
        {
            bool enableTracking = isAlwaysVisible || isSimulating;
            if (!state.IsTracked && enableTracking)
            {
                ResetHand(state, isSimulating);
            }

            if (isSimulating)
            {
                state.SimulateInput(mouseDelta, useMouseRotation, profile.MouseRotationSensitivity, profile.MouseHandRotationSpeed, profile.HandJitterAmount);

                if (isAlwaysVisible)
                {
                    // Toggle gestures on/off
                    state.Gesture = ToggleGesture(state.Gesture);
                }
                else
                {
                    // Enable gesture while mouse button is pressed
                    state.Gesture = SelectGesture();
                }
            }

            // Update tracked state of a hand.
            // If hideTimeout value is null, hands will stay visible after tracking stops.
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            DateTime currentTime = DateTime.UtcNow;
            if (enableTracking)
            {
                state.IsTracked = true;
                lastHandTrackedTimestamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastHandTrackedTimestamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    state.IsTracked = false;
                }
            }
        }

        public void ResetHand(Handedness handedness)
        {
            if (handedness == Handedness.Left)
            {
                ResetHand(HandStateLeft, isSimulatingLeft);
            }
            else
            {
                ResetHand(HandStateRight, isSimulatingRight);
            }
        }

        private void ResetHand(SimulatedHandState state, bool isSimulating)
        {
            if (isSimulating)
            {
                // Start at current mouse position
                Vector3 mousePos = UnityEngine.Input.mousePosition;
                state.ResetPosition(CameraCache.Main.ScreenToViewportPoint(new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance)));
            }
            else
            {
                state.ResetPosition(new Vector3(0.5f, 0.5f, profile.DefaultHandDistance));
            }

            state.Gesture = profile.DefaultHandGesture;
            state.ResetGesture();
            state.ResetRotation();
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
    }
}
