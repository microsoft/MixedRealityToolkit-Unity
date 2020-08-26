// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// Utility class to use a simulated motion controller
    /// </summary>
    public class TestMotionController : TestController
    {
        private SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState();

        public TestMotionController(Handedness handedness) : base(handedness) { }

        /// <inheritdoc />
        public override IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true)
        {
            this.position = position;
            yield return PlayModeTestUtilities.ShowMontionController(handedness, simulationService, new SimulatedMotionControllerButtonState(), position);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator Hide(bool waitForFixedUpdate = true)
        {
            yield return PlayModeTestUtilities.HideController(handedness, simulationService);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator MoveTo(Vector3 newPosition, int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true)
        {
            Vector3 oldPosition = position;
            position = newPosition;
            for (var iter = PlayModeTestUtilities.MoveMotionController(oldPosition, newPosition, buttonState, handedness, simulationService, numSteps); iter.MoveNext();)
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <inheritdoc />
        public override IEnumerator Move(Vector3 delta, int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue)
        {
            for (var iter = MoveTo(position + delta, PlayModeTestUtilities.CalculateNumSteps(numSteps)); iter.MoveNext();)
            {
                yield return iter.Current;
            }
        }

        /// <inheritdoc />
        public override IEnumerator SetRotation(
            Quaternion newRotation,
            int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue)
        {
            Quaternion oldRotation = rotation;
            rotation = newRotation;
            yield return PlayModeTestUtilities.SetMotionControllerRotation(
                oldRotation,
                newRotation,
                position,
                buttonState,
                handedness,
                PlayModeTestUtilities.CalculateNumSteps(numSteps),
                simulationService);
        }

        /// <summary>
        /// Changes the state of the simulated motion controller.
        /// </summary>
        /// <param name="isSelecting">Whether the motion controller should be selecting something.</param>
        /// <param name="isGrabbing">Whether the motion controller should be grabbing something.</param>
        /// <param name="isPressingMenu">Whether the menu button of the motion controller should be pressed down.</param>
        /// <param name="waitForFixedUpdate">If true, waits for a fixed update after moving to the new state.</param>
        public IEnumerator SetState(SimulatedMotionControllerButtonState buttonStateNew, bool waitForFixedUpdate = true)
        {
            buttonState = buttonStateNew;
            for (var iter = PlayModeTestUtilities.MoveMotionController(position, position, buttonState, handedness, simulationService, 1); iter.MoveNext();)
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Combined sequence of selecting and unselecting
        /// </summary>
        public override IEnumerator Click()
        {
            SimulatedMotionControllerButtonState selectButtonState = new SimulatedMotionControllerButtonState
            {
                IsSelecting = true
            };
            yield return SetState(selectButtonState);
            yield return null;
            SimulatedMotionControllerButtonState defaultButtonState = new SimulatedMotionControllerButtonState();
            yield return SetState(defaultButtonState);
            yield return null;
        }

        /// <summary>
        /// Combined sequence of selecting, moving, and releasing.
        /// </summary>
        /// <param name="positionToRelease">The position to which the hand moves while pinching</param>
        /// <param name="waitForFinalFixedUpdate">Wait for a final physics update after releasing</param>
        /// <param name="numSteps">Number of steps of the hand movement</param>
        public IEnumerator SelectAndThrowAt(Vector3 positionToRelease, bool waitForFinalFixedUpdate, int numSteps = 30)
        {
            SimulatedMotionControllerButtonState selectButtonState = new SimulatedMotionControllerButtonState
            {
                IsSelecting = true
            };
            for (var iter = SetState(selectButtonState); iter.MoveNext();)
            {
                yield return iter.Current;
            }
            for (var iter = MoveTo(positionToRelease, numSteps); iter.MoveNext();)
            {
                yield return iter.Current;
            }
            SimulatedMotionControllerButtonState defaultButtonState = new SimulatedMotionControllerButtonState();
            for (var iter = SetState(defaultButtonState, waitForFinalFixedUpdate); iter.MoveNext();)
            {
                yield return iter.Current;
            }
        }
    }
}
#endif
