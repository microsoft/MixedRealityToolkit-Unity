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
    ///  Utility class to use a simulated controller
    /// </summary>
    public abstract class TestController
    {
        protected Handedness handedness;
        protected Vector3 position;
        protected Quaternion rotation = Quaternion.identity;
        protected InputSimulationService simulationService;

        public TestController(Handedness handedness)
        {
            this.handedness = handedness;
            simulationService = PlayModeTestUtilities.GetInputSimulationService();
        }

        /// <summary>
        /// Return the velocity of the simulated controller
        /// </summary>
        public Vector3 GetVelocity()
        {
            var controller = simulationService.GetControllerDevice(handedness);
            return controller.Velocity;
        }

        /// <summary>
        /// Show the controller at a specified position
        /// </summary>
        /// <param name="position">Where to show the controller</param>
        /// <param name="waitForFixedUpdate">If true, will wait for a physics frame after showing the controller.</param>
        public abstract IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true);

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
        public abstract IEnumerator MoveTo(Vector3 newPosition, int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue, bool waitForFixedUpdate = true);

        /// <summary>
        /// Move the controller by some given delta
        /// </summary>
        /// <param name="delta">Amount to move the controller by.</param>
        /// <param name="numSteps">
        /// How many frames to move over. This defaults to the "sentinel" value which tells the system
        /// to use the default number of steps. For more information on this value, see
        /// <see cref="PlayModeTestUtilities.ControllerMoveStepsSentinelValue"/>
        /// </param>
        public abstract IEnumerator Move(Vector3 delta, int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue);

        /// <summary>
        /// Rotate the controller to new rotation
        /// </summary>
        /// <param name="newRotation">New rotation of controller</param>
        /// <param name="numSteps">Number of frames to rotate over.</param>
        public abstract IEnumerator SetRotation(
            Quaternion newRotation,
            int numSteps = PlayModeTestUtilities.ControllerMoveStepsSentinelValue);

        /// <summary>
        /// Perform a sequence of actions that represent a click for the controller
        /// </summary>
        public abstract IEnumerator Click();

        /// <summary>
        /// Return the first pointer of given type that is associated with this controller
        /// </summary>
        /// <typeparam name="T">Type of pointer to look for.</typeparam>
        public T GetPointer<T>() where T : class, IMixedRealityPointer
        {
            var controller = simulationService.GetControllerDevice(handedness);
            foreach (var pointer in controller.InputSource.Pointers)
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