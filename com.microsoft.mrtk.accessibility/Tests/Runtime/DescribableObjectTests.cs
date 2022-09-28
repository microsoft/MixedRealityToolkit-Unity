// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

using SCG = System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Accessibility.Tests
{
    /// <summary>
    /// Tests for verifying the behavior of the describable objects.
    /// </summary>
    public class DescribableObjectTests : BaseRuntimeTests
    {
        #region Setup / TearDown

        private readonly SCG.List<GameObject> sceneContents = new SCG.List<GameObject>();

        [UnitySetUp]
        public override IEnumerator Setup()
        {
            base.Setup();

            yield return RuntimeTestUtilities.WaitForUpdates();

            // The accessibility subsystem requires the MRTK lifecycle manager. This
            // is currently attached to the rig, which resides in the input package.
            InputTestUtilities.InstantiateRig();
        }

        /// <summary>
        /// Cleans up the scene contents after testing describable object registration
        /// and filtering.
        /// </summary>
        [UnityTearDown]
        public override IEnumerator TearDown()
        {
            foreach (GameObject gameObj in sceneContents)
            {
                GameObject.Destroy(gameObj);
            }

            yield return null;

            sceneContents.Clear();

            base.TearDown();

            yield return null;
        }

        #endregion Setup / TearDown

        #region Test cases

        private readonly Vector3 testObjectScale = Vector3.one * 0.1f;

        private readonly Vector3[] inViewPositions =
        {
            // Top row
            new Vector3(-0.5f,  0.5f,   4f),
            new Vector3(0f,     0.5f,   4f),
            new Vector3(0.5f,   0.5f,   4f),
            // Center row
            new Vector3(-0.5f,  0f,     4f),
            new Vector3(0f,     0f,     4f),
            new Vector3(0.5f,   0f,     4f),
            // Bottom row
            new Vector3(-0.5f,  -0.5f,  4f),
            new Vector3(0f,     -0.5f,  4f),
            new Vector3(0.5f,   -0.5f,  4f)
        };

        private readonly Vector3[] outOfViewPositions =
        {
            // Top row, behind
            new Vector3(-0.5f,  0.5f,   -4f),
            new Vector3(0f,     0.5f,   -4f),
            new Vector3(0.5f,   0.5f,   -4f),
            // Center row, behind
            new Vector3(-0.5f,  0f,     -4f),
            new Vector3(0f,     0f,     -4f),
            new Vector3(0.5f,   0f,     -4f),
            // Bottom row, behind
            new Vector3(-0.5f,  -0.5f,  -4f),
            new Vector3(0f,     -0.5f,  -4f),
            new Vector3(0.5f,   -0.5f,  -4f)
        };

        /// <summary>
        /// Creates a test object and places it at the specified location.
        /// </summary>
        /// <param name="location">The position at which to place the created object.</param>
        /// <param name="isDescribable">Should the object have the DescribableObject script attached?</param>
        private void CreateTestObject(
            Vector3 location,
            bool isDescribable)
        {
            GameObject gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObj.transform.position = location;
            gameObj.transform.localScale = testObjectScale;
            sceneContents.Add(gameObj);

            if (isDescribable)
            {
                gameObj.AddComponent<DescribableObject>();
            }
        }

        /// <summary>
        /// Test case to verify that an empty scene is properly handled.
        /// </summary>
        [UnityTest]
        public IEnumerator EmptyScene()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                SCG.List<GameObject> describableObjects = new SCG.List<GameObject>();
                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    (ObjectClassification)(-1),
                    ReaderView.Surround,
                    float.MaxValue,
                    describableObjects);
                Assert.IsTrue(success, "Failed to get the collection of describable objects.");
                Assert.IsTrue(describableObjects.Count == 0, "Should not have found any describable objects an empty scene.");
            }
        }

        /// <summary>
        /// Test case to verify that a scene with only objects that are not describable is properly handled.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectsNotDescribable()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                SCG.List<GameObject> describableObjects = new SCG.List<GameObject>();

                foreach (Vector3 pos in inViewPositions)
                {
                    // Create the objects without adding DescribableObject.
                    CreateTestObject(pos, false);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    (ObjectClassification)(-1),
                    ReaderView.FieldOfView,
                    float.MaxValue,
                    describableObjects);
                Assert.IsTrue(success, "Failed to get the collection of describable objects.");
                Assert.IsTrue(describableObjects.Count == 0, "Should not have found any describable objects in the scene.");
            }
        }

        /// <summary>
        /// Test case to verify that only describable objects in the camera view are reported.
        /// </summary>
        [UnityTest]
        public IEnumerator CameraViewObjects()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                SCG.List<GameObject> describableObjects = new SCG.List<GameObject>();

                // Create objects in the field of view.
                foreach (Vector3 pos in inViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                // Create objects outside of the field of view.
                foreach (Vector3 pos in outOfViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    (ObjectClassification)(-1),
                    ReaderView.FieldOfView,
                    float.MaxValue,
                    describableObjects);
                Assert.IsTrue(success, "Failed to get the collection of describable objects.");
                Assert.AreEqual(
                    inViewPositions.Length,
                    describableObjects.Count,
                    "Failed to find the correct number of describable objects in the scene.");
            }

            yield return null;
        }

        /// <summary>
        /// Test case to verify that all describable objects in the scene are reported.
        /// </summary>
        [UnityTest]
        public IEnumerator SurroundObjects()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                SCG.List<GameObject> describableObjects = new SCG.List<GameObject>();

                // Create objects in the field of view.
                foreach (Vector3 pos in inViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                // Create objects outside of the field of view.
                foreach (Vector3 pos in outOfViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    (ObjectClassification)(-1),
                    ReaderView.Surround,
                    float.MaxValue,
                    describableObjects);
                Assert.IsTrue(success, "Failed to get the collection of describable objects.");
                Assert.AreEqual(
                    inViewPositions.Length + outOfViewPositions.Length,
                    describableObjects.Count,
                    "Failed to find the correct number of describable objects in the scene.");
            }

            yield return null;
        }

        #endregion Test cases
    }
}
