// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Convenience wrapper around the underlying <see cref="RuntimeInputTestUtilities"> functions
    /// for manipulating test hands/controllers.
    /// </summary>
    public abstract class TestController
    {
        protected Handedness handedness;

        public TestController(Handedness handedness)
        {
            this.handedness = handedness;
        }

        /// <summary>
        /// Show the controller at a specified position
        /// </summary>
        /// <param name="position">Where to show the controller</param>
        /// <param name="waitForFixedUpdate">If true, will wait for a physics frame after showing the controller.</param>
        public abstract IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true);

        /// <summary>
        /// Show the controller without moving it.
        /// </summary>
        /// <param name="waitForFixedUpdate">If true, will wait for a physics frame after showing the controller.</param>
        public abstract IEnumerator Show(bool waitForFixedUpdate = true);

        /// <summary>
        /// Hide the controller
        /// </summary>
        /// <param name="waitForFixedUpdate">If true, will wait a physics frame after hiding</param>
        public abstract IEnumerator Hide(bool waitForFixedUpdate = true);

        /// <summary>
        /// Move controller to given position over some number of frames
        /// </summary>
        /// <param name="newPosition">Where to move controller to</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.ControllerMoveStepsSentinelValue"/>
        /// </param>
        /// <param name="waitForFixedUpdate">If true, waits a physics frame after moving the controller</param>
        public abstract IEnumerator MoveTo(Vector3 newPosition, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true);

        /// <summary>
        /// Move the controller by some given delta
        /// </summary>
        /// <param name="delta">Amount to move the controller by.</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.ControllerMoveStepsSentinelValue"/>
        /// </param>
        public abstract IEnumerator Move(Vector3 delta, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true);

        /// <summary>
        /// Rotates the controller to given position over some number of frames
        /// </summary>
        /// <param name="newRotation">New rotation of controller<</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.ControllerMoveStepsSentinelValue"/>
        /// </param>
        /// <param name="waitForFixedUpdate">If true, waits a physics frame after moving the controller</param>
        public abstract IEnumerator RotateTo(Quaternion newRotation, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true);

        /// <summary>
        /// Rotates the controller to aim at the given world-relative position over some number of frames.
        /// This forces the controller's anchor point to be ControllerAnchorPoint.Device.
        /// </summary>
        /// <param name="target">Point in worldspace to aim at (i.e., rotate the device's pose to aim at)</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.ControllerMoveStepsSentinelValue"/>
        /// </param>
        /// <param name="waitForFixedUpdate">If true, waits a physics frame after moving the controller</param>
        public abstract IEnumerator AimAt(Vector3 target, int numSteps = InputTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true);

        /// <summary>
        /// Perform a sequence of actions that represent a click for the controller
        /// </summary>
        public abstract IEnumerator Click();
    }
}