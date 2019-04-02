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
    public class SimulatedHandData
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        // Timestamp of hand data, as FileTime, e.g. DateTime.UtcNow.ToFileTime() 
        public long Timestamp;
        public bool IsTracked;
        public Vector3[] Joints = new Vector3[jointCount];
        public bool IsPinching;
    }

    internal class SimulatedHandState
    {
        private Handedness handedness = Handedness.None;
        public Handedness Handedness => handedness;

        // Show a tracked hand device
        public bool IsVisible = false;
        // Hand is simulated
        public bool IsSimulated = false;
        // Device is always tracked, regardless if simulating
        public bool IsAlwaysTracked = false;
        // Activate the pinch gesture
        public bool IsPinching { get; private set; }

        private Vector3 screenPosition;
        // Rotation of the hand
        private Vector3 handRotateEulerAngles = Vector3.zero;
        // Random offset to simulate tracking inaccuracy
        private Vector3 jitterOffset = Vector3.zero;
        // Remaining time until the hand is hidden
        private float timeUntilHide = 0.0f;

        // Interpolation between current pose and target gesture
        private float lastGestureAnim = 0.0f;
        private float currentGestureAnim = 0.0f;
        private SimulatedHandPose.GestureId gesture = SimulatedHandPose.GestureId.None;
        public SimulatedHandPose.GestureId Gesture => gesture;

        private SimulatedHandPose pose = new SimulatedHandPose();

        public SimulatedHandState(Handedness _handedness)
        {
            handedness = _handedness;
        }

        public void Reset(float defaultHandDistance, SimulatedHandPose.GestureId defaultGesture)
        {
            // Start at current mouse position
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            screenPosition = new Vector3(mousePos.x, mousePos.y, defaultHandDistance);

            gesture = defaultGesture;
            lastGestureAnim = 1.0f;
            currentGestureAnim = 1.0f;

            SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.Copy(gesturePose);
            }

            handRotateEulerAngles = Vector3.zero;
            jitterOffset = Vector3.zero;
        }

        // Update hand state
        // If hideTimeout value is null, hands will stay visible after tracking stops
        public void UpdateVisibility(float hideTimeout)
        {
            if (IsAlwaysTracked)
            {
                IsVisible = true;
            }
            else
            {
                timeUntilHide = IsSimulated ? hideTimeout : timeUntilHide - Time.deltaTime;
                IsVisible = (timeUntilHide > 0.0f);
            }
        }

        public void SimulateInput(Vector3 mouseDelta, float noiseAmount, Vector3 rotationDeltaEulerAngles)
        {
            if (!IsSimulated)
            {
                return;
            }

            // Apply mouse delta x/y in screen space, but depth offset in world space
            screenPosition.x += mouseDelta.x;
            screenPosition.y += mouseDelta.y;
            Vector3 newWorldPoint = CameraCache.Main.ScreenToWorldPoint(screenPosition);
            newWorldPoint += CameraCache.Main.transform.forward * mouseDelta.z;
            screenPosition = CameraCache.Main.WorldToScreenPoint(newWorldPoint);

            handRotateEulerAngles += rotationDeltaEulerAngles;

            jitterOffset = UnityEngine.Random.insideUnitSphere * noiseAmount;
        }

        public void AnimateGesture(SimulatedHandPose.GestureId newGesture, float gestureAnimDelta)
        {
            if (!IsSimulated)
            {
                return;
            }

            if (newGesture != SimulatedHandPose.GestureId.None && newGesture != gesture)
            {
                gesture = newGesture;
                lastGestureAnim = 0.0f;
                currentGestureAnim = Mathf.Clamp01(gestureAnimDelta);
            }
            else
            {
                lastGestureAnim = currentGestureAnim;
                currentGestureAnim = Mathf.Clamp01(currentGestureAnim + gestureAnimDelta);
            }

            SimulatedHandPose gesturePose = SimulatedHandPose.GetGesturePose(gesture);
            if (gesturePose != null)
            {
                pose.TransitionTo(gesturePose, lastGestureAnim, currentGestureAnim);
            }

            // Pinch is a special gesture that triggers the Select and TriggerPress input actions
            IsPinching = (gesture == SimulatedHandPose.GestureId.Pinch && currentGestureAnim > 0.9f);
        }

        internal void FillCurrentFrame(Vector3[] jointsOut)
        {
            Quaternion rotation = Quaternion.Euler(handRotateEulerAngles);
            Vector3 position = CameraCache.Main.ScreenToWorldPoint(screenPosition + jitterOffset);
            pose.ComputeJointPositions(handedness, rotation, position, jointsOut);
        }
    }

    public class SimulatedHandDataProvider
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// This event is raised whenever the hand data changes.
        /// Hand data changes at 45 fps.
        /// </summary>
        public event Action OnHandDataChanged = delegate { };

        public SimulatedHandData CurrentFrameLeft = new SimulatedHandData();
        public SimulatedHandData CurrentFrameRight = new SimulatedHandData();

        private MixedRealityInputSimulationProfile profile;

        private SimulatedHandState stateLeft;
        private SimulatedHandState stateRight;
        // Last frame's mouse position for computing delta
        private Vector3? lastMousePosition = null;

        public SimulatedHandDataProvider(MixedRealityInputSimulationProfile _profile)
        {
            profile = _profile;

            stateLeft = new SimulatedHandState(Handedness.Left);
            stateRight = new SimulatedHandState(Handedness.Right);
        }

        public void Update()
        {
            bool wasLeftVisible = stateLeft.IsVisible;
            bool wasRightVisible = stateRight.IsVisible;

            if (UnityEngine.Input.GetKeyDown(profile.ToggleLeftHandKey))
            {
                stateLeft.IsAlwaysTracked = !stateLeft.IsAlwaysTracked;
            }
            if (UnityEngine.Input.GetKeyDown(profile.ToggleRightHandKey))
            {
                stateRight.IsAlwaysTracked = !stateRight.IsAlwaysTracked;
            }

            if (UnityEngine.Input.GetKeyDown(profile.LeftHandManipulationKey))
            {
                stateLeft.IsSimulated = true;
            }
            if (UnityEngine.Input.GetKeyUp(profile.LeftHandManipulationKey))
            {
                stateLeft.IsSimulated = false;
            }

            if (UnityEngine.Input.GetKeyDown(profile.RightHandManipulationKey))
            {
                stateRight.IsSimulated = true;
            }
            if (UnityEngine.Input.GetKeyUp(profile.RightHandManipulationKey))
            {
                stateRight.IsSimulated = false;
            }

            // Hide cursor if either of the hands is simulated
            Cursor.visible = !stateLeft.IsSimulated && !stateRight.IsSimulated;

            stateLeft.UpdateVisibility(profile.HandHideTimeout);
            stateRight.UpdateVisibility(profile.HandHideTimeout);
            // Reset when enabling
            if (!wasLeftVisible && stateLeft.IsVisible)
            {
                stateLeft.Reset(profile.DefaultHandDistance, profile.DefaultHandGesture);
            }
            if (!wasRightVisible && stateRight.IsVisible)
            {
                stateRight.Reset(profile.DefaultHandDistance, profile.DefaultHandGesture);
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
            stateLeft.SimulateInput(mouseDelta, profile.HandJitterAmount, rotationDeltaEulerAngles);
            stateRight.SimulateInput(mouseDelta, profile.HandJitterAmount, rotationDeltaEulerAngles);

            float gestureAnimDelta = profile.HandGestureAnimationSpeed * Time.deltaTime;
            AnimateGesture(stateLeft, gestureAnimDelta);
            AnimateGesture(stateRight, gestureAnimDelta);

            lastMousePosition = UnityEngine.Input.mousePosition;

            ApplyHandData();
        }

        private void ApplyHandData()
        {
            bool handDataChanged = false;
            handDataChanged |= UpdateHandDataFromState(CurrentFrameLeft, stateLeft);
            handDataChanged |= UpdateHandDataFromState(CurrentFrameRight, stateRight);

            if (handDataChanged)
            {
                OnHandDataChanged();
            }
        }

        private void AnimateGesture(SimulatedHandState state, float gestureAnimDelta)
        {
            if (state.IsAlwaysTracked)
            {
                // Toggle gestures on/off
                state.AnimateGesture(ToggleGesture(state.Gesture), gestureAnimDelta);
            }
            else
            {
                // Enable gesture while mouse button is pressed
                state.AnimateGesture(SelectGesture(), gestureAnimDelta);
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

        private bool UpdateHandDataFromState(SimulatedHandData frame, SimulatedHandState state)
        {
            bool handDataChanged = false;
            bool wasTracked = frame.IsTracked;
            bool wasPinching = frame.IsPinching;

            frame.IsTracked = state.IsVisible;
            frame.IsPinching = state.IsPinching;
            if (wasTracked != frame.IsTracked || wasPinching != frame.IsPinching)
            {
                handDataChanged = true;
            }

            if (frame.IsTracked)
            {
                var prevTime = frame.Timestamp;
                frame.Timestamp = DateTime.UtcNow.Ticks;
                if (frame.Timestamp != prevTime)
                {
                    state.FillCurrentFrame(frame.Joints);
                    handDataChanged = true;
                }
            }
            else
            {
                // If frame is not tracked, set timestamp to zero
                frame.Timestamp = 0;
            }

            return handDataChanged;
        }
    }
}