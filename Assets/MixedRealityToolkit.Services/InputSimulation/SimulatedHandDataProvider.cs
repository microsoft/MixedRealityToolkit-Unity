// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    [Serializable]
    internal class SimulatedHandState
    {
        [SerializeField]
        private Handedness handedness = Handedness.None;
        public Handedness Handedness => handedness;

        // Show a tracked hand device
        public bool IsTracked = false;
        // Activate the pinch gesture
        public bool IsPinching { get; private set; }

        public Vector3 ScreenPosition;
        // Rotation of the hand
        public Vector3 HandRotateEulerAngles = Vector3.zero;
        // Random offset to simulate tracking inaccuracy
        public Vector3 JitterOffset = Vector3.zero;

        [SerializeField]
        private SimulatedHandPose.GestureId gesture = SimulatedHandPose.GestureId.None;
        public SimulatedHandPose.GestureId Gesture
        {
            get { return gesture; }
            set
            {
                if (value != SimulatedHandPose.GestureId.None && value != gesture)
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

                // Pinch is a special gesture that triggers the Select and TriggerPress input actions
                IsPinching = (gesture == SimulatedHandPose.GestureId.Pinch && gestureBlending > 0.9f);
            }
        }

        private float poseBlending = 0.0f;
        private SimulatedHandPose pose = new SimulatedHandPose();

        public SimulatedHandState(Handedness _handedness)
        {
            handedness = _handedness;
        }

        public void Reset()
        {
            ScreenPosition = Vector3.zero;
            HandRotateEulerAngles = Vector3.zero;
            JitterOffset = Vector3.zero;

            ResetGesture();
        }

        /// <summary>
        /// Set the position in viewport space rather than screen space (pixels).
        /// </summary>
        public void SetViewportPosition(Vector3 point)
        {
            ScreenPosition = CameraCache.Main.ViewportToScreenPoint(point);
        }

        public void SimulateInput(Vector3 mouseDelta, float noiseAmount, Vector3 rotationDeltaEulerAngles)
        {
            // Apply mouse delta x/y in screen space, but depth offset in world space
            ScreenPosition.x += mouseDelta.x;
            ScreenPosition.y += mouseDelta.y;
            Vector3 newWorldPoint = CameraCache.Main.ScreenToWorldPoint(ScreenPosition);
            newWorldPoint += CameraCache.Main.transform.forward * mouseDelta.z;
            ScreenPosition = CameraCache.Main.WorldToScreenPoint(newWorldPoint);

            HandRotateEulerAngles += rotationDeltaEulerAngles;

            JitterOffset = UnityEngine.Random.insideUnitSphere * noiseAmount;
        }

        public void ResetGesture()
        {
            gestureBlending = 1.0f;

            SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.Copy(gesturePose);
            }
        }

        internal void FillCurrentFrame(Vector3[] jointsOut)
        {
            SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.TransitionTo(gesturePose, poseBlending, gestureBlending);
            }
            poseBlending = gestureBlending;

            Quaternion rotation = Quaternion.Euler(HandRotateEulerAngles);
            Vector3 position = CameraCache.Main.ScreenToWorldPoint(ScreenPosition + JitterOffset);
            pose.ComputeJointPositions(handedness, rotation, position, jointsOut);
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

        private SimulatedHandState HandStateLeft;
        private SimulatedHandState HandStateRight;

        // If true then hands are controlled by user input
        private bool isSimulatingLeft = false;
        private bool isSimulatingRight = false;
        // Last frame's mouse position for computing delta
        private Vector3? lastMousePosition = null;
        // Last timestamp when hands were tracked
        private long lastSimulatedTimestampLeft = 0;
        private long lastSimulatedTimestampRight = 0;
        // Cached delegates for hand joint generation
        private SimulatedHandData.HandJointDataGenerator generatorLeft;
        private SimulatedHandData.HandJointDataGenerator generatorRight;

        public SimulatedHandDataProvider(MixedRealityInputSimulationProfile _profile)
        {
            profile = _profile;

            HandStateLeft = new SimulatedHandState(Handedness.Left);
            HandStateRight = new SimulatedHandState(Handedness.Right);

            HandStateLeft.Gesture = profile.DefaultHandGesture;
            HandStateRight.Gesture = profile.DefaultHandGesture;

            HandStateLeft.Reset();
            HandStateRight.Reset();
        }

        /// <summary>
        /// Capture a snapshot of simulated hand data based on current state.
        /// </summary>
        public bool UpdateHandData(SimulatedHandData handDataLeft, SimulatedHandData handDataRight)
        {
            SimulateUserInput();

            bool handDataChanged = false;
            // TODO: DateTime.UtcNow can be quite imprecise, better use Stopwatch.GetTimestamp
            // https://stackoverflow.com/questions/2143140/c-sharp-datetime-now-precision
            long timestamp = DateTime.UtcNow.Ticks;

            // Cache the generator delegates so we don't gc alloc every frame
            if (generatorLeft == null)
            {
                generatorLeft = HandStateLeft.FillCurrentFrame;
            }

            if (generatorRight == null)
            {
                generatorRight = HandStateRight.FillCurrentFrame;
            }

            handDataChanged |= handDataLeft.UpdateWithTimestamp(timestamp, HandStateLeft.IsTracked, HandStateLeft.IsPinching, generatorLeft);
            handDataChanged |= handDataRight.UpdateWithTimestamp(timestamp, HandStateRight.IsTracked, HandStateRight.IsPinching, generatorRight);

            return handDataChanged;
        }

        /// <summary>
        /// Update hand state based on keyboard and mouse input
        /// </summary>
        private void SimulateUserInput()
        {
            if (UnityEngine.Input.GetKeyDown(profile.ToggleLeftHandKey))
            {
                IsAlwaysVisibleLeft = !IsAlwaysVisibleLeft;
            }
            if (UnityEngine.Input.GetKeyDown(profile.ToggleRightHandKey))
            {
                IsAlwaysVisibleRight = !IsAlwaysVisibleRight;
            }

            if (UnityEngine.Input.GetKeyDown(profile.LeftHandManipulationKey))
            {
                isSimulatingLeft = true;
            }
            if (UnityEngine.Input.GetKeyUp(profile.LeftHandManipulationKey))
            {
                isSimulatingLeft = false;
            }

            if (UnityEngine.Input.GetKeyDown(profile.RightHandManipulationKey))
            {
                isSimulatingRight = true;
            }
            if (UnityEngine.Input.GetKeyUp(profile.RightHandManipulationKey))
            {
                isSimulatingRight = false;
            }

            Vector3 mouseDelta = (lastMousePosition.HasValue ? UnityEngine.Input.mousePosition - lastMousePosition.Value : Vector3.zero);
            mouseDelta.z += UnityEngine.Input.GetAxis("Mouse ScrollWheel") * profile.HandDepthMultiplier;
            float rotationDelta = profile.HandRotationSpeed * Time.deltaTime;
            Vector3 rotationDeltaEulerAngles = Vector3.zero;
            if (UnityEngine.Input.GetKey(profile.YawHandCCWKey))
            {
                rotationDeltaEulerAngles.y = -rotationDelta;
            }
            if (UnityEngine.Input.GetKey(profile.YawHandCWKey))
            {
                rotationDeltaEulerAngles.y = rotationDelta;
            }
            if (UnityEngine.Input.GetKey(profile.PitchHandCCWKey))
            {
                rotationDeltaEulerAngles.x = rotationDelta;
            }
            if (UnityEngine.Input.GetKey(profile.PitchHandCWKey))
            {
                rotationDeltaEulerAngles.x = -rotationDelta;
            }
            if (UnityEngine.Input.GetKey(profile.RollHandCCWKey))
            {
                rotationDeltaEulerAngles.z = rotationDelta;
            }
            if (UnityEngine.Input.GetKey(profile.RollHandCWKey))
            {
                rotationDeltaEulerAngles.z = -rotationDelta;
            }

            SimulateHandInput(ref lastSimulatedTimestampLeft, HandStateLeft, isSimulatingLeft, IsAlwaysVisibleLeft, mouseDelta, rotationDeltaEulerAngles);
            SimulateHandInput(ref lastSimulatedTimestampRight, HandStateRight, isSimulatingRight, IsAlwaysVisibleRight, mouseDelta, rotationDeltaEulerAngles);

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            HandStateLeft.GestureBlending += gestureAnimDelta;
            HandStateRight.GestureBlending += gestureAnimDelta;

            lastMousePosition = UnityEngine.Input.mousePosition;
        }

        /// Apply changes to one hand and update tracking
        private void SimulateHandInput(
            ref long lastSimulatedTimestamp,
            SimulatedHandState state,
            bool isSimulating,
            bool isAlwaysVisible,
            Vector3 mouseDelta,
            Vector3 rotationDeltaEulerAngles)
        {
            if (!state.IsTracked && isSimulating)
            {
                // Start at current mouse position
                Vector3 mousePos = UnityEngine.Input.mousePosition;
                state.ScreenPosition = new Vector3(mousePos.x, mousePos.y, profile.DefaultHandDistance);
            }

            if (isSimulating)
            {
                state.SimulateInput(mouseDelta, profile.HandJitterAmount, rotationDeltaEulerAngles);

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
            if (isAlwaysVisible || isSimulating)
            {
                state.IsTracked = true;
                lastSimulatedTimestamp = currentTime.Ticks;
            }
            else
            {
                float timeSinceTracking = (float)currentTime.Subtract(new DateTime(lastSimulatedTimestamp)).TotalSeconds;
                if (timeSinceTracking > profile.HandHideTimeout)
                {
                    state.IsTracked = false;
                }
            }
        }

        private SimulatedHandPose.GestureId SelectGesture()
        {
            if (UnityEngine.Input.GetMouseButton(0))
            {
                return profile.LeftMouseHandGesture;
            }
            else if (UnityEngine.Input.GetMouseButton(1))
            {
                return profile.RightMouseHandGesture;
            }
            else if (UnityEngine.Input.GetMouseButton(2))
            {
                return profile.MiddleMouseHandGesture;
            }
            else
            {
                return profile.DefaultHandGesture;
            }
        }

        private SimulatedHandPose.GestureId ToggleGesture(SimulatedHandPose.GestureId gesture)
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                return (gesture != profile.LeftMouseHandGesture ? profile.LeftMouseHandGesture : profile.DefaultHandGesture);
            }
            else if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                return (gesture != profile.RightMouseHandGesture ? profile.RightMouseHandGesture : profile.DefaultHandGesture);
            }
            else if (UnityEngine.Input.GetMouseButtonDown(2))
            {
                return (gesture != profile.MiddleMouseHandGesture ? profile.MiddleMouseHandGesture : profile.DefaultHandGesture);
            }
            else
            {
                // 'None' will not change the gesture
                return SimulatedHandPose.GestureId.None;
            }
        }
    }
}
