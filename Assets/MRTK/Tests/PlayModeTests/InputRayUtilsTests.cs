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

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify that the ray utilities methods are functioning correctly
    public class InputRayUtilsTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        [UnityTest]
        public IEnumerator HeadGazeRayTest()
        {
            yield return null;

            TestContext.Out.WriteLine("Get the head gaze ray");
            Ray ray = InputRayUtils.GetHeadGazeRay();
            Assert.True(ray.origin == Vector3.zero);
            Assert.True(ray.direction == new Vector3(0.0f, 0.0f, 1.0f));

            yield return null;

            // Rotate the head (camera) 180 degrees
            TestContext.Out.WriteLine("Rotate the camera");
            CameraCache.Main.transform.Rotate(0, 180, 0);

            yield return null;

            TestContext.Out.WriteLine("Get the head gaze ray");
            ray = InputRayUtils.GetHeadGazeRay();
            TestContext.Out.WriteLine($"origin: {ray.origin}");
            Assert.True(ray.origin == Vector3.zero);
            TestContext.Out.WriteLine($"direction: {ray.direction}");
            Assert.True(ray.direction == new Vector3(0.0f, 0.0f, -1.0f));
        }

        [UnityTest]
        public IEnumerator HandRayTest()
        {
            yield return null;

            Vector3 rightHandOrigin = new Vector3(-0.3f, -0.1f, 0.5f);

            // Create a hand (we will use the right hand)
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(rightHandOrigin);

            Ray ray;
            bool success;

            TestContext.Out.WriteLine("Get the right hand ray");
            success = InputRayUtils.TryGetHandRay(Handedness.Right, out ray);
            Assert.True(success, "TryGetHandRay did not succeed");
            TestUtilities.AssertAboutEqual(ray.origin, rightHandOrigin, "hand ray origin is not correct", 0.1f);
            TestUtilities.AssertAboutEqual(ray.direction, new Vector3(-0.7f, 0.2f, 0.7f), "hand ray direction is not correct", 0.1f);
        }
    }
}

#endif
