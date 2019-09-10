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
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Verify that pointers can be turned on and offvia FocusProvider.SetPointerBehavior
    /// </summary>
    public class PointerOnOffTests : BasePlayModeTests
    {
        private class PointerStateContainer
        {
            public bool? LinePointerEnabled { get; set; }
            public bool? SpherePointerEnabled { get; set; }
            public bool? PokePointerEnabled { get; set; }
            public bool? GazePointerEnabled { get; set; }
            public bool? GGVPointerEnabled { get; set; }
        }

        private void EnsurePointerStates(Handedness h, PointerStateContainer c)
        {
            Action<IMixedRealityPointer, string, bool?> helper = (ptr, name, expected) =>
            {
                if (!expected.HasValue)
                {
                    Assert.Null(ptr, $"Expected {h} {name} to be null but it was not null");
                }
                else
                {
                    Assert.NotNull(ptr, $"Expected {name} to not be null, but it was null");
                    Assert.AreEqual(expected.Value, ptr.IsInteractionEnabled,
                    $"Incorrect state for {h} {name}.IsInteractionEnabled");
                }

            };
            helper(PointerUtils.GetPointer<LinePointer>(h), "Line Pointer", c.LinePointerEnabled);
            helper(PointerUtils.GetPointer<SpherePointer>(h), "Sphere Pointer", c.SpherePointerEnabled);
            helper(PointerUtils.GetPointer<PokePointer>(h), "Poke Pointer", c.PokePointerEnabled);
            helper(PointerUtils.GetPointer<GGVPointer>(h), "GGV Pointer", c.GGVPointerEnabled);
            helper(CoreServices.InputSystem.GazeProvider.GazePointer, "Gaze Pointer", c.GazePointerEnabled);
        }

        /// <summary>
        /// Tests that the gaze pointer can be turned on and off
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGaze()
        {
            PointerStateContainer gazeOn = new PointerStateContainer()
            {
                GazePointerEnabled = true,
                GGVPointerEnabled = true,
                PokePointerEnabled = null,
                SpherePointerEnabled = null,
                LinePointerEnabled = null
            };

            // set input simulation mode to GGV
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(Vector3.zero);
            yield return leftHand.Show(Vector3.zero);

            TestContext.Out.WriteLine("Show both hands");
            EnsurePointerStates(Handedness.Right, gazeOn);
            EnsurePointerStates(Handedness.Left, gazeOn);

            TestContext.Out.WriteLine("Turn off gaze cursor");
            PointerUtils.SetGazePointerBehavior(PointerBehavior.Off);

            yield return null;

            PointerStateContainer gazeOff = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = false,
                PokePointerEnabled = null,
                SpherePointerEnabled = null,
                LinePointerEnabled = null
            };
            EnsurePointerStates(Handedness.Right, gazeOff);
            EnsurePointerStates(Handedness.Left, gazeOff);
        }

        /// <summary>
        /// Tests that poke pointer can be turned on/off
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestPoke()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionTouchableVolume>();
            cube.transform.position = Vector3.forward * 0.5f;

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            TestContext.Out.WriteLine("Show both hands near touchable cube");
            yield return rightHand.Show(Vector3.zero);
            yield return leftHand.Show(Vector3.zero);
            yield return new WaitForFixedUpdate();

            PointerStateContainer touchOn = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = true,
                SpherePointerEnabled = false,
                LinePointerEnabled = false
            };

            EnsurePointerStates(Handedness.Right, touchOn);
            EnsurePointerStates(Handedness.Left, touchOn);

            TestContext.Out.WriteLine("Turn off poke pointer right hand");
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Off, Handedness.Right);
            yield return null;

            PointerStateContainer touchOff = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = false,
                SpherePointerEnabled = false,
                LinePointerEnabled = true
            };

            EnsurePointerStates(Handedness.Right, touchOff);
            EnsurePointerStates(Handedness.Left, touchOn);

            TestContext.Out.WriteLine("Turn off poke pointer both hands");
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Off);
            yield return null;

            EnsurePointerStates(Handedness.Right, touchOff);
            EnsurePointerStates(Handedness.Left, touchOff);
        }

        /// <summary>
        /// Tests the grab pointer can be turned on/off
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestGrab()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.transform.position = Vector3.zero;

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            TestContext.Out.WriteLine("Show both hands near grabbable cube");
            yield return rightHand.Show(Vector3.zero);
            yield return leftHand.Show(Vector3.zero);
            yield return new WaitForFixedUpdate();

            PointerStateContainer grabOn = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = false,
                SpherePointerEnabled = true,
                LinePointerEnabled = false
            };

            EnsurePointerStates(Handedness.Right, grabOn);
            EnsurePointerStates(Handedness.Left, grabOn);

            TestContext.Out.WriteLine("Turn off grab pointer right hand");
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off, Handedness.Right);
            yield return null;

            PointerStateContainer grabOff = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = false,
                SpherePointerEnabled = false,
                LinePointerEnabled = true
            };

            EnsurePointerStates(Handedness.Right, grabOff);
            EnsurePointerStates(Handedness.Left, grabOn);

            TestContext.Out.WriteLine("Turn off grab pointer both hands");
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off);
            yield return null;

            EnsurePointerStates(Handedness.Right, grabOff);
            EnsurePointerStates(Handedness.Left, grabOff);
        }

        /// <summary>
        /// Tests that rays can be turned on and off
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestRays()
        {
            PointerStateContainer lineOn = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = false,
                SpherePointerEnabled = false,
                LinePointerEnabled = true
            };

            TestHand rightHand = new TestHand(Handedness.Right);
            TestHand leftHand = new TestHand(Handedness.Left);

            yield return rightHand.Show(Vector3.zero);
            yield return leftHand.Show(Vector3.zero);

            TestContext.Out.WriteLine("Show both hands");
            EnsurePointerStates(Handedness.Right, lineOn);
            EnsurePointerStates(Handedness.Left, lineOn);

            TestContext.Out.WriteLine("Turn off ray pointer both hands");
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Off);
            yield return null;

            PointerStateContainer lineOff = new PointerStateContainer()
            {
                GazePointerEnabled = false,
                GGVPointerEnabled = null,
                PokePointerEnabled = false,
                SpherePointerEnabled = false,
                LinePointerEnabled = false
            };

            EnsurePointerStates(Handedness.Right, lineOff);
            EnsurePointerStates(Handedness.Left, lineOff);

            TestContext.Out.WriteLine("Turn on ray right hand.");
            PointerUtils.SetRayPointerBehavior(PointerBehavior.On, Handedness.Right);
            yield return null;

            EnsurePointerStates(Handedness.Right, lineOn);
            EnsurePointerStates(Handedness.Left, lineOff);

            TestContext.Out.WriteLine("Turn on ray (default behavior) right hand.");
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Right);
            yield return null;
            EnsurePointerStates(Handedness.Right, lineOn);
            EnsurePointerStates(Handedness.Left, lineOff);

            TestContext.Out.WriteLine("Turn on ray (default behavior) left hand.");
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default, Handedness.Left);
            yield return null;
            EnsurePointerStates(Handedness.Right, lineOn);
            EnsurePointerStates(Handedness.Left, lineOn);
        }

    }
}

#endif
