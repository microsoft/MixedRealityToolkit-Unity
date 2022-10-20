// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Accessibility.Tests
{
    /// <summary>
    /// Tests for verifying the behavior of the describable objects.
    /// </summary>
    public class DescribableObjectTests : BaseRuntimeInputTests
    {
        #region Test cases

        private static string testCubeGuid = "d10f05ae3a6402045b70860918544ed9";
        private static string testCubeAssetPath = AssetDatabase.GUIDToAssetPath(testCubeGuid);

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
            GameObject gameObj;

            if (isDescribable)
            {
                gameObj = Object.Instantiate(
                    AssetDatabase.LoadAssetAtPath<GameObject>(testCubeAssetPath));
            }
            else
            {
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObj.transform.localScale = Vector3.one * 0.1f;
            }
            gameObj.transform.position = location;
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
                List<DescribableObjectClassification> classifications = new List<DescribableObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of describable object classifications.");

                List<GameObject> describableObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    classifications,
                    DescribableObjectVisibility.Surround,
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
                foreach (Vector3 pos in inViewPositions)
                {
                    // Create the objects without adding DescribableObject.
                    CreateTestObject(pos, false);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                List<DescribableObjectClassification> classifications = new List<DescribableObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of describable object classifications.");

                List<GameObject> describableObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    classifications,
                    DescribableObjectVisibility.FieldOfView,
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

                List<DescribableObjectClassification> classifications = new List<DescribableObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of describable object classifications.");

                List<GameObject> describableObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    classifications,
                    DescribableObjectVisibility.FieldOfView,
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

                List<DescribableObjectClassification> classifications = new List<DescribableObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetDescribableObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of describable object classifications.");

                List<GameObject> describableObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetDescribableObjects(
                    classifications,
                    DescribableObjectVisibility.Surround,
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
