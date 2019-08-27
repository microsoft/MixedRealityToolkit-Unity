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

using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    public class GazeProviderTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Verifies that when there is no gaze cursor visibility override
        /// set, that the cursor visibility rules follow the typical state
        /// machine transitions.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNullGazeCursorVisibilityOverride()
        {
            // The gaze cursor should be initially visible
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Showing the hand should cause the gaze cursor to be not visible.
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);

            // Verify that since the hand is shown, the gaze cursor is not visible.
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);
        }

        /// <summary>
        /// Verifies that when the gaze cursor visibility override is set to false,
        /// the gaze cursor is hidden (regardless of the current state) and that
        /// setting it back to null will undo the hidden transition.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFalseGazeCursorVisibilityOverride()
        {
            // The gaze cursor should be initially visible
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Set the override to false, and then wait a frame in order for the
            // internal state machine to reconcile this property change.
            CoreServices.InputSystem.GazeProvider.GazeCursorVisibilityOverride = false;
            yield return null;
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Clear the override and validate that the cursor shows up again.
            CoreServices.InputSystem.GazeProvider.GazeCursorVisibilityOverride = null;
            yield return null;
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);
        }

        /// <summary>
        /// Verifies that when the gaze cursor visibility override is set to true,
        /// the gaze cursor is visible (regardless of the current state) and that
        /// setting it back to null will undo the visible transition.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTrueGazeCursorVisibilityOverride()
        {
            // The gaze cursor should be initially visible
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Showing the hand should cause the gaze cursor to be not visible.
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);

            // Verify that since the hand is shown, the gaze cursor is not visible.
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Set the override to true, and then wait a frame in order for the
            // internal state machine to reconcile this property change.
            CoreServices.InputSystem.GazeProvider.GazeCursorVisibilityOverride = false;
            yield return null;
            Assert.IsTrue(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);

            // Clear the override - since the cursor would have been invisible without
            // the override, it should now be hidden.
            CoreServices.InputSystem.GazeProvider.GazeCursorVisibilityOverride = null;
            yield return null;
            Assert.IsFalse(CoreServices.InputSystem.GazeProvider.GazeCursor.IsVisible);
        }
    }
}
#endif