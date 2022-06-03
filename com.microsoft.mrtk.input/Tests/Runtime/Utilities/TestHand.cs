// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;

using GestureId = Microsoft.MixedReality.Toolkit.Input.GestureTypes.GestureId;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Convenience wrapper around the underlying <see cref="RuntimeInputTestUtilities"> functions
    /// for manipulating test hands.
    /// </summary>
    public class TestHand : TestController
    {
        private GestureId gestureId = GestureId.Open;

        public TestHand(Handedness handedness) : base(handedness) { }

        /// <inheritdoc />
        public override IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true)
        {
            yield return InputTestUtilities.SetHandTrackingState(handedness, true);
            yield return MoveTo(position, 1);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator Show(bool waitForFixedUpdate = true)
        {
            yield return InputTestUtilities.SetHandTrackingState(handedness, true);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator Hide(bool waitForFixedUpdate = true)
        {
            yield return InputTestUtilities.SetHandTrackingState(handedness, false);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator MoveTo(Vector3 newPosition, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            yield return InputTestUtilities.MoveHandTo(newPosition, gestureId, handedness, numSteps);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator Move(Vector3 delta, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            Vector3 currentPosition = InputTestUtilities.GetHandPose(handedness).position;
            yield return MoveTo(currentPosition + delta, InputTestUtilities.CalculateNumSteps(numSteps));
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator RotateTo(Quaternion newRotation, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            yield return InputTestUtilities.RotateHandTo(newRotation, gestureId, handedness, numSteps);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Changes the hand's pose to the given gesture.  Does not animate the hand between the current pose and new pose.
        /// </summary>
        /// <param name="newGestureId">The new hand pose</param>
        /// <param name="waitForFixedUpdate">If true, waits for a fixed update after moving to the new pose.</param>
        public IEnumerator SetGesture(GestureId newGestureId, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            gestureId = newGestureId;
            yield return InputTestUtilities.SetHandGesture(gestureId, handedness, numSteps);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Combined sequence of pinching and unpinching
        /// </summary>
        public override IEnumerator Click()
        {
            yield return SetGesture(GestureId.Pinch);
            yield return null;
            yield return SetGesture(GestureId.Open);
            yield return null;
        }
    }
}
