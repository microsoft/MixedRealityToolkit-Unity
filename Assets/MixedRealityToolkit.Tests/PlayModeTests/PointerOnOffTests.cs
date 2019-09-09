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

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Verify that pointers can be turned on and offvia FocusProvider.SetPointerBehavior
    /// </summary>
    public class PointerOnOffTests : BasePlayModeTests
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

        }

        private void EnsurePointerStates(Handedness h, bool rayEnabled, bool grabEnabled, bool pokeEnabled, bool gazeEnabled)
        {
            Assert.True(PointerUtils.GetPointer<LinePointer>(h).IsInteractionEnabled == rayEnabled);
            Assert.True(PointerUtils.GetPointer<SpherePointer>(h).IsInteractionEnabled == grabEnabled);
            Assert.True(PointerUtils.GetPointer<PokePointer>(h).IsInteractionEnabled == pokeEnabled);
            Assert.True(PointerUtils.GetPointer<GGVPointer>(h).IsInteractionEnabled == gazeEnabled);
        }

        [UnityTest]
        public IEnumerator TestSetStates()
        {
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            EnsurePointerStates(Handedness.Right, true, true, true, false);

            // Ray
            PointerUtils.SetRayPointerBehavior(PointerBehavior.Off);
            yield return null;

            EnsurePointerStates(Handedness.Right, false, true, true, false);

            PointerUtils.SetRayPointerBehavior(PointerBehavior.Default);
            yield return null;

            EnsurePointerStates(Handedness.Right, true, true, true, false);

            // Grab
            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Off);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, false, true, false);

            PointerUtils.SetGrabPointerBehavior(PointerBehavior.Default);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, true, true, false);

            // Poke
            PointerUtils.SetPokePointerBehavior(PointerBehavior.Off);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, true, false, false);

            PointerUtils.SetPokePointerBehavior(PointerBehavior.Default);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, true, true, false);

            // Gaze
            PointerUtils.SetGGVBehavior(PointerBehavior.On);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, true, true, true);

            PointerUtils.SetGGVBehavior(PointerBehavior.Default);
            yield return null;
            EnsurePointerStates(Handedness.Right, true, true, true, false);
        }

    }
}

#endif
