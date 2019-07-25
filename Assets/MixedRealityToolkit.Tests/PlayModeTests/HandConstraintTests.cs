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

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class HandConstraintTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            if (handConstraint != null)
            {
                GameObject.Destroy(handConstraint.gameObject);
            }
            PlayModeTestUtilities.TearDown();
        }

        private HandConstraint handConstraint;

        /// <summary>
        /// Creates a game object with hand menu from code
        /// </summary>
        private void InstantiateHandConstraintFromCode()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.01f);
            cube.transform.position = Vector3.zero;
            cube.AddComponent<SolverHandler>();
            handConstraint = cube.AddComponent<HandConstraint>();
        }

        /// <summary>
        /// Ensure hand menu follows hand when you move the hand.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator HandConstraintFollowHand()
        {
            InstantiateHandConstraintFromCode();

            TestUtilities.AssertAboutEqual(handConstraint.transform.position, Vector3.zero, "hand constraint not starting at zero");

            TestHand h = new TestHand(Handedness.Right);
            yield return h.Show(Vector3.zero);

            // Move the hand to 0, 0, 1 and ensure that hte hand constraint matches position
            yield return h.MoveTo(Vector3.forward, 60);
            // Tip: Wait for enter key before continuing, to make sure things look okay.
            // Remove this for final test
            yield return PlayModeTestUtilities.WaitForEnterKey();

            TestUtilities.AssertAboutEqual(handConstraint.transform.position, Vector3.forward, "menu did not move to (0, 0, 1)");

        }
    }
        
}
#endif