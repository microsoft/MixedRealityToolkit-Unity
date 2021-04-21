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
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class NearInteractionGrabbableTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            PlayModeTestUtilities.EnsureInputModule();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        /// <summary>
        /// Verify that NearInteractionGrabbable logs an error when created without an appropriate collider.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNoCollider()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Cubes are created with a BoxCollider by default so it has to be removed
            // in order to test the no collider case.
            var collider = testObject.GetComponent<Collider>();
            Object.Destroy(collider);
            yield return null;

            LogAssert.Expect(LogType.Error,
                new Regex("NearInteractionGrabbable requires a BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider on an object.*"));
            testObject.AddComponent<NearInteractionGrabbable>();
            yield return null;

            GameObject.Destroy(testObject);
            yield return null;
        }

        /// <summary>
        /// Verify that NearInteractionGrabbable doesn't log an error when given an appropriate collider.
        /// </summary>
        [UnityTest]
        public IEnumerator TestValidCollider()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.AddComponent<NearInteractionGrabbable>();
            yield return null;

            GameObject.Destroy(testObject);
            yield return null;
        }

        /// <summary>
        /// Verify that NearInteractionGrabbable doesn't log an error when there are multiple colliders
        /// and at least one of them is valid
        /// </summary>
        [UnityTest]
        public IEnumerator TestMultipleColliders()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Purposely destroy the collider so that we can add new ones in a specific order (such
            // that the invalid collider is the first that would be retrieved from a GetComponent
            // call)
            var collider = testObject.GetComponent<Collider>();
            Object.Destroy(collider);
            yield return null;

            // Convex mesh colliders are not valid.
            var meshCollider = testObject.AddComponent<MeshCollider>();
            meshCollider.convex = false;

            // Box colliders are valid.
            testObject.AddComponent<BoxCollider>();

            testObject.AddComponent<NearInteractionGrabbable>();
            yield return null;

            GameObject.Destroy(testObject);
            yield return null;
        }

    }
}
#endif
