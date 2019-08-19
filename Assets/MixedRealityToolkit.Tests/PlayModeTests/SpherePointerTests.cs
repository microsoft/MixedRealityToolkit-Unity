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
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify sphere pointer distances
    public class SpherePointerTests
    {
        // Keeping this low by default so the test runs fast. Increase it to be able to see hand movements in the editor.
        private const int numFramesPerMove = 3;

        private float colliderSurfaceZ;

        // Initializes MRTK, instantiates the test content prefab and adds a pointer handler to the test collider
        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();

            float centerZ = 2.0f;
            float scale = 0.2f;
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, centerZ);
            cube.transform.localScale = Vector3.one * scale;

            colliderSurfaceZ = centerZ - scale * 0.5f;

            var collider = cube.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        [UnityTest]
        public IEnumerator SpherePointerDistances()
        {
            Vector3 margin = new Vector3(0, 0, 0.001f);

            var rightHand = new TestHand(Handedness.Right);

            // Show hand far enough from the test collider
            Vector3 idlePos = new Vector3(0.05f, 0, 1.0f);
            yield return rightHand.Show(idlePos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            var pointer = rightHand.GetPointer<SpherePointer>();
            Assert.IsNotNull(pointer, "Expected to find SpherePointer in the hand controller");
            Vector3 nearObjectPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.NearObjectRadius);
            Vector3 interactionEnabledPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.SphereCastRadius);

            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand closer to the collider to enable IsNearObject
            yield return rightHand.MoveTo(nearObjectPos - margin, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(nearObjectPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand closer to the collider to enable IsInteractionEnabled
            yield return rightHand.MoveTo(interactionEnabledPos - margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(interactionEnabledPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);
            // Move hand back out to disable IsInteractionEnabled
            yield return rightHand.MoveTo(interactionEnabledPos - margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand back out to disable IsNearObject
            yield return rightHand.MoveTo(nearObjectPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(nearObjectPos - margin, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            yield return rightHand.MoveTo(idlePos, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
        }
    }
}

#endif
