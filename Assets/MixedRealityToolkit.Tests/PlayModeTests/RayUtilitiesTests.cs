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
    // Tests to verify that the ray utilities methods are functioning correctly
    public class RayUtilitiesTests
    {
        // todo

        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        [UnityTest]
        public IEnumerator HeadGazeRayTest()
        {
            yield return null;

            Debug.Log("Get the head gaze ray");
            Ray ray = RayUtilities.GetHeadGazeRay();
            Assert.True(ray.origin == Vector3.zero);
            Assert.True(ray.direction == new Vector3(0.0f, 0.0f, 1.0f));

            yield return null;

            // Rotate the head (camera) 180 degrees
            Debug.Log("Rotate the camera");
            CameraCache.Main.transform.Rotate(0, 180, 0);

            yield return null;

            Debug.Log("Get the head gaze ray");
            ray = RayUtilities.GetHeadGazeRay();
            Debug.Log($"origin: {ray.origin}");
            Assert.True(ray.origin == Vector3.zero);
            Debug.Log($"direction: {ray.direction}");
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

            Debug.Log("Get the right hand ray");
            success = RayUtilities.TryGetHandRay(Handedness.Right, out ray);
            Debug.Log($"TryGetHandRaySucceeded : {success}");
            Assert.True(success);
            // There appears to be an amplification of normal floating point error when using
            // the test hand. Comparing the values by string vs Unity's epsilon (Vector3==) gives the expected
            // and observed results.
            Debug.Log($"origin: {ray.origin}");
            Assert.True(ray.origin.ToString() == rightHandOrigin.ToString());
            Debug.Log($"direction: {ray.direction}");
            Assert.True(ray.direction.ToString() == new Vector3(-0.7f, 0.2f, 0.7f).ToString());
        }
    }
}

#endif
