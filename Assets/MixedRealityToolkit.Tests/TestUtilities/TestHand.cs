// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    ///  Utility class to use a simulated hand
    /// </summary>
    public class TestHand
    {
        private Handedness handedness;
        private Vector3 position;
        private Quaternion rotation = Quaternion.identity;
        private ArticulatedHandPose.GestureId gestureId = ArticulatedHandPose.GestureId.Open;
        private InputSimulationService simulationService;

        public TestHand(Handedness handedness)
        {
            this.handedness = handedness;
            simulationService = PlayModeTestUtilities.GetInputSimulationService();
        }

        /// <summary>
        /// Returns the velocity of the simulated hand
        /// </summary>
        public Vector3 GetVelocity()
        {
            var hand = simulationService.GetHandDevice(handedness);
            return hand.Velocity;
        }

        /// <summary>
        /// Show the hand at a specified position
        /// </summary>
        /// <param name="position">Where to show the hand</param>
        /// <param name="waitForFixedUpdate">If true, will wait for a physics frame after showing the hand.</param>
        public IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true)
        {
            this.position = position;
            yield return PlayModeTestUtilities.ShowHand(handedness, simulationService, gestureId, position);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Hide the hand
        /// </summary>
        /// <param name="waitForFixedUpdate">If true, will wait a physics frame after hiding</param>
        public IEnumerator Hide(bool waitForFixedUpdate = true)
        {
            yield return PlayModeTestUtilities.HideHand(handedness, simulationService);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Moves hand to given position over some number of frames.
        /// </summary>
        /// <param name="newPosition">Where to move hand to</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.HandMoveStepsSentinelValue"/>
        /// </param>
        /// <param name="waitForFixedUpdate">If true, waits a physics frame after moving the hand</param>
        public IEnumerator MoveTo(Vector3 newPosition, int numSteps = PlayModeTestUtilities.HandMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            Vector3 oldPosition = position;
            position = newPosition;
            for (var iter = PlayModeTestUtilities.MoveHand(oldPosition, newPosition, gestureId, handedness, simulationService, numSteps); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Move the hand by some given delta.
        /// </summary>
        /// <param name="delta">Amount to move the hand by.</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.HandMoveStepsSentinelValue"/>
        /// </param>
        public IEnumerator Move(Vector3 delta, int numSteps = PlayModeTestUtilities.HandMoveStepsSentinelValue)
        {
            for (var iter = MoveTo(position + delta, PlayModeTestUtilities.CalculateNumSteps(numSteps)); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Rotates the hand to new rotation.
        /// </summary>
        /// <param name="newRotation">New rotation of hand</param>
        /// <param name="numSteps">Number of frames to rotate over.</param>
        public IEnumerator SetRotation(
            Quaternion newRotation,
            int numSteps = PlayModeTestUtilities.HandMoveStepsSentinelValue)
        {
            Quaternion oldRotation = rotation;
            rotation = newRotation;
            yield return PlayModeTestUtilities.SetHandRotation(
                oldRotation,
                newRotation,
                position,
                gestureId,
                handedness,
                PlayModeTestUtilities.CalculateNumSteps(numSteps),
                simulationService);
        }

        /// <summary>
        /// Changes the hand's pose to the given gesture.  Does not animate the hand between the current pose and new pose.
        /// </summary>
        /// <param name="newGestureId">The new hand pose</param>
        /// <param name="waitForFixedUpdate">If true, waits for a fixed update after moving to the new pose.</param>
        public IEnumerator SetGesture(ArticulatedHandPose.GestureId newGestureId, bool waitForFixedUpdate = true)
        {
            gestureId = newGestureId;
            for (var iter = PlayModeTestUtilities.MoveHand(position, position, gestureId, handedness, simulationService, 1); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Combined sequence of pinching and unpinching
        /// </summary>
        public IEnumerator Click()
        {
            yield return SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;
            yield return SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return null;
        }

        /// <summary>
        /// Combined sequence of pinching, moving, and releasing.
        /// </summary>
        /// <param name="positionToRelease">The position to which the hand moves while pinching</param>
        /// <param name="waitForFinalFixedUpdate">Wait for a final physics update after releasing</param>
        /// <param name="numSteps">Number of steps of the hand movement</param>
        public IEnumerator GrabAndThrowAt(Vector3 positionToRelease, bool waitForFinalFixedUpdate, int numSteps = 30)
        {
            for (var iter = SetGesture(ArticulatedHandPose.GestureId.Pinch); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            for (var iter = MoveTo(positionToRelease, numSteps); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            for (var iter = SetGesture(ArticulatedHandPose.GestureId.Open, waitForFinalFixedUpdate); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
        }

        /// <summary>
        /// Returns the first pointer of given type that is associated with this hand.
        /// </summary>
        /// <typeparam name="T">Type of pointer to look for.</typeparam>
        public T GetPointer<T>() where T : class, IMixedRealityPointer
        {
            var hand = simulationService.GetHandDevice(handedness);
            foreach (var pointer in hand.InputSource.Pointers)
            {
                if (pointer is T)
                {
                    return pointer as T;
                }
            }
            return null;
        }
    }
}
#endif